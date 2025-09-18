using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Control = System.Windows.Forms.Control;
using View = System.Windows.Forms.View;
using ComboBox = System.Windows.Forms.ComboBox;
using TextBox = System.Windows.Forms.TextBox;
using Application = System.Windows.Forms.Application;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MacroArmaduraAvancado
{
    public partial class FormularioPrincipalAvancado : System.Windows.Forms.Form
    {
        private Document doc;
        private UIDocument uidoc;
        private DetectorElementosAvancado detectorElementos;
        private GestorDefinicoesAvancado gestorDefinicoes;
        private CalculadorAmarracao calculadorAmarracao;

        // Controlos da interface
        private GroupBox groupTipoElemento;
        private RadioButton radioVigas;

        private GroupBox groupFiltros;
        private ComboBox comboDesignacao;
        private CheckedListBox listNiveis;
        private CheckBox checkSeleccaoActual;
        private Label labelContagem;
        private Label labelInfoElementos;

        private GroupBox groupConfigArmadura;
        private Button buttonAdicionarVarao;
        private Button buttonRemoverVarao;
        private ListView listViewVaroes;
        private ColumnHeader colQuantidade;
        private ColumnHeader colDiametro;
        private ColumnHeader colTipo;
        private ComboBox comboTipoDistribuicao;
        private TextBox textComprimentoBase;
        private CheckBox checkComprimentoAuto;
        private CheckBox checkAmarracaoAutomatica;
        private Label labelComprimentoCalculado;

        private GroupBox groupAmarracao;
        private NumericUpDown numMultiplicadorAmarracao;
        private ComboBox comboTipoAmarracao;
        private CheckBox checkDeteccaoAutomaticaAmarracao;
        private Label labelAmarracaoInfo;

        private GroupBox groupEstribos;
        private Button buttonAdicionarEstribo;
        private Button buttonRemoverEstribo;
        private ListView listViewEstribos;
        private ColumnHeader colDiametroEstribo;
        private ColumnHeader colEspacamento;
        private ColumnHeader colAlternado;
        private CheckBox checkEstribosAutomaticos;

        private Button buttonDefinicoes;
        private Button buttonPreVisualizacao;
        private Button buttonExecutar;
        private Button buttonCancelar;
        private ProgressBar progressBar;

        public FormularioPrincipalAvancado(Document documento, UIDocument uiDocumento)
        {
            doc = documento;
            uidoc = uiDocumento;
            detectorElementos = new DetectorElementosAvancado(doc);
            gestorDefinicoes = new GestorDefinicoesAvancado();
            calculadorAmarracao = new CalculadorAmarracao();

            InicializarComponentes();
            CarregarDados();
        }

        private void InicializarComponentes()
        {
            this.Text = "Automação de Armaduras - Pilares e Vigas v1.0";
            this.Size = new System.Drawing.Size(950, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Grupo de selecção do tipo de elemento (APENAS VIGAS)
            groupTipoElemento = new GroupBox();
            groupTipoElemento.Text = "Tipo de Elemento Estrutural";
            groupTipoElemento.Location = new System.Drawing.Point(12, 12);
            groupTipoElemento.Size = new System.Drawing.Size(910, 70);

            radioVigas = new RadioButton();
            radioVigas.Text = "Vigas";
            radioVigas.Location = new System.Drawing.Point(20, 25);
            radioVigas.Size = new System.Drawing.Size(80, 25);
            radioVigas.Checked = true;
            radioVigas.Enabled = false;

            labelInfoElementos = new Label();
            labelInfoElementos.Text = "Informações: Detecção automática de posição para amarrações";
            labelInfoElementos.Location = new System.Drawing.Point(120, 25);
            labelInfoElementos.Size = new System.Drawing.Size(450, 40);
            labelInfoElementos.ForeColor = System.Drawing.Color.DarkBlue;

            groupTipoElemento.Controls.AddRange(new Control[] {
                radioVigas, labelInfoElementos
            });

            // Resto da interface... (similar mas corrigida)
            InicializarFiltros();
            InicializarConfigArmadura();
            InicializarConfigAmarracao();
            InicializarConfigEstribos();
            InicializarBotoes();

            this.Controls.AddRange(new Control[] {
                groupTipoElemento, groupFiltros, groupConfigArmadura,
                groupAmarracao, groupEstribos, buttonDefinicoes,
                buttonPreVisualizacao, buttonExecutar, buttonCancelar, progressBar
            });

            AdicionarConfigPadrao();
        }

        private void InicializarFiltros()
        {
            groupFiltros = new GroupBox();
            groupFiltros.Text = "Filtros de Selecção";
            groupFiltros.Location = new System.Drawing.Point(12, 95);
            groupFiltros.Size = new System.Drawing.Size(910, 120);

            Label labelDesignacao = new Label();
            labelDesignacao.Text = "Designação:";
            labelDesignacao.Location = new System.Drawing.Point(20, 30);

            comboDesignacao = new ComboBox();
            comboDesignacao.Location = new System.Drawing.Point(110, 28);
            comboDesignacao.Size = new System.Drawing.Size(200, 25);
            comboDesignacao.DropDownStyle = ComboBoxStyle.DropDownList;
            comboDesignacao.SelectedIndexChanged += ComboDesignacao_SelectedIndexChanged;

            Label labelNiveis = new Label();
            labelNiveis.Text = "Níveis:";
            labelNiveis.Location = new System.Drawing.Point(330, 30);

            listNiveis = new CheckedListBox();
            listNiveis.Location = new System.Drawing.Point(380, 28);
            listNiveis.Size = new System.Drawing.Size(180, 80);
            listNiveis.ItemCheck += ListNiveis_ItemCheck;

            checkSeleccaoActual = new CheckBox();
            checkSeleccaoActual.Text = "Usar selecção actual";
            checkSeleccaoActual.Location = new System.Drawing.Point(580, 30);
            checkSeleccaoActual.Size = new System.Drawing.Size(150, 25);
            checkSeleccaoActual.CheckedChanged += CheckSeleccaoActual_CheckedChanged;

            labelContagem = new Label();
            labelContagem.Text = "Elementos encontrados: 0";
            labelContagem.Location = new System.Drawing.Point(20, 85);
            labelContagem.Size = new System.Drawing.Size(300, 20);
            labelContagem.Font = new System.Drawing.Font(labelContagem.Font, System.Drawing.FontStyle.Bold);

            groupFiltros.Controls.AddRange(new Control[] {
                labelDesignacao, comboDesignacao, labelNiveis, listNiveis,
                checkSeleccaoActual, labelContagem
            });
        }

        private void InicializarConfigArmadura()
        {
            groupConfigArmadura = new GroupBox();
            groupConfigArmadura.Text = "Configuração de Armadura Longitudinal";
            groupConfigArmadura.Location = new System.Drawing.Point(12, 230);
            groupConfigArmadura.Size = new System.Drawing.Size(450, 250);

            // ListView para varões
            listViewVaroes = new ListView();
            listViewVaroes.Location = new System.Drawing.Point(20, 30);
            listViewVaroes.Size = new System.Drawing.Size(300, 120);
            listViewVaroes.View = View.Details;
            listViewVaroes.FullRowSelect = true;
            listViewVaroes.GridLines = true;

            colQuantidade = new ColumnHeader();
            colQuantidade.Text = "Quant.";
            colQuantidade.Width = 60;

            colDiametro = new ColumnHeader();
            colDiametro.Text = "Diâm. (mm)";
            colDiametro.Width = 80;

            colPosicao = new ColumnHeader();
            colPosicao.Text = "Pos.";
            colPosicao.Width = 60;

            listViewVaroes.Columns.AddRange(new ColumnHeader[] {
                colQuantidade, colDiametro, colPosicao
            });

            buttonAdicionarVarao = new Button();
            buttonAdicionarVarao.Text = "Adicionar";
            buttonAdicionarVarao.Location = new System.Drawing.Point(330, 30);
            buttonAdicionarVarao.Size = new System.Drawing.Size(80, 25);
            buttonAdicionarVarao.Click += ButtonAdicionarVarao_Click;

            buttonRemoverVarao = new Button();
            buttonRemoverVarao.Text = "Remover";
            buttonRemoverVarao.Location = new System.Drawing.Point(330, 65);
            buttonRemoverVarao.Size = new System.Drawing.Size(80, 25);
            buttonRemoverVarao.Click += ButtonRemoverVarao_Click;

            // Resto dos controlos de armadura...
            InicializarControlsArmadura();

            groupConfigArmadura.Controls.AddRange(new Control[] {
                listViewVaroes, buttonAdicionarVarao, buttonRemoverVarao
                // Adicionar outros controlos conforme necessário
            });
        }

        private void InicializarControlsArmadura()
        {
            Label labelDistribuicao = new Label();
            labelDistribuicao.Text = "Distribuição:";
            labelDistribuicao.Location = new System.Drawing.Point(20, 160);

            comboTipoDistribuicao = new ComboBox();
            comboTipoDistribuicao.Location = new System.Drawing.Point(110, 158);
            comboTipoDistribuicao.Size = new System.Drawing.Size(200, 25);
            comboTipoDistribuicao.DropDownStyle = ComboBoxStyle.DropDownList;
            comboTipoDistribuicao.Items.AddRange(new string[] {
                "Uniforme",
                "ConcentradaNasBordas",
                "MistaComMaioresNasBordas"
            });
            comboTipoDistribuicao.SelectedIndex = 2;

            Label labelComprimentoBase = new Label();
            labelComprimentoBase.Text = "Compr. base (m):";
            labelComprimentoBase.Location = new System.Drawing.Point(20, 195);

            textComprimentoBase = new TextBox();
            textComprimentoBase.Location = new System.Drawing.Point(130, 193);
            textComprimentoBase.Size = new System.Drawing.Size(80, 25);
            textComprimentoBase.Text = "3.00";
            textComprimentoBase.TextChanged += TextComprimentoBase_TextChanged;

            checkComprimentoAuto = new CheckBox();
            checkComprimentoAuto.Text = "Altura automática";
            checkComprimentoAuto.Location = new System.Drawing.Point(220, 195);
            checkComprimentoAuto.Size = new System.Drawing.Size(130, 25);
            checkComprimentoAuto.Checked = true;
            checkComprimentoAuto.CheckedChanged += CheckComprimentoAuto_CheckedChanged;

            checkAmarracaoAutomatica = new CheckBox();
            checkAmarracaoAutomatica.Text = "Amarração automática";
            checkAmarracaoAutomatica.Location = new System.Drawing.Point(20, 225);
            checkAmarracaoAutomatica.Size = new System.Drawing.Size(150, 25);
            checkAmarracaoAutomatica.Checked = true;
            checkAmarracaoAutomatica.CheckedChanged += CheckAmarracaoAutomatica_CheckedChanged;

            labelComprimentoCalculado = new Label();
            labelComprimentoCalculado.Text = "Comprimento calculado: -- m";
            labelComprimentoCalculado.Location = new System.Drawing.Point(180, 225);
            labelComprimentoCalculado.Size = new System.Drawing.Size(250, 25);
            labelComprimentoCalculado.ForeColor = System.Drawing.Color.DarkGreen;
            labelComprimentoCalculado.Font = new System.Drawing.Font(labelComprimentoCalculado.Font, System.Drawing.FontStyle.Bold);

            groupConfigArmadura.Controls.AddRange(new Control[] {
                labelDistribuicao, comboTipoDistribuicao,
                labelComprimentoBase, textComprimentoBase, checkComprimentoAuto,
                checkAmarracaoAutomatica, labelComprimentoCalculado
            });
        }

        private void InicializarConfigAmarracao()
        {
            groupAmarracao = new GroupBox();
            groupAmarracao.Text = "Configuração de Amarração";
            groupAmarracao.Location = new System.Drawing.Point(470, 230);
            groupAmarracao.Size = new System.Drawing.Size(452, 200);

            Label labelMultiplicador = new Label();
            labelMultiplicador.Text = "Comprimento amarração:";
            labelMultiplicador.Location = new System.Drawing.Point(20, 30);

            numMultiplicadorAmarracao = new NumericUpDown();
            numMultiplicadorAmarracao.Location = new System.Drawing.Point(160, 28);
            numMultiplicadorAmarracao.Size = new System.Drawing.Size(50, 25);
            numMultiplicadorAmarracao.Minimum = 30;
            numMultiplicadorAmarracao.Maximum = 100;
            numMultiplicadorAmarracao.Value = 70;
            numMultiplicadorAmarracao.ValueChanged += NumMultiplicadorAmarracao_ValueChanged;

            Label labelPhi = new Label();
            labelPhi.Text = "φ (70φ = 70 × diâmetro)";
            labelPhi.Location = new System.Drawing.Point(220, 30);
            labelPhi.Size = new System.Drawing.Size(150, 25);

            Label labelTipoAmarracao = new Label();
            labelTipoAmarracao.Text = "Tipo de amarração:";
            labelTipoAmarracao.Location = new System.Drawing.Point(20, 65);

            comboTipoAmarracao = new ComboBox();
            comboTipoAmarracao.Location = new System.Drawing.Point(140, 63);
            comboTipoAmarracao.Size = new System.Drawing.Size(150, 25);
            comboTipoAmarracao.DropDownStyle = ComboBoxStyle.DropDownList;
            comboTipoAmarracao.Items.AddRange(new string[] {
                "Automático", "Reta", "Dobrada 90°", "Gancho 135°"
            });
            comboTipoAmarracao.SelectedItem = "Automático";

            checkDeteccaoAutomaticaAmarracao = new CheckBox();
            checkDeteccaoAutomaticaAmarracao.Text = "Detectar posição automaticamente";
            checkDeteccaoAutomaticaAmarracao.Location = new System.Drawing.Point(20, 100);
            checkDeteccaoAutomaticaAmarracao.Size = new System.Drawing.Size(200, 25);
            checkDeteccaoAutomaticaAmarracao.Checked = true;

            labelAmarracaoInfo = new Label();
            labelAmarracaoInfo.Text = "Info: Fundação/Último piso = Dobrada 90°\nPisos intermédios = Reta";
            labelAmarracaoInfo.Location = new System.Drawing.Point(20, 130);
            labelAmarracaoInfo.Size = new System.Drawing.Size(300, 40);
            labelAmarracaoInfo.ForeColor = System.Drawing.Color.DarkBlue;

            groupAmarracao.Controls.AddRange(new Control[] {
                labelMultiplicador, numMultiplicadorAmarracao, labelPhi,
                labelTipoAmarracao, comboTipoAmarracao,
                checkDeteccaoAutomaticaAmarracao, labelAmarracaoInfo
            });
        }

        private void InicializarConfigEstribos()
        {
            groupEstribos = new GroupBox();
            groupEstribos.Text = "Configuração de Estribos";
            groupEstribos.Location = new System.Drawing.Point(12, 490);
            groupEstribos.Size = new System.Drawing.Size(910, 160);

            // ListView para estribos
            listViewEstribos = new ListView();
            listViewEstribos.Location = new System.Drawing.Point(20, 30);
            listViewEstribos.Size = new System.Drawing.Size(400, 80);
            listViewEstribos.View = View.Details;
            listViewEstribos.FullRowSelect = true;
            listViewEstribos.GridLines = true;

            colDiametroEstribo = new ColumnHeader();
            colDiametroEstribo.Text = "Diâm. (mm)";
            colDiametroEstribo.Width = 80;

            colEspacamento = new ColumnHeader();
            colEspacamento.Text = "Espaç. (mm)";
            colEspacamento.Width = 100;

            colAlternado = new ColumnHeader();
            colAlternado.Text = "Alternado";
            colAlternado.Width = 80;

            listViewEstribos.Columns.AddRange(new ColumnHeader[] {
                colDiametroEstribo, colEspacamento, colAlternado
            });

            buttonAdicionarEstribo = new Button();
            buttonAdicionarEstribo.Text = "Adicionar";
            buttonAdicionarEstribo.Location = new System.Drawing.Point(430, 30);
            buttonAdicionarEstribo.Size = new System.Drawing.Size(80, 25);
            buttonAdicionarEstribo.Click += ButtonAdicionarEstribo_Click;

            buttonRemoverEstribo = new Button();
            buttonRemoverEstribo.Text = "Remover";
            buttonRemoverEstribo.Location = new System.Drawing.Point(430, 65);
            buttonRemoverEstribo.Size = new System.Drawing.Size(80, 25);
            buttonRemoverEstribo.Click += ButtonRemoverEstribo_Click;

            checkEstribosAutomaticos = new CheckBox();
            checkEstribosAutomaticos.Text = "Distribuição automática constante";
            checkEstribosAutomaticos.Location = new System.Drawing.Point(20, 120);
            checkEstribosAutomaticos.Size = new System.Drawing.Size(300, 25);
            checkEstribosAutomaticos.Checked = true;

            groupEstribos.Controls.AddRange(new Control[] {
                listViewEstribos, buttonAdicionarEstribo, buttonRemoverEstribo,
                checkEstribosAutomaticos
            });
        }

        private void InicializarBotoes()
        {
            buttonDefinicoes = new Button();
            buttonDefinicoes.Text = "⚙ Definições";
            buttonDefinicoes.Location = new System.Drawing.Point(12, 670);
            buttonDefinicoes.Size = new System.Drawing.Size(120, 35);
            buttonDefinicoes.Click += ButtonDefinicoes_Click;

            buttonPreVisualizacao = new Button();
            buttonPreVisualizacao.Text = "👁 Pré-visualização";
            buttonPreVisualizacao.Location = new System.Drawing.Point(150, 670);
            buttonPreVisualizacao.Size = new System.Drawing.Size(130, 35);
            buttonPreVisualizacao.BackColor = System.Drawing.Color.LightBlue;
            buttonPreVisualizacao.Click += ButtonPreVisualizacao_Click;

            buttonExecutar = new Button();
            buttonExecutar.Text = "▶ Executar";
            buttonExecutar.Location = new System.Drawing.Point(650, 670);
            buttonExecutar.Size = new System.Drawing.Size(120, 35);
            buttonExecutar.BackColor = System.Drawing.Color.LightGreen;
            buttonExecutar.Font = new System.Drawing.Font(buttonExecutar.Font, System.Drawing.FontStyle.Bold);
            buttonExecutar.Click += ButtonExecutar_Click;

            buttonCancelar = new Button();
            buttonCancelar.Text = "✖ Cancelar";
            buttonCancelar.Location = new System.Drawing.Point(790, 670);
            buttonCancelar.Size = new System.Drawing.Size(90, 35);
            buttonCancelar.Click += ButtonCancelar_Click;

            progressBar = new ProgressBar();
            progressBar.Location = new System.Drawing.Point(300, 680);
            progressBar.Size = new System.Drawing.Size(330, 15);
            progressBar.Visible = false;
        }

        private void AdicionarConfigPadrao()
        {
            // Adicionar varão padrão
            ListViewItem itemVaraoSuperior = new ListViewItem(new string[] { "4", "16", "Superior" });
            listViewVaroes.Items.Add(itemVaraoSuperior);
            ListViewItem itemVaraoInferior = new ListViewItem(new string[] { "4", "16", "Inferior" });
            listViewVaroes.Items.Add(itemVaraoInferior);

            // Adicionar estribo padrão
            ListViewItem itemEstribo = new ListViewItem(new string[] { "8", "200", "Não" });
            listViewEstribos.Items.Add(itemEstribo);
        }

        private void CarregarDados()
        {
            ActualizarElementos();
            ActualizarCalculosAutomaticos();
        }

        // Event Handlers

        private void ComboDesignacao_SelectedIndexChanged(object sender, EventArgs e)
        {
            ActualizarContagem();
        }

        private void ListNiveis_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            this.BeginInvoke(new MethodInvoker(() => ActualizarContagem()));
        }

        private void CheckSeleccaoActual_CheckedChanged(object sender, EventArgs e)
        {
            comboDesignacao.Enabled = !checkSeleccaoActual.Checked;
            listNiveis.Enabled = !checkSeleccaoActual.Checked;
            ActualizarContagem();
        }

        private void CheckComprimentoAuto_CheckedChanged(object sender, EventArgs e)
        {
            textComprimentoBase.Enabled = !checkComprimentoAuto.Checked;
            ActualizarCalculosAutomaticos();
        }

        private void CheckAmarracaoAutomatica_CheckedChanged(object sender, EventArgs e)
        {
            numMultiplicadorAmarracao.Enabled = checkAmarracaoAutomatica.Checked;
            comboTipoAmarracao.Enabled = !checkAmarracaoAutomatica.Checked;
            ActualizarCalculosAutomaticos();
        }

        private void TextComprimentoBase_TextChanged(object sender, EventArgs e)
        {
            ActualizarCalculosAutomaticos();
        }

        private void NumMultiplicadorAmarracao_ValueChanged(object sender, EventArgs e)
        {
            ActualizarCalculosAutomaticos();
        }

        // Métodos de interface
        private void ButtonAdicionarVarao_Click(object sender, EventArgs e)
        {
            using (FormularioConfiguracaoVarao form = new FormularioConfiguracaoVarao(true))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    ListViewItem item = new ListViewItem(new string[] {
                        form.Quantidade.ToString(),
                        form.Diametro.ToString(),
                        form.Posicao
                    });
                    listViewVaroes.Items.Add(item);
                    ActualizarCalculosAutomaticos();
                }
            }
        }

        private void ButtonRemoverVarao_Click(object sender, EventArgs e)
        {
            if (listViewVaroes.SelectedItems.Count > 0)
            {
                listViewVaroes.SelectedItems[0].Remove();
                ActualizarCalculosAutomaticos();
            }
            else
            {
                MessageBox.Show("Seleccione um varão para remover.", "Aviso",
                               MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ButtonAdicionarEstribo_Click(object sender, EventArgs e)
        {
            using (FormularioConfiguracaoEstribo form = new FormularioConfiguracaoEstribo())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    ListViewItem item = new ListViewItem(new string[] {
                        form.Diametro.ToString(),
                        form.Espacamento.ToString(),
                        form.Alternado ? "Sim" : "Não"
                    });
                    listViewEstribos.Items.Add(item);
                }
            }
        }

        private void ButtonRemoverEstribo_Click(object sender, EventArgs e)
        {
            if (listViewEstribos.SelectedItems.Count > 0)
            {
                listViewEstribos.SelectedItems[0].Remove();
            }
            else
            {
                MessageBox.Show("Seleccione um estribo para remover.", "Aviso",
                               MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ButtonDefinicoes_Click(object sender, EventArgs e)
        {
            using (FormularioDefinicoesAvancadas formDefinicoes = new FormularioDefinicoesAvancadas(gestorDefinicoes))
            {
                formDefinicoes.ShowDialog();
            }
        }

        private void ButtonPreVisualizacao_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidarEntrada()) return;

                List<Element> elementosProcessar = ObterElementosParaProcessar();
                if (elementosProcessar.Count == 0)
                {
                    MessageBox.Show("Nenhum elemento para pré-visualização.", "Aviso",
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                ArmConfigExec config = CriarConfiguracao();
                string relatorio = config.GerarRelatorioPreVisualizacao(elementosProcessar);

                using (FormularioPreVisualizacao formPreview = new FormularioPreVisualizacao(relatorio))
                {
                    formPreview.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro na pré-visualização: " + ex.Message, "Erro",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ButtonExecutar_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidarEntrada()) return;

                List<Element> elementosProcessar = ObterElementosParaProcessar();
                if (elementosProcessar.Count == 0)
                {
                    MessageBox.Show("Nenhum elemento encontrado para processar.", "Aviso",
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult resultado = MessageBox.Show(
                    $"Confirma a execução da colocação de armaduras em {elementosProcessar.Count} elementos?",
                    "Confirmação",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (resultado != DialogResult.Yes) return;

                ExecutarColocacaoArmaduras(elementosProcessar);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro durante a execução: " + ex.Message, "Erro",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ButtonCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        // Métodos principais
        private void ActualizarElementos()
        {
            try
            {
                TipoElementoEstruturalEnum tipoSeleccionado = ObterTipoElementoSeleccionado();
                List<Element> elementos = detectorElementos.DetectarElementos(tipoSeleccionado);

                ActualizarDesignacoes(elementos);
                ActualizarNiveis(elementos);

                labelContagem.Text = $"Elementos encontrados: {elementos.Count}";
                ActualizarInformacoesTipo(elementos);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao actualizar elementos: " + ex.Message,
                               "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ActualizarInformacoesTipo(List<Element> elementos)
        {
            if (elementos.Count > 0)
            {
                var analise = detectorElementos.AnalisarElementos(elementos);

                string infoTexto = $"Análise: {analise.TotalElementos} elementos | " +
                                  $"Fundação: {analise.ElementosFundacao} | " +
                                  $"Último piso: {analise.ElementosUltimoPiso} | " +
                                  $"Intermédios: {analise.ElementosIntermedios}";

                labelInfoElementos.Text = infoTexto;
            }
            else
            {
                labelInfoElementos.Text = "Nenhum elemento detectado";
            }
        }

        private void ActualizarDesignacoes(List<Element> elementos)
        {
            comboDesignacao.Items.Clear();

            HashSet<string> designacoes = new HashSet<string>();
            foreach (Element elemento in elementos)
            {
                string designacao = elemento.get_Parameter(BuiltInParameter.ELEM_FAMILY_PARAM)?.AsValueString() ??
                                   elemento.get_Parameter(BuiltInParameter.ELEM_TYPE_PARAM)?.AsValueString() ??
                                   "Sem designação";
                designacoes.Add(designacao);
            }

            comboDesignacao.Items.Add("Todas as designações");
            foreach (string designacao in designacoes.OrderBy(d => d))
            {
                comboDesignacao.Items.Add(designacao);
            }

            if (comboDesignacao.Items.Count > 0)
            {
                comboDesignacao.SelectedIndex = 0;
            }
        }

        private void ActualizarNiveis(List<Element> elementos)
        {
            listNiveis.Items.Clear();

            HashSet<string> niveisSet = new HashSet<string>();
            foreach (Element elemento in elementos)
            {
                Parameter paramNivel = elemento.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM) ??
                                      elemento.get_Parameter(BuiltInParameter.SCHEDULE_LEVEL_PARAM);

                if (paramNivel != null)
                {
                    Element nivel = doc.GetElement(paramNivel.AsElementId());
                    if (nivel != null)
                    {
                        niveisSet.Add(nivel.Name);
                    }
                }
            }

            foreach (string nivel in niveisSet.OrderBy(n => n))
            {
                listNiveis.Items.Add(nivel, true);
            }
        }

        private void ActualizarCalculosAutomaticos()
        {
            if (!checkComprimentoAuto.Checked && !checkAmarracaoAutomatica.Checked) return;

            try
            {
                double diametroMedio = 16; // Padrão
                if (listViewVaroes.Items.Count > 0)
                {
                    double somaDiametros = 0;
                    foreach (ListViewItem item in listViewVaroes.Items)
                    {
                        if (double.TryParse(item.SubItems[1].Text, out double d))
                        {
                            somaDiametros += d;
                        }
                    }
                    diametroMedio = somaDiametros / listViewVaroes.Items.Count;
                }

                double multiplicador = (double)numMultiplicadorAmarracao.Value;
                double alturaBase = 3000; // mm padrão

                if (!checkComprimentoAuto.Checked && double.TryParse(textComprimentoBase.Text, out double alturaManual))
                {
                    alturaBase = alturaManual * 1000; // Converter m para mm
                }

                double amarracao = (multiplicador * diametroMedio) / 1000.0; // Converter mm para m
                double comprimentoTotal = (alturaBase / 1000.0) + (2 * amarracao);

                labelComprimentoCalculado.Text = $"Comprimento calculado: {comprimentoTotal:F2} m " +
                                               $"(Base: {alturaBase / 1000.0:F2}m + Amarração: {2 * amarracao:F2}m)";
            }
            catch
            {
                labelComprimentoCalculado.Text = "Comprimento calculado: Erro no cálculo";
            }
        }

        private void ActualizarContagem()
        {
            try
            {
                List<Element> elementos = ObterElementosParaProcessar();
                labelContagem.Text = $"Elementos encontrados: {elementos.Count}";
                ActualizarInformacoesTipo(elementos);
            }
            catch (Exception ex)
            {
                labelContagem.Text = $"Erro na contagem: {ex.Message}";
            }
        }

        private bool ValidarEntrada()
        {
            // Validar varões
            if (listViewVaroes.Items.Count == 0)
            {
                MessageBox.Show("Adicione pelo menos um tipo de varão.", "Erro de Validação",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Validar quantidade mínima para pilares
            if (radioPilares.Checked)
            {
                int totalVaroes = 0;
                foreach (ListViewItem item in listViewVaroes.Items)
                {
                    if (int.TryParse(item.SubItems[0].Text, out int quantidade))
                    {
                        totalVaroes += quantidade;
                    }
                }

                if (totalVaroes < 4)
                {
                    MessageBox.Show("A quantidade mínima de varões para pilares é 4.", "Erro de Validação",
                                   MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            // Validar comprimento se manual
            if (!checkComprimentoAuto.Checked)
            {
                if (!double.TryParse(textComprimentoBase.Text, out double comprimento) || comprimento <= 0)
                {
                    MessageBox.Show("Por favor introduza um comprimento base válido.", "Erro de Validação",
                                   MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            // Validar estribos
            if (listViewEstribos.Items.Count == 0)
            {
                MessageBox.Show("Adicione pelo menos uma configuração de estribo.", "Erro de Validação",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private List<Element> ObterElementosParaProcessar()
        {
            List<Element> elementos = new List<Element>();

            if (checkSeleccaoActual.Checked)
            {
                ICollection<ElementId> seleccaoIds = uidoc.Selection.GetElementIds();
                foreach (ElementId id in seleccaoIds)
                {
                    Element elemento = doc.GetElement(id);
                    if (elemento != null && detectorElementos.EElementoEstruturalValido(elemento))
                    {
                        elementos.Add(elemento);
                    }
                }
            }
            else
            {
                TipoElementoEstruturalEnum tipo = ObterTipoElementoSeleccionado();
                elementos = detectorElementos.DetectarElementos(tipo);

                // Aplicar filtros
                if (comboDesignacao.SelectedIndex > 0)
                {
                    string designacaoSeleccionada = comboDesignacao.SelectedItem.ToString();
                    elementos = elementos.Where(e =>
                    {
                        string designacao = e.get_Parameter(BuiltInParameter.ELEM_FAMILY_PARAM)?.AsValueString() ??
                                           e.get_Parameter(BuiltInParameter.ELEM_TYPE_PARAM)?.AsValueString() ??
                                           "Sem designação";
                        return designacao == designacaoSeleccionada;
                    }).ToList();
                }

                List<string> niveisSeleccionados = new List<string>();
                for (int i = 0; i < listNiveis.Items.Count; i++)
                {
                    if (listNiveis.GetItemChecked(i))
                    {
                        niveisSeleccionados.Add(listNiveis.Items[i].ToString());
                    }
                }

                if (niveisSeleccionados.Count > 0)
                {
                    elementos = elementos.Where(e =>
                    {
                        Parameter paramNivel = e.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM) ??
                                              e.get_Parameter(BuiltInParameter.SCHEDULE_LEVEL_PARAM);

                        if (paramNivel != null)
                        {
                            Element nivel = doc.GetElement(paramNivel.AsElementId());
                            return nivel != null && niveisSeleccionados.Contains(nivel.Name);
                        }
                        return false;
                    }).ToList();
                }
            }

            return elementos;
        }

        private ArmConfigExec CriarConfiguracao()
        {
            ArmConfigExec config = new ArmConfigExec(doc);

            // Limpar configurações padrão
            config.Varoes.Clear();
            config.Estribos.Clear();

            // Adicionar varões configurados
            foreach (ListViewItem item in listViewVaroes.Items)
            {
                int quantidade = int.Parse(item.SubItems[0].Text);
                double diametro = double.Parse(item.SubItems[1].Text);
                string posicao = item.SubItems[2].Text;

                config.Varoes.Add(new ArmVar(quantidade, diametro) { TipoArmadura = posicao });
            }

            // Configurar distribuição
            if (comboTipoDistribuicao.SelectedItem != null)
            {
                config.TipoDistribuicao = (TipoDistribuicaoArmaduraEnum)Enum.Parse(
                    typeof(TipoDistribuicaoArmaduraEnum), comboTipoDistribuicao.SelectedItem.ToString());
            }

            // Configurar comprimento
            config.ComprimentoAuto = checkComprimentoAuto.Checked;
            if (!config.ComprimentoAuto && double.TryParse(textComprimentoBase.Text, out double comprimentoBase))
            {
                config.ComprimentoBase = comprimentoBase * 1000; // Converter m para mm
            }

            // Configurar amarração
            config.AmarracaoAuto = checkAmarracaoAutomatica.Checked;
            config.MultAmarracao = (double)numMultiplicadorAmarracao.Value;
            config.TipoAmarracao = comboTipoAmarracao.SelectedItem?.ToString() ?? "Automático";
            config.DeteccaoAmarracaoAuto = checkDeteccaoAutomaticaAmarracao.Checked;

            // Adicionar estribos configurados
            foreach (ListViewItem item in listViewEstribos.Items)
            {
                double diametro = double.Parse(item.SubItems[0].Text);
                double espacamento = double.Parse(item.SubItems[1].Text);
                bool alternado = item.SubItems[2].Text == "Sim";

                config.Estribos.Add(new ArmStirrup(diametro, espacamento) { Alternado = alternado });
            }

            // Configurar definições
            config.Defs = gestorDefinicoes.ObterDefinicoes();

            return config;
        }

        private TipoElementoEstruturalEnum ObterTipoElementoSeleccionado()
        {
            return TipoElementoEstruturalEnum.Vigas;
        }

        private void ExecutarColocacaoArmaduras(List<Element> elementos)
        {
            progressBar.Visible = true;
            progressBar.Value = 0;

            ArmConfigExec config = CriarConfiguracao();

            // Move these declarations OUTSIDE the using block
            int totalElementos = elementos.Count;
            int elementosProcessados = 0;
            int elementosComSucesso = 0;  // ← Move this line here
            List<string> erros = new List<string>(); // ← Move this line here

            using (Transaction trans = new Transaction(doc, "Colocação de Armaduras - Pilares e Vigas"))
            {
                trans.Start();

                foreach (Element elemento in elementos)
                {
                    try
                    {
                        bool sucesso = config.ColocarArmadura(elemento);
                        if (sucesso)
                            elementosComSucesso++;
                        else
                            erros.Add($"Elemento {elemento.Id}: Falha na execução");

                        elementosProcessados++;
                        progressBar.Value = (elementosProcessados * 100) / totalElementos;
                        Application.DoEvents();
                    }
                    catch (Exception ex)
                    {
                        erros.Add($"Elemento {elemento.Id}: {ex.Message}");
                        elementosProcessados++;
                        progressBar.Value = (elementosProcessados * 100) / totalElementos;
                        Application.DoEvents();
                    }
                }

                trans.Commit();
            }

            progressBar.Visible = false;

            // Mostrar resultados
            string mensagemResultado = $"Processo concluído!\n\n" +
                                     $"Elementos processados: {elementos.Count}\n" +
                                     $"Armaduras colocadas com sucesso: {elementosComSucesso}\n" +
                                     $"Elementos com erro: {erros.Count}";

            if (erros.Count > 0 && erros.Count <= 5)
            {
                mensagemResultado += "\n\nErros:\n" + string.Join("\n", erros.Take(5));
            }
            else if (erros.Count > 5)
            {
                mensagemResultado += $"\n\nPrimeiros 5 erros:\n" + string.Join("\n", erros.Take(5));
                mensagemResultado += $"\n... e mais {erros.Count - 5} erros.";
            }

            MessageBox.Show(mensagemResultado,
                elementosComSucesso > 0 ? "Sucesso" : "Aviso",
                MessageBoxButtons.OK,
                elementosComSucesso > 0 ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

            if (elementosComSucesso > 0)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}
