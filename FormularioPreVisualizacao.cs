using System;
using System.Windows.Forms;

namespace MacroArmaduraAvancado
{
    /// <summary>
    /// Formulário de pré-visualização
    /// </summary>
    public partial class FormularioPreVisualizacao : Form
    {
        private TextBox textRelatorio;
        private Button buttonFechar;
        private Button buttonImprimir;

        public FormularioPreVisualizacao(string relatorio)
        {
            InicializarComponentes();
            textRelatorio.Text = relatorio;
        }

        private void InicializarComponentes()
        {
            this.Text = "Pré-visualização da Execução";
            this.Size = new System.Drawing.Size(700, 500);
            this.StartPosition = FormStartPosition.CenterParent;

            textRelatorio = new TextBox();
            textRelatorio.Multiline = true;
            textRelatorio.ScrollBars = ScrollBars.Both;
            textRelatorio.ReadOnly = true;
            textRelatorio.Dock = DockStyle.Fill;
            textRelatorio.Font = new System.Drawing.Font("Consolas", 9);

            Panel panelBotoes = new Panel();
            panelBotoes.Height = 50;
            panelBotoes.Dock = DockStyle.Bottom;

            buttonImprimir = new Button();
            buttonImprimir.Text = "Imprimir";
            buttonImprimir.Size = new System.Drawing.Size(100, 30);
            buttonImprimir.Location = new System.Drawing.Point(480, 10);
            buttonImprimir.Click += (s, e) => MessageBox.Show("Função de impressão não implementada.", "Info");

            buttonFechar = new Button();
            buttonFechar.Text = "Fechar";
            buttonFechar.Size = new System.Drawing.Size(100, 30);
            buttonFechar.Location = new System.Drawing.Point(590, 10);
            buttonFechar.Click += (s, e) => this.Close();

            panelBotoes.Controls.AddRange(new Control[] { buttonImprimir, buttonFechar });
            this.Controls.AddRange(new Control[] { textRelatorio, panelBotoes });
        }
    }
}