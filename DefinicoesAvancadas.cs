using System;
using System.Windows.Forms;

namespace MacroArmaduraAvancado
{
    public partial class DefinicoesAvancadas : Form
    {
        private GestorDefinicoesAvancado gestor;

        public DefinicoesAvancadas(GestorDefinicoesAvancado gestorDefinicoes)
        {
            gestor = gestorDefinicoes;
            InitializeComponent();
            CarregarDefinicoes();
        }

        private void CarregarDefinicoes()
        {
            var definicoes = gestor.ObterDefinicoes();

            numCoberturaVigas.Value = (decimal)definicoes.CoberturaVigas;
            numMultiplicadorMin.Value = (decimal)definicoes.MultiplicadorAmarracaoMinimo;
            numMultiplicadorMax.Value = (decimal)definicoes.MultiplicadorAmarracaoMaximo;
            checkValidarEurocodigo.Checked = definicoes.ValidarEurocodigo;
            checkGerarRelatorio.Checked = definicoes.GerarRelatorioQuantidades;
        }

        private void ButtonOK_Click(object sender, EventArgs e)
        {
            try
            {
                var definicoes = gestor.ObterDefinicoes();

                definicoes.CoberturaVigas = (double)numCoberturaVigas.Value;
                definicoes.MultiplicadorAmarracaoMinimo = (double)numMultiplicadorMin.Value;
                definicoes.MultiplicadorAmarracaoMaximo = (double)numMultiplicadorMax.Value;
                definicoes.ValidarEurocodigo = checkValidarEurocodigo.Checked;
                definicoes.GerarRelatorioQuantidades = checkGerarRelatorio.Checked;

                gestor.SalvarDefinicoes(definicoes);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao salvar definições: " + ex.Message, "Erro",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ButtonRestaurarPadrao_Click(object sender, EventArgs e)
        {
            DialogResult resultado = MessageBox.Show(
                "Confirma o restauro das definições padrão?",
                "Confirmação",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (resultado == DialogResult.Yes)
            {
                gestor.RestaurarDefinicoesPadrao();
                CarregarDefinicoes();
                MessageBox.Show("Definições padrão restauradas.", "Sucesso",
                               MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}