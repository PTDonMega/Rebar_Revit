using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rebar_Revit
{
    public class EstriboHelper
    {
        private Document doc;
        private ArmConfigExec config;
        public EstriboHelper(Document documento, ArmConfigExec configExec)
        {
            doc = documento;
            config = configExec;
        }

        // Classe auxiliar para criação de estribos em vigas
        public bool CriarEstribosViga(FamilyInstance elemento, PropriedadesViga props, double recobrimento, List<ArmStirrup> estribos)
        {
            try
            {
                if (estribos.Count == 0) return true;
                foreach (var estribo in estribos)
                {
                    var tipoEstribo = ObterTipoArmaduraPorDiametro(estribo.Diametro);
                    if (tipoEstribo == null) continue;
                    double espacamento = Uteis.MilimetrosParaFeet(estribo.Espacamento);
                    int numeroEstribos = Math.Max(1, (int)(props.Comprimento / espacamento) - 1);
                    Transform transformViga = config.ObterTransformViga(props);
                    for (int i = 1; i <= numeroEstribos; i++)
                    {
                        double posicaoX = i * espacamento;
                        if (posicaoX >= props.Comprimento) break;
                        CriarEstriboVigaIndividual(elemento, posicaoX, props, tipoEstribo, recobrimento, transformViga);
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
        /// Cria um estribo individual numa viga, testando diferentes orientações e vetores normais
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
                // Gera pontos e curvas do estribo
                double y1 = -larguraUtil / 2;
                double y2 = larguraUtil / 2;
                double z1 = recobrimento;
                double z2 = recobrimento + alturaUtil;
                List<XYZ> pontosLocais = new List<XYZ>
                {
                    new XYZ(posicaoX, y1, z1),
                    new XYZ(posicaoX, y2, z1),
                    new XYZ(posicaoX, y2, z2),
                    new XYZ(posicaoX, y1, z2),
                    new XYZ(posicaoX, y1, z1)
                };
                List<XYZ> pontosGlobais = Uteis.ConverterPontosParaGlobais(pontosLocais, transformViga);
                List<Curve> curvasEstribo = new List<Curve>();
                for (int i = 0; i < pontosGlobais.Count - 1; i++)
                {
                    double distancia = pontosGlobais[i].DistanceTo(pontosGlobais[i + 1]);
                    if (distancia > 1e-6)
                        curvasEstribo.Add(Line.CreateBound(pontosGlobais[i], pontosGlobais[i + 1]));
                }
                if (curvasEstribo.Count < 3)
                    return false;
                // Obtém o tipo de gancho 135° pré-definido
                RebarHookType gancho135 = null;
                try
                {
                    gancho135 = ObterOuCriarRebarHookType135(tipoEstribo.BarNominalDiameter);
                }
                catch
                {
                    return false;
                }
                // Tenta criar o estribo com diferentes orientações e vetores normais
                RebarHookOrientation[] orientacoes = new[] {
                    RebarHookOrientation.Left,
                    RebarHookOrientation.Right
                };
                bool criado = false;
                Rebar estriboCriado = null;
                foreach (var orient in orientacoes)
                {
                    try
                    {
                        estriboCriado = Rebar.CreateFromCurves(
                            doc,
                            RebarStyle.StirrupTie,
                            tipoEstribo,
                            gancho135,
                            gancho135,
                            elemento,
                            transformViga.BasisY,
                            curvasEstribo,
                            orient,
                            orient,
                            true,
                            true);
                        criado = estriboCriado != null;
                        if (criado) break;
                    }
                    catch { }
                }
                if (!criado)
                {
                    XYZ[] vetoresNormais = new[] {
                        transformViga.BasisY,
                        transformViga.BasisZ,
                        transformViga.BasisX
                    };
                    foreach (var vetor in vetoresNormais)
                    {
                        foreach (var orient in orientacoes)
                        {
                            try
                            {
                                estriboCriado = Rebar.CreateFromCurves(
                                    doc,
                                    RebarStyle.StirrupTie,
                                    tipoEstribo,
                                    gancho135,
                                    gancho135,
                                    elemento,
                                    vetor,
                                    curvasEstribo,
                                    orient,
                                    orient,
                                    true,
                                    true);
                                criado = estriboCriado != null;
                                if (criado) break;
                            }
                            catch { }
                        }
                        if (criado) break;
                    }
                }
                if (estriboCriado == null)
                    return false;
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Obtém o tipo de gancho 135° pré-definido do projeto
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
                System.Windows.Forms.MessageBox.Show(
                    "Não foi encontrado um gancho com 135° definido no projeto (Stirrup/Tie Hook - 135 deg.).\nCarregue-o se estiver ausente do varão do estribo.",
                    "Erro Gancho - Revit");
                return null;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        // Obtém o tipo de armadura pelo diâmetro
        private RebarBarType ObterTipoArmaduraPorDiametro(double diametro)
        {
            var collector = new FilteredElementCollector(doc);
            var tipos = collector.OfClass(typeof(RebarBarType)).Cast<RebarBarType>().ToList();
            string diametroStr = diametro.ToString("F0");
            return tipos.FirstOrDefault(t => t.Name.Contains(diametroStr)) ?? tipos.FirstOrDefault();
        }
    }
}
