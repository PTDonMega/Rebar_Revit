using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

namespace Rebar_Revit
{
    /// <summary>
    /// Calculadora especializada para amarra��es de vigas
    /// </summary>
    public class Amarracao
    {
        /// <summary>
        /// Calcula comprimento de amarra��o baseado no di�metro e regulamento
        /// </summary>
        public double CalcularComprimentoAmarracao(double diametro, double multiplicador = 50)
        {
            double multiplicadorFinal = Math.Max(30, Math.Min(80, multiplicador));
            return multiplicadorFinal * diametro; // resultado em mm
        }

        /// <summary>
        /// Determina o tipo de gancho baseado na posi��o do elemento
        /// </summary>
        public RebarHookType DeterminarTipoGancho(Document doc, TipoAmarracaoEnum tipoAmarracao)
        {
            if (doc == null) return null;

            try
            {
                FilteredElementCollector collector = new FilteredElementCollector(doc);
                var ganchos = collector.OfClass(typeof(RebarHookType))
                                      .Cast<RebarHookType>()
                                      .ToList();

                if (ganchos.Count == 0) return null;

                switch (tipoAmarracao)
                {
                    case TipoAmarracaoEnum.Dobrada90:
                        // Procurar gancho de 90� primeiro
                        var gancho90 = ganchos.FirstOrDefault(g =>
                            g.Name.Contains("90") ||
                            g.Name.ToLower().Contains("standard") ||
                            g.Name.ToLower().Contains("hook"));

                        return gancho90 ?? ganchos.FirstOrDefault();

                    case TipoAmarracaoEnum.Gancho135:
                        // Procurar gancho de 135�
                        var gancho135 = ganchos.FirstOrDefault(g =>
                            g.Name.Contains("135") ||
                            g.Name.ToLower().Contains("seismic"));

                        return gancho135 ?? ganchos.FirstOrDefault();

                    case TipoAmarracaoEnum.Reta:
                    case TipoAmarracaoEnum.Automatico:
                    default:
                        return null; // Sem gancho para amarra��o reta
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao determinar tipo de gancho: {ex.Message}");
            }
        }

        /// <summary>
        /// Calcula pontos de ancoragem para diferentes tipos de amarra��o em vigas
        /// </summary>
        public List<XYZ> CalcularPontosAncoragem(XYZ pontoInicial, XYZ pontoFinal,
                                                TipoAmarracaoEnum tipoAmarracao,
                                                double comprimentoAmarracao)
        {
            List<XYZ> pontos = new List<XYZ>();

            try
            {
                XYZ direcao = (pontoFinal - pontoInicial).Normalize();
                double comprimentoAmarracaoPes = Uteis.MilimetrosParaFeet(comprimentoAmarracao); // Converter mm para p�s

                switch (tipoAmarracao)
                {
                    case TipoAmarracaoEnum.Reta:
                        // Amarra��o reta - estender nas duas extremidades
                        XYZ pontoInicialReta = pontoInicial - direcao * comprimentoAmarracaoPes;
                        XYZ pontoFinalReta = pontoFinal + direcao * comprimentoAmarracaoPes;
                        
                        pontos.Add(pontoInicialReta);
                        pontos.Add(pontoInicial);
                        pontos.Add(pontoFinal);
                        pontos.Add(pontoFinalReta);
                        break;

                    case TipoAmarracaoEnum.Dobrada90:
                        // Amarra��o com dobra de 90� (ganchos verticais)
                        XYZ pontoInicialDobrado = pontoInicial - direcao * comprimentoAmarracaoPes;
                        XYZ ganchoInicial = pontoInicialDobrado + new XYZ(0, 0, comprimentoAmarracaoPes * 0.5);
                        
                        XYZ pontoFinalDobrado = pontoFinal + direcao * comprimentoAmarracaoPes;
                        XYZ ganchoFinal = pontoFinalDobrado + new XYZ(0, 0, comprimentoAmarracaoPes * 0.5);
                        
                        pontos.Add(ganchoInicial);
                        pontos.Add(pontoInicialDobrado);
                        pontos.Add(pontoInicial);
                        pontos.Add(pontoFinal);
                        pontos.Add(pontoFinalDobrado);
                        pontos.Add(ganchoFinal);
                        break;

                    case TipoAmarracaoEnum.Gancho135:
                        // Amarra��o com gancho de 135�
                        XYZ pontoInicialGancho = pontoInicial - direcao * comprimentoAmarracaoPes;
                        XYZ ganchoInicial135 = pontoInicialGancho + new XYZ(0, 0, comprimentoAmarracaoPes * 0.7);
                        
                        XYZ pontoFinalGancho = pontoFinal + direcao * comprimentoAmarracaoPes;
                        XYZ ganchoFinal135 = pontoFinalGancho + new XYZ(0, 0, comprimentoAmarracaoPes * 0.7);
                        
                        pontos.Add(ganchoInicial135);
                        pontos.Add(pontoInicialGancho);
                        pontos.Add(pontoInicial);
                        pontos.Add(pontoFinal);
                        pontos.Add(pontoFinalGancho);
                        pontos.Add(ganchoFinal135);
                        break;

                    case TipoAmarracaoEnum.Automatico:
                    default:
                        // Comportamento autom�tico - usar amarra��o reta como padr�o
                        pontos.Add(pontoInicial);
                        pontos.Add(pontoFinal);
                        break;
                }
            }
            catch (Exception ex)
            {
                // Em caso de erro, retornar pontos b�sicos
                pontos.Clear();
                pontos.Add(pontoInicial);
                pontos.Add(pontoFinal);
            }

            return pontos;
        }

        /// <summary>
        /// Determina o tipo de amarra��o autom�tico baseado na posi��o da armadura na viga
        /// </summary>
        public TipoAmarracaoEnum DeterminarTipoAmarracaoViga(string posicaoArmadura)
        {
            switch (posicaoArmadura.ToLower())
            {
                case "superior":
                    return TipoAmarracaoEnum.Dobrada90; // Armadura superior com ganchos

                case "inferior":
                    return TipoAmarracaoEnum.Reta; // Armadura inferior geralmente reta

                case "lateral":
                    return TipoAmarracaoEnum.Reta; // Armadura lateral reta

                default:
                    return TipoAmarracaoEnum.Reta;
            }
        }

        /// <summary>
        /// Calcula espa�amento m�nimo entre var�es baseado no di�metro
        /// </summary>
        public double CalcularEspacamentoMinimo(double diametro)
        {
            // Espa�amento m�nimo = maior entre: di�metro, 20mm, ou tamanho do agregado + 5mm
            double espacamentoMinimo = Math.Max(diametro, 20); // mm
            return Uteis.MilimetrosParaFeet(espacamentoMinimo); // Converter para p�s
        }

        /// <summary>
        /// Verifica se a configura��o de armadura atende aos requisitos m�nimos
        /// </summary>
        public bool ValidarConfiguracaoArmadura(List<ArmVar> varoes, double larguraViga, double alturaViga)
        {
            try
            {
                // Verificar se h� armadura m�nima
                if (varoes == null || varoes.Count == 0)
                    return false;

                // Verificar se h� pelo menos armadura superior e inferior
                bool temSuperior = varoes.Any(v => v.TipoArmadura.ToLower() == "superior");
                bool temInferior = varoes.Any(v => v.TipoArmadura.ToLower() == "inferior");

                if (!temSuperior || !temInferior)
                    return false;

                // Verificar espa�amento m�nimo
                foreach (var varao in varoes)
                {
                    double espacamentoMinimo = CalcularEspacamentoMinimo(varao.Diametro);
                    double larguraUtil = larguraViga - Uteis.MilimetrosParaFeet(50);
                    double espacamentoDisponivel = larguraUtil / (varao.Quantidade + 1);

                    if (espacamentoDisponivel < espacamentoMinimo)
                    {
                        return false; // Espa�amento insuficiente
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Calcula a taxa de armadura para verifica��o regulamentar
        /// </summary>
        public double CalcularTaxaArmadura(List<ArmVar> varoes, double larguraViga, double alturaViga)
        {
            try
            {
                double areaTotal = 0;

                foreach (var varao in varoes)
                {
                    double raio = varao.Diametro / 2000; // Converter mm para m
                    double areaVarao = Math.PI * raio * raio; // m�
                    areaTotal += varao.Quantidade * areaVarao;
                }

                double larguraM = larguraViga * 0.3048; // Converter p�s para m
                double alturaM = alturaViga * 0.3048; // Converter p�s para m
                double areaSecao = larguraM * alturaM;

                return (areaTotal / areaSecao) * 100; // Percentagem
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Sugere configura��o de estribos baseada nas dimens�es da viga
        /// </summary>
        public List<ArmStirrup> SugerirEstribos(double alturaViga, double comprimentoViga)
        {
            List<ArmStirrup> estribos = new List<ArmStirrup>();

            try
            {
                double alturaM = alturaViga * 0.3048; // Converter para metros
                double comprimentoM = comprimentoViga * 0.3048;

                // Determinar di�metro do estribo baseado na altura
                double diametroEstribo = 8; // mm, padr�o
                if (alturaM > 0.6) diametroEstribo = 10;
                if (alturaM > 1.0) diametroEstribo = 12;

                // Determinar espa�amento
                double espacamentoZonaCorte = Math.Min(alturaM * 1000 / 4, 150); // mm
                double espacamentoZonaCentral = Math.Min(alturaM * 1000 / 2, 300); // mm

                // Estribo para zonas de corte elevado (extremidades)
                estribos.Add(new ArmStirrup(diametroEstribo, espacamentoZonaCorte));

                // Se a viga for longa, adicionar estribo para zona central
                if (comprimentoM > 4.0)
                {
                    estribos.Add(new ArmStirrup(diametroEstribo, espacamentoZonaCentral));
                }

                return estribos;
            }
            catch
            {
                // Configura��o padr�o em caso de erro
                estribos.Add(new ArmStirrup(6, 125));
                return estribos;
            }
        }
    }
}