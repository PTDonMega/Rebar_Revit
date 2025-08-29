using System;
using System.Windows.Forms;

namespace MacroArmaduraAvancado
{
    public partial class FormularioConfiguracaoVarao : Form
    {
        private NumericUpDown numQuantidade;
        private NumericUpDown numDiametro;
        private ComboBox comboTipoArmadura;
        private Button buttonOK;
        private Button buttonCancelar;

        public int Quantidade => (int)numQuantidade.Value;
        public double Diametro => (double)numDiametro.Value;
        public string TipoArmadura => comboTipoArmadura.SelectedItem?.ToString() ?? "Principal";

        public FormularioConfiguracaoVarao()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Configuração de Varão";
            this.Size = new System.Drawing.Size(300, 200);
            this.StartPosition = FormStartPosition.CenterParent;

            Label labelQuantidade = new Label();
            labelQuantidade.Text = "Quantidade:";
            labelQuantidade.Location = new System.Drawing.Point(20, 20);
            labelQuantidade.Size = new System.Drawing.Size(80, 20);

            numQuantidade = new NumericUpDown();
            numQuantidade.Location = new System.Drawing.Point(120, 18);
            numQuantidade.Size = new System.Drawing.Size(60, 20);
            numQuantidade.Minimum = 1;
            numQuantidade.Maximum = 100;
            numQuantidade.Value = 4;

            Label labelDiametro = new Label();
            labelDiametro.Text = "Diâmetro (mm):";
            labelDiametro.Location = new System.Drawing.Point(20, 50);
            labelDiametro.Size = new System.Drawing.Size(80, 20);

            numDiametro = new NumericUpDown();
            numDiametro.Location = new System.Drawing.Point(120, 48);
            numDiametro.Size = new System.Drawing.Size(60, 20);
            numDiametro.Minimum = 6;
            numDiametro.Maximum = 40;
            numDiametro.Value = 16;
            numDiametro.Increment = 2;

            Label labelTipo = new Label();
            labelTipo.Text = "Tipo:";
            labelTipo.Location = new System.Drawing.Point(20, 80);
            labelTipo.Size = new System.Drawing.Size(80, 20);

            comboTipoArmadura = new ComboBox();
            comboTipoArmadura.Location = new System.Drawing.Point(120, 78);
            comboTipoArmadura.Size = new System.Drawing.Size(120, 20);
            comboTipoArmadura.DropDownStyle = ComboBoxStyle.DropDownList;
            comboTipoArmadura.Items.AddRange(new string[] { "Principal", "Secundária", "Distribuição" });
            comboTipoArmadura.SelectedIndex = 0;

            buttonOK = new Button();
            buttonOK.Text = "OK";
            buttonOK.DialogResult = DialogResult.OK;
            buttonOK.Location = new System.Drawing.Point(60, 120);
            buttonOK.Size = new System.Drawing.Size(75, 25);

            buttonCancelar = new Button();
            buttonCancelar.Text = "Cancelar";
            buttonCancelar.DialogResult = DialogResult.Cancel;
            buttonCancelar.Location = new System.Drawing.Point(150, 120);
            buttonCancelar.Size = new System.Drawing.Size(75, 25);

            this.Controls.AddRange(new Control[] {
                labelQuantidade, numQuantidade,
                labelDiametro, numDiametro,
                labelTipo, comboTipoArmadura,
                buttonOK, buttonCancelar
            });
        }
    }

    public partial class FormularioConfiguracaoEstribo : Form
    {
        private NumericUpDown numDiametro;
        private NumericUpDown numEspacamento;
        private CheckBox checkAlternado;
        private Button buttonOK;
        private Button buttonCancelar;

        public double Diametro => (double)numDiametro.Value;
        public double Espacamento => (double)numEspacamento.Value;
        public bool Alternado => checkAlternado.Checked;

        public FormularioConfiguracaoEstribo()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Configuração de Estribo";
            this.Size = new System.Drawing.Size(300, 180);
            this.StartPosition = FormStartPosition.CenterParent;

            Label labelDiametro = new Label();
            labelDiametro.Text = "Diâmetro (mm):";
            labelDiametro.Location = new System.Drawing.Point(20, 20);
            labelDiametro.Size = new System.Drawing.Size(80, 20);

            numDiametro = new NumericUpDown();
            numDiametro.Location = new System.Drawing.Point(120, 18);
            numDiametro.Size = new System.Drawing.Size(60, 20);
            numDiametro.Minimum = 6;
            numDiametro.Maximum = 16;
            numDiametro.Value = 8;
            numDiametro.Increment = 2;

            Label labelEspacamento = new Label();
            labelEspacamento.Text = "Espaçamento (mm):";
            labelEspacamento.Location = new System.Drawing.Point(20, 50);
            labelEspacamento.Size = new System.Drawing.Size(100, 20);

            numEspacamento = new NumericUpDown();
            numEspacamento.Location = new System.Drawing.Point(120, 48);
            numEspacamento.Size = new System.Drawing.Size(60, 20);
            numEspacamento.Minimum = 50;
            numEspacamento.Maximum = 500;
            numEspacamento.Value = 200;
            numEspacamento.Increment = 25;

            checkAlternado = new CheckBox();
            checkAlternado.Text = "Estribo alternado";
            checkAlternado.Location = new System.Drawing.Point(20, 80);
            checkAlternado.Size = new System.Drawing.Size(150, 20);

            buttonOK = new Button();
            buttonOK.Text = "OK";
            buttonOK.DialogResult = DialogResult.OK;
            buttonOK.Location = new System.Drawing.Point(60, 110);
            buttonOK.Size = new System.Drawing.Size(75, 25);

            buttonCancelar = new Button();
            buttonCancelar.Text = "Cancelar";
            buttonCancelar.DialogResult = DialogResult.Cancel;
            buttonCancelar.Location = new System.Drawing.Point(150, 110);
            buttonCancelar.Size = new System.Drawing.Size(75, 25);

            this.Controls.AddRange(new Control[] {
                labelDiametro, numDiametro,
                labelEspacamento, numEspacamento,
                checkAlternado,
                buttonOK, buttonCancelar
            });
        }
    }
}