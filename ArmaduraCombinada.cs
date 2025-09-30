using System;
using System.Collections.Generic;
using System.Linq;

namespace Rebar_Revit
{
    /// <summary>
    /// Representa uma combinação de varões de diferentes diâmetros
    /// </summary>
    public class CombinacaoVaroes
    {
        public List<VaraoIndividual> Varoes { get; set; } = new List<VaraoIndividual>();
        public string TipoArmadura { get; set; } = "Superior"; // Superior, Inferior, Lateral
        public TipoDistribuicaoCombinada TipoDistribuicao { get; set; } = TipoDistribuicaoCombinada.MaioresNasExtremidades;

        /// <summary>
        /// Adiciona um tipo de varão à combinação
        /// </summary>
        public void AdicionarVarao(int quantidade, double diametro)
        {
            Varoes.Add(new VaraoIndividual { Quantidade = quantidade, Diametro = diametro });
        }

        /// <summary>
        /// Quantidade total de varões
        /// </summary>
        public int QuantidadeTotal => Varoes.Sum(v => v.Quantidade);

        /// <summary>
        /// Obtém a distribuição ordenada dos varões conforme o tipo especificado
        /// </summary>
        public List<double> ObterDistribuicaoOrdenada()
        {
            var resultado = new List<double>();

            if (!Varoes.Any()) return resultado;

            switch (TipoDistribuicao)
            {
                case TipoDistribuicaoCombinada.MaioresNasExtremidades:
                    resultado = DistribuirMaioresNasExtremidades();
                    break;

                case TipoDistribuicaoCombinada.IntercaladoRegular:
                    resultado = DistribuirIntercaladoRegular();
                    break;

                case TipoDistribuicaoCombinada.Uniforme:
                    resultado = DistribuirUniforme();
                    break;

                default:
                    resultado = DistribuirMaioresNasExtremidades();
                    break;
            }

            return resultado;
        }

        private List<double> DistribuirMaioresNasExtremidades()
        {
            var resultado = new List<double>();
            var varoesOrdenados = Varoes.OrderByDescending(v => v.Diametro).ToList();

            // Para superior/inferior: maiores nas extremidades
            if (TipoArmadura.ToLower() != "lateral")
            {
                if (!varoesOrdenados.Any()) return resultado;

                var maiores = varoesOrdenados.First();
                var menores = varoesOrdenados.Skip(1).ToList();

                int total = QuantidadeTotal;
                if (total <= 0) return resultado;

                double[] slots = new double[total];
                for (int i = 0; i < total; i++) slots[i] = double.NaN;

                int leftCount = (maiores.Quantidade + 1) / 2; // ceil
                int rightCount = maiores.Quantidade - leftCount;

                int leftIndex = 0;
                int rightIndex = total - 1;

                for (int i = 0; i < leftCount && leftIndex <= rightIndex; i++)
                {
                    slots[leftIndex++] = maiores.Diametro;
                }

                for (int i = 0; i < rightCount && leftIndex <= rightIndex; i++)
                {
                    slots[rightIndex--] = maiores.Diametro;
                }

                var meio = new List<double>();
                foreach (var menor in menores)
                {
                    for (int i = 0; i < menor.Quantidade; i++) meio.Add(menor.Diametro);
                }

                int posMeio = 0;
                for (int i = 0; i < total; i++)
                {
                    if (double.IsNaN(slots[i]))
                    {
                        if (posMeio < meio.Count)
                        {
                            slots[i] = meio[posMeio++];
                        }
                        else
                        {
                            slots[i] = maiores.Diametro; // fallback
                        }
                    }
                }

                resultado = slots.ToList();
            }
            else
            {
                // Para lateral: usar intercalado
                resultado = DistribuirIntercaladoRegular();
            }

            return resultado;
        }

        private List<double> DistribuirIntercaladoRegular()
        {
            var resultado = new List<double>();
            if (!Varoes.Any()) return resultado;

            var tiposVaroes = Varoes.ToList();
            int totalVaroes = QuantidadeTotal;

            for (int posicao = 0; posicao < totalVaroes; posicao++)
            {
                int tipoIndex = posicao % tiposVaroes.Count;
                resultado.Add(tiposVaroes[tipoIndex].Diametro);
            }

            return resultado;
        }

        private List<double> DistribuirUniforme()
        {
            var resultado = new List<double>();

            foreach (var varao in Varoes)
            {
                for (int i = 0; i < varao.Quantidade; i++)
                {
                    resultado.Add(varao.Diametro);
                }
            }

            return resultado.OrderByDescending(d => d).ToList();
        }

        public override string ToString()
        {
            if (!Varoes.Any()) return "Sem varões";
            var partes = Varoes.Select(v => $"{v.Quantidade}?{v.Diametro}").ToList();
            return string.Join(" + ", partes);
        }

        /// <summary>
        /// Parse de string no formato "2?20+1?16" para CombinacaoVaroes
        /// </summary>
        public static CombinacaoVaroes Parse(string texto, string tipoArmadura = "Superior")
        {
            var combinacao = new CombinacaoVaroes { TipoArmadura = tipoArmadura };
            if (string.IsNullOrWhiteSpace(texto)) return combinacao;

            try
            {
                var partes = texto.Split(new char[] { '+', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var parte in partes)
                {
                    var limpo = parte.Trim();
                    if (string.IsNullOrEmpty(limpo)) continue;

                    var separadores = new char[] { '?', 'f', 'F', '?' };
                    var indiceSeparador = -1;

                    foreach (var sep in separadores)
                    {
                        indiceSeparador = limpo.IndexOf(sep);
                        if (indiceSeparador >= 0) break;
                    }

                    if (indiceSeparador > 0)
                    {
                        var quantidadeStr = limpo.Substring(0, indiceSeparador);
                        var diametroStr = limpo.Substring(indiceSeparador + 1);

                        if (int.TryParse(quantidadeStr, out int quantidade) && 
                            double.TryParse(diametroStr, out double diametro))
                        {
                            combinacao.AdicionarVarao(quantidade, diametro);
                        }
                    }
                }
            }
            catch
            {
                // Retornar combinação vazia em caso de erro
            }

            return combinacao;
        }
    }

    /// <summary>
    /// Varão individual na combinação
    /// </summary>
    public class VaraoIndividual
    {
        public int Quantidade { get; set; }
        public double Diametro { get; set; }

        public override string ToString() => $"{Quantidade}?{Diametro}";
    }

    /// <summary>
    /// Tipos de distribuição para combinações de varões
    /// </summary>
    public enum TipoDistribuicaoCombinada
    {
        /// Maiores diâmetros nas extremidades
        MaioresNasExtremidades,
        /// Intercalação regular
        IntercaladoRegular,
        /// Agrupado/uniforme
        Uniforme
    }

    /// <summary>
    /// Representa uma combinação de estribos
    /// </summary>
    public class CombinacaoEstribos
    {
        public List<EstriboIndividual> Estribos { get; set; } = new List<EstriboIndividual>();
        public bool Intercalado { get; set; } = true;

        public void AdicionarEstribo(double diametro, double espacamento)
        {
            Estribos.Add(new EstriboIndividual { Diametro = diametro, Espacamento = espacamento });
        }

        public List<PosicaoEstribo> GerarSequenciaEstribos(double comprimentoViga)
        {
            var resultado = new List<PosicaoEstribo>();
            if (!Estribos.Any() || comprimentoViga <= 0) return resultado;

            if (Intercalado)
            {
                resultado = GerarSequenciaIntercalada(comprimentoViga);
            }
            else
            {
                resultado = GerarSequenciaAgrupada(comprimentoViga);
            }

            return resultado;
        }

        private List<PosicaoEstribo> GerarSequenciaIntercalada(double comprimentoViga)
        {
            var resultado = new List<PosicaoEstribo>();
            if (!Estribos.Any()) return resultado;

            double espacamentoBase = 0;
            if (Estribos.Count > 0 && Estribos[0].Espacamento > 0)
            {
                espacamentoBase = Estribos[0].Espacamento;
            }
            else
            {
                var positivos = Estribos.Select(e => e.Espacamento).Where(s => s > 0).ToList();
                if (positivos.Any()) espacamentoBase = positivos.Min();
                else
                {
                    int approx = Math.Max(1, (int)Math.Round(comprimentoViga / 250.0));
                    espacamentoBase = comprimentoViga / (approx + 1);
                }
            }

            if (espacamentoBase <= 0) return resultado;

            double posicaoAtual = espacamentoBase;
            int indiceEstribo = 0;

            while (posicaoAtual < comprimentoViga)
            {
                var estriboAtual = Estribos[indiceEstribo % Estribos.Count];

                resultado.Add(new PosicaoEstribo
                {
                    Posicao = posicaoAtual,
                    Diametro = estriboAtual.Diametro,
                    EspacamentoOriginal = estriboAtual.Espacamento
                });

                posicaoAtual += espacamentoBase;
                indiceEstribo++;
            }

            return resultado;
        }

        private List<PosicaoEstribo> GerarSequenciaAgrupada(double comprimentoViga)
        {
            var resultado = new List<PosicaoEstribo>();
            double posicaoAtual = 0;
            double comprimentoPorTipo = comprimentoViga / Estribos.Count;

            foreach (var estribo in Estribos)
            {
                double limiteSecao = posicaoAtual + comprimentoPorTipo;
                double p = posicaoAtual + estribo.Espacamento;
                while (p < limiteSecao && p < comprimentoViga)
                {
                    resultado.Add(new PosicaoEstribo
                    {
                        Posicao = p,
                        Diametro = estribo.Diametro,
                        EspacamentoOriginal = estribo.Espacamento
                    });

                    p += estribo.Espacamento;
                }

                posicaoAtual = limiteSecao;
            }

            return resultado;
        }

        public override string ToString()
        {
            if (!Estribos.Any()) return "Sem estribos";
            var partes = Estribos.Select(e => $"?{e.Diametro}//{e.Espacamento}").ToList();
            string tipo = Intercalado ? "intercalado" : "agrupado";
            return string.Join(" + ", partes) + $" ({tipo})";
        }

        public static CombinacaoEstribos Parse(string texto)
        {
            var combinacao = new CombinacaoEstribos();
            if (string.IsNullOrWhiteSpace(texto)) return combinacao;

            try
            {
                var partes = texto.Split(new char[] { '+' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var parte in partes)
                {
                    var limpo = parte.Trim();
                    if (string.IsNullOrEmpty(limpo)) continue;

                    var indiceSeparador = limpo.IndexOf("//");
                    if (indiceSeparador > 0)
                    {
                        var diametroParte = limpo.Substring(0, indiceSeparador);
                        var espacamentoParte = limpo.Substring(indiceSeparador + 2);

                        var diametroStr = diametroParte.Replace("F", "").Replace("f", "").Replace("?", "").Trim();

                        if (double.TryParse(diametroStr, out double diametro) &&
                            double.TryParse(espacamentoParte, out double espacamento))
                        {
                            combinacao.AdicionarEstribo(diametro, espacamento);
                        }
                    }
                }
            }
            catch
            {
            }

            return combinacao;
        }
    }

    public class EstriboIndividual
    {
        public double Diametro { get; set; }
        public double Espacamento { get; set; }

        public override string ToString() => $"?{Diametro}//{Espacamento}";
    }

    public class PosicaoEstribo
    {
        public double Posicao { get; set; } // mm
        public double Diametro { get; set; }
        public double EspacamentoOriginal { get; set; }
    }
}