using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

namespace MacroArmaduraAvancado
{
    /// <summary>
    /// Configuração e execução de armadura para Pilares e Vigas
    /// </summary>
    public class ArmConfigExec
    {
        // Configuração principal
        public List<ArmVar> Varoes { get; set; } = new List<ArmVar>();
        public List<ArmStirrup> Estribos { get; set; } = new List<ArmStirrup>();
        public TipoDistribuicaoArmaduraEnum TipoDistribuicao { get; set; } = TipoDistribuicaoArmaduraEnum.MistaComMaioresNasBordas;
        public double ComprimentoBase { get; set; } = 3000; // mm
        public bool ComprimentoAuto { get; set; } = true;
        public bool AmarracaoAuto { get; set; } = true;
        public double MultAmarracao { get; set; } = 70;
        public string TipoAmarracao { get; set; } = "Automático";
        public bool DeteccaoAmarracaoAuto { get; set; } = true;
        public DefinicoesProjectoAvancadas Defs { get; set; }

        private Document doc;
        private CalculadorAmarracao calcAmarracao;
        private DetectorElementosAvancado detector;

        public ArmConfigExec(Document documento)
        {
            doc = documento;
            calcAmarracao = new CalculadorAmarracao();
            detector = new DetectorElementosAvancado(documento);
            Defs = new DefinicoesProjectoAvancadas();
        }

        public double CalcCompTotal(double alturaElemento)
        {
            double alturaFinal = ComprimentoAuto ? alturaElemento : ComprimentoBase;
            if (AmarracaoAuto && Varoes.Count > 0)
            {
                double maiorDiam = Varoes.Max(v => v.Diametro);
                double compAmarracao = (MultAmarracao * maiorDiam);
                return alturaFinal + (2 * compAmarracao);
            }
            return alturaFinal;
        }

        public int QtdTotalVaroes() => Varoes.Sum(v => v.Quantidade);

        public bool ColocarArmadura(Element elemento)
        {
            try
            {
                switch (TipoElemento)
                {
                    case TipoElementoEstruturalEnum.Pilares:
                        return ColocarArmaduraPilar(elemento);

                    case TipoElementoEstruturalEnum.Vigas:
                        return ColocarArmaduraViga(elemento);

                    default:
                        return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro na colocação de armadura: {ex.Message}");
            }
        }

        private bool ColocarArmaduraPilar(Element pilar)
        {
            FamilyInstance inst = pilar as FamilyInstance;
            if (inst == null) return false;

            try
            {
                // Obter propriedades do pilar
                double altura = ObterAlturaElemento(inst);
                double largura = inst.get_Parameter(BuiltInParameter.FAMILY_WIDTH_PARAM)?.AsDouble() ?? 0;
                double profundidade = inst.get_Parameter(BuiltInParameter.FAMILY_HEIGHT_PARAM)?.AsDouble() ?? 0;

                if (altura <= 0 || largura <= 0 || profundidade <= 0)
                    return false;

                double cobertura = (Defs?.CoberturaPilares ?? 40) / 304.8; // Converter mm para pés
                double larguraUtil = largura - 2 * cobertura;
                double profundidadeUtil = profundidade - 2 * cobertura;

                // Determinar tipo de amarração
                TipoAmarracaoEnum tipoAmarracao = TipoAmarracaoEnum.Reta;
                if (DeteccaoAmarracaoAuto)
                {
                    tipoAmarracao = detector.DeterminarTipoAmarracaoAutomatico(pilar);
                }

                // Calcular posições dos varões
                List<PosicaoVarao> posicoes = CalcularDistribuicaoVaroes(larguraUtil, profundidadeUtil);

                // Criar armaduras longitudinais
                bool sucessoLongitudinal = CriarArmadurasLongitudinais(inst, posicoes, altura, tipoAmarracao);

                // Criar estribos
                bool sucessoEstribos = CriarEstribos(inst, altura, largura, profundidade, cobertura);

                return sucessoLongitudinal && sucessoEstribos;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro no pilar {pilar.Id}: {ex.Message}");
            }
        }

        private bool ColocarArmaduraViga(Element viga)
        {
            FamilyInstance inst = viga as FamilyInstance;
            if (inst == null) return false;

            try
            {
                // Obter propriedades da viga
                double comprimento = ObterComprimentoViga(inst);
                double altura = inst.get_Parameter(BuiltInParameter.FAMILY_HEIGHT_PARAM)?.AsDouble() ?? 0;
                double largura = inst.get_Parameter(BuiltInParameter.FAMILY_WIDTH_PARAM)?.AsDouble() ?? 0;

                if (comprimento <= 0 || altura <= 0 || largura <= 0)
                    return false;

                double cobertura = (Defs?.CoberturaVigas ?? 25) / 304.8; // Converter mm para pés

                // Criar armadura superior e inferior
                bool sucessoLongitudinal = CriarArmaduraVigaLongitudinal(inst, comprimento, altura, cobertura);

                // Criar estribos da viga
                bool sucessoEstribos = CriarEstribosViga(inst, comprimento, altura, largura, cobertura);

                return sucessoLongitudinal && sucessoEstribos;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro na viga {viga.Id}: {ex.Message}");
            }
        }

        private List<PosicaoVarao> CalcularDistribuicaoVaroes(double largura, double prof)
        {
            List<PosicaoVarao> posicoes = new List<PosicaoVarao>();
            int totalVaroes = QtdTotalVaroes();

            switch (TipoDistribuicao)
            {
                case TipoDistribuicaoArmaduraEnum.MistaComMaioresNasBordas:
                    posicoes = DistribuirMistaComMaioresNasBordas(largura, prof, totalVaroes);
                    break;

                case TipoDistribuicaoArmaduraEnum.ConcentradaNasBordas:
                    posicoes = DistribuirConcentradaNasBordas(largura, prof, totalVaroes);
                    break;

                case TipoDistribuicaoArmaduraEnum.Uniforme:
                default:
                    posicoes = DistribuirUniforme(largura, prof, totalVaroes);
                    break;
            }

            return posicoes;
        }

        private List<PosicaoVarao> DistribuirMistaComMaioresNasBordas(double largura, double prof, int total)
        {
            List<PosicaoVarao> posicoes = new List<PosicaoVarao>();

            // Sempre 4 nos cantos (mínimo para pilares)
            posicoes.Add(new PosicaoVarao(new XYZ(-largura / 2, -prof / 2, 0), TipoPosicaoVarao.Canto));
            posicoes.Add(new PosicaoVarao(new XYZ(largura / 2, -prof / 2, 0), TipoPosicaoVarao.Canto));
            posicoes.Add(new PosicaoVarao(new XYZ(largura / 2, prof / 2, 0), TipoPosicaoVarao.Canto));
            posicoes.Add(new PosicaoVarao(new XYZ(-largura / 2, prof / 2, 0), TipoPosicaoVarao.Canto));

            // Distribuir restantes nas faces se necessário
            int restantes = total - 4;
            if (restantes > 0)
            {
                int porFace = restantes / 4;
                for (int i = 1; i <= porFace; i++)
                {
                    double x = -largura / 2 + (i * largura / (porFace + 1));
                    posicoes.Add(new PosicaoVarao(new XYZ(x, -prof / 2, 0), TipoPosicaoVarao.Face));
                }
                // Adicionar nas outras faces conforme necessário...
            }

            return posicoes.Take(total).ToList();
        }

        private List<PosicaoVarao> DistribuirConcentradaNasBordas(double largura, double prof, int total)
        {
            List<PosicaoVarao> posicoes = new List<PosicaoVarao>();

            double perimetro = 2 * (largura + prof);
            double espacamento = perimetro / total;

            for (int i = 0; i < total; i++)
            {
                double distancia = i * espacamento;
                XYZ posicao = CalcularPosicaoNoPerimetro(largura, prof, distancia);
                posicoes.Add(new PosicaoVarao(posicao, TipoPosicaoVarao.Face));
            }

            return posicoes;
        }

        private List<PosicaoVarao> DistribuirUniforme(double largura, double prof, int total)
        {
            List<PosicaoVarao> posicoes = new List<PosicaoVarao>();

            int filas = (int)Math.Ceiling(Math.Sqrt(total));
            int colunas = (int)Math.Ceiling((double)total / filas);

            double espX = largura / (colunas + 1);
            double espY = prof / (filas + 1);

            int contador = 0;
            for (int f = 0; f < filas && contador < total; f++)
            {
                for (int c = 0; c < colunas && contador < total; c++)
                {
                    double x = -largura / 2 + (c + 1) * espX;
                    double y = -prof / 2 + (f + 1) * espY;

                    posicoes.Add(new PosicaoVarao(new XYZ(x, y, 0), TipoPosicaoVarao.Interior));
                    contador++;
                }
            }

            return posicoes;
        }

        private XYZ CalcularPosicaoNoPerimetro(double largura, double prof, double distancia)
        {
            double perimetro = 2 * (largura + prof);
            distancia = distancia % perimetro;

            if (distancia <= largura)
                return new XYZ(-largura / 2 + distancia, -prof / 2, 0);
            else if (distancia <= largura + prof)
                return new XYZ(largura / 2, -prof / 2 + (distancia - largura), 0);
            else if (distancia <= 2 * largura + prof)
                return new XYZ(largura / 2 - (distancia - largura - prof), prof / 2, 0);
            else
                return new XYZ(-largura / 2, prof / 2 - (distancia - 2 * largura - prof), 0);
        }

        private bool CriarArmadurasLongitudinais(FamilyInstance elemento, List<PosicaoVarao> posicoes,
                                               double altura, TipoAmarracaoEnum tipoAmarracao)
        {
            try
            {
                var tiposArmadura = ObterTiposArmaduraDisponiveis();
                if (tiposArmadura.Count == 0) return false;

                int varaoIndex = 0;
                foreach (var varao in Varoes)
                {
                    var tipoVarao = tiposArmadura.FirstOrDefault(t =>
                        t.Name.Contains(varao.Diametro.ToString("F0"))) ?? tiposArmadura.First();

                    var posVarao = posicoes.Skip(varaoIndex).Take(varao.Quantidade).ToList();

                    foreach (var pos in posVarao)
                    {
                        if (!CriarArmaduraLongitudinalIndividual(elemento, pos.Posicao, varao.Diametro,
                                                               altura, tipoVarao, tipoAmarracao))
                        {
                            return false;
                        }
                    }

                    varaoIndex += varao.Quantidade;
                    if (varaoIndex >= posicoes.Count) break;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro na criação de armaduras longitudinais: {ex.Message}");
            }
        }

        private bool CriarArmaduraLongitudinalIndividual(FamilyInstance elemento, XYZ posicao, double diametro,
                                                       double altura, RebarBarType tipoArmadura, TipoAmarracaoEnum tipoAmarracao)
        {
            try
            {
                double comprimentoTotal = CalcCompTotal(altura * 304.8) / 304.8; // Converter para pés
                double comprimentoAmarracao = (MultAmarracao * diametro);

                XYZ pontoInicial = posicao;
                XYZ pontoFinal = new XYZ(posicao.X, posicao.Y, posicao.Z + altura);

                // Calcular pontos com amarração
                List<XYZ> pontosCompletos = calcAmarracao.CalcularPontosAncoragem(
                    pontoInicial, pontoFinal, tipoAmarracao, comprimentoAmarracao);

                // Criar curvas
                List<Curve> curvas = new List<Curve>();
                for (int i = 0; i < pontosCompletos.Count - 1; i++)
                {
                    if (pontosCompletos[i].DistanceTo(pontosCompletos[i + 1]) > 1e-6)
                    {
                        curvas.Add(Line.CreateBound(pontosCompletos[i], pontosCompletos[i + 1]));
                    }
                }

                if (curvas.Count == 0)
                    curvas.Add(Line.CreateBound(pontoInicial, pontoFinal));

                // Obter ganchos se necessário
                RebarHookType gancho = calcAmarracao.DeterminarTipoGancho(doc, tipoAmarracao);

                // Criar armadura
                Rebar armadura = Rebar.CreateFromCurves(
                    doc, RebarStyle.Standard, tipoArmadura,
                    gancho, gancho, elemento,
                    XYZ.BasisZ, curvas,
                    RebarHookOrientation.Right, RebarHookOrientation.Right,
                    true, true);

                if (armadura != null)
                {
                    // Definir comentário
                    string comentario = $"Ø{diametro}mm - Amarração: {MultAmarracao}? - {tipoAmarracao}";
                    var paramComentario = armadura.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS);
                    paramComentario?.Set(comentario);
                }

                return armadura != null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro na criação de armadura Ø{diametro}mm: {ex.Message}");
            }
        }

        private bool CriarEstribos(FamilyInstance elemento, double altura, double largura,
                                 double profundidade, double cobertura)
        {
            try
            {
                foreach (var estribo in Estribos)
                {
                    var tipoEstribo = ObterTipoArmaduraPorDiametro(estribo.Diametro);
                    if (tipoEstribo == null) continue;

                    double espacamento = estribo.Espacamento / 304.8; // mm para pés
                    int numeroEstribos = (int)(altura / espacamento);

                    for (int i = 1; i < numeroEstribos; i++) // Evitar base e topo
                    {
                        double cota = i * espacamento;

                        if (estribo.Alternado && i % 2 == 0)
                        {
                            if (!CriarEstriboAlternado(elemento, cota, largura, profundidade, cobertura, tipoEstribo))
                                return false;
                        }
                        else
                        {
                            if (!CriarEstriboNormal(elemento, cota, largura, profundidade, cobertura, tipoEstribo))
                                return false;
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

        private bool CriarEstriboNormal(FamilyInstance elemento, double cota, double largura,
                                      double profundidade, double cobertura, RebarBarType tipoEstribo)
        {
            try
            {
                double x1 = -largura / 2 + cobertura;
                double x2 = largura / 2 - cobertura;
                double y1 = -profundidade / 2 + cobertura;
                double y2 = profundidade / 2 - cobertura;

                List<Curve> curvasEstribo = new List<Curve>
                {
                    Line.CreateBound(new XYZ(x1, y1, cota), new XYZ(x2, y1, cota)),
                    Line.CreateBound(new XYZ(x2, y1, cota), new XYZ(x2, y2, cota)),
                    Line.CreateBound(new XYZ(x2, y2, cota), new XYZ(x1, y2, cota)),
                    Line.CreateBound(new XYZ(x1, y2, cota), new XYZ(x1, y1, cota))
                };

                var ganchos = new FilteredElementCollector(doc)
                    .OfClass(typeof(RebarHookType))
                    .Cast<RebarHookType>();

                RebarHookType ganchoEstribo = ganchos.FirstOrDefault(g => g.Name.Contains("90")) ??
                                            ganchos.FirstOrDefault();

                Rebar estribo = Rebar.CreateFromCurves(
                    doc, RebarStyle.StirrupTie, tipoEstribo,
                    ganchoEstribo, ganchoEstribo, elemento,
                    XYZ.BasisZ, curvasEstribo,
                    RebarHookOrientation.Right, RebarHookOrientation.Right,
                    true, true);

                return estribo != null;
            }
            catch
            {
                return false;
            }
        }

        private bool CriarEstriboAlternado(FamilyInstance elemento, double cota, double largura,
                                         double profundidade, double cobertura, RebarBarType tipoEstribo)
        {
            try
            {
                double deslocamento = cobertura * 0.25;

                double x1 = -largura / 2 + cobertura + deslocamento;
                double x2 = largura / 2 - cobertura + deslocamento;
                double y1 = -profundidade / 2 + cobertura + deslocamento;
                double y2 = profundidade / 2 - cobertura + deslocamento;

                List<Curve> curvasEstribo = new List<Curve>
                {
                    Line.CreateBound(new XYZ(x1, y1, cota), new XYZ(x2, y1, cota)),
                    Line.CreateBound(new XYZ(x2, y1, cota), new XYZ(x2, y2, cota)),
                    Line.CreateBound(new XYZ(x2, y2, cota), new XYZ(x1, y2, cota)),
                    Line.CreateBound(new XYZ(x1, y2, cota), new XYZ(x1, y1, cota))
                };

                var ganchos = new FilteredElementCollector(doc)
                    .OfClass(typeof(RebarHookType))
                    .Cast<RebarHookType>();

                RebarHookType ganchoEstribo = ganchos.FirstOrDefault(g => g.Name.Contains("90")) ??
                                            ganchos.FirstOrDefault();

                Rebar estribo = Rebar.CreateFromCurves(
                    doc, RebarStyle.StirrupTie, tipoEstribo,
                    ganchoEstribo, ganchoEstribo, elemento,
                    XYZ.BasisZ, curvasEstribo,
                    RebarHookOrientation.Left, RebarHookOrientation.Left,
                    true, true);

                return estribo != null;
            }
            catch
            {
                return false;
            }
        }

        private bool CriarArmaduraVigaLongitudinal(FamilyInstance elemento, double comprimento,
                                                 double altura, double cobertura)
        {
            try
            {
                var tiposArmadura = ObterTiposArmaduraDisponiveis();
                if (tiposArmadura.Count == 0) return false;

                foreach (var varao in Varoes)
                {
                    var tipoVarao = tiposArmadura.FirstOrDefault(t =>
                        t.Name.Contains(varao.Diametro.ToString("F0"))) ?? tiposArmadura.First();

                    // Armadura superior
                    for (int i = 0; i < varao.Quantidade / 2; i++)
                    {
                        double y = -0.1 + (i * 0.05); // Espaçamento pequeno
                        XYZ pontoInicial = new XYZ(0, y, altura - cobertura);
                        XYZ pontoFinal = new XYZ(comprimento, y, altura - cobertura);

                        List<Curve> curvas = new List<Curve>
                        {
                            Line.CreateBound(pontoInicial, pontoFinal)
                        };

                        Rebar armadura = Rebar.CreateFromCurves(
                            doc, RebarStyle.Standard, tipoVarao,
                            null, null, elemento,
                            XYZ.BasisX, curvas,
                            RebarHookOrientation.Right, RebarHookOrientation.Right,
                            true, true);

                        if (armadura == null) return false;
                    }

                    // Armadura inferior
                    for (int i = 0; i < (varao.Quantidade + 1) / 2; i++)
                    {
                        double y = -0.1 + (i * 0.05); // Espaçamento pequeno
                        XYZ pontoInicial = new XYZ(0, y, cobertura);
                        XYZ pontoFinal = new XYZ(comprimento, y, cobertura);

                        List<Curve> curvas = new List<Curve>
                        {
                            Line.CreateBound(pontoInicial, pontoFinal)
                        };

                        Rebar armadura = Rebar.CreateFromCurves(
                            doc, RebarStyle.Standard, tipoVarao,
                            null, null, elemento,
                            XYZ.BasisX, curvas,
                            RebarHookOrientation.Right, RebarHookOrientation.Right,
                            true, true);

                        if (armadura == null) return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro na criação de armadura longitudinal da viga: {ex.Message}");
            }
        }

        private bool CriarEstribosViga(FamilyInstance elemento, double comprimento, double altura,
                                     double largura, double cobertura)
        {
            try
            {
                foreach (var estribo in Estribos)
                {
                    var tipoEstribo = ObterTipoArmaduraPorDiametro(estribo.Diametro);
                    if (tipoEstribo == null) continue;

                    double espacamento = estribo.Espacamento / 304.8; // mm para pés
                    int numeroEstribos = (int)(comprimento / espacamento);

                    for (int i = 1; i < numeroEstribos; i++)
                    {
                        double posicaoX = i * espacamento;

                        // Criar estribo vertical retangular
                        double y1 = -largura / 2 + cobertura;
                        double y2 = largura / 2 - cobertura;
                        double z1 = cobertura;
                        double z2 = altura - cobertura;

                        List<Curve> curvasEstribo = new List<Curve>
                        {
                            Line.CreateBound(new XYZ(posicaoX, y1, z1), new XYZ(posicaoX, y2, z1)),
                            Line.CreateBound(new XYZ(posicaoX, y2, z1), new XYZ(posicaoX, y2, z2)),
                            Line.CreateBound(new XYZ(posicaoX, y2, z2), new XYZ(posicaoX, y1, z2)),
                            Line.CreateBound(new XYZ(posicaoX, y1, z2), new XYZ(posicaoX, y1, z1))
                        };

                        var ganchos = new FilteredElementCollector(doc)
                            .OfClass(typeof(RebarHookType))
                            .Cast<RebarHookType>();

                        RebarHookType ganchoEstribo = ganchos.FirstOrDefault(g => g.Name.Contains("90")) ??
                                                    ganchos.FirstOrDefault();

                        Rebar estriboCriado = Rebar.CreateFromCurves(
                            doc, RebarStyle.StirrupTie, tipoEstribo,
                            ganchoEstribo, ganchoEstribo, elemento,
                            XYZ.BasisX, curvasEstribo,
                            RebarHookOrientation.Right, RebarHookOrientation.Right,
                            true, true);

                        if (estriboCriado == null) return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro na criação de estribos da viga: {ex.Message}");
            }
        }

        // Métodos auxiliares
        private double ObterAlturaElemento(FamilyInstance inst)
        {
            var paramAltura = inst.get_Parameter(BuiltInParameter.FAMILY_HEIGHT_PARAM) ??
                             inst.get_Parameter(BuiltInParameter.INSTANCE_LENGTH_PARAM);

            return paramAltura?.AsDouble() ?? (3000.0 / 304.8);
        }

        private double ObterComprimentoViga(FamilyInstance inst)
        {
            var paramComprimento = inst.get_Parameter(BuiltInParameter.INSTANCE_LENGTH_PARAM);
            return paramComprimento?.AsDouble() ?? 0;
        }

        private List<RebarBarType> ObterTiposArmaduraDisponiveis()
        {
            var collector = new FilteredElementCollector(doc);
            return collector.OfClass(typeof(RebarBarType))
                          .Cast<RebarBarType>()
                          .OrderBy(t => t.Name)
                          .ToList();
        }

        private RebarBarType ObterTipoArmaduraPorDiametro(double diametro)
        {
            var tipos = ObterTiposArmaduraDisponiveis();
            string diametroStr = diametro.ToString("F0");

            return tipos.FirstOrDefault(t => t.Name.Contains(diametroStr)) ??
                   tipos.FirstOrDefault();
        }

        /// <summary>
        /// Gera relatório de pré-visualização
        /// </summary>
        public string GerarRelatorioPreVisualizacao(List<Element> elementos)
        {
            var relatorio = new System.Text.StringBuilder();

            relatorio.AppendLine("=== RELATÓRIO DE PRÉ-VISUALIZAÇÃO ===");
            relatorio.AppendLine($"Data/Hora: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
            relatorio.AppendLine($"Tipo de Elemento: {TipoElemento}");
            relatorio.AppendLine($"Total de Elementos: {elementos.Count}");
            relatorio.AppendLine();

            // Análise de elementos
            if (elementos.Count > 0)
            {
                var analise = detector.AnalisarElementos(elementos);
                relatorio.AppendLine("ANÁLISE DE POSICIONAMENTO:");
                relatorio.AppendLine($"- Fundação: {analise.ElementosFundacao} elementos");
                relatorio.AppendLine($"- Último piso: {analise.ElementosUltimoPiso} elementos");
                relatorio.AppendLine($"- Intermédios: {analise.ElementosIntermedios} elementos");
                relatorio.AppendLine();
            }

            // Configuração de armadura
            relatorio.AppendLine("CONFIGURAÇÃO DE ARMADURA LONGITUDINAL:");
            relatorio.AppendLine($"- Distribuição: {TipoDistribuicao}");
            relatorio.AppendLine($"- Quantidade total de varões: {QtdTotalVaroes()}");

            foreach (var varao in Varoes)
            {
                relatorio.AppendLine($"  * {varao.Quantidade}Ø{varao.Diametro}mm ({varao.TipoArmadura})");
            }

            relatorio.AppendLine($"- Comprimento automático: {(ComprimentoAuto ? "Sim" : "Não")}");
            if (!ComprimentoAuto)
            {
                relatorio.AppendLine($"- Comprimento base: {ComprimentoBase / 1000:F2}m");
            }

            if (AmarracaoAuto)
            {
                relatorio.AppendLine($"- Amarração: {MultAmarracao}? (automática por posição)");
            }
            relatorio.AppendLine();

            // Configuração de estribos
            relatorio.AppendLine("CONFIGURAÇÃO DE ESTRIBOS:");
            foreach (var estribo in Estribos)
            {
                string alternado = estribo.Alternado ? " (Alternado)" : "";
                relatorio.AppendLine($"- Ø{estribo.Diametro}mm // {estribo.Espacamento}mm{alternado}");
            }
            relatorio.AppendLine();

            // Cálculos estimativos
            if (elementos.Count > 0)
            {
                relatorio.AppendLine("CÁLCULOS ESTIMATIVOS:");

                double alturaMedia = 3000; // mm
                double comprimentoMedioVarao = CalcCompTotal(alturaMedia);
                double totalMetrosLongitudinal = 0;

                foreach (var varao in Varoes)
                {
                    double metros = (elementos.Count * varao.Quantidade * comprimentoMedioVarao) / 1000.0;
                    totalMetrosLongitudinal += metros;
                    relatorio.AppendLine($"- {varao.Quantidade}Ø{varao.Diametro}: ~{metros:F1}m");
                }

                relatorio.AppendLine($"- Total armadura longitudinal: ~{totalMetrosLongitudinal:F1}m");

                // Estribos
                double totalMetrosEstribos = 0;
                foreach (var estribo in Estribos)
                {
                    double perimetroMedio = 1.2; // m, estimativa
                    int estribosporElemento = (int)(alturaMedia / estribo.Espacamento);
                    double metrosEstribo = elementos.Count * estribosporElemento * perimetroMedio;
                    totalMetrosEstribos += metrosEstribo;
                    relatorio.AppendLine($"- Ø{estribo.Diametro}mm: ~{metrosEstribo:F1}m ({elementos.Count * estribosporElemento} unidades)");
                }

                relatorio.AppendLine($"- Total estribos: ~{totalMetrosEstribos:F1}m");
                relatorio.AppendLine();
                relatorio.AppendLine($"TOTAL GERAL: ~{totalMetrosLongitudinal + totalMetrosEstribos:F1}m de armadura");
            }

            // Definições aplicadas
            if (Defs != null)
            {
                relatorio.AppendLine();
                relatorio.AppendLine("DEFINIÇÕES APLICADAS:");
                relatorio.AppendLine($"- Cobertura pilares: {Defs.CoberturaPilares}mm");
                relatorio.AppendLine($"- Cobertura vigas: {Defs.CoberturaVigas}mm");
                relatorio.AppendLine($"- Validação Eurocódigo: {(Defs.ValidarEurocodigo ? "Ativada" : "Desativada")}");
            }

            return relatorio.ToString();
        }
    }

    /// <summary>
    /// Classe para representar um varão individual
    /// </summary>
    public class ArmVar
    {
        public int Quantidade { get; set; }
        public double Diametro { get; set; }
        public string TipoArmadura { get; set; } = "Principal";

        public ArmVar(int quantidade, double diametro)
        {
            Quantidade = quantidade;
            Diametro = diametro;
        }

        public override string ToString() => $"{Quantidade}Ø{Diametro}mm";
    }

    /// <summary>
    /// Classe para representar configuração de estribos
    /// </summary>
    public class ArmStirrup
    {
        public double Diametro { get; set; }
        public double Espacamento { get; set; }
        public bool Alternado { get; set; } = false;

        public ArmStirrup(double diametro, double espacamento)
        {
            Diametro = diametro;
            Espacamento = espacamento;
        }

        public override string ToString() =>
            $"Ø{Diametro}mm//{Espacamento}mm{(Alternado ? " Alt." : "")}";
    }

    /// <summary>
    /// Posição de varão com tipo de localização
    /// </summary>
    public class PosicaoVarao
    {
        public XYZ Posicao { get; set; }
        public TipoPosicaoVarao Tipo { get; set; }

        public PosicaoVarao(XYZ posicao, TipoPosicaoVarao tipo)
        {
            Posicao = posicao;
            Tipo = tipo;
        }
    }

    /// <summary>
    /// Tipo de posição do varão
    /// </summary>
    public enum TipoPosicaoVarao
    {
        Canto,      // Nos cantos da seção
        Face,       // Nas faces da seção
        Interior    // No interior da seção
    }
}