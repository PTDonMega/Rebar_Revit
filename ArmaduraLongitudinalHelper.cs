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
                    var tipoVarao = tiposArmadura.FirstOrDefault(t => t.Name.Contains(varao.Diametro.ToString("F0"))) ?? tiposArmadura.First();
                    string tipoLower = varao.TipoArmadura.ToLower();
                    if (tipoLower == "superior")
                    {
                        if (!CriarArmaduraSuperior(elemento, props, varao, tipoVarao, recobrimento))
                            return false;
                    }
                    else if (tipoLower == "inferior")
                    {
                        if (!CriarArmaduraInferior(elemento, props, varao, tipoVarao, recobrimento))
                            return false;
                    }
                    else if (tipoLower == "lateral")
                    {
                        if (!CriarArmaduraLateral(elemento, props, varao, tipoVarao, recobrimento))
                            return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro na criação de armadura longitudinal: {ex.Message}");
            }
        }

        private bool CriarArmaduraSuperior(FamilyInstance elemento, PropriedadesViga props,
                                         ArmVar varao, RebarBarType tipoVarao, double recobrimento)
        {
            try
            {
                if (varao.Quantidade <= 0) return true;
                Transform transformViga = config.ObterTransformViga(props);
                XYZ larguraDir = transformViga.BasisY.Normalize();
                XYZ alturaDir = transformViga.BasisZ.Normalize();
                double larguraUtil = props.Largura - 2 * recobrimento;
                double diamEstribo = config.Estribos.Count > 0 ? Uteis.MilimetrosParaFeet(config.Estribos[0].Diametro) : 0;
                double diamVarao = Uteis.MilimetrosParaFeet(varao.Diametro);
                // Ponto base na face inferior
                XYZ pontoInicialBase = props.PontoInicial - alturaDir * props.Altura;
                XYZ pontoFinalBase = props.PontoFinal - alturaDir * props.Altura;
                double offsetAltura = props.Altura - recobrimento - diamEstribo / 2 - diamVarao / 2;
                for (int i = 0; i < varao.Quantidade; i++)
                {
                    double offsetY = varao.Quantidade == 1 ? 0 : -larguraUtil / 2 + i * (larguraUtil / (varao.Quantidade - 1));
                    XYZ pontoInicial = pontoInicialBase + larguraDir * offsetY + alturaDir * offsetAltura;
                    XYZ pontoFinal = pontoFinalBase + larguraDir * offsetY + alturaDir * offsetAltura;
                    List<XYZ> pontosComAmarracao = config.CalcularPontosAmarracao(pontoInicial, pontoFinal, varao.Diametro, "superior");
                    if (!config.CriarArmaduraIndividual(elemento, pontosComAmarracao, tipoVarao, varao.Diametro))
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
                                         ArmVar varao, RebarBarType tipoVarao, double recobrimento)
        {
            try
            {
                if (varao.Quantidade <= 0) return true;
                Transform transformViga = config.ObterTransformViga(props);
                XYZ larguraDir = transformViga.BasisY.Normalize();
                XYZ alturaDir = transformViga.BasisZ.Normalize();
                double larguraUtil = props.Largura - 2 * recobrimento;
                double diamEstribo = config.Estribos.Count > 0 ? Uteis.MilimetrosParaFeet(config.Estribos[0].Diametro) : 0;
                double diamVarao = Uteis.MilimetrosParaFeet(varao.Diametro);
                // Ponto base na face inferior
                XYZ pontoInicialBase = props.PontoInicial - alturaDir * props.Altura;
                XYZ pontoFinalBase = props.PontoFinal - alturaDir * props.Altura;
                double offsetAltura = recobrimento + diamEstribo / 2 + diamVarao / 2;
                for (int i = 0; i < varao.Quantidade; i++)
                {
                    double offsetY = varao.Quantidade == 1 ? 0 : -larguraUtil / 2 + i * (larguraUtil / (varao.Quantidade - 1));
                    XYZ pontoInicial = pontoInicialBase + larguraDir * offsetY + alturaDir * offsetAltura;
                    XYZ pontoFinal = pontoFinalBase + larguraDir * offsetY + alturaDir * offsetAltura;
                    List<XYZ> pontosComAmarracao = config.CalcularPontosAmarracao(pontoInicial, pontoFinal, varao.Diametro, "inferior");
                    if (!config.CriarArmaduraIndividual(elemento, pontosComAmarracao, tipoVarao, varao.Diametro))
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
                                        ArmVar varao, RebarBarType tipoVarao, double recobrimento)
        {
            try
            {
                if (varao.Quantidade <= 0) return true;
                Transform transformViga = config.ObterTransformViga(props);
                XYZ alturaDir = transformViga.BasisZ.Normalize();
                XYZ larguraDir = transformViga.BasisY.Normalize();
                double alturaUtil = props.Altura - 2 * recobrimento;
                double diamEstribo = config.Estribos.Count > 0 ? Uteis.MilimetrosParaFeet(config.Estribos[0].Diametro) : 0;
                double diamVarao = Uteis.MilimetrosParaFeet(varao.Diametro);
                double larguraUtil = props.Largura - 2 * recobrimento;
                int quantidadePorFace = varao.Quantidade;
                double espacamentoZ = alturaUtil / (quantidadePorFace + 1);
                // Ponto base na face inferior
                XYZ pontoInicialBase = props.PontoInicial - alturaDir * props.Altura;
                XYZ pontoFinalBase = props.PontoFinal - alturaDir * props.Altura;
                double offsetYEsq = -(props.Largura / 2 - recobrimento - diamEstribo / 2 - diamVarao / 2);
                for (int i = 0; i < quantidadePorFace; i++)
                {
                    double offsetAltura = recobrimento + diamEstribo / 2 + (i + 1) * espacamentoZ;
                    XYZ pontoInicialEsq = pontoInicialBase + larguraDir * offsetYEsq + alturaDir * offsetAltura;
                    XYZ pontoFinalEsq = pontoFinalBase + larguraDir * offsetYEsq + alturaDir * offsetAltura;
                    List<XYZ> pontosEsq = config.CalcularPontosAmarracao(pontoInicialEsq, pontoFinalEsq, varao.Diametro, "lateral");
                    if (!config.CriarArmaduraIndividual(elemento, pontosEsq, tipoVarao, varao.Diametro))
                        return false;
                }
                double offsetYDir = props.Largura / 2 - recobrimento - diamEstribo / 2 - diamVarao / 2;
                for (int i = 0; i < quantidadePorFace; i++)
                {
                    double offsetAltura = recobrimento + diamEstribo / 2 + (i + 1) * espacamentoZ;
                    XYZ pontoInicialDir = pontoInicialBase + larguraDir * offsetYDir + alturaDir * offsetAltura;
                    XYZ pontoFinalDir = pontoFinalBase + larguraDir * offsetYDir + alturaDir * offsetAltura;
                    List<XYZ> pontosDir = config.CalcularPontosAmarracao(pontoInicialDir, pontoFinalDir, varao.Diametro, "lateral");
                    if (!config.CriarArmaduraIndividual(elemento, pontosDir, tipoVarao, varao.Diametro))
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
