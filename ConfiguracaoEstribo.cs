using System;
using System.Windows.Forms;

namespace Rebar_Revit
{
    public partial class ConfiguracaoEstribo : Form
    {
        public double DiametroValue { get; private set; }
        public double EspacamentoValue { get; private set; }
        public bool AlternadoValue { get; private set; }

        public ConfiguracaoEstribo()
        {
            InitializeComponent();
            ConfigurarValoresPadrao();
        }

        private void ConfigurarValoresPadrao()
        {
            comboDiametro.SelectedItem = "8";
        }

        private void ButtonOK_Click(object sender, EventArgs e)
        {
            if (comboDiametro.SelectedItem == null)
            {
                MessageBox.Show("Seleccione um diâmetro.", "Erro", 
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Validação de espaçamento
            if (numEspacamento.Value < 50 || numEspacamento.Value > 500)
            {
                MessageBox.Show("Espaçamento deve estar entre 50mm e 500mm.", "Erro", 
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DiametroValue = double.Parse(comboDiametro.SelectedItem.ToString());
            EspacamentoValue = (double)numEspacamento.Value;
            AlternadoValue = checkAlternado.Checked;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}