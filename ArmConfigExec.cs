using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

namespace MacroArmaduraAvancado
{
    /// <summary>
    /// Configuração e execução de armadura especializada para Vigas
    /// </summary>
    public class ArmConfigExec
    {
        // Configuração principal
        public List<ArmVar> Varoes { get; set; } = new List<ArmVar>();
        public List<ArmStirrup> Estribos { get; set; } = new List<ArmStirrup>();
        public TipoDistribuicaoArmaduraEnum TipoDistribuicao { get; set; } = TipoDistribuicaoArmaduraEnum.MistaComMaioresNasBordas;
        public bool AmarracaoAuto { get; set; } = true;
        public double MultAmarracao { get; set; } = 50; // Padrão para vigas
        public string TipoAmarracao { get; set; } = "Automático";
        public TipoElementoEstruturalEnum TipoElemento { get; set; }
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

        public int QtdTotalVaroes() => Varoes.Sum(v => v.Quantidade);

        public bool ColocarArmadura(Element elemento)
        {
            try
            {
                if (TipoElemento == TipoElementoEstruturalEnum.Vigas)
                {
                    return ColocarArmaduraViga(elemento);
                }
                
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro na colocação de armadura: {ex.Message}");
            }
        }

        private bool ColocarArmaduraViga(Element viga)
        {
            FamilyInstance inst = viga as FamilyInstance;
            if (inst == null) return false;

            try
            {
                // Obter propriedades da viga
                var propriedades = ObterPropriedadesViga(inst);
                if (propriedades == null) return false;

                double cobertura = (Defs?.CoberturaVigas ?? 25) / 304.8; // Converter mm para pés

                // Criar armadura longitudinal (superior, inferior, lateral)
                bool sucessoLongitudinal = CriarArmaduraVigaLongitudinal(inst, propriedades, cobertura);

                // Criar estribos da viga
                bool sucessoEstribos = CriarEstribosViga(inst, propriedades, cobertura);

                return sucessoLongitudinal && sucessoEstribos;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro na viga {viga.Id}: {ex.Message}");
            }
        }

        private PropriedadesViga ObterPropriedadesViga(FamilyInstance inst)
        {
            try
            {
                LocationCurve locCurve = inst.Location as LocationCurve;
                if (locCurve == null) return null;

                Curve curvaViga = locCurve.Curve;
                double comprimento = curvaViga.Length;
                
                double altura = inst.get_Parameter(BuiltInParameter.FAMILY_HEIGHT_PARAM)?.AsDouble() ?? 0;
                double largura = inst.get_Parameter(BuiltInParameter.FAMILY_WIDTH_PARAM)?.AsDouble() ?? 0;

                if (comprimento <= 0 || altura <= 0 || largura <= 0)
                    return null;

                return new PropriedadesViga
                {
                    Comprimento = comprimento,
                    Altura = altura,
                    Largura = largura,
                    CurvaEixo = curvaViga,
                    PontoInicial = curvaViga.GetEndPoint(0),
                    PontoFinal = curvaViga.GetEndPoint(1)
                };
            }
            catch
            {
                return null;
            }
        }

        private bool CriarArmaduraVigaLongitudinal(FamilyInstance elemento, PropriedadesViga props, double cobertura)
        {
            try
            {
                var tiposArmadura = ObterTiposArmaduraDisponiveis();
                if (tiposArmadura.Count == 0) return false;

                foreach (var varao in Varoes)
                {
                    var tipoVarao = tiposArmadura.FirstOrDefault(t =>
                        t.Name.Contains(varao.Diametro.ToString("F0"))) ?? tiposArmadura.First();

                    switch (varao.TipoArmadura.ToLower())
                    {
                        case "superior":
                            if (!CriarArmaduraSuperior(elemento, props, varao, tipoVarao, cobertura))
                                return false;
                            break;

                        case "inferior":
                            if (!CriarArmaduraInferior(elemento, props, varao, tipoVarao, cobertura))
                                return false;
                            break;

                        case "lateral":
                            if (!CriarArmaduraLateral(elemento, props, varao, tipoVarao, cobertura))
                                return false;
                            break;
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
                                         ArmVar varao, RebarBarType tipoVarao, double cobertura)
        {
            try
            {
                double espacamento = (props.Largura - 2 * cobertura) / (varao.Quantidade + 1);

                for (int i = 1; i <= varao.Quantidade; i++)
                {
                    double offsetY = -props.Largura / 2 + cobertura + (i * espacamento);
                    double alturaZ = props.Altura - cobertura;

                    XYZ pontoInicial = new XYZ(props.PontoInicial.X, props.PontoInicial.Y + offsetY, props.PontoInicial.Z + alturaZ);
                    XYZ pontoFinal = new XYZ(props.PontoFinal.X, props.PontoFinal.Y + offsetY, props.PontoFinal.Z + alturaZ);

                    // Calcular amarração
                    List<XYZ> pontosComAmarracao = CalcularPontosAmarracao(pontoInicial, pontoFinal, varao.Diametro, "superior");

                    if (!CriarArmaduraIndividual(elemento, pontosComAmarracao, tipoVarao, varao.Diametro))
                        return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool CriarArmaduraInferior(FamilyInstance elemento, PropriedadesViga props,
                                         ArmVar varao, RebarBarType tipoVarao, double cobertura)
        {
            try
            {
                double espacamento = (props.Largura - 2 * cobertura) / (varao.Quantidade + 1);

                for (int i = 1; i <= varao.Quantidade; i++)
                {
                    double offsetY = -props.Largura / 2 + cobertura + (i * espacamento);
                    double alturaZ = cobertura;

                    XYZ pontoInicial = new XYZ(props.PontoInicial.X, props.PontoInicial.Y + offsetY, props.PontoInicial.Z + alturaZ);
                    XYZ pontoFinal = new XYZ(props.PontoFinal.X, props.PontoFinal.Y + offsetY, props.PontoFinal.Z + alturaZ);

                    // Calcular amarração
                    List<XYZ> pontosComAmarracao = CalcularPontosAmarracao(pontoInicial, pontoFinal, varao.Diametro, "inferior");

                    if (!CriarArmaduraIndividual(elemento, pontosComAmarracao, tipoVarao, varao.Diametro))
                        return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool CriarArmaduraLateral(FamilyInstance elemento, PropriedadesViga props,
                                        ArmVar varao, RebarBarType tipoVarao, double cobertura)
        {
            try
            {
                double alturaUtil = props.Altura - 2 * cobertura;
                double espacamento = alturaUtil / (varao.Quantidade + 1);

                for (int i = 1; i <= varao.Quantidade; i++)
                {
                    double alturaZ = cobertura + (i * espacamento);

                    // Lado esquerdo
                    double offsetYEsq = -props.Largura / 2 + cobertura;
                    XYZ pontoInicialEsq = new XYZ(props.PontoInicial.X, props.PontoInicial.Y + offsetYEsq, props.PontoInicial.Z + alturaZ);
                    XYZ pontoFinalEsq = new XYZ(props.PontoFinal.X, props.PontoFinal.Y + offsetYEsq, props.PontoFinal.Z + alturaZ);

                    List<XYZ> pontosEsq = CalcularPontosAmarracao(pontoInicialEsq, pontoFinalEsq, varao.Diametro, "lateral");
                    if (!CriarArmaduraIndividual(elemento, pontosEsq, tipoVarao, varao.Diametro))
                        return false;

                    // Lado direito (se mais de 1 varão lateral)
                    if (varao.Quantidade > 1)
                    {
                        double offsetYDir = props.Largura / 2 - cobertura;
                        XYZ pontoInicialDir = new XYZ(props.PontoInicial.X, props.PontoInicial.Y + offsetYDir, props.PontoInicial.Z + alturaZ);
                        XYZ pontoFinalDir = new XYZ(props.PontoFinal.X, props.PontoFinal.Y + offsetYDir, props.PontoFinal.Z + alturaZ);

                        List<XYZ> pontosDir = CalcularPontosAmarracao(pontoInicialDir, pontoFinalDir, varao.Diametro, "lateral");
                        if (!CriarArmaduraIndividual(elemento, pontosDir, tipoVarao, varao.Diametro))
                            return false;
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private List<XYZ> CalcularPontosAmarracao(XYZ pontoInicial, XYZ pontoFinal, double diametro, string posicao)
        {
            List<XYZ> pontos = new List<XYZ>();

            if (AmarracaoAuto)
            {
                double comprimentoAmarracao = (MultAmarracao * diametro) / 304.8; // Converter mm para pés

                // Determinar tipo de amarração baseado na posição
                TipoAmarracaoEnum tipoAmarracao = TipoAmarracaoEnum.Reta;
                
                if (posicao == "superior")
                {
                    tipoAmarracao = TipoAmarracaoEnum.Dobrada90; // Ganchos na armadura superior
                }
                else if (posicao == "inferior")
                {
                    tipoAmarracao = TipoAmarracaoEnum.Reta; // Armadura inferior geralmente reta
                }

                pontos = calcAmarracao.CalcularPontosAncoragem(pontoInicial, pontoFinal, tipoAmarracao, comprimentoAmarracao);
            }
            else
            {
                pontos.Add(pontoInicial);
                pontos.Add(pontoFinal);
            }

            return pontos;
        }

        private bool CriarArmaduraIndividual(FamilyInstance elemento, List<XYZ> pontos, 
                                           RebarBarType tipoArmadura, double diametro)
        {
            try
            {
                // Criar curvas
                List<Curve> curvas = new List<Curve>();
                for (int i = 0; i < pontos.Count - 1; i++)
                {
                    if (pontos[i].DistanceTo(pontos[i + 1]) > 1e-6)
                    {
                        curvas.Add(Line.CreateBound(pontos[i], pontos[i + 1]));
                    }
                }

                if (curvas.Count == 0) return false;

                // Obter ganchos se necessário
                RebarHookType gancho = calcAmarracao.DeterminarTipoGancho(doc, TipoAmarracaoEnum.Dobrada90);

                // Criar armadura
                Rebar armadura = Rebar.CreateFromCurves(
                    doc, RebarStyle.Standard, tipoArmadura,
                    gancho, gancho, elemento,
                    XYZ.BasisX, curvas,
                    RebarHookOrientation.Right, RebarHookOrientation.Right,
                    true, true);

                if (armadura != null)
                {
                    // Definir comentário
                    string comentario = $"Ø{diametro}mm - Amarração: {MultAmarracao}?";
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

        private bool CriarEstribosViga(FamilyInstance elemento, PropriedadesViga props, double cobertura)
        {
            try
            {
                foreach (var estribo in Estribos)
                {
                    var tipoEstribo = ObterTipoArmaduraPorDiametro(estribo.Diametro);
                    if (tipoEstribo == null) continue;

                    double espacamento = estribo.Espacamento / 304.8; // mm para pés
                    int numeroEstribos = (int)(props.Comprimento / espacamento);

                    for (int i = 1; i < numeroEstribos; i++)
                    {
                        double posicaoX = i * espacamento;

                        // Calcular posição ao longo da viga
                        XYZ pontoEstribo = props.PontoInicial + (props.PontoFinal - props.PontoInicial).Normalize() * posicaoX;

                        if (!CriarEstriboVigaIndividual(elemento, pontoEstribo, props, tipoEstribo, cobertura))
                            return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro na criação de estribos: {ex.Message}");
            }
        }

        private bool CriarEstriboVigaIndividual(FamilyInstance elemento, XYZ posicao, 
                                              PropriedadesViga props, RebarBarType tipoEstribo, double cobertura)
        {
            try
            {
                // Criar estribo retangular vertical
                double y1 = -props.Largura / 2 + cobertura;
                double y2 = props.Largura / 2 - cobertura;
                double z1 = cobertura;
                double z2 = props.Altura - cobertura;

                List<Curve> curvasEstribo = new List<Curve>
                {
                    Line.CreateBound(new XYZ(posicao.X, posicao.Y + y1, posicao.Z + z1), 
                                   new XYZ(posicao.X, posicao.Y + y2, posicao.Z + z1)),
                    Line.CreateBound(new XYZ(posicao.X, posicao.Y + y2, posicao.Z + z1), 
                                   new XYZ(posicao.X, posicao.Y + y2, posicao.Z + z2)),
                    Line.CreateBound(new XYZ(posicao.X, posicao.Y + y2, posicao.Z + z2), 
                                   new XYZ(posicao.X, posicao.Y + y1, posicao.Z + z2)),
                    Line.CreateBound(new XYZ(posicao.X, posicao.Y + y1, posicao.Z + z2), 
                                   new XYZ(posicao.X, posicao.Y + y1, posicao.Z + z1))
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

                return estriboCriado != null;
            }
            catch
            {
                return false;
            }
        }

        // Métodos auxiliares
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
        /// Gera relatório de pré-visualização especializado para vigas
        /// </summary>
        public string GerarRelatorioPreVisualizacao(List<Element> elementos)
        {
            var relatorio = new System.Text.StringBuilder();

            relatorio.AppendLine("=== RELATÓRIO DE PRÉ-VISUALIZAÇÃO - VIGAS ===");
            relatorio.AppendLine($"Data/Hora: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
            relatorio.AppendLine($"Total de Vigas: {elementos.Count}");
            relatorio.AppendLine();

            // Configuração de armadura longitudinal
            relatorio.AppendLine("ARMADURA LONGITUDINAL:");
            relatorio.AppendLine($"- Quantidade total de varões: {QtdTotalVaroes()}");

            var armaduraSuperior = Varoes.Where(v => v.TipoArmadura.ToLower() == "superior").ToList();
            var armaduraInferior = Varoes.Where(v => v.TipoArmadura.ToLower() == "inferior").ToList();
            var armaduraLateral = Varoes.Where(v => v.TipoArmadura.ToLower() == "lateral").ToList();

            if (armaduraSuperior.Any())
            {
                relatorio.AppendLine("  SUPERIOR:");
                foreach (var varao in armaduraSuperior)
                    relatorio.AppendLine($"    * {varao.Quantidade}Ø{varao.Diametro}mm");
            }

            if (armaduraInferior.Any())
            {
                relatorio.AppendLine("  INFERIOR:");
                foreach (var varao in armaduraInferior)
                    relatorio.AppendLine($"    * {varao.Quantidade}Ø{varao.Diametro}mm");
            }

            if (armaduraLateral.Any())
            {
                relatorio.AppendLine("  LATERAL:");
                foreach (var varao in armaduraLateral)
                    relatorio.AppendLine($"    * {varao.Quantidade}Ø{varao.Diametro}mm");
            }

            if (AmarracaoAuto)
            {
                relatorio.AppendLine($"- Amarração: {MultAmarracao}? (automática por posição)");
                relatorio.AppendLine("  * Superior: Ganchos 90°");
                relatorio.AppendLine("  * Inferior: Reta");
                relatorio.AppendLine("  * Lateral: Reta");
            }
            relatorio.AppendLine();

            // Configuração de estribos
            relatorio.AppendLine("ESTRIBOS:");
            foreach (var estribo in Estribos)
            {
                string tipo = estribo.Alternado ? " (Alternado)" : " (Uniforme)";
                relatorio.AppendLine($"- Ø{estribo.Diametro}mm // {estribo.Espacamento}mm{tipo}");
            }
            relatorio.AppendLine();

            // Cálculos estimativos para vigas
            if (elementos.Count > 0)
            {
                relatorio.AppendLine("ESTIMATIVA DE MATERIAIS:");

                double comprimentoMedioViga = 4000; // mm estimativa
                double totalMetrosLongitudinal = 0;

                foreach (var varao in Varoes)
                {
                    double comprimentoComAmarracao = comprimentoMedioViga + (2 * MultAmarracao * varao.Diametro);
                    double metros = (elementos.Count * varao.Quantidade * comprimentoComAmarracao) / 1000.0;
                    totalMetrosLongitudinal += metros;
                    relatorio.AppendLine($"- {varao.TipoArmadura} Ø{varao.Diametro}: ~{metros:F1}m");
                }

                relatorio.AppendLine($"- Total armadura longitudinal: ~{totalMetrosLongitudinal:F1}m");

                // Estribos
                double totalMetrosEstribos = 0;
                foreach (var estribo in Estribos)
                {
                    double perimetroMedio = 1.0; // m, estimativa para estribos de viga
                    int estribosporViga = (int)(comprimentoMedioViga / estribo.Espacamento);
                    double metrosEstribo = elementos.Count * estribosporViga * perimetroMedio;
                    totalMetrosEstribos += metrosEstribo;
                    relatorio.AppendLine($"- Estribos Ø{estribo.Diametro}mm: ~{metrosEstribo:F1}m ({elementos.Count * estribosporViga} unidades)");
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
                relatorio.AppendLine($"- Cobertura vigas: {Defs.CoberturaVigas}mm");
                relatorio.AppendLine($"- Validação regulamentar: {(Defs.ValidarEurocodigo ? "Ativada" : "Desativada")}");
            }

            return relatorio.ToString();
        }
    }

    /// <summary>
    /// Classe para representar um varão individual para vigas
    /// </summary>
    public class ArmVar
    {
        public int Quantidade { get; set; }
        public double Diametro { get; set; }
        public string TipoArmadura { get; set; } = "Superior"; // Superior, Inferior, Lateral

        public ArmVar(int quantidade, double diametro)
        {
            Quantidade = quantidade;
            Diametro = diametro;
        }

        public override string ToString() => $"{Quantidade}Ø{Diametro}mm ({TipoArmadura})";
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
    /// Propriedades geométricas de uma viga
    /// </summary>
    public class PropriedadesViga
    {
        public double Comprimento { get; set; }
        public double Altura { get; set; }
        public double Largura { get; set; }
        public Curve CurvaEixo { get; set; }
        public XYZ PontoInicial { get; set; }
        public XYZ PontoFinal { get; set; }
    }
}