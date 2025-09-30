using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using Application = System.Windows.Forms.Application;
using Control = System.Windows.Forms.Control;
using Form = System.Windows.Forms.Form;
using Rebar_Revit;

namespace Rebar_Revit
{
    public partial class FormularioPrincipal : Form
    {
        private Document doc;
        private UIDocument uidoc;
        private DetectorElementos detectorElementos;
        private Amarracao calculadorAmarracao;

        // Dados carregados
        private List<Element> vigasDisponiveis;
        private Dictionary<string, List<Element>> vigasAgrupadas;
        private List<string> gruposOrdenados;
        private Element vigaSelecionada;

        private ElementId lastAssumedElementId = null;

        private CreateArmaduraHandler createHandler;
        private ExternalEvent createEvent;

        public FormularioPrincipal(Document documento, UIDocument uiDocumento)
        {
            doc = documento;
            uidoc = uiDocumento;
            detectorElementos = new DetectorElementos(doc);
            calculadorAmarracao = new Amarracao();

            InitializeComponent();
            InicializarFormulario();
        }

        private void InicializarFormulario()
        {
            ConfigurarCombosDiametros();
            ConfigurarCombosDistribuicao();
            ConfigurarEstadosIniciais();
            ConectarEventHandlers();
            CarregarVigasDisponiveis();

            this.TopMost = false;
            this.ShowInTaskbar = true;
            this.StartPosition = FormStartPosition.CenterScreen;

            // ExternalEvent para executar transações no contexto Revit
            createHandler = new CreateArmaduraHandler();
            createEvent = ExternalEvent.Create(createHandler);

            createHandler.ProgressChanged += CreateHandler_ProgressChanged;
            createHandler.ExecutionCompleted += CreateHandler_ExecutionCompleted;
        }

        private void ConfigurarCombosDiametros()
        {
            var diametros = Uteis.ObterDiametrosPadrao();

            var todosCombos = new[] { 
                comboDiam1Superior, comboDiam2Superior,
                comboDiam1Inferior, comboDiam2Inferior,
                comboDiam1Lateral, comboDiam2Lateral,
                comboDiam1Estribo, comboDiam2Estribo
            };

            foreach (var combo in todosCombos)
            {
                combo.Items.Clear();
                foreach (var diametro in diametros)
                {
                    combo.Items.Add(diametro.ToString());
                }
            }

            // Valores padrão
            comboDiam1Superior.SelectedItem = "20";
            comboDiam2Superior.SelectedItem = "16";
            comboDiam1Inferior.SelectedItem = "25";
            comboDiam2Inferior.SelectedItem = "20";
            comboDiam1Lateral.SelectedItem = "16";
            comboDiam2Lateral.SelectedItem = "12";
            comboDiam1Estribo.SelectedItem = "10";
            comboDiam2Estribo.SelectedItem = "8";
        }

        private void ConfigurarCombosDistribuicao()
        {
            // Controis de distribuição não existentes no designer atual
        }

        private void ConfigurarEstadosIniciais()
        {
            // Inicializar estados dos controles conforme funcionalidade padrão
            checkCombinacaoSuperior.Checked = false;
            labelQuant1Superior.Enabled = true;
            numQuant1Superior.Enabled = true;
            labelDiam1Superior.Enabled = true;
            comboDiam1Superior.Enabled = true;
            labelQuant2Superior.Enabled = false;
            numQuant2Superior.Enabled = false;
            labelDiam2Superior.Enabled = false;
            comboDiam2Superior.Enabled = false;

            checkCombinacaoInferior.Checked = false;
            labelQuant1Inferior.Enabled = true;
            numQuant1Inferior.Enabled = true;
            labelDiam1Inferior.Enabled = true;
            comboDiam1Inferior.Enabled = true;
            labelQuant2Inferior.Enabled = false;
            numQuant2Inferior.Enabled = false;
            labelDiam2Inferior.Enabled = false;
            comboDiam2Inferior.Enabled = false;

            checkArmaduraLateral.Checked = false;
            checkCombinacaoLateral.Checked = false;
            checkCombinacaoLateral.Enabled = false;

            labelQuant1Lateral.Enabled = false;
            numQuant1Lateral.Enabled = false;
            labelDiam1Lateral.Enabled = false;
            comboDiam1Lateral.Enabled = false;
            labelQuant2Lateral.Enabled = false;
            numQuant2Lateral.Enabled = false;
            labelDiam2Lateral.Enabled = false;
            comboDiam2Lateral.Enabled = false;

            checkCombinacaoEstribos.Checked = false;
            labelDiam1Estribo.Enabled = true;
            comboDiam1Estribo.Enabled = true;
            labelEspac1Estribo.Enabled = true;
            numEspac1Estribo.Enabled = true;
            labelDiam2Estribo.Enabled = false;
            comboDiam2Estribo.Enabled = false;
        }

        private void DesabilitarControlesCombinacao()
        {
            // Mantido por compatibilidade
            labelQuant2Superior.Enabled = false;
            numQuant2Superior.Enabled = false;
            labelDiam2Superior.Enabled = false;
            comboDiam2Superior.Enabled = false;

            // Inferior
            labelQuant2Inferior.Enabled = false;
            numQuant2Inferior.Enabled = false;
            labelDiam2Inferior.Enabled = false;
            comboDiam2Inferior.Enabled = false;

            // Lateral
            labelQuant1Lateral.Enabled = false;
            numQuant1Lateral.Enabled = false;
            labelDiam1Lateral.Enabled = false;
            comboDiam1Lateral.Enabled = false;
            labelQuant2Lateral.Enabled = false;
            numQuant2Lateral.Enabled = false;
            labelDiam2Lateral.Enabled = false;
            comboDiam2Lateral.Enabled = false;

            // Estribos
            labelDiam1Estribo.Enabled = false;
            comboDiam1Estribo.Enabled = false;
            labelEspac1Estribo.Enabled = false;
            numEspac1Estribo.Enabled = false;
            labelDiam2Estribo.Enabled = false;
            comboDiam2Estribo.Enabled = false;
        }

        private void ConectarEventHandlers()
        {
            // Seleção de viga
            comboVigasDisponiveis.SelectedIndexChanged += ComboVigasDisponiveis_SelectedIndexChanged;
            radioFiltrarPorDescricao.CheckedChanged += RadioFiltro_CheckedChanged;
            radioFiltrarPorDesignacao.CheckedChanged += RadioFiltro_CheckedChanged;
            buttonAtualizarLista.Click += ButtonAtualizarLista_Click;

            // Armadura lateral
            checkArmaduraLateral.CheckedChanged += CheckArmaduraLateral_CheckedChanged;

            checkCombinacaoSuperior.CheckedChanged += CheckCombinacao_CheckedChanged;
            checkCombinacaoInferior.CheckedChanged += CheckCombinacao_CheckedChanged;
            checkCombinacaoLateral.CheckedChanged += CheckCombinacao_CheckedChanged;
            checkCombinacaoEstribos.CheckedChanged += CheckCombinacao_CheckedChanged;

            buttonExecutar.Click += ButtonExecutar_Click;
            buttonCancelar.Click += ButtonCancelar_Click;

            foreach (var control in new Control[] { numCobrimento, numMultAmarracao })
            {
                if (control is NumericUpDown num)
                {
                    num.ValueChanged += ParametroArmadura_Changed;
                }
            }

            foreach (var control in new Control[] { numQuant1Superior, numQuant2Superior,
                                                   numQuant1Inferior, numQuant2Inferior,
                                                   numQuant1Lateral, numQuant2Lateral,
                                                   numEspac1Estribo })
            {
                if (control is NumericUpDown num)
                {
                    num.ValueChanged += ParametroArmadura_Changed;
                }
            }

            foreach (var combo in new[] { comboDiam1Superior, comboDiam2Superior,
                                         comboDiam1Inferior, comboDiam2Inferior,
                                         comboDiam1Lateral, comboDiam2Lateral,
                                         comboDiam1Estribo, comboDiam2Estribo })
            {
                combo.SelectedIndexChanged += ParametroArmadura_Changed;
            }
        }

        #region Carregamento e Filtros de Vigas

        private void CarregarVigasDisponiveis()
        {
            try
            {
                vigasDisponiveis = Uteis.DetectarVigas(doc);
                vigasDisponiveis = Uteis.FiltrarVigasValidas(vigasDisponiveis);

                AtualizarListaVigas();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar vigas: {ex.Message}", "Erro",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AtualizarListaVigas()
        {
            try
            {
                comboVigasDisponiveis.Items.Clear();

                if (radioFiltrarPorDescricao.Checked)
                {
                    // Agrupa por Type (símbolo)
                    vigasAgrupadas = Uteis.AgruparVigasPorTipo(vigasDisponiveis, doc);
                }
                else if (radioFiltrarPorDesignacao.Checked)
                {
                    // Agrupa por Designacao
                    vigasAgrupadas = new Dictionary<string, List<Element>>();
                    foreach (var viga in vigasDisponiveis)
                    {
                        string designacao = Uteis.ObterDesignacaoViga(viga, doc);
                        if (string.IsNullOrEmpty(designacao)) designacao = "(Sem Designação)";
                        if (!vigasAgrupadas.ContainsKey(designacao))
                        {
                            vigasAgrupadas[designacao] = new List<Element>();
                        }
                        vigasAgrupadas[designacao].Add(viga);
                    }
                }

                gruposOrdenados = vigasAgrupadas.Keys.OrderBy(k => k).ToList();

                foreach (var chave in gruposOrdenados)
                {
                    var grupo = vigasAgrupadas[chave];
                    string itemTexto = $"{chave} ({grupo.Count} vigas)";
                    comboVigasDisponiveis.Items.Add(itemTexto);
                }

                if (comboVigasDisponiveis.Items.Count > 0)
                {
                    comboVigasDisponiveis.SelectedIndex = 0;
                }
                else
                {
                    LimparVisualizacao();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao atualizar lista: {ex.Message}", "Erro",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Event Handlers

        private void ButtonAtualizarLista_Click(object sender, EventArgs e)
        {
            CarregarVigasDisponiveis();
        }

        private void RadioFiltro_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked)
            {
                AtualizarListaVigas();
            }
        }

        private void ComboVigasDisponiveis_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboVigasDisponiveis.SelectedIndex >= 0 && gruposOrdenados != null)
            {
                string grupoSelecionado = gruposOrdenados[comboVigasDisponiveis.SelectedIndex];
                List<Element> vigasDoGrupo = vigasAgrupadas[grupoSelecionado];

                vigaSelecionada = vigasDoGrupo.FirstOrDefault();

                AtualizarVisualizacaoViga();
            }
        }

        private void CheckArmaduraLateral_CheckedChanged(object sender, EventArgs e)
        {
            bool ativo = checkArmaduraLateral.Checked;

            checkCombinacaoLateral.Enabled = ativo;

            if (!ativo)
            {
                checkCombinacaoLateral.Checked = false;
            }

            // Atualiza controles de combinação lateral
            labelQuant1Lateral.Enabled = ativo;
            numQuant1Lateral.Enabled = ativo;
            labelDiam1Lateral.Enabled = ativo;
            comboDiam1Lateral.Enabled = ativo;

            // Segundo diâmetro lateral
            bool ativoSegundo = ativo && checkCombinacaoLateral.Checked;
            labelQuant2Lateral.Enabled = ativoSegundo;
            numQuant2Lateral.Enabled = ativoSegundo;
            labelDiam2Lateral.Enabled = ativoSegundo;
            comboDiam2Lateral.Enabled = ativoSegundo;

            AtualizarVisualizacaoArmadura();
        }

        private void ParametroArmadura_Changed(object sender, EventArgs e)
        {
            AtualizarVisualizacaoArmadura();
        }

        private void ButtonExecutar_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidarConfiguracao()) return;

                var vigasParaProcessar = ObterVigasParaProcessar();
                if (vigasParaProcessar.Count == 0)
                {
                    MessageBox.Show("Nenhuma viga selecionada para processar.", "Aviso",
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult confirmacao = MessageBox.Show(
                    $"Confirma a criação de armaduras em {vigasParaProcessar.Count} viga(s)?",
                    "Confirmação",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirmacao == DialogResult.Yes)
                {
                    var cfg = ObterConfiguracaoArmadura();

                    createHandler.Data.ElementIds = vigasParaProcessar.Select(v => v.Id).ToList();
                    createHandler.Data.Varoes = new List<ArmVar>();
                    createHandler.Data.Estribos = new List<ArmStirrup>();

                    // superior
                    if (cfg.CombinacaoSuperior != null)
                    {
                        createHandler.Data.Varoes.Add(new ArmVar(cfg.CombinacaoSuperior, "Superior"));
                    }
                    else
                    {
                        createHandler.Data.Varoes.Add(new ArmVar(cfg.QuantSuperior, cfg.DiamSuperior) { TipoArmadura = "Superior" });
                    }

                    // inferior
                    if (cfg.CombinacaoInferior != null)
                    {
                        createHandler.Data.Varoes.Add(new ArmVar(cfg.CombinacaoInferior, "Inferior"));
                    }
                    else
                    {
                        createHandler.Data.Varoes.Add(new ArmVar(cfg.QuantInferior, cfg.DiamInferior) { TipoArmadura = "Inferior" });
                    }

                    // lateral
                    if (cfg.ArmaduraLateral)
                    {
                        if (cfg.CombinacaoLateral != null)
                        {
                            createHandler.Data.Varoes.Add(new ArmVar(cfg.CombinacaoLateral, "Lateral"));
                        }
                        else
                        {
                            createHandler.Data.Varoes.Add(new ArmVar(cfg.QuantLateral, cfg.DiamLateral) { TipoArmadura = "Lateral" });
                        }
                    }

                    // estribos
                    if (cfg.CombinacaoEstribos != null)
                    {
                        createHandler.Data.Estribos.Add(new ArmStirrup(cfg.CombinacaoEstribos));
                    }
                    else
                    {
                        createHandler.Data.Estribos.Add(new ArmStirrup(cfg.DiamEstribo, cfg.EspacamentoEstribo) { Alternado = cfg.EspacamentoVariavel });
                    }

                    createHandler.Data.MultAmarracao = cfg.MultAmarracao;
                    createHandler.Data.AmarracaoAuto = true;
                    createHandler.Data.Defs = ObterDefinicoesProjeto();
                    createHandler.Data.TipoElemento = TipoElementoEstruturalEnum.Vigas;

                    // Preparar UI
                    progressBar.Visible = true;
                    progressBar.Value = 0;
                    buttonExecutar.Enabled = false;
                    buttonCancelar.Enabled = false;
                    buttonAtualizarLista.Enabled = false;
                    comboVigasDisponiveis.Enabled = false;

                    createEvent.Raise();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro durante a execução: {ex.Message}", "Erro",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ButtonCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void CheckUsarVigaSelecionada_CheckedChanged(object sender, EventArgs e)
        {
            if (checkUsarVigaSelecionada.Checked)
            {
                selectionTimer.Start();
                TryAssumirVigaSelecionada(false);
            }
            else
            {
                selectionTimer.Stop();
                lastAssumedElementId = null;
            }

            comboVigasDisponiveis.Enabled = !checkUsarVigaSelecionada.Checked;
            buttonAtualizarLista.Enabled = !checkUsarVigaSelecionada.Checked;
        }

        private void SelectionTimer_Tick(object sender, EventArgs e)
        {
            TryAssumirVigaSelecionada(false);
        }

        private void TryAssumirVigaSelecionada(bool showMessages)
        {
            try
            {
                if (uidoc == null)
                {
                    if (showMessages) MessageBox.Show("UIDocument não disponível", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var sel = uidoc.Selection.GetElementIds();
                if (sel == null || sel.Count == 0)
                {
                    if (showMessages) MessageBox.Show("Nenhum elemento selecionado no Revit.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var firstId = sel.First();
                if (lastAssumedElementId != null && firstId == lastAssumedElementId) return;

                Element el = doc.GetElement(firstId);
                if (el == null)
                {
                    if (showMessages) MessageBox.Show("Elemento selecionado não encontrado no documento.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                bool isBeam = false;
                if (el is FamilyInstance) isBeam = true;
                if (el.Category != null && el.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFraming) isBeam = true;

                if (!isBeam)
                {
                    if (showMessages) MessageBox.Show("O elemento selecionado não parece ser uma viga estrutural.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                vigaSelecionada = el;
                lastAssumedElementId = firstId;

                TrySelecionarGrupoDaViga(el);

                AtualizarVisualizacaoViga();

                if (showMessages) MessageBox.Show("Viga assumida com sucesso.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                if (showMessages) MessageBox.Show($"Erro ao assumir viga selecionada: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TrySelecionarGrupoDaViga(Element el)
        {
            try
            {
                if (vigasAgrupadas == null || vigasAgrupadas.Count == 0 || gruposOrdenados == null) return;

                foreach (var kvp in vigasAgrupadas)
                {
                    if (kvp.Value.Any(v => v.Id == el.Id))
                    {
                        int index = gruposOrdenados.IndexOf(kvp.Key);
                        if (index >= 0 && index < comboVigasDisponiveis.Items.Count)
                        {
                            comboVigasDisponiveis.SelectedIndex = index;
                        }
                        return;
                    }
                }
            }
            catch
            {
                // ignorar erros
            }
        }

        #endregion

        #region Visualização

        private void AtualizarVisualizacaoViga()
        {
            if (vigaSelecionada == null)
            {
                LimparVisualizacao();
                return;
            }

            try
            {
                var propriedades = Uteis.ObterPropriedadesViga(vigaSelecionada, doc);
                if (propriedades != null)
                {
                    lblAlturaValor.Text = Uteis.FormatarMilimetros(propriedades.Altura);
                    lblLarguraValor.Text = Uteis.FormatarMilimetros(propriedades.Largura);

                    AtualizarVisualizacaoArmadura();
                }
                else
                {
                    LimparVisualizacao();
                }
            }
            catch (Exception ex)
            {
                labelInfoViga.Text = $"Erro ao carregar propriedades: {ex.Message}";
            }
        }

        private void AtualizarVisualizacaoArmadura()
        {
            if (vigaSelecionada == null) return;

            try
            {
                var propriedades = Uteis.ObterPropriedadesViga(vigaSelecionada, doc);
                if (propriedades == null) return;

                var cfg = ObterConfiguracaoArmadura();

                var info = new VisualizadorArmaduraViga.InformacaoArmaduraViga
                {
                    Altura = propriedades.Altura,
                    Largura = propriedades.Largura,
                    Recobrimento = cfg.Cobrimento,
                    AmarracaoAutomatica = true,
                    Designacao = propriedades.Designacao,
                    TipoFamilia = propriedades.Tipo
                };

                info.VaroesLongitudinais.Clear();
                if (cfg.UsaCombinacaoSuperior && cfg.CombinacaoSuperior != null)
                {
                    info.VaroesLongitudinais.Add(new ArmVar(cfg.CombinacaoSuperior, "Superior"));
                }
                else
                {
                    info.VaroesLongitudinais.Add(new ArmVar(cfg.QuantSuperior, cfg.DiamSuperior) { TipoArmadura = "Superior" });
                }

                if (cfg.UsaCombinacaoInferior && cfg.CombinacaoInferior != null)
                {
                    info.VaroesLongitudinais.Add(new ArmVar(cfg.CombinacaoInferior, "Inferior"));
                }
                else
                {
                    info.VaroesLongitudinais.Add(new ArmVar(cfg.QuantInferior, cfg.DiamInferior) { TipoArmadura = "Inferior" });
                }

                if (cfg.ArmaduraLateral)
                {
                    if (cfg.UsaCombinacaoLateral && cfg.CombinacaoLateral != null)
                    {
                        info.VaroesLongitudinais.Add(new ArmVar(cfg.CombinacaoLateral, "Lateral"));
                    }
                    else
                    {
                        info.VaroesLongitudinais.Add(new ArmVar(cfg.QuantLateral, cfg.DiamLateral) { TipoArmadura = "Lateral" });
                    }
                }

                info.Estribos.Clear();
                if (cfg.CombinacaoEstribos != null)
                {
                    info.Estribos.Add(new ArmStirrup(cfg.CombinacaoEstribos));
                }
                else
                {
                    info.Estribos.Add(new ArmStirrup(cfg.DiamEstribo, cfg.EspacamentoEstribo) { Alternado = cfg.EspacamentoVariavel });
                }

                visualizador.InformacaoViga = info;
                visualizador.AtualizarVisualizacao();
            }
            catch (Exception ex)
            {
                labelInfoViga.Text = $"Erro na visualização: {ex.Message}";
                labelInfoViga.Visible = true;
            }
        }

        private void LimparVisualizacao()
        {
            lblAlturaValor.Text = "0";
            lblLarguraValor.Text = "0";
            labelInfoViga.Text = "Selecione uma viga para visualizar as propriedades e configuração da armadura.";
            labelInfoViga.Visible = true;
        }

#endregion

        #region Configuração e Validação

        private class ConfiguracaoArmadura
        {
            // Armadura Superior
            public int QuantSuperior { get; set; }
            public int DiamSuperior { get; set; }
            public bool UsaCombinacaoSuperior { get; set; }
            public CombinacaoVaroes CombinacaoSuperior { get; set; }
            
            // Armadura Inferior
            public int QuantInferior { get; set; }
            public int DiamInferior { get; set; }
            public bool UsaCombinacaoInferior { get; set; }
            public CombinacaoVaroes CombinacaoInferior { get; set; }
            
            // Armadura Lateral
            public bool ArmaduraLateral { get; set; }
            public int QuantLateral { get; set; }
            public int DiamLateral { get; set; }
            public bool UsaCombinacaoLateral { get; set; }
            public CombinacaoVaroes CombinacaoLateral { get; set; }
            
            // Estribos
            public int DiamEstribo { get; set; }
            public int EspacamentoEstribo { get; set; }
            public bool EspacamentoVariavel { get; set; }
            public bool UsaCombinacaoEstribos { get; set; }
            public CombinacaoEstribos CombinacaoEstribos { get; set; }
            
            // Parâmetros gerais
            public int Cobrimento { get; set; }
            public int MultAmarracao { get; set; }
        }

        private ConfiguracaoArmadura ObterConfiguracaoArmadura()
        {
            var config = new ConfiguracaoArmadura();

            config.UsaCombinacaoSuperior = checkCombinacaoSuperior.Checked;
            if (checkCombinacaoSuperior.Checked)
            {
                config.CombinacaoSuperior = new CombinacaoVaroes { TipoArmadura = "Superior" };

                int quant1 = (int)numQuant1Superior.Value;
                int quant2 = (int)numQuant2Superior.Value;
                double diam1 = double.Parse(comboDiam1Superior.SelectedItem?.ToString() ?? "20");
                double diam2 = double.Parse(comboDiam2Superior.SelectedItem?.ToString() ?? "16");

                config.CombinacaoSuperior.AdicionarVarao(quant1, diam1);
                if (quant2 > 0)
                {
                    config.CombinacaoSuperior.AdicionarVarao(quant2, diam2);
                }

                config.CombinacaoSuperior.TipoDistribuicao = TipoDistribuicaoCombinada.MaioresNasExtremidades;
                config.QuantSuperior = config.CombinacaoSuperior.QuantidadeTotal;
                config.DiamSuperior = (int)diam1;
            }
            else
            {
                config.QuantSuperior = (int)numQuant1Superior.Value;
                config.DiamSuperior = int.Parse(comboDiam1Superior.SelectedItem?.ToString() ?? "16");
            }
            
            config.UsaCombinacaoInferior = checkCombinacaoInferior.Checked;
            if (checkCombinacaoInferior.Checked)
            {
                config.CombinacaoInferior = new CombinacaoVaroes { TipoArmadura = "Inferior" };
                
                int quant1 = (int)numQuant1Inferior.Value;
                int quant2 = (int)numQuant2Inferior.Value;
                double diam1 = double.Parse(comboDiam1Inferior.SelectedItem?.ToString() ?? "25");
                double diam2 = double.Parse(comboDiam2Inferior.SelectedItem?.ToString() ?? "20");
                
                config.CombinacaoInferior.AdicionarVarao(quant1, diam1);
                if (quant2 > 0)
                {
                    config.CombinacaoInferior.AdicionarVarao(quant2, diam2);
                }
                
                config.CombinacaoInferior.TipoDistribuicao = TipoDistribuicaoCombinada.MaioresNasExtremidades;
                config.QuantInferior = config.CombinacaoInferior.QuantidadeTotal;
                config.DiamInferior = (int)diam1;
            }
            else
            {
                config.QuantInferior = (int)numQuant1Inferior.Value;
                config.DiamInferior = int.Parse(comboDiam1Inferior.SelectedItem?.ToString() ?? "20");
            }
            
            config.ArmaduraLateral = checkArmaduraLateral.Checked;
            if (config.ArmaduraLateral)
            {
                config.UsaCombinacaoLateral = checkCombinacaoLateral.Checked;
                if (checkCombinacaoLateral.Checked)
                {
                    config.CombinacaoLateral = new CombinacaoVaroes { TipoArmadura = "Lateral" };
                    
                    int quant1 = (int)numQuant1Lateral.Value;
                    int quant2 = (int)numQuant2Lateral.Value;
                    double diam1 = double.Parse(comboDiam1Lateral.SelectedItem?.ToString() ?? "16");
                    double diam2 = double.Parse(comboDiam2Lateral.SelectedItem?.ToString() ?? "12");
                    
                    config.CombinacaoLateral.AdicionarVarao(quant1, diam1);
                    if (quant2 > 0)
                    {
                        config.CombinacaoLateral.AdicionarVarao(quant2, diam2);
                    }
                    
                    config.CombinacaoLateral.TipoDistribuicao = TipoDistribuicaoCombinada.IntercaladoRegular;
                    config.QuantLateral = config.CombinacaoLateral.QuantidadeTotal;
                    config.DiamLateral = (int)diam1;
                }
                else
                {
                    config.QuantLateral = (int)numQuant1Lateral.Value;
                    config.DiamLateral = int.Parse(comboDiam1Lateral.SelectedItem?.ToString() ?? "12");
                }
            }
            
            // Estribos
            config.UsaCombinacaoEstribos = checkCombinacaoEstribos.Checked;
            if (checkCombinacaoEstribos.Checked)
            {
                config.CombinacaoEstribos = new CombinacaoEstribos();

                double diam1 = double.Parse(comboDiam1Estribo.SelectedItem?.ToString() ?? "10");
                double diam2 = double.Parse(comboDiam2Estribo.SelectedItem?.ToString() ?? "8");
                double esp_stirup = (double)numEspac1Estribo.Value;

                config.CombinacaoEstribos.AdicionarEstribo(diam1, esp_stirup);
                config.CombinacaoEstribos.AdicionarEstribo(diam2, esp_stirup);
                config.CombinacaoEstribos.Intercalado = true;

                config.DiamEstribo = (int)diam1;
                config.EspacamentoEstribo = (int)esp_stirup;
            }
            else
            {
                double diam = double.Parse(comboDiam1Estribo.SelectedItem?.ToString() ?? "8");
                config.DiamEstribo = (int)diam;
                config.EspacamentoEstribo = (int)numEspac1Estribo.Value;
            }
            
            config.EspacamentoVariavel = false;
            config.Cobrimento = (int)numCobrimento.Value;
            config.MultAmarracao = (int)numMultAmarracao.Value;
            
            return config;
        }

        private bool ValidarConfiguracao()
        {
            if (vigaSelecionada == null)
            {
                MessageBox.Show("Selecione uma viga antes de executar.", "Validação",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (checkCombinacaoSuperior.Checked)
            {
                if (comboDiam1Superior.SelectedItem == null)
                {
                    MessageBox.Show("Selecione o primeiro diâmetro da combinação superior.", "Validação",
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                if (numQuant2Superior.Value > 0 && comboDiam2Superior.SelectedItem == null)
                {
                    MessageBox.Show("Selecione o segundo diâmetro da combinação superior.", "Validação",
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

            if (checkCombinacaoInferior.Checked)
            {
                if (comboDiam1Inferior.SelectedItem == null)
                {
                    MessageBox.Show("Selecione o primeiro diâmetro da combinação inferior.", "Validação",
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                if (numQuant2Inferior.Value > 0 && comboDiam2Inferior.SelectedItem == null)
                {
                    MessageBox.Show("Selecione o segundo diâmetro da combinação inferior.", "Validação",
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

            if (checkArmaduraLateral.Checked && checkCombinacaoLateral.Checked)
            {
                if (comboDiam1Lateral.SelectedItem == null)
                {
                    MessageBox.Show("Selecione o primeiro diâmetro da combinação lateral.", "Validação",
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                if (numQuant2Lateral.Value > 0 && comboDiam2Lateral.SelectedItem == null)
                {
                    MessageBox.Show("Selecione o segundo diâmetro da combinação lateral.", "Validação",
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

            if (checkCombinacaoEstribos.Checked)
            {
                if (comboDiam1Estribo.SelectedItem == null || comboDiam2Estribo.SelectedItem == null)
                {
                    MessageBox.Show("Selecione ambos os diâmetros da combinação de estribos.", "Validação",
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
            else
            {
                if (comboDiam1Estribo.SelectedItem == null)
                {
                    MessageBox.Show("Selecione o diâmetro do estribo.", "Validação",
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

            return true;
        }

        private List<Element> ObterVigasParaProcessar()
        {
            if (vigaSelecionada == null) return new List<Element>();
            if (gruposOrdenados == null || comboVigasDisponiveis.SelectedIndex < 0) return new List<Element>();

            string grupoSelecionado = gruposOrdenados[comboVigasDisponiveis.SelectedIndex];
            return vigasAgrupadas[grupoSelecionado];
        }

        #endregion

        #region Execução

        private void ExecutarCriacaoArmaduras(List<Element> vigas)
        {
            progressBar.Visible = true;
            progressBar.Value = 0;

            var config = CriarConfigExecucao();
            int processadas = 0;
            int sucesso = 0;
            var erros = new List<string>();

            using (Transaction trans = new Transaction(doc, "Criação de Armaduras em Vigas"))
            {
                trans.Start();

                foreach (var viga in vigas)
                {
                    try
                    {
                        bool resultado = config.ColocarArmadura(viga);
                        if (resultado)
                        {
                            sucesso++;
                        }
                        else
                        {
                            erros.Add($"Viga ID {viga.Id}: Falha na colocação");
                        }

                        processadas++;
                        progressBar.Value = (processadas * 100) / vigas.Count;
                        Application.DoEvents();
                    }
                    catch (Exception ex)
                    {
                        erros.Add($"Viga ID {viga.Id}: {ex.Message}");
                        processadas++;
                        progressBar.Value = (processadas * 100) / vigas.Count;
                        Application.DoEvents();
                    }
                }

                trans.Commit();
            }

            progressBar.Visible = false;

            // Mostrar resultados
            string mensagem = $"Processo concluído!\n\n" +
                             $"Vigas processadas: {vigas.Count}\n" +
                             $"Armaduras criadas: {sucesso}\n" +
                             $"Erros: {erros.Count}";

            if (erros.Count > 0)
            {
                mensagem += "\n\nPrimeiros erros:";
                for (int i = 0; i < Math.Min(erros.Count, 3); i++)
                {
                    mensagem += $"\n• {erros[i]}";
                }
            }

            MessageBox.Show(mensagem, "Resultado",
                           MessageBoxButtons.OK,
                           sucesso > 0 ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

            if (sucesso > 0)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private DefinicoesProjecto ObterDefinicoesProjeto()
        {
            return new DefinicoesProjecto
            {
                RecobrimentoVigas = (double)numCobrimento.Value,
                MultiplicadorAmarracaoMinimo = (double)numMultAmarracao.Value,
                MultiplicadorAmarracaoMaximo = (double)numMultAmarracao.Value,
                AmarracaoAutomaticaGlobal = true,
                EspacamentoMinimoEstribos = 50,
                EspacamentoMaximoEstribos = 300,
                ConfinamentoSismicoAutomatico = true,
                RatioConfinamento = 0.2,
                IncluirEsperasFundacao = true,
                ComprimentoEspera = 500,
                EspacamentoMinimoMalha = 150,
                EspacamentoMaximoMalha = 300,
                ArmaduraDuplaObrigatoria = false,
                RatioArmaduraSuperior = 0.4,
                ComprimentoZonaConfinamento = 600,
                ValidarEurocodigo = true,
                ValidarEspacamentosMinimos = true,
                GerarAvisosSobreposicao = true,
                GerarRelatorioQuantidades = true,
                CriarEtiquetasAutomaticas = true,
                FormatoEtiqueta = "{Diametro}? L={Comprimento}mm"
            };
        }

        private ArmConfigExec CriarConfigExecucao()
        {
            var config = new ArmConfigExec(doc);
            var armaduraConfig = ObterConfiguracaoArmadura();
            config.TipoElemento = TipoElementoEstruturalEnum.Vigas;
            config.Varoes.Clear();
            config.Estribos.Clear();

            if (armaduraConfig.CombinacaoSuperior != null)
            {
                config.Varoes.Add(new ArmVar(armaduraConfig.CombinacaoSuperior, "Superior"));
            }
            else
            {
                config.Varoes.Add(new ArmVar(armaduraConfig.QuantSuperior, armaduraConfig.DiamSuperior)
                {
                    TipoArmadura = "Superior"
                });
            }

            if (armaduraConfig.CombinacaoInferior != null)
            {
                config.Varoes.Add(new ArmVar(armaduraConfig.CombinacaoInferior, "Inferior"));
            }
            else
            {
                config.Varoes.Add(new ArmVar(armaduraConfig.QuantInferior, armaduraConfig.DiamInferior)
                {
                    TipoArmadura = "Inferior"
                });
            }

            if (armaduraConfig.ArmaduraLateral)
            {
                if (armaduraConfig.CombinacaoLateral != null)
                {
                    config.Varoes.Add(new ArmVar(armaduraConfig.CombinacaoLateral, "Lateral"));
                }
                else
                {
                    config.Varoes.Add(new ArmVar(armaduraConfig.QuantLateral, armaduraConfig.DiamLateral)
                    {
                        TipoArmadura = "Lateral"
                    });
                }
            }

            if (armaduraConfig.UsaCombinacaoEstribos && armaduraConfig.CombinacaoEstribos != null)
            {
                config.Estribos.Add(new ArmStirrup(armaduraConfig.CombinacaoEstribos));
            }
            else
            {
                config.Estribos.Add(new ArmStirrup(armaduraConfig.DiamEstribo, armaduraConfig.EspacamentoEstribo)
                {
                    Alternado = armaduraConfig.EspacamentoVariavel
                });
            }
            // Configurações
            config.AmarracaoAuto = true;
            config.MultAmarracao = armaduraConfig.MultAmarracao;
            config.Defs = ObterDefinicoesProjeto();
            config.Defs.RecobrimentoVigas = armaduraConfig.Cobrimento;

            return config;
        }

        #endregion

        private void labelInfoViga_Click(object sender, EventArgs e)
        {

        }

        private void visualizador_Load(object sender, EventArgs e)
        {

        }

        private void groupArmaduraLateral_Enter(object sender, EventArgs e)
        {

        }

        private void ButtonMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void CreateHandler_ProgressChanged(object sender, CreateArmaduraHandler.ProgressEventArgs e)
        {
            if (this.IsDisposed) return;
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => CreateHandler_ProgressChanged(sender, e)));
                return;
            }

            try
            {
                progressBar.Visible = true;
                if (e.Total > 0)
                {
                    int value = (int)Math.Min(100, (e.Processed * 100) / e.Total);
                    progressBar.Value = value;
                }
                else
                {
                    progressBar.Value = 0;
                }
            }
            catch { }
        }

        private void CreateHandler_ExecutionCompleted(object sender, CreateArmaduraHandler.ExecutionCompletedEventArgs e)
        {
            if (this.IsDisposed) return;
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => CreateHandler_ExecutionCompleted(sender, e)));
                return;
            }

            try
            {
                progressBar.Value = 100;
                progressBar.Visible = false;

                buttonExecutar.Enabled = true;
                buttonCancelar.Enabled = true;
                buttonAtualizarLista.Enabled = !checkUsarVigaSelecionada.Checked;
                comboVigasDisponiveis.Enabled = !checkUsarVigaSelecionada.Checked;
            }
            catch { }
        }

        private void CheckCombinacao_CheckedChanged(object sender, EventArgs e)
        {
            if (sender == checkCombinacaoSuperior)
            {
                bool ativo = checkCombinacaoSuperior.Checked;
                labelQuant2Superior.Enabled = ativo;
                numQuant2Superior.Enabled = ativo;
                labelDiam2Superior.Enabled = ativo;
                comboDiam2Superior.Enabled = ativo;

                labelQuant1Superior.Enabled = true;
                numQuant1Superior.Enabled = true;
                labelDiam1Superior.Enabled = true;
                comboDiam1Superior.Enabled = true;
            }
            else if (sender == checkCombinacaoInferior)
            {
                bool ativo = checkCombinacaoInferior.Checked;
                labelQuant2Inferior.Enabled = ativo;
                numQuant2Inferior.Enabled = ativo;
                labelDiam2Inferior.Enabled = ativo;
                comboDiam2Inferior.Enabled = ativo;

                labelQuant1Inferior.Enabled = true;
                numQuant1Inferior.Enabled = true;
                labelDiam1Inferior.Enabled = true;
                comboDiam1Inferior.Enabled = true;
            }
            else if (sender == checkCombinacaoLateral)
            {
                bool armaduraLateralAtiva = checkArmaduraLateral.Checked;

                if (checkCombinacaoLateral.Checked && !armaduraLateralAtiva)
                {
                    checkCombinacaoLateral.Checked = false;
                    MessageBox.Show("Ative primeiro a 'Armadura Alma' antes de usar combinações.", "Aviso", 
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                bool ativoSegundo = checkCombinacaoLateral.Checked && armaduraLateralAtiva;
                labelQuant2Lateral.Enabled = ativoSegundo;
                numQuant2Lateral.Enabled = ativoSegundo;
                labelDiam2Lateral.Enabled = ativoSegundo;
                comboDiam2Lateral.Enabled = ativoSegundo;

                labelQuant1Lateral.Enabled = armaduraLateralAtiva;
                numQuant1Lateral.Enabled = armaduraLateralAtiva;
                labelDiam1Lateral.Enabled = armaduraLateralAtiva;
                comboDiam1Lateral.Enabled = armaduraLateralAtiva;
            }
            else if (sender == checkCombinacaoEstribos)
            {
                bool ativo = checkCombinacaoEstribos.Checked;

                if (ativo)
                {
                    labelDiam1Estribo.Enabled = true;
                    comboDiam1Estribo.Enabled = true;
                    labelEspac1Estribo.Enabled = true;
                    numEspac1Estribo.Enabled = true;
                    labelDiam2Estribo.Enabled = true;
                    comboDiam2Estribo.Enabled = true;
                }
                else
                {
                    labelDiam1Estribo.Enabled = true;
                    comboDiam1Estribo.Enabled = true;
                    labelEspac1Estribo.Enabled = true;
                    numEspac1Estribo.Enabled = true;
                    labelDiam2Estribo.Enabled = false;
                    comboDiam2Estribo.Enabled = false;
                }
            }

            AtualizarVisualizacaoArmadura();
        }
    }
}
