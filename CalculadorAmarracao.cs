using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

namespace MacroArmaduraAvancado
{
    /// <summary>
    /// Calculadora especializada para amarrações - Versão Completa e Funcional
    /// </summary>
    public class CalculadorAmarracao
    {
        /// <summary>
        /// Calcula comprimento de amarração baseado no diâmetro e regulamento
        /// </summary>
        public double CalcularComprimentoAmarracao(double diametro, double multiplicador = 70)
        {
            // Aplicar limites regulamentares
            double multiplicadorFinal = Math.Max(30, Math.Min(100, multiplicador));

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
        /// Calcula pontos de ancoragem para diferentes tipos de amarração
        /// </summary>
        public List<XYZ> CalcularPontosAncoragem(XYZ pontoInicial, XYZ pontoFinal,
                                                TipoAmarracaoEnum tipoAmarracao,
                                                double comprimentoAmarracao)
        {
            List<XYZ> pontos = new List<XYZ>();

            if (pontoInicial == null || pontoFinal == null)
            {
                throw new ArgumentException("Pontos inicial e final não podem ser nulos");
            }

            try
            {
                double comprimentoAmPes = comprimentoAmarracao / 304.8; // Converter mm para pés

                switch (tipoAmarracao)
                {
                    case TipoAmarracaoEnum.Dobrada90:
                        pontos = CalcularPontosAmarracaoDobrada90(pontoInicial, pontoFinal, comprimentoAmPes);
                        break;

                    case TipoAmarracaoEnum.Gancho135:
                        pontos = CalcularPontosAmarracaoGancho135(pontoInicial, pontoFinal, comprimentoAmPes);
                        break;

                    case TipoAmarracaoEnum.Reta:
                    case TipoAmarracaoEnum.Automatico:
                    default:
                        pontos = CalcularPontosAmarracaoReta(pontoInicial, pontoFinal, comprimentoAmPes);
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro no cálculo de pontos de ancoragem: {ex.Message}");
            }

            return pontos;
        }

        /// <summary>
        /// Calcula pontos para amarração reta (simples)
        /// </summary>
        private List<XYZ> CalcularPontosAmarracaoReta(XYZ pontoInicial, XYZ pontoFinal, double comprimentoAmPes)
        {
            List<XYZ> pontos = new List<XYZ>();

            // Amarração reta: estender o varão nas extremidades
            XYZ direcao = (pontoFinal - pontoInicial).Normalize();

            // Ponto inicial estendido para baixo
            XYZ pontoInicialEstendido = pontoInicial - (direcao * comprimentoAmPes);

            // Ponto final estendido para cima
            XYZ pontoFinalEstendido = pontoFinal + (direcao * comprimentoAmPes);

            pontos.Add(pontoInicialEstendido);
            pontos.Add(pontoInicial);
            pontos.Add(pontoFinal);
            pontos.Add(pontoFinalEstendido);

            return pontos;
        }

        /// <summary>
        /// Calcula pontos para amarração dobrada a 90°
        /// </summary>
        private List<XYZ> CalcularPontosAmarracaoDobrada90(XYZ pontoInicial, XYZ pontoFinal, double comprimentoAmPes)
        {
            List<XYZ> pontos = new List<XYZ>();

            // Amarração dobrada: criar dobras horizontais nas extremidades

            // Ponto de dobra inferior (para dentro da seção)
            XYZ pontoDobraInferior = new XYZ(pontoInicial.X + comprimentoAmPes,
                                           pontoInicial.Y,
                                           pontoInicial.Z);

            // Ponto de dobra superior (para dentro da seção) 
            XYZ pontoDobraSuperior = new XYZ(pontoFinal.X + comprimentoAmPes,
                                           pontoFinal.Y,
                                           pontoFinal.Z);

            // Sequência de pontos para formar a amarração dobrada
            pontos.Add(pontoDobraInferior);    // Início da dobra inferior
            pontos.Add(pontoInicial);          // Base do varão
            pontos.Add(pontoFinal);            // Topo do varão
            pontos.Add(pontoDobraSuperior);    // Fim da dobra superior

            return pontos;
        }

        /// <summary>
        /// Calcula pontos para amarração com gancho a 135°
        /// </summary>
        private List<XYZ> CalcularPontosAmarracaoGancho135(XYZ pontoInicial, XYZ pontoFinal, double comprimentoAmPes)
        {
            List<XYZ> pontos = new List<XYZ>();

            // Gancho 135°: mais complexo, com ângulo específico
            double anguloRadianos = Math.PI * 135 / 180; // 135° em radianos

            // Comprimento da projeção horizontal do gancho
            double projeccaoHorizontal = comprimentoAmPes * Math.Cos(anguloRadianos);
            double projeccaoVertical = comprimentoAmPes * Math.Sin(anguloRadianos);

            // Gancho inferior
            XYZ pontoGanchoInferior = new XYZ(pontoInicial.X + projeccaoHorizontal,
                                            pontoInicial.Y,
                                            pontoInicial.Z - projeccaoVertical);

            // Gancho superior
            XYZ pontoGanchoSuperior = new XYZ(pontoFinal.X + projeccaoHorizontal,
                                            pontoFinal.Y,
                                            pontoFinal.Z + projeccaoVertical);

            pontos.Add(pontoGanchoInferior);
            pontos.Add(pontoInicial);
            pontos.Add(pontoFinal);
            pontos.Add(pontoGanchoSuperior);

            return pontos;
        }

        /// <summary>
        /// Calcula comprimento total da armadura incluindo amarrações
        /// </summary>
        public double CalcularComprimentoTotalArmadura(double alturaElemento, double comprimentoAmarracao,
                                                     TipoAmarracaoEnum tipoAmarracao)
        {
            try
            {
                double comprimentoBase = alturaElemento; // em mm

                switch (tipoAmarracao)
                {
                    case TipoAmarracaoEnum.Reta:
                        // Amarração reta: adicionar comprimento nas duas extremidades
                        return comprimentoBase + (2 * comprimentoAmarracao);

                    case TipoAmarracaoEnum.Dobrada90:
                        // Amarração dobrada: adicionar comprimento das dobras
                        return comprimentoBase + (2 * comprimentoAmarracao);

                    case TipoAmarracaoEnum.Gancho135:
                        // Gancho 135°: adicionar comprimento dos ganchos
                        double comprimentoGancho = comprimentoAmarracao * 1.2; // 20% adicional para o gancho
                        return comprimentoBase + (2 * comprimentoGancho);

                    default:
                        return comprimentoBase + (2 * comprimentoAmarracao);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro no cálculo de comprimento total: {ex.Message}");
            }
        }

        /// <summary>
        /// Valida se o comprimento de amarração está dentro dos limites regulamentares
        /// </summary>
        public bool ValidarComprimentoAmarracao(double diametro, double comprimentoAmarracao)
        {
            try
            {
                // Limites conforme Eurocódigo 2
                double comprimentoMinimo = 30 * diametro; // 30φ mínimo
                double comprimentoMaximo = 100 * diametro; // 100φ máximo prático

                return comprimentoAmarracao >= comprimentoMinimo &&
                       comprimentoAmarracao <= comprimentoMaximo;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Calcula comprimento de desenvolvimento conforme Eurocódigo
        /// </summary>
        public double CalcularComprimentoDesenvolvimento(double diametro, double fcm, double fyk,
                                                        bool condicoesBoas = true)
        {
            try
            {
                // Cálculo simplificado baseado no Eurocódigo 2
                // lb,rqd = (φ/4) * (fyd/fbd)

                double fyd = fyk / 1.15; // Tensão de cálculo do aço
                double fctm = 0.3 * Math.Pow(fcm, 2.0 / 3.0); // Resistência à tração média do betão

                // Tensão de aderência de cálculo
                double fbd = 2.25 * fctm;
                if (!condicoesBoas) fbd *= 0.7; // Redução para condições de aderência desfavoráveis

                double lbRqd = (diametro / 4.0) * (fyd / fbd);

                // Aplicar fatores de segurança e limites mínimos
                double lbMin = Math.Max(0.3 * lbRqd, 10 * diametro);
                lbMin = Math.Max(lbMin, 100); // Mínimo absoluto de 100mm

                return lbMin;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro no cálculo de comprimento de desenvolvimento: {ex.Message}");
            }
        }

        /// <summary>
        /// Determina o fator de amarração baseado na posição e condições
        /// </summary>
        public double ObterFatorAmarracao(TipoAmarracaoEnum tipoAmarracao, bool zonaCompressao = false,
                                        bool confinamentoTransversal = false)
        {
            try
            {
                double fator = 1.0;

                // Fator baseado no tipo de amarração
                switch (tipoAmarracao)
                {
                    case TipoAmarracaoEnum.Reta:
                        fator = 1.0;
                        break;
                    case TipoAmarracaoEnum.Dobrada90:
                        fator = 0.7; // Redução devido à dobra
                        break;
                    case TipoAmarracaoEnum.Gancho135:
                        fator = 0.8; // Redução devido ao gancho
                        break;
                }

                // Ajustes baseados nas condições
                if (zonaCompressao)
                {
                    fator *= 0.75; // Redução em zona de compressão
                }

                if (confinamentoTransversal)
                {
                    fator *= 0.85; // Redução com confinamento adequado
                }

                return Math.Max(fator, 0.5); // Mínimo de 50% do comprimento base
            }
            catch
            {
                return 1.0; // Valor seguro
            }
        }

        /// <summary>
        /// Cria curvas Revit a partir dos pontos de amarração
        /// </summary>
        public List<Curve> CriarCurvasAmarracao(List<XYZ> pontos)
        {
            List<Curve> curvas = new List<Curve>();

            if (pontos == null || pontos.Count < 2)
            {
                throw new ArgumentException("É necessário pelo menos 2 pontos para criar curvas");
            }

            try
            {
                for (int i = 0; i < pontos.Count - 1; i++)
                {
                    XYZ ponto1 = pontos[i];
                    XYZ ponto2 = pontos[i + 1];

                    // Verificar se os pontos são diferentes
                    if (ponto1.DistanceTo(ponto2) > 1e-6) // Tolerância mínima
                    {
                        Line linha = Line.CreateBound(ponto1, ponto2);
                        curvas.Add(linha);
                    }
                }

                if (curvas.Count == 0)
                {
                    throw new Exception("Nenhuma curva válida foi criada a partir dos pontos");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro na criação de curvas: {ex.Message}");
            }

            return curvas;
        }

        /// <summary>
        /// Valida geometria de amarração antes da criação
        /// </summary>
        public bool ValidarGeometriaAmarracao(List<XYZ> pontos, double tolerancia = 1e-3)
        {
            if (pontos == null || pontos.Count < 2)
                return false;

            try
            {
                // Verificar se há pontos coincidentes consecutivos
                for (int i = 0; i < pontos.Count - 1; i++)
                {
                    if (pontos[i].DistanceTo(pontos[i + 1]) < tolerancia)
                    {
                        return false; // Pontos muito próximos
                    }
                }

                // Verificar se há auto-intersecções óbvias
                // (implementação simplificada)

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Otimiza pontos de amarração para evitar problemas geométricos
        /// </summary>
        public List<XYZ> OtimizarPontosAmarracao(List<XYZ> pontosOriginais, double toleranciaMinima = 0.01)
        {
            if (pontosOriginais == null || pontosOriginais.Count == 0)
                return pontosOriginais;

            List<XYZ> pontosOtimizados = new List<XYZ>();

            try
            {
                // Adicionar primeiro ponto
                pontosOtimizados.Add(pontosOriginais[0]);

                // Filtrar pontos muito próximos
                for (int i = 1; i < pontosOriginais.Count; i++)
                {
                    XYZ pontoAtual = pontosOriginais[i];
                    XYZ ultimoPonto = pontosOtimizados[pontosOtimizados.Count - 1];

                    if (pontoAtual.DistanceTo(ultimoPonto) >= toleranciaMinima / 0.3048) // Converter para pés
                    {
                        pontosOtimizados.Add(pontoAtual);
                    }
                }

                // Garantir pelo menos 2 pontos
                if (pontosOtimizados.Count < 2 && pontosOriginais.Count >= 2)
                {
                    pontosOtimizados.Clear();
                    pontosOtimizados.Add(pontosOriginais[0]);
                    pontosOtimizados.Add(pontosOriginais[pontosOriginais.Count - 1]);
                }
            }
            catch
            {
                // Em caso de erro, retornar pontos originais
                return pontosOriginais;
            }

            return pontosOtimizados;
        }
    }

    /// <summary>
    /// Enumeração para tipos de amarração expandida
    /// </summary>
    public enum TipoAmarracaoEnum
    {
        Automatico,    // Determinação automática baseada na posição
        Reta,          // Amarração reta simples
        Dobrada90,     // Dobra a 90° (fundação/último piso)
        Gancho135      // Gancho a 135° (situações especiais)
    }
}