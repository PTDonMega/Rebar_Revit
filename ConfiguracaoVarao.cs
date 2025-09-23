using System;
using System.Windows.Forms;

namespace Rebar_Revit
{
    public partial class ConfiguracaoVarao : Form
    {
        public int QuantidadeValue { get; private set; }
        public double DiametroValue { get; private set; }
        public string PosicaoValue { get; private set; }

        private bool isPilar;

        public ConfiguracaoVarao(bool isPilar = false)
        {
            this.isPilar = isPilar;
            InitializeComponent();
            ConfigurarParaTipo();
        }

        private void ConfigurarParaTipo()
        {
            if (isPilar)
            {
                numQuantidade.Maximum = 20;
                numQuantidade.Value = 4;
                comboPosicao.Items.Clear();
                comboPosicao.Items.AddRange(new string[] { "Principal", "Secundária" });
                comboPosicao.SelectedItem = "Principal";
            }
            else // Viga
            {
                numQuantidade.Maximum = 10;
                numQuantidade.Value = 3;
                comboPosicao.Items.Clear();
                comboPosicao.Items.AddRange(new string[] { "Superior", "Inferior", "Lateral" });
                comboPosicao.SelectedItem = "Superior";
            }

            comboDiametro.SelectedItem = "16";
        }

        private void ButtonOK_Click(object sender, EventArgs e)
        {
            if (comboDiametro.SelectedItem == null)
            {
                MessageBox.Show("Seleccione um diâmetro.", "Erro", 
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (comboPosicao.SelectedItem == null)
            {
                MessageBox.Show("Seleccione uma posição.", "Erro", 
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            QuantidadeValue = (int)numQuantidade.Value;
            DiametroValue = double.Parse(comboDiametro.SelectedItem.ToString());
            PosicaoValue = comboPosicao.SelectedItem.ToString();

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}