using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rebar_Revit
{
    /// <summary>
    /// Helper para criação de estribos em vigas.
    /// </summary>
    public class EstriboHelper
    {
        private Document doc;
        private ArmConfigExec config;

        public EstriboHelper(Document documento, ArmConfigExec configExec)
        {
            doc = documento;
            config = configExec;
        }

        /// <summary>
        /// Cria estribos conforme definicoes; aceita combinações.
        /// </summary>
        public bool CriarEstribosViga(FamilyInstance elemento, PropriedadesViga props, double recobrimento, List<ArmStirrup> estribos)
        {
            try
            {
                if (estribos.Count == 0) return true;

                Transform transformViga = config.ObterTransformViga(props);

                foreach (var estribo in estribos)
                {
                    if (estribo.UsaCombinacao && estribo.Combinacao != null)
                    {
                        double comprimentoMm = Uteis.FeetParaMilimetros(props.Comprimento);
                        var sequencia = estribo.Combinacao.GerarSequenciaEstribos(comprimentoMm);

                        var diametrosOrdem = estribo.Combinacao.Estribos.Select(e => e.Diametro).ToList();
                        var sequenciaAlternada = new List<PosicaoEstribo>();

                        if (diametrosOrdem.Count >= 1 && sequencia.Count > 0)
                        {
                            for (int i = 0; i < sequencia.Count; i++)
                            {
                                var pos = sequencia[i];
                                var diam = diametrosOrdem[i % diametrosOrdem.Count];
                                sequenciaAlternada.Add(new PosicaoEstribo { Posicao = pos.Posicao, Diametro = diam, EspacamentoOriginal = pos.EspacamentoOriginal });
                            }
                        }
                        else
                        {
                            sequenciaAlternada = sequencia.ToList();
                        }

                        var diametrosDistintos = sequenciaAlternada.Select(s => s.Diametro).Distinct().ToList();
                        var tiposPorDiam = new Dictionary<double, RebarBarType>();

                        foreach (var d in diametrosDistintos)
                        {
                            var tipo = ObterTipoArmaduraPorDiametro(d);
                            if (tipo != null) tiposPorDiam[d] = tipo;
                        }

                        foreach (var pos in sequenciaAlternada)
                        {
                            if (!tiposPorDiam.TryGetValue(pos.Diametro, out var tipoEstribo))
                                continue;

                            double posicaoFeet = Uteis.MilimetrosParaFeet(pos.Posicao);
                            if (posicaoFeet <= 0 || posicaoFeet >= props.Comprimento) continue;

                            CriarEstriboVigaIndividual(elemento, posicaoFeet, props, tipoEstribo, recobrimento, transformViga);
                        }
                    }
                    else
                    {
                        var tipoEstribo = ObterTipoArmaduraPorDiametro(estribo.Diametro);
                        if (tipoEstribo == null) continue;
                        double espacamentoFeet = Uteis.MilimetrosParaFeet(estribo.Espacamento);
                        if (espacamentoFeet <= 0) continue;

                        int numeroEstribos = Math.Max(1, (int)(props.Comprimento / espacamentoFeet) - 1);
                        for (int i = 1; i <= numeroEstribos; i++)
                        {
                            double posicaoX = i * espacamentoFeet;
                            if (posicaoX >= props.Comprimento) break;
                            CriarEstriboVigaIndividual(elemento, posicaoX, props, tipoEstribo, recobrimento, transformViga);
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro na criação de estribos: {ex.Message}");
            }
        }

        /// <summary>
        /// Cria um estribo individual numa posição X ao longo da viga.
        /// </summary>
        public bool CriarEstriboVigaIndividual(FamilyInstance elemento, double posicaoX,
                                              PropriedadesViga props, RebarBarType tipoEstribo,
                                              double recobrimento, Transform transformViga)
        {
            try
            {
                double larguraUtil = props.Largura - 2 * recobrimento;
                double alturaUtil = props.Altura - 2 * recobrimento;
                if (larguraUtil <= 0 || alturaUtil <= 0)
                    return false;

                XYZ eixoViga = (props.PontoFinal - props.PontoInicial).Normalize();
                XYZ larguraDir = transformViga.BasisY.Normalize();
                XYZ alturaDir = transformViga.BasisZ.Normalize();

                XYZ pontoInicialBase = props.PontoInicial - alturaDir * props.Altura;
                XYZ pontoFinalBase = props.PontoFinal - alturaDir * props.Altura;

                XYZ pontoBasePosicao = pontoInicialBase + eixoViga * posicaoX;

                XYZ faceInferiorInterna = pontoBasePosicao + alturaDir * recobrimento;

                XYZ p0 = faceInferiorInterna - larguraDir * (larguraUtil / 2.0);
                XYZ p1 = faceInferiorInterna + larguraDir * (larguraUtil / 2.0);
                XYZ p2 = p1 + alturaDir * alturaUtil;
                XYZ p3 = p0 + alturaDir * alturaUtil;

                var curvasOrig = new List<Curve>
                {
                    Line.CreateBound(p0, p1),
                    Line.CreateBound(p1, p2),
                    Line.CreateBound(p2, p3),
                    Line.CreateBound(p3, p0)
                };

                var curvas = new List<Curve>
                {
                    Line.CreateBound(p3, p0),
                    Line.CreateBound(p0, p1),
                    Line.CreateBound(p1, p2),
                    Line.CreateBound(p2, p3)
                };

                RebarHookType gancho135 = ObterOuCriarRebarHookType135(tipoEstribo.BarNominalDiameter);
                if (gancho135 == null) return false;

                RebarHookOrientation startOrient = RebarHookOrientation.Right;
                RebarHookOrientation endOrient = RebarHookOrientation.Right;

                try
                {
                    XYZ centroid = new XYZ((p0.X + p1.X + p2.X + p3.X) / 4.0,
                                           (p0.Y + p1.Y + p2.Y + p3.Y) / 4.0,
                                           (p0.Z + p1.Z + p2.Z + p3.Z) / 4.0);

                    XYZ hookStartPoint = curvas.First().GetEndPoint(0); // p3
                    XYZ hookStartTangent = (curvas.First().GetEndPoint(1) - curvas.First().GetEndPoint(0)).Normalize(); // p3->p0

                    XYZ hookEndPoint = curvas.Last().GetEndPoint(1); // p3
                    XYZ hookEndTangent = (curvas.Last().GetEndPoint(1) - curvas.Last().GetEndPoint(0)).Normalize(); // p2->p3

                    XYZ normal = eixoViga;

                    Func<XYZ, XYZ, XYZ, RebarHookOrientation> OrientacaoRelativa = (hookPoint, tangent, nrm) =>
                    {
                        XYZ toCentroid = (centroid - hookPoint);
                        if (toCentroid.GetLength() == 0) return RebarHookOrientation.Right;
                        toCentroid = toCentroid.Normalize();

                        XYZ side = nrm.CrossProduct(tangent);
                        if (side.GetLength() == 0) return RebarHookOrientation.Right;
                        side = side.Normalize();

                        double dot = toCentroid.DotProduct(side);
                        return dot > 0 ? RebarHookOrientation.Left : RebarHookOrientation.Right;
                    };

                    startOrient = OrientacaoRelativa(hookStartPoint, hookStartTangent, normal);
                    endOrient = OrientacaoRelativa(hookEndPoint, hookEndTangent, normal);
                }
                catch
                {
                    startOrient = RebarHookOrientation.Right;
                    endOrient = RebarHookOrientation.Right;
                }

                try
                {
                    var estribo = Rebar.CreateFromCurves(
                        doc,
                        RebarStyle.StirrupTie,
                        tipoEstribo,
                        gancho135,
                        gancho135,
                        elemento,
                        eixoViga,
                        curvas,
                        startOrient,
                        endOrient,
                        true,
                        true);

                    if (estribo != null)
                        return true;
                }
                catch
                {
                    try
                    {
                        var estribo = Rebar.CreateFromCurves(
                            doc,
                            RebarStyle.StirrupTie,
                            tipoEstribo,
                            gancho135,
                            gancho135,
                            elemento,
                            eixoViga,
                            curvas,
                            startOrient == RebarHookOrientation.Left ? RebarHookOrientation.Right : RebarHookOrientation.Left,
                            endOrient == RebarHookOrientation.Left ? RebarHookOrientation.Right : RebarHookOrientation.Left,
                            true,
                            true);

                        if (estribo != null)
                            return true;
                    }
                    catch
                    {
                        return false;
                    }
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Procura gancho 135° no projeto.
        /// </summary>
        public RebarHookType ObterOuCriarRebarHookType135(double diametro)
        {
            try
            {
                var collector = new FilteredElementCollector(doc);
                var gancho135 = collector.OfClass(typeof(RebarHookType))
                    .Cast<RebarHookType>()
                    .FirstOrDefault(h => h.Name.Equals("Stirrup/Tie Hook - 135 deg.", StringComparison.OrdinalIgnoreCase));
                if (gancho135 != null)
                    return gancho135;
                try { Autodesk.Revit.UI.TaskDialog.Show("Erro Gancho - Revit", "Não foi encontrado um gancho com 135° definido no projeto (Stirrup/Tie Hook - 135 deg.).\nCarregue-o se estiver ausente do varão do estribo."); }
                catch { }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        // Obtém RebarBarType pelo diâmetro.
        private RebarBarType ObterTipoArmaduraPorDiametro(double diametro)
        {
            var collector = new FilteredElementCollector(doc);
            var tipos = collector.OfClass(typeof(RebarBarType)).Cast<RebarBarType>().ToList();
            if (tipos == null || tipos.Count == 0) return null;

            string diametroStr = diametro.ToString("F0");

            try
            {
                var rxToken = new System.Text.RegularExpressions.Regex($"(?<!\\d){System.Text.RegularExpressions.Regex.Escape(diametroStr)}(?!\\d)");
                var matchByToken = tipos.FirstOrDefault(t => !string.IsNullOrEmpty(t.Name) && rxToken.IsMatch(t.Name));
                if (matchByToken != null) { return matchByToken; }

                var patterns = new[] { $"[Ff]\\s*{diametroStr}", $"[?]\\s*{diametroStr}", $"[Ø??]\\s*{diametroStr}", $"{diametroStr}\\s*mm", $"phi\\s*{diametroStr}" };
                foreach (var pat in patterns)
                {
                    var rx = new System.Text.RegularExpressions.Regex(pat, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    var found = tipos.FirstOrDefault(t => !string.IsNullOrEmpty(t.Name) && rx.IsMatch(t.Name));
                    if (found != null) { return found; }
                }

                foreach (var t in tipos)
                {
                    try
                    {
                        double nominalFeet = t.BarNominalDiameter;
                        double nominalMm = Uteis.FeetParaMilimetros(nominalFeet);
                        if (Math.Abs(nominalMm - diametro) < 0.5)
                        {
                            return t;
                        }
                    }
                    catch { }
                }

                var matchSubstring = tipos.FirstOrDefault(t => !string.IsNullOrEmpty(t.Name) && t.Name.IndexOf(diametroStr, StringComparison.OrdinalIgnoreCase) >= 0);
                if (matchSubstring != null) { return matchSubstring; }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
