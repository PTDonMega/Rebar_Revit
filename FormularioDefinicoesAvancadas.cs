using System;
using System.Windows.Forms;

namespace MacroArmaduraAvancado
{
    /// <summary>
    /// Formulário de definições avançadas
    /// </summary>
    public partial class FormularioDefinicoesAvancadas : Form
    {
        private GestorDefinicoesAvancado gestor;
        private DefinicoesProjectoAvancadas definicoes;

        private NumericUpDown numCoberturaPilares;
        private NumericUpDown numCoberturaVigas;
        private NumericUpDown numCoberturaFundacoes;
        private NumericUpDown numCoberturaLajes;
        private NumericUpDown numEspacamentoMinimoEstribos;
        private NumericUpDown numEspacamentoMaximoEstribos;
        private CheckBox checkValidarEurocodigo;
        private CheckBox checkGerarRelatorioQuantidades;

        public FormularioDefinicoesAvancadas(GestorDefinicoesAvancado gestorDefinicoes)
        {
            gestor = gestorDefinicoes;
            definicoes = gestor.ObterDefinicoes();
            
            InicializarComponentes();
            CarregarDefinicoes();
        }

        private void InicializarComponentes()
        {
            this.Text = "Definições Avançadas do Projecto";
            this.Size = new System.Drawing.Size(500, 400);
            this.StartPosition = FormStartPosition.CenterParent;

            // Coberturas
            Label labelCoberturas = new Label();
            labelCoberturas.Text = "Coberturas (mm):";
            labelCoberturas.Location = new System.Drawing.Point(20, 20);
            labelCoberturas.Size = new System.Drawing.Size(150, 20);

            Label labelCoberturaPilares = new Label();
            labelCoberturaPilares.Text = "Pilares:";
            labelCoberturaPilares.Location = new System.Drawing.Point(40, 50);
            labelCoberturaPilares.Size = new System.Drawing.Size(80, 20);

            numCoberturaPilares = new NumericUpDown();
            numCoberturaPilares.Location = new System.Drawing.Point(130, 48);
            numCoberturaPilares.Size = new System.Drawing.Size(60, 20);
            numCoberturaPilares.Minimum = 15;
            numCoberturaPilares.Maximum = 100;
            numCoberturaPilares.Value = 40;

            Label labelCoberturaVigas = new Label();
            labelCoberturaVigas.Text = "Vigas:";
            labelCoberturaVigas.Location = new System.Drawing.Point(40, 80);
            labelCoberturaVigas.Size = new System.Drawing.Size(80, 20);

            numCoberturaVigas = new NumericUpDown();
            numCoberturaVigas.Location = new System.Drawing.Point(130, 78);
            numCoberturaVigas.Size = new System.Drawing.Size(60, 20);
            numCoberturaVigas.Minimum = 15;
            numCoberturaVigas.Maximum = 100;
            numCoberturaVigas.Value = 25;

            Label labelCoberturaFundacoes = new Label();
            labelCoberturaFundacoes.Text = "Fundações:";
            labelCoberturaFundacoes.Location = new System.Drawing.Point(40, 110);
            labelCoberturaFundacoes.Size = new System.Drawing.Size(80, 20);

            numCoberturaFundacoes = new NumericUpDown();
            numCoberturaFundacoes.Location = new System.Drawing.Point(130, 108);
            numCoberturaFundacoes.Size = new System.Drawing.Size(60, 20);
            numCoberturaFundacoes.Minimum = 15;
            numCoberturaFundacoes.Maximum = 100;
            numCoberturaFundacoes.Value = 50;

            Label labelCoberturaLajes = new Label();
            labelCoberturaLajes.Text = "Lajes:";
            labelCoberturaLajes.Location = new System.Drawing.Point(40, 140);
            labelCoberturaLajes.Size = new System.Drawing.Size(80, 20);

            numCoberturaLajes = new NumericUpDown();
            numCoberturaLajes.Location = new System.Drawing.Point(130, 138);
            numCoberturaLajes.Size = new System.Drawing.Size(60, 20);
            numCoberturaLajes.Minimum = 15;
            numCoberturaLajes.Maximum = 100;
            numCoberturaLajes.Value = 20;

            // Estribos
            Label labelEstribos = new Label();
            labelEstribos.Text = "Estribos (mm):";
            labelEstribos.Location = new System.Drawing.Point(20, 180);
            labelEstribos.Size = new System.Drawing.Size(150, 20);

            Label labelEspacamentoMinimo = new Label();
            labelEspacamentoMinimo.Text = "Esp. mínimo:";
            labelEspacamentoMinimo.Location = new System.Drawing.Point(40, 210);
            labelEspacamentoMinimo.Size = new System.Drawing.Size(80, 20);

            numEspacamentoMinimoEstribos = new NumericUpDown();
            numEspacamentoMinimoEstribos.Location = new System.Drawing.Point(130, 208);
            numEspacamentoMinimoEstribos.Size = new System.Drawing.Size(60, 20);
            numEspacamentoMinimoEstribos.Minimum = 25;
            numEspacamentoMinimoEstribos.Maximum = 200;
            numEspacamentoMinimoEstribos.Value = 50;

            Label labelEspacamentoMaximo = new Label();
            labelEspacamentoMaximo.Text = "Esp. máximo:";
            labelEspacamentoMaximo.Location = new System.Drawing.Point(40, 240);
            labelEspacamentoMaximo.Size = new System.Drawing.Size(80, 20);

            numEspacamentoMaximoEstribos = new NumericUpDown();
            numEspacamentoMaximoEstribos.Location = new System.Drawing.Point(130, 238);
            numEspacamentoMaximoEstribos.Size = new System.Drawing.Size(60, 20);
            numEspacamentoMaximoEstribos.Minimum = 50;
            numEspacamentoMaximoEstribos.Maximum = 500;
            numEspacamentoMaximoEstribos.Value = 300;

            // Validação
            Label labelValidacao = new Label();
            labelValidacao.Text = "Validação:";
            labelValidacao.Location = new System.Drawing.Point(250, 20);
            labelValidacao.Size = new System.Drawing.Size(150, 20);

            checkValidarEurocodigo = new CheckBox();
            checkValidarEurocodigo.Text = "Validar Eurocódigo";
            checkValidarEurocodigo.Location = new System.Drawing.Point(270, 50);
            checkValidarEurocodigo.Size = new System.Drawing.Size(150, 20);
            checkValidarEurocodigo.Checked = true;

            // Produtividade
            Label labelProdutividade = new Label();
            labelProdutividade.Text = "Produtividade:";
            labelProdutividade.Location = new System.Drawing.Point(250, 80);
            labelProdutividade.Size = new System.Drawing.Size(150, 20);

            checkGerarRelatorioQuantidades = new CheckBox();
            checkGerarRelatorioQuantidades.Text = "Gerar relatório quantidades";
            checkGerarRelatorioQuantidades.Location = new System.Drawing.Point(270, 110);
            checkGerarRelatorioQuantidades.Size = new System.Drawing.Size(180, 20);
            checkGerarRelatorioQuantidades.Checked = true;

            // Botões
            Button buttonOK = new Button();
            buttonOK.Text = "OK";
            buttonOK.Location = new System.Drawing.Point(300, 300);
            buttonOK.Size = new System.Drawing.Size(75, 30);
            buttonOK.Click += ButtonOK_Click;

            Button buttonCancelar = new Button();
            buttonCancelar.Text = "Cancelar";
            buttonCancelar.Location = new System.Drawing.Point(390, 300);
            buttonCancelar.Size = new System.Drawing.Size(75, 30);
            buttonCancelar.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            this.Controls.AddRange(new Control[] {
                labelCoberturas, labelCoberturaPilares, numCoberturaPilares,
                labelCoberturaVigas, numCoberturaVigas,
                labelCoberturaFundacoes, numCoberturaFundacoes,
                labelCoberturaLajes, numCoberturaLajes,
                labelEstribos, labelEspacamentoMinimo, numEspacamentoMinimoEstribos,
                labelEspacamentoMaximo, numEspacamentoMaximoEstribos,
                labelValidacao, checkValidarEurocodigo,
                labelProdutividade, checkGerarRelatorioQuantidades,
                buttonOK, buttonCancelar
            });
        }

        private void CarregarDefinicoes()
        {
            numCoberturaPilares.Value = (decimal)definicoes.CoberturaPilares;
            numCoberturaVigas.Value = (decimal)definicoes.CoberturaVigas;
            numCoberturaFundacoes.Value = (decimal)definicoes.CoberturaFundacoes;
            numCoberturaLajes.Value = (decimal)definicoes.CoberturaLajes;
            numEspacamentoMinimoEstribos.Value = (decimal)definicoes.EspacamentoMinimoEstribos;
            numEspacamentoMaximoEstribos.Value = (decimal)definicoes.EspacamentoMaximoEstribos;
            checkValidarEurocodigo.Checked = definicoes.ValidarEurocodigo;
            checkGerarRelatorioQuantidades.Checked = definicoes.GerarRelatorioQuantidades;
        }

        private void ButtonOK_Click(object sender, EventArgs e)
        {
            // Guardar definições
            definicoes.CoberturaPilares = (double)numCoberturaPilares.Value;
            definicoes.CoberturaVigas = (double)numCoberturaVigas.Value;
            definicoes.CoberturaFundacoes = (double)numCoberturaFundacoes.Value;
            definicoes.CoberturaLajes = (double)numCoberturaLajes.Value;
            definicoes.EspacamentoMinimoEstribos = (double)numEspacamentoMinimoEstribos.Value;
            definicoes.EspacamentoMaximoEstribos = (double)numEspacamentoMaximoEstribos.Value;
            definicoes.ValidarEurocodigo = checkValidarEurocodigo.Checked;
            definicoes.GerarRelatorioQuantidades = checkGerarRelatorioQuantidades.Checked;

            gestor.GuardarDefinicoes(definicoes);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}