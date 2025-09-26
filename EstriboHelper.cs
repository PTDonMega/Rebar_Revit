using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rebar_Revit
{
    /// <summary>
    /// Helper respons�vel pela cria��o e configura��o de estribos em vigas.
    /// Cont�m m�todos para gerar estribos em posi��es ao longo da viga e criar um estribo individual.
    /// Coment�rios e descri��es em pt-pt.
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
        /// Cria estribos ao longo da viga conforme a lista fornecida.
        /// Retorna true se o processo terminar sem lan�ar uma excep��o; os estribos individuais podem falhar silenciosamente.
        /// </summary>
        /// <param name="elemento">Inst�ncia da fam�lia da viga onde os estribos ser�o criados.</param>
        /// <param name="props">Propriedades geom�tricas e posicionais da viga.</param>
        /// <param name="recobrimento">Recobrimento interior a considerar (unidades em feet).</param>
        /// <param name="estribos">Lista de defini��es de estribo (di�metro, espa�amento, ...).</param>
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
                throw new Exception($"Erro na cria��o de estribos: {ex.Message}");
            }
        }

        /// <summary>
        /// Cria um estribo individual numa posi��o X ao longo da viga.
        /// A geometria do estribo � constru�da considerando o recobrimento interno e usando o mesmo sistema de coordenadas
        /// da armadura longitudinal, de forma a manter coer�ncia com as barras longitudinais.
        /// </summary>
        /// <returns>True se o estribo for criado com sucesso, false em caso contr�rio.</returns>
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

                // Vetores base da viga: eixo longitudinal, dire��o da largura e dire��o da altura.
                // Usar o mesmo sistema de coordenadas que a armadura longitudinal garante alinhamento correto.
                XYZ eixoViga = (props.PontoFinal - props.PontoInicial).Normalize();
                XYZ larguraDir = transformViga.BasisY.Normalize();
                XYZ alturaDir = transformViga.BasisZ.Normalize();

                // A l�gica segue a mesma conven��o utilizada nas barras longitudinais.
                // props.PontoInicial e props.PontoFinal referem-se ao topo/linha de refer�ncia da viga;
                // para obter a base (face inferior) deslocamos pela altura total.
                XYZ pontoInicialBase = props.PontoInicial - alturaDir * props.Altura;
                XYZ pontoFinalBase = props.PontoFinal - alturaDir * props.Altura;

                // Calcular o ponto na base correspondente � posi��o X desejada ao longo da viga.
                XYZ pontoBasePosicao = pontoInicialBase + eixoViga * posicaoX;

                // Face inferior interna do estribo (considerando o recobrimento).
                XYZ faceInferiorInterna = pontoBasePosicao + alturaDir * recobrimento;

                // Construir o ret�ngulo interno do estribo (considerando recobrimento em todas as dire��es relevantes).
                XYZ p0 = faceInferiorInterna - larguraDir * (larguraUtil / 2.0);  // canto inferior esquerdo
                XYZ p1 = faceInferiorInterna + larguraDir * (larguraUtil / 2.0);  // canto inferior direito
                XYZ p2 = p1 + alturaDir * alturaUtil;                               // canto superior direito
                XYZ p3 = p0 + alturaDir * alturaUtil;                               // canto superior esquerdo

                // Curvas que definem o contorno fechado do estribo (ret�ngulo).
                var curvas = new List<Curve>
                {
                    Line.CreateBound(p0, p1),  // base inferior
                    Line.CreateBound(p1, p2),  // lateral direita
                    Line.CreateBound(p2, p3),  // base superior
                    Line.CreateBound(p3, p0)   // lateral esquerda
                };

                // Obter tipo de gancho 135� presente no projeto.
                RebarHookType gancho135 = ObterOuCriarRebarHookType135(tipoEstribo.BarNominalDiameter);
                if (gancho135 == null) return false;

                // Determinar orienta��es dos ganchos com base na geometria real das curvas.
                RebarHookOrientation startOrient = RebarHookOrientation.Right;
                RebarHookOrientation endOrient = RebarHookOrientation.Right;

                try
                {
                    // Calcular centroid do ret�ngulo
                    XYZ centroid = new XYZ((p0.X + p1.X + p2.X + p3.X) / 4.0,
                                           (p0.Y + p1.Y + p2.Y + p3.Y) / 4.0,
                                           (p0.Z + p1.Z + p2.Z + p3.Z) / 4.0);

                    // Obter pontos exactos onde os ganchos ser�o colocados: in�cio da primeira curva e fim da �ltima curva
                    XYZ hookStartPoint = curvas.First().GetEndPoint(0);
                    XYZ hookStartTangent = (curvas.First().GetEndPoint(1) - curvas.First().GetEndPoint(0)).Normalize();

                    XYZ hookEndPoint = curvas.Last().GetEndPoint(1);
                    XYZ hookEndTangent = (curvas.Last().GetEndPoint(1) - curvas.Last().GetEndPoint(0)).Normalize();

                    // Determinar normal seguro para o estribo: usar eixoViga (normal ao plano do estribo)
                    XYZ normal = eixoViga;

                    // Fun��o local para determinar Left/Right relativo ao tangent e normal
                    Func<XYZ, XYZ, XYZ, RebarHookOrientation> OrientacaoRelativa = (hookPoint, tangent, nrm) =>
                    {
                        XYZ toCentroid = (centroid - hookPoint);
                        if (toCentroid.GetLength() == 0) return RebarHookOrientation.Right;
                        toCentroid = toCentroid.Normalize();

                        XYZ side = nrm.CrossProduct(tangent);
                        if (side.GetLength() == 0) return RebarHookOrientation.Right;
                        side = side.Normalize();

                        double dot = toCentroid.DotProduct(side);
                        // Nota: dependendo da conven��o do Revit, o sinal pode estar invertido.
                        // Se os ganchos continuarem virados para fora, inverter a condi��o abaixo (dot > 0 => Left).
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

                // Tentar criar o estribo com as orienta��es calculadas; se falhar tentar inverter as orienta��es (fallback).
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
                    // Fallback: inverter orienta��es e tentar novamente
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
        /// Procura no projecto um tipo de gancho de 135� predefinido (Stirrup/Tie Hook - 135 deg.).
        /// Se n�o for encontrado, mostra uma mensagem ao utilizador e devolve null.
        /// </summary>
        /// <param name="diametro">Di�metro nominal usado apenas para eventual sele��o (n�o utilizado na pesquisa atual).</param>
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
                    "N�o foi encontrado um gancho com 135� definido no projeto (Stirrup/Tie Hook - 135 deg.).\nCarregue-o se estiver ausente do var�o do estribo.",
                    "Erro Gancho - Revit");
                return null;
            }
            catch (Exception)
            {
                // Propagar ou tratar conforme necess�rio; por agora retorna null para manter comportamento resiliente.
                return null;
            }
        }

        // Obt�m o tipo de armadura pelo di�metro (procura por string que contenha o di�metro).
        private RebarBarType ObterTipoArmaduraPorDiametro(double diametro)
        {
            var collector = new FilteredElementCollector(doc);
            var tipos = collector.OfClass(typeof(RebarBarType)).Cast<RebarBarType>().ToList();
            string diametroStr = diametro.ToString("F0");
            return tipos.FirstOrDefault(t => t.Name.Contains(diametroStr)) ?? tipos.FirstOrDefault();
        }
    }
}
