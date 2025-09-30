using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rebar_Revit
{
    public class ArmaduraLongitudinalHelper
    {
        private Document doc;
        private ArmConfigExec config;
        public ArmaduraLongitudinalHelper(Document documento, ArmConfigExec configExec)
        {
            doc = documento;
            config = configExec;
        }

        public bool CriarArmaduraVigaLongitudinal(FamilyInstance elemento, PropriedadesViga props, double recobrimento)
        {
            try
            {
                var tiposArmadura = config.ObterTiposArmaduraDisponiveis();
                if (tiposArmadura.Count == 0) return false;
                foreach (var varao in config.Varoes)
                {
                    // Escolher método conforme tipo de varão
                    string tipoLower = varao.TipoArmadura.ToLower();
                    if (tipoLower == "superior")
                    {
                        if (!CriarArmaduraSuperior(elemento, props, varao, tiposArmadura, recobrimento))
                            return false;
                    }
                    else if (tipoLower == "inferior")
                    {
                        if (!CriarArmaduraInferior(elemento, props, varao, tiposArmadura, recobrimento))
                            return false;
                    }
                    else if (tipoLower == "lateral")
                    {
                        if (!CriarArmaduraLateral(elemento, props, varao, tiposArmadura, recobrimento))
                            return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro na cria\u00e7\u00e3o de armadura longitudinal: {ex.Message}");
            }
        }

        // Resolve RebarBarType por diâmetro (mm) com tolerância e fallback.
        private RebarBarType ResolverTipoPorDiametro(List<RebarBarType> tipos, double diametroMm)
        {
            try
            {
                if (tipos == null || tipos.Count == 0) return null;

                // 1) Tentar match numérico via BarNominalDiameter (converter para mm)
                foreach (var t in tipos)
                {
                    try
                    {
                        double nominalFeet = t.BarNominalDiameter;
                        double nominalMm = Uteis.FeetParaMilimetros(nominalFeet);
                        if (Math.Abs(nominalMm - diametroMm) < 0.5)
                        {
                            return t;
                        }
                    }
                    catch { }
                }

                // 2) Fallback: procurar token inteiro no nome
                string token = ((int)Math.Round(diametroMm)).ToString();
                var byName = tipos.FirstOrDefault(t => !string.IsNullOrEmpty(t.Name) && t.Name.IndexOf(token, StringComparison.OrdinalIgnoreCase) >= 0);
                if (byName != null) { return byName; }

                // 3) Último recurso: primeiro disponível
                return tipos.First();
            }
            catch (Exception)
            {
                return tipos.FirstOrDefault();
            }
        }

        private bool CriarArmaduraSuperior(FamilyInstance elemento, PropriedadesViga props,
                                         ArmVar varao, List<RebarBarType> tiposArmadura, double recobrimento)
        {
            try
            {
                if (varao.Quantidade <= 0) return true;
                Transform transformViga = config.ObterTransformViga(props);
                XYZ larguraDir = transformViga.BasisY.Normalize();
                XYZ alturaDir = transformViga.BasisZ.Normalize();
                double larguraUtil = props.Largura - 2 * recobrimento;
                double diamEstribo = config.Estribos.Count > 0 ? Uteis.MilimetrosParaFeet(config.Estribos[0].Diametro) : 0;

                // Distribuição de diâmetros para combinações
                var distribuicao = varao.ObterDistribuicaoDiametros();

                for (int i = 0; i < varao.Quantidade; i++)
                {
                    double diamAtual = (distribuicao != null && i < distribuicao.Count) ? distribuicao[i] : varao.Diametro;
                    double diamVarao = Uteis.MilimetrosParaFeet(diamAtual);

                    var tipoVarao = ResolverTipoPorDiametro(tiposArmadura, diamAtual) ?? tiposArmadura.First();

                    double offsetAltura = props.Altura - recobrimento - diamEstribo / 2 - diamVarao / 2;

                    // Calcular margem lateral considerando recobrimento + metade do estribo + metade do varão (tudo em feet)
                    double innerMargin = recobrimento + diamEstribo / 2.0 + diamVarao / 2.0;
                    double availableWidthFeet = props.Largura - 2.0 * innerMargin;
                    if (availableWidthFeet < 0) availableWidthFeet = Math.Max(0.0, props.Largura - 2.0 * recobrimento);

                    double offsetY = 0.0;
                    if (varao.Quantidade == 1)
                        offsetY = 0.0;
                    else
                        offsetY = -(props.Largura / 2.0 - innerMargin) + i * (availableWidthFeet / (varao.Quantidade - 1));

                    XYZ pontoInicialBase = props.PontoInicial - alturaDir * props.Altura;
                    XYZ pontoFinalBase = props.PontoFinal - alturaDir * props.Altura;
                    XYZ pontoInicial = pontoInicialBase + larguraDir * offsetY + alturaDir * offsetAltura;
                    XYZ pontoFinal = pontoFinalBase + larguraDir * offsetY + alturaDir * offsetAltura;
                    List<XYZ> pontosComAmarracao = config.CalcularPontosAmarracao(pontoInicial, pontoFinal, diamAtual, "superior");
                    if (!config.CriarArmaduraIndividual(elemento, pontosComAmarracao, tipoVarao, diamAtual))
                        return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro na armadura superior: {ex.Message}");
            }
        }

        private bool CriarArmaduraInferior(FamilyInstance elemento, PropriedadesViga props,
                                         ArmVar varao, List<RebarBarType> tiposArmadura, double recobrimento)
        {
            try
            {
                if (varao.Quantidade <= 0) return true;
                Transform transformViga = config.ObterTransformViga(props);
                XYZ larguraDir = transformViga.BasisY.Normalize();
                XYZ alturaDir = transformViga.BasisZ.Normalize();
                double larguraUtil = props.Largura - 2 * recobrimento;
                double diamEstribo = config.Estribos.Count > 0 ? Uteis.MilimetrosParaFeet(config.Estribos[0].Diametro) : 0;

                var distribuicao = varao.ObterDistribuicaoDiametros();

                for (int i = 0; i < varao.Quantidade; i++)
                {
                    double diamAtual = (distribuicao != null && i < distribuicao.Count) ? distribuicao[i] : varao.Diametro;
                    double diamVarao = Uteis.MilimetrosParaFeet(diamAtual);

                    var tipoVarao = ResolverTipoPorDiametro(tiposArmadura, diamAtual) ?? tiposArmadura.First();

                    double offsetAltura = recobrimento + diamEstribo / 2 + diamVarao / 2;

                    // Calcular margem lateral considerando recobrimento + metade do estribo + metade do varão (tudo em feet)
                    double innerMarginInf = recobrimento + diamEstribo / 2.0 + diamVarao / 2.0;
                    double availableWidthFeetInf = props.Largura - 2.0 * innerMarginInf;
                    if (availableWidthFeetInf < 0) availableWidthFeetInf = Math.Max(0.0, props.Largura - 2.0 * recobrimento);

                    double offsetY = 0.0;
                    if (varao.Quantidade == 1)
                        offsetY = 0.0;
                    else
                        offsetY = -(props.Largura / 2.0 - innerMarginInf) + i * (availableWidthFeetInf / (varao.Quantidade - 1));

                    XYZ pontoInicialBase = props.PontoInicial - alturaDir * props.Altura;
                    XYZ pontoFinalBase = props.PontoFinal - alturaDir * props.Altura;
                    XYZ pontoInicial = pontoInicialBase + larguraDir * offsetY + alturaDir * offsetAltura;
                    XYZ pontoFinal = pontoFinalBase + larguraDir * offsetY + alturaDir * offsetAltura;
                    List<XYZ> pontosComAmarracao = config.CalcularPontosAmarracao(pontoInicial, pontoFinal, diamAtual, "inferior");
                    if (!config.CriarArmaduraIndividual(elemento, pontosComAmarracao, tipoVarao, diamAtual))
                        return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro na armadura inferior: {ex.Message}");
            }
        }

        private bool CriarArmaduraLateral(FamilyInstance elemento, PropriedadesViga props,
                                        ArmVar varao, List<RebarBarType> tiposArmadura, double recobrimento)
        {
            try
            {
                if (varao.Quantidade <= 0) return true;
                Transform transformViga = config.ObterTransformViga(props);
                XYZ alturaDir = transformViga.BasisZ.Normalize();
                XYZ larguraDir = transformViga.BasisY.Normalize();
                double alturaUtil = props.Altura - 2 * recobrimento;
                double diamEstribo = config.Estribos.Count > 0 ? Uteis.MilimetrosParaFeet(config.Estribos[0].Diametro) : 0;

                var distribuicao = varao.ObterDistribuicaoDiametros();

                int quantidadePorFace = varao.Quantidade;
                double espacamentoZ = alturaUtil / (quantidadePorFace + 1);
                XYZ pontoInicialBase = props.PontoInicial - alturaDir * props.Altura;
                XYZ pontoFinalBase = props.PontoFinal - alturaDir * props.Altura;
                double offsetYEsq = -(props.Largura / 2 - recobrimento - diamEstribo / 2 - (distribuicao != null && distribuicao.Count>0 ? Uteis.MilimetrosParaFeet(distribuicao[0]) : 0));
                for (int i = 0; i < quantidadePorFace; i++)
                {
                    double diamAtual = (distribuicao != null && i < distribuicao.Count) ? distribuicao[i] : varao.Diametro;
                    double diamVarao = Uteis.MilimetrosParaFeet(diamAtual);

                    var tipoVarao = ResolverTipoPorDiametro(tiposArmadura, diamAtual) ?? tiposArmadura.First();

                    double offsetAltura = recobrimento + diamEstribo / 2 + (i + 1) * espacamentoZ;
                    XYZ pontoInicialEsq = pontoInicialBase + larguraDir * offsetYEsq + alturaDir * offsetAltura;
                    XYZ pontoFinalEsq = pontoFinalBase + larguraDir * offsetYEsq + alturaDir * offsetAltura;
                    List<XYZ> pontosEsq = config.CalcularPontosAmarracao(pontoInicialEsq, pontoFinalEsq, diamAtual, "lateral");
                    if (!config.CriarArmaduraIndividual(elemento, pontosEsq, tipoVarao, diamAtual))
                        return false;
                }
                double offsetYDir = props.Largura / 2 - recobrimento - diamEstribo / 2 - (distribuicao != null && distribuicao.Count>0 ? Uteis.MilimetrosParaFeet(distribuicao[0]) : 0);
                for (int i = 0; i < quantidadePorFace; i++)
                {
                    double diamAtual = (distribuicao != null && i < distribuicao.Count) ? distribuicao[i] : varao.Diametro;
                    double diamVarao = Uteis.MilimetrosParaFeet(diamAtual);

                    var tipoVarao = ResolverTipoPorDiametro(tiposArmadura, diamAtual) ?? tiposArmadura.First();

                    double offsetAltura = recobrimento + diamEstribo / 2 + (i + 1) * espacamentoZ;
                    XYZ pontoInicialDir = pontoInicialBase + larguraDir * offsetYDir + alturaDir * offsetAltura;
                    XYZ pontoFinalDir = pontoFinalBase + larguraDir * offsetYDir + alturaDir * offsetAltura;
                    List<XYZ> pontosDir = config.CalcularPontosAmarracao(pontoInicialDir, pontoFinalDir, diamAtual, "lateral");
                    if (!config.CriarArmaduraIndividual(elemento, pontosDir, tipoVarao, diamAtual))
                        return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro na armadura lateral: {ex.Message}");
            }
        }
    }
}
