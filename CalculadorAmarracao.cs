using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

namespace Rebar_Revit
{
    /// <summary>
    /// Calculadora especializada para amarrações de vigas
    /// </summary>
    public class CalculadorAmarracao
    {
        /// <summary>
        /// Calcula comprimento de amarração baseado no diâmetro e regulamento
        /// </summary>
        public double CalcularComprimentoAmarracao(double diametro, double multiplicador = 50)
        {
            // Para vigas, usar multiplicadores menores que pilares
            double multiplicadorFinal = Math.Max(30, Math.Min(80, multiplicador));
            return multiplicadorFinal * diametro; // resultado em mm
        }

        /// <summary>
        /// Determina o tipo de gancho baseado na posição do elemento
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
                        // Procurar gancho de 90° primeiro
                        var gancho90 = ganchos.FirstOrDefault(g =>
                            g.Name.Contains("90") ||
                            g.Name.ToLower().Contains("standard") ||
                            g.Name.ToLower().Contains("hook"));

                        return gancho90 ?? ganchos.FirstOrDefault();

                    case TipoAmarracaoEnum.Gancho135:
                        // Procurar gancho de 135°
                        var gancho135 = ganchos.FirstOrDefault(g =>
                            g.Name.Contains("135") ||
                            g.Name.ToLower().Contains("seismic"));

                        return gancho135 ?? ganchos.FirstOrDefault();

                    case TipoAmarracaoEnum.Reta:
                    case TipoAmarracaoEnum.Automatico:
                    default:
                        return null; // Sem gancho para amarração reta
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao determinar tipo de gancho: {ex.Message}");
            }
        }

        /// <summary>
        /// Calcula pontos de ancoragem para diferentes tipos de amarração em vigas
        /// </summary>
        public List<XYZ> CalcularPontosAncoragem(XYZ pontoInicial, XYZ pontoFinal,
                                                TipoAmarracaoEnum tipoAmarracao,
                                                double comprimentoAmarracao)
        {
            List<XYZ> pontos = new List<XYZ>();

            try
            {
                XYZ direcao = (pontoFinal - pontoInicial).Normalize();
                double comprimentoAmarracaoPes = Uteis.MilimetrosParaFeet(comprimentoAmarracao); // Converter mm para pés

                switch (tipoAmarracao)
                {
                    case TipoAmarracaoEnum.Reta:
                        // Amarração reta - estender nas duas extremidades
                        XYZ pontoInicialReta = pontoInicial - direcao * comprimentoAmarracaoPes;
                        XYZ pontoFinalReta = pontoFinal + direcao * comprimentoAmarracaoPes;
                        
                        pontos.Add(pontoInicialReta);
                        pontos.Add(pontoInicial);
                        pontos.Add(pontoFinal);
                        pontos.Add(pontoFinalReta);
                        break;

                    case TipoAmarracaoEnum.Dobrada90:
                        // Amarração com dobra de 90° (ganchos verticais)
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
                        // Amarração com gancho de 135°
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
                        // Comportamento automático - usar amarração reta como padrão
                        pontos.Add(pontoInicial);
                        pontos.Add(pontoFinal);
                        break;
                }
            }
            catch (Exception ex)
            {
                // Em caso de erro, retornar pontos básicos
                pontos.Clear();
                pontos.Add(pontoInicial);
                pontos.Add(pontoFinal);
            }

            return pontos;
        }

        /// <summary>
        /// Determina o tipo de amarração automático baseado na posição da armadura na viga
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
        /// Calcula espaçamento mínimo entre varões baseado no diâmetro
        /// </summary>
        public double CalcularEspacamentoMinimo(double diametro)
        {
            // Espaçamento mínimo = maior entre: diâmetro, 20mm, ou tamanho do agregado + 5mm
            double espacamentoMinimo = Math.Max(diametro, 20); // mm
            return Uteis.MilimetrosParaFeet(espacamentoMinimo); // Converter para pés
        }

        /// <summary>
        /// Verifica se a configuração de armadura atende aos requisitos mínimos
        /// </summary>
        public bool ValidarConfiguracaoArmadura(List<ArmVar> varoes, double larguraViga, double alturaViga)
        {
            try
            {
                // Verificar se há armadura mínima
                if (varoes == null || varoes.Count == 0)
                    return false;

                // Verificar se há pelo menos armadura superior e inferior
                bool temSuperior = varoes.Any(v => v.TipoArmadura.ToLower() == "superior");
                bool temInferior = varoes.Any(v => v.TipoArmadura.ToLower() == "inferior");

                if (!temSuperior || !temInferior)
                    return false;

                // Verificar espaçamento mínimo
                foreach (var varao in varoes)
                {
                    double espacamentoMinimo = CalcularEspacamentoMinimo(varao.Diametro);
                    double larguraUtil = larguraViga - Uteis.MilimetrosParaFeet(50);
                    double espacamentoDisponivel = larguraUtil / (varao.Quantidade + 1);

                    if (espacamentoDisponivel < espacamentoMinimo)
                    {
                        return false; // Espaçamento insuficiente
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
        /// Calcula a taxa de armadura para verificação regulamentar
        /// </summary>
        public double CalcularTaxaArmadura(List<ArmVar> varoes, double larguraViga, double alturaViga)
        {
            try
            {
                double areaTotal = 0;

                foreach (var varao in varoes)
                {
                    double raio = varao.Diametro / 2000; // Converter mm para m
                    double areaVarao = Math.PI * raio * raio; // m²
                    areaTotal += varao.Quantidade * areaVarao;
                }

                double larguraM = larguraViga * 0.3048; // Converter pés para m
                double alturaM = alturaViga * 0.3048; // Converter pés para m
                double areaSecao = larguraM * alturaM;

                return (areaTotal / areaSecao) * 100; // Percentagem
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Sugere configuração de estribos baseada nas dimensões da viga
        /// </summary>
        public List<ArmStirrup> SugerirEstribos(double alturaViga, double comprimentoViga)
        {
            List<ArmStirrup> estribos = new List<ArmStirrup>();

            try
            {
                double alturaM = alturaViga * 0.3048; // Converter para metros
                double comprimentoM = comprimentoViga * 0.3048;

                // Determinar diâmetro do estribo baseado na altura
                double diametroEstribo = 8; // mm, padrão
                if (alturaM > 0.6) diametroEstribo = 10;
                if (alturaM > 1.0) diametroEstribo = 12;

                // Determinar espaçamento
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
                // Configuração padrão em caso de erro
                estribos.Add(new ArmStirrup(8, 150));
                return estribos;
            }
        }
    }
}