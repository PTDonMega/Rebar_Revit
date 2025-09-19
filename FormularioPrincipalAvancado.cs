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
using Form = System.Windows.Forms.Form;

namespace MacroArmaduraAvancado
{
    public partial class FormularioPrincipalAvancado : Form
    {
        private Document doc;
        private UIDocument uidoc;
        private DetectorElementosAvancado detectorElementos;
        private GestorDefinicoesAvancado gestorDefinicoes;
        private CalculadorAmarracao calculadorAmarracao;

        // Adicionamos a declaração do visualizador que será criado programaticamente
        private VisualizadorArmaduraViga visualizadorArmadura;

        public FormularioPrincipalAvancado(Document documento, UIDocument uiDocumento)
        {
            doc = documento;
            uidoc = uiDocumento;
            detectorElementos = new DetectorElementosAvancado(doc);
            gestorDefinicoes = new GestorDefinicoesAvancado();
            calculadorAmarracao = new CalculadorAmarracao();

            InitializeComponent();
            InicializarFormulario();
            CarregarDados();
        }

        private void InicializarFormulario()
        {
            // Configurar valores padrão
            comboTipoDistribuicao.SelectedIndex = 0;
            comboTipoAmarracao.SelectedItem = "Automático";
            
            // Criar e adicionar o visualizador programaticamente
            visualizadorArmadura = new VisualizadorArmaduraViga();
            visualizadorArmadura.Location = new System.Drawing.Point(20, 80);
            visualizadorArmadura.Size = new System.Drawing.Size(520, 350);
            visualizadorArmadura.BackColor = System.Drawing.Color.White;
            visualizadorArmadura.BorderStyle = BorderStyle.FixedSingle;
            visualizadorArmadura.ModoEdicao = false;
            visualizadorArmadura.MostrarCorteTransversal = true;
            
            // Conectar eventos do visualizador
            visualizadorArmadura.ArmaduraEditada += VisualizadorArmadura_ArmaduraEditada;
            visualizadorArmadura.PontoArmaduraSelecionado += VisualizadorArmadura_PontoSelecionado;
            
            // Adicionar o visualizador ao grupo
            groupVisualizador.Controls.Add(visualizadorArmadura);
            
            AdicionarConfigPadrao();
        }

        private void AdicionarConfigPadrao()
        {
            // Configuração padrão para vigas
            ListViewItem itemSuperior = new ListViewItem(new string[] { "3", "16", "Superior" });
            listViewVaroes.Items.Add(itemSuperior);
            
            ListViewItem itemInferior = new ListViewItem(new string[] { "4", "20", "Inferior" });
            listViewVaroes.Items.Add(itemInferior);

            // Estribos padrão
            ListViewItem itemEstribo = new ListViewItem(new string[] { "8", "150", "Uniforme" });
            listViewEstribos.Items.Add(itemEstribo);

            // Atualizar visualizador com configuração padrão
            AtualizarVisualizador();
        }

        private void CarregarDados()
        {
            ActualizarElementos();
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

        private void ButtonDefinicoes_Click(object sender, EventArgs e)
        {
            using (DefinicoesAvancadas formDefinicoes = new DefinicoesAvancadas(gestorDefinicoes))
            {
                if (formDefinicoes.ShowDialog() == DialogResult.OK)
                {
                    // Atualizar visualizador com novas definições
                    AtualizarVisualizador();
                }
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
                    MessageBox.Show("Nenhuma viga encontrada para pré-visualização.", "Aviso",
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
                    MessageBox.Show("Nenhuma viga encontrada para processar.", "Aviso",
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult resultado = MessageBox.Show(
                    $"Confirma a execução da colocação de armaduras em {elementosProcessar.Count} vigas?",
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

        // Novos event handlers para o visualizador
        private void DimensoesViga_ValueChanged(object sender, EventArgs e)
        {
            AtualizarVisualizador();
        }

        private void ButtonAlternarVista_Click(object sender, EventArgs e)
        {
            visualizadorArmadura.MostrarCorteTransversal = !visualizadorArmadura.MostrarCorteTransversal;
            buttonAlternarVista.Text = visualizadorArmadura.MostrarCorteTransversal ? "🔄 Longi." : "🔄 Corte";
        }

        private void ButtonModoEdicao_Click(object sender, EventArgs e)
        {
            visualizadorArmadura.ModoEdicao = !visualizadorArmadura.ModoEdicao;
            buttonModoEdicao.Text = visualizadorArmadura.ModoEdicao ? "✏ Sair" : "✏ Editar";
            buttonModoEdicao.BackColor = visualizadorArmadura.ModoEdicao ? 
                System.Drawing.Color.LightCoral : System.Drawing.SystemColors.Control;
        }

        private void VisualizadorArmadura_ArmaduraEditada(object sender, ArmaduraEditadaEventArgs e)
        {
            // Sincronizar com ListView
            AtualizarListViewVaroes();
            
            labelInfoVisualizador.Text = $"Armadura {e.TipoOperacao.ToLower()}: {e.VaraoModificado}";
            labelInfoVisualizador.ForeColor = System.Drawing.Color.Green;
            
            // Reset da cor após 3 segundos
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 3000;
            timer.Tick += (s, args) => {
                labelInfoVisualizador.Text = "Clique duplo para editar. Clique direito para menu.";
                labelInfoVisualizador.ForeColor = System.Drawing.Color.DarkBlue;
                timer.Stop();
            };
            timer.Start();
        }

        private void VisualizadorArmadura_PontoSelecionado(object sender, VisualizadorArmaduraViga.PontoArmadura ponto)
        {
            labelInfoVisualizador.Text = $"Selecionado: {ponto.Tipo} Ø{ponto.Diametro}mm";
            labelInfoVisualizador.ForeColor = System.Drawing.Color.DarkBlue;
        }

        private void AtualizarVisualizador()
        {
            if (visualizadorArmadura?.InformacaoViga == null) return;

            visualizadorArmadura.InformacaoViga.Comprimento = (double)numComprimentoViga.Value;
            visualizadorArmadura.InformacaoViga.Altura = (double)numAlturaViga.Value;
            visualizadorArmadura.InformacaoViga.Largura = (double)numLarguraViga.Value;
            visualizadorArmadura.InformacaoViga.Cobertura = gestorDefinicoes?.ObterDefinicoes()?.CoberturaVigas ?? 25;
            visualizadorArmadura.InformacaoViga.MultiplicadorAmarracao = numMultiplicadorAmarracao?.Value != null ? (double)numMultiplicadorAmarracao.Value : 50.0;
            visualizadorArmadura.InformacaoViga.AmarracaoAutomatica = checkAmarracaoAutomatica?.Checked ?? true;

            // Sincronizar varões do ListView
            visualizadorArmadura.InformacaoViga.VaroesLongitudinais.Clear();
            foreach (ListViewItem item in listViewVaroes.Items)
            {
                var varao = new ArmVar(
                    int.Parse(item.SubItems[0].Text),
                    double.Parse(item.SubItems[1].Text))
                {
                    TipoArmadura = item.SubItems[2].Text
                };
                visualizadorArmadura.InformacaoViga.VaroesLongitudinais.Add(varao);
            }

            // Sincronizar estribos
            visualizadorArmadura.InformacaoViga.Estribos.Clear();
            foreach (ListViewItem item in listViewEstribos.Items)
            {
                var estribo = new ArmStirrup(
                    double.Parse(item.SubItems[0].Text),
                    double.Parse(item.SubItems[1].Text))
                {
                    Alternado = item.SubItems[2].Text == "Alternado"
                };
                visualizadorArmadura.InformacaoViga.Estribos.Add(estribo);
            }

            visualizadorArmadura.AtualizarVisualizacao();
        }

        private void AtualizarListViewVaroes()
        {
            listViewVaroes.Items.Clear();

            foreach (var varao in visualizadorArmadura.InformacaoViga.VaroesLongitudinais)
            {
                ListViewItem item = new ListViewItem(new string[] {
                    varao.Quantidade.ToString(),
                    varao.Diametro.ToString(),
                    varao.TipoArmadura
                });
                listViewVaroes.Items.Add(item);
            }
        }

        private void CheckAmarracaoAutomatica_CheckedChanged(object sender, EventArgs e)
        {
            numMultiplicadorAmarracao.Enabled = checkAmarracaoAutomatica.Checked;
            comboTipoAmarracao.Enabled = !checkAmarracaoAutomatica.Checked;
            AtualizarVisualizador();
        }

        private void NumMultiplicadorAmarracao_ValueChanged(object sender, EventArgs e)
        {
            AtualizarVisualizador();
        }

        // Métodos de interface existentes modificados para sincronizar com visualizador
        private void ButtonAdicionarVarao_Click(object sender, EventArgs e)
        {
            try
            {
                using (var form = new ConfiguracaoVarao(false)) // false = viga
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        ListViewItem item = new ListViewItem(new string[] {
                            form.QuantidadeValue.ToString(),
                            form.DiametroValue.ToString(),
                            form.PosicaoValue
                        });
                        listViewVaroes.Items.Add(item);
                        AtualizarVisualizador(); // Sincronizar visualizador
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao adicionar varão: " + ex.Message, "Erro",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ButtonRemoverVarao_Click(object sender, EventArgs e)
        {
            if (listViewVaroes.SelectedItems.Count > 0)
            {
                listViewVaroes.SelectedItems[0].Remove();
                AtualizarVisualizador(); // Sincronizar visualizador
            }
            else
            {
                MessageBox.Show("Seleccione uma armadura para remover.", "Aviso",
                               MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ButtonAdicionarEstribo_Click(object sender, EventArgs e)
        {
            try
            {
                using (var form = new ConfiguracaoEstribo())
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        string tipo = form.AlternadoValue ? "Alternado" : "Uniforme";
                        ListViewItem item = new ListViewItem(new string[] {
                            form.DiametroValue.ToString(),
                            form.EspacamentoValue.ToString(),
                            tipo
                        });
                        listViewEstribos.Items.Add(item);
                        AtualizarVisualizador(); // Sincronizar visualizador
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao adicionar estribo: " + ex.Message, "Erro",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ButtonRemoverEstribo_Click(object sender, EventArgs e)
        {
            if (listViewEstribos.SelectedItems.Count > 0)
            {
                listViewEstribos.SelectedItems[0].Remove();
                AtualizarVisualizador(); // Sincronizar visualizador
            }
            else
            {
                MessageBox.Show("Seleccione um estribo para remover.", "Aviso",
                               MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Métodos principais
        private void ActualizarElementos()
        {
            try
            {
                // Detectar apenas vigas
                List<Element> elementos = detectorElementos.DetectarElementos(TipoElementoEstruturalEnum.Vigas);

                ActualizarDesignacoes(elementos);
                ActualizarNiveis(elementos);

                labelContagem.Text = $"Vigas encontradas: {elementos.Count}";
                ActualizarInformacoesTipo(elementos);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao actualizar vigas: " + ex.Message,
                               "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ActualizarInformacoesTipo(List<Element> elementos)
        {
            if (elementos.Count > 0)
            {
                var analise = detectorElementos.AnalisarElementos(elementos);
                string infoTexto = $"Análise: {analise.TotalElementos} vigas detectadas";
                labelInfoElementos.Text = infoTexto;
            }
            else
            {
                labelInfoElementos.Text = "Nenhuma viga detectada";
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

            comboDesignacao.Items.Add("Todos os tipos");
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

        private void ActualizarContagem()
        {
            try
            {
                List<Element> elementos = ObterElementosParaProcessar();
                labelContagem.Text = $"Vigas encontradas: {elementos.Count}";
                ActualizarInformacoesTipo(elementos);
            }
            catch (Exception ex)
            {
                labelContagem.Text = $"Erro na contagem: {ex.Message}";
            }
        }

        private bool ValidarEntrada()
        {
            // Validar armadura longitudinal
            if (listViewVaroes.Items.Count == 0)
            {
                MessageBox.Show("Adicione pelo menos uma armadura longitudinal.", "Erro de Validação",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
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
                elementos = detectorElementos.DetectarElementos(TipoElementoEstruturalEnum.Vigas);

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
            config.TipoElemento = TipoElementoEstruturalEnum.Vigas;

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

            // Configurar amarração
            config.AmarracaoAuto = checkAmarracaoAutomatica.Checked;
            config.MultAmarracao = (double)numMultiplicadorAmarracao.Value;
            config.TipoAmarracao = comboTipoAmarracao.SelectedItem?.ToString() ?? "Automático";

            // Adicionar estribos configurados
            foreach (ListViewItem item in listViewEstribos.Items)
            {
                double diametro = double.Parse(item.SubItems[0].Text);
                double espacamento = double.Parse(item.SubItems[1].Text);
                bool alternado = item.SubItems[2].Text == "Alternado";

                config.Estribos.Add(new ArmStirrup(diametro, espacamento) { Alternado = alternado });
            }

            // Configurar definições
            config.Defs = gestorDefinicoes.ObterDefinicoes();

            return config;
        }

        private void ExecutarColocacaoArmaduras(List<Element> elementos)
        {
            progressBar.Visible = true;
            progressBar.Value = 0;

            ArmConfigExec config = CriarConfiguracao();

            int totalElementos = elementos.Count;
            int elementosProcessados = 0;
            int elementosComSucesso = 0;
            List<string> erros = new List<string>();

            using (Transaction trans = new Transaction(doc, "Colocação de Armaduras - Vigas"))
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
                            erros.Add($"Viga {elemento.Id}: Falha na execução");

                        elementosProcessados++;
                        progressBar.Value = (elementosProcessados * 100) / totalElementos;
                        Application.DoEvents();
                    }
                    catch (Exception ex)
                    {
                        erros.Add($"Viga {elemento.Id}: {ex.Message}");
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
                                     $"Vigas processadas: {elementos.Count}\n" +
                                     $"Armaduras colocadas com sucesso: {elementosComSucesso}\n" +
                                     $"Vigas com erro: {erros.Count}";

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
