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
        private Element vigaSelecionada;

        // last assumed element id to avoid repeated work
        private ElementId lastAssumedElementId = null;

        // ExternalEvent handler and event to run transactions in Revit API context
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
            // Configurar combos de diâmetros
            ConfigurarCombosDiametros();

            // Conectar event handlers
            ConectarEventHandlers();

            // Carregar vigas do projeto
            CarregarVigasDisponiveis();

            // Configurar formulário
            this.TopMost = false;
            this.ShowInTaskbar = true;
            this.StartPosition = FormStartPosition.CenterScreen;

            // Create the ExternalEvent and handler so we can raise it from the modeless form
            createHandler = new CreateArmaduraHandler();
            createEvent = ExternalEvent.Create(createHandler);

            // The controls for using the currently selected Revit element are
            // created in the designer (checkUsarVigaSelecionada, btnAssumirVigaSelecionada, selectionTimer)
            // so no dynamic creation is needed here.
        }

        private void ConfigurarCombosDiametros()
        {
            var diametros = Uteis.ObterDiametrosPadrao();

            foreach (var combo in new[] { comboDiamSuperior, comboDiamInferior, comboDiamLateral, comboDiamEstribo })
            {
                combo.Items.Clear();
                foreach (var diametro in diametros)
                {
                    combo.Items.Add(diametro.ToString());
                }
            }

            // Valores padrão
            comboDiamSuperior.SelectedItem = "16";
            comboDiamInferior.SelectedItem = "20";
            comboDiamLateral.SelectedItem = "12";
            comboDiamEstribo.SelectedItem = "8";
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

            // Botões de ação
            buttonExecutar.Click += ButtonExecutar_Click;
            buttonCancelar.Click += ButtonCancelar_Click;
            // Minimize button (designer added)
            if (this.Controls.ContainsKey("buttonMinimize"))
            {
                var btn = this.Controls.Find("buttonMinimize", true).FirstOrDefault() as Button;
                if (btn != null) btn.Click += ButtonMinimize_Click;
            }

            // Atualização automática da visualização
            foreach (var control in new Control[] { numQuantSuperior, numQuantInferior, numQuantLateral,
                                                   numEspacamentoEstribo, numCobrimento, numMultAmarracao })
            {
                if (control is NumericUpDown num)
                {
                    num.ValueChanged += ParametroArmadura_Changed;
                }
            }

            foreach (var combo in new[] { comboDiamSuperior, comboDiamInferior, comboDiamLateral, comboDiamEstribo })
            {
                combo.SelectedIndexChanged += ParametroArmadura_Changed;
            }

            checkEspacamentoVariavel.CheckedChanged += ParametroArmadura_Changed;

            // The designer wires the handlers for checkUsarVigaSelecionada, btnAssumirVigaSelecionada and selectionTimer.
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
                        string designacao = Uteis.ObterDesignacaoViga(viga, doc); // Função que retorna o parâmetro "Designacao"
                        if (string.IsNullOrEmpty(designacao)) designacao = "(Sem Designação)";
                        if (!vigasAgrupadas.ContainsKey(designacao))
                        {
                            vigasAgrupadas[designacao] = new List<Element>();
                        }
                        vigasAgrupadas[designacao].Add(viga);
                    }
                }

                foreach (var grupo in vigasAgrupadas.OrderBy(g => g.Key))
                {
                    string itemTexto = $"{grupo.Key} ({grupo.Value.Count} vigas)";
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
            if (comboVigasDisponiveis.SelectedIndex >= 0)
            {
                string grupoSelecionado = vigasAgrupadas.Keys.ElementAt(comboVigasDisponiveis.SelectedIndex);
                List<Element> vigasDoGrupo = vigasAgrupadas[grupoSelecionado];

                // Selecionar a primeira viga do grupo
                vigaSelecionada = vigasDoGrupo.FirstOrDefault();

                AtualizarVisualizacaoViga();
            }
        }

        private void CheckArmaduraLateral_CheckedChanged(object sender, EventArgs e)
        {
            bool ativo = checkArmaduraLateral.Checked;

            labelQuantLateral.Enabled = ativo;
            numQuantLateral.Enabled = ativo;
            labelDiamLateral.Enabled = ativo;
            comboDiamLateral.Enabled = ativo;

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
                    // Preencher dados na handler e disparar evento (passar ElementId e configurações)
                    var cfg = ObterConfiguracaoArmadura();
                    createHandler.Data.ElementIds = vigasParaProcessar.Select(v => v.Id).ToList();
                    createHandler.Data.Varoes = new List<ArmVar>();
                    createHandler.Data.Estribos = new List<ArmStirrup>();

                    // populate varoes from cfg
                    createHandler.Data.Varoes.Add(new ArmVar(cfg.QuantSuperior, cfg.DiamSuperior) { TipoArmadura = "Superior" });
                    createHandler.Data.Varoes.Add(new ArmVar(cfg.QuantInferior, cfg.DiamInferior) { TipoArmadura = "Inferior" });
                    if (cfg.ArmaduraLateral)
                    {
                        createHandler.Data.Varoes.Add(new ArmVar(cfg.QuantLateral, cfg.DiamLateral) { TipoArmadura = "Lateral" });
                    }

                    // populate estribos
                    createHandler.Data.Estribos.Add(new ArmStirrup(cfg.DiamEstribo, cfg.EspacamentoEstribo) { Alternado = cfg.EspacamentoVariavel });

                    createHandler.Data.MultAmarracao = cfg.MultAmarracao;
                    createHandler.Data.AmarracaoAuto = true;
                    createHandler.Data.Defs = ObterDefinicoesProjeto();
                    createHandler.Data.TipoElemento = TipoElementoEstruturalEnum.Vigas;

                    // Raise ExternalEvent to perform transaction inside Revit API context
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
                // Start polling selection
                selectionTimer.Start();
                // Try immediate assume
                TryAssumirVigaSelecionada(false);
            }
            else
            {
                selectionTimer.Stop();
                // Clear last assumed id so next check can re-detect
                lastAssumedElementId = null;
            }
        }

        private void BtnAssumirVigaSelecionada_Click(object sender, EventArgs e)
        {
            TryAssumirVigaSelecionada(true);
        }

        private void SelectionTimer_Tick(object sender, EventArgs e)
        {
            // Poll selection silently
            TryAssumirVigaSelecionada(false);
        }

        /// <summary>
        /// Attempts to take the current Revit selection as the beam to work with.
        /// If showMessages is true, user-facing messages will be shown on failures.
        /// </summary>
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
                if (lastAssumedElementId != null && firstId == lastAssumedElementId)
                {
                    // same as last assumed - nothing to do
                    return;
                }

                Element el = doc.GetElement(firstId);
                if (el == null)
                {
                    if (showMessages) MessageBox.Show("Elemento selecionado não encontrado no documento.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Verify element is a beam (Structural Framing) or FamilyInstance
                bool isBeam = false;
                if (el is FamilyInstance) isBeam = true;
                if (el.Category != null && el.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFraming) isBeam = true;

                if (!isBeam)
                {
                    if (showMessages) MessageBox.Show("O elemento selecionado não parece ser uma viga estrutural.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Assign and update UI
                vigaSelecionada = el;
                lastAssumedElementId = firstId;

                // If this viga exists in our grouped list, select corresponding group in combo
                TrySelecionarGrupoDaViga(el);

                AtualizarVisualizacaoViga();

                if (showMessages) MessageBox.Show("Viga assumida com sucesso.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                if (showMessages) MessageBox.Show($"Erro ao assumir viga selecionada: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// If the assumed element exists in vigasAgrupadas, selects the corresponding group in the combo box.
        /// </summary>
        private void TrySelecionarGrupoDaViga(Element el)
        {
            try
            {
                if (vigasAgrupadas == null || vigasAgrupadas.Count == 0) return;

                foreach (var kvp in vigasAgrupadas)
                {
                    if (kvp.Value.Any(v => v.Id == el.Id))
                    {
                        int index = vigasAgrupadas.Keys.ToList().IndexOf(kvp.Key);
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
                // ignore errors
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
                var propiedades = Uteis.ObterPropriedadesViga(vigaSelecionada, doc);
                if (propiedades != null)
                {
                    lblAlturaValor.Text = Uteis.FormatarMilimetros(propiedades.Altura);
                    lblLarguraValor.Text = Uteis.FormatarMilimetros(propiedades.Largura);

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

                // Atualizar o controle gráfico
                var info = new VisualizadorArmaduraViga.InformacaoArmaduraViga
                {
                    Comprimento = propriedades.Comprimento,
                    Altura = propriedades.Altura,
                    Largura = propriedades.Largura,
                    Recobrimento = cfg.Cobrimento,
                    AmarracaoAutomatica = true,
                    Designacao = propriedades.Designacao,
                    TipoFamilia = propriedades.Tipo
                };

                info.VaroesLongitudinais.Clear();
                info.VaroesLongitudinais.Add(new ArmVar(cfg.QuantSuperior, cfg.DiamSuperior) { TipoArmadura = "Superior" });
                info.VaroesLongitudinais.Add(new ArmVar(cfg.QuantInferior, cfg.DiamInferior) { TipoArmadura = "Inferior" });
                if (cfg.ArmaduraLateral)
                {
                    info.VaroesLongitudinais.Add(new ArmVar(cfg.QuantLateral, cfg.DiamLateral) { TipoArmadura = "Lateral" });
                }

                info.Estribos.Clear();
                info.Estribos.Add(new ArmStirrup(cfg.DiamEstribo, cfg.EspacamentoEstribo) { Alternado = cfg.EspacamentoVariavel });

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
            public int QuantSuperior { get; set; }
            public int DiamSuperior { get; set; }
            public int QuantInferior { get; set; }
            public int DiamInferior { get; set; }
            public bool ArmaduraLateral { get; set; }
            public int QuantLateral { get; set; }
            public int DiamLateral { get; set; }
            public int DiamEstribo { get; set; }
            public int EspacamentoEstribo { get; set; }
            public bool EspacamentoVariavel { get; set; }
            public int Cobrimento { get; set; }
            public int MultAmarracao { get; set; }
        }

        private ConfiguracaoArmadura ObterConfiguracaoArmadura()
        {
            return new ConfiguracaoArmadura
            {
                QuantSuperior = (int)numQuantSuperior.Value,
                DiamSuperior = int.Parse(comboDiamSuperior.SelectedItem?.ToString() ?? "16"),
                QuantInferior = (int)numQuantInferior.Value,
                DiamInferior = int.Parse(comboDiamInferior.SelectedItem?.ToString() ?? "20"),
                ArmaduraLateral = checkArmaduraLateral.Checked,
                QuantLateral = (int)numQuantLateral.Value,
                DiamLateral = int.Parse(comboDiamLateral.SelectedItem?.ToString() ?? "12"),
                DiamEstribo = int.Parse(comboDiamEstribo.SelectedItem?.ToString() ?? "8"),
                EspacamentoEstribo = (int)numEspacamentoEstribo.Value,
                EspacamentoVariavel = checkEspacamentoVariavel.Checked,
                Cobrimento = (int)numCobrimento.Value,
                MultAmarracao = (int)numMultAmarracao.Value
            };
        }

        private bool ValidarConfiguracao()
        {
            if (vigaSelecionada == null)
            {
                MessageBox.Show("Selecione uma viga antes de executar.", "Validação",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (comboDiamSuperior.SelectedItem == null || comboDiamInferior.SelectedItem == null)
            {
                MessageBox.Show("Selecione os diâmetros das armaduras.", "Validação",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (comboDiamEstribo.SelectedItem == null)
            {
                MessageBox.Show("Selecione o diâmetro dos estribos.", "Validação",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private List<Element> ObterVigasParaProcessar()
        {
            if (vigaSelecionada == null) return new List<Element>();

            string grupoSelecionado = vigasAgrupadas.Keys.ElementAt(comboVigasDisponiveis.SelectedIndex);
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
            // Armadura superior
            config.Varoes.Add(new ArmVar(armaduraConfig.QuantSuperior, armaduraConfig.DiamSuperior)
            {
                TipoArmadura = "Superior"
            });
            // Armadura inferior
            config.Varoes.Add(new ArmVar(armaduraConfig.QuantInferior, armaduraConfig.DiamInferior)
            {
                TipoArmadura = "Inferior"
            });
            // Armadura lateral (se ativada)
            if (armaduraConfig.ArmaduraLateral)
            {
                config.Varoes.Add(new ArmVar(armaduraConfig.QuantLateral, armaduraConfig.DiamLateral)
                {
                    TipoArmadura = "Lateral"
                });
            }
            // Estribos
            config.Estribos.Add(new ArmStirrup(armaduraConfig.DiamEstribo, armaduraConfig.EspacamentoEstribo)
            {
                Alternado = armaduraConfig.EspacamentoVariavel
            });
            // Configurações
            config.AmarracaoAuto = true;
            config.MultAmarracao = armaduraConfig.MultAmarracao;
            // Definições do projeto diretamente do formulário
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
    }
}
