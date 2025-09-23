using System;
using System.Windows.Forms;

namespace Rebar_Revit
{
    /// <summary>
    /// Formulário de pré-visualização
    /// </summary>
    public partial class FormularioPreVisualizacao : Form
    {
        private TextBox textRelatorio;
        private Button buttonFechar;
        private Button buttonExportar;

        public FormularioPreVisualizacao(string relatorio)
        {
            InicializarComponentes();
            textRelatorio.Text = relatorio;
        }

        private void InicializarComponentes()
        {
            this.Text = "Pré-visualização de Armaduras";
            this.Size = new System.Drawing.Size(700, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.MinimizeBox = false;

            // Área de texto para o relatório
            textRelatorio = new TextBox();
            textRelatorio.Location = new System.Drawing.Point(12, 12);
            textRelatorio.Size = new System.Drawing.Size(660, 520);
            textRelatorio.Multiline = true;
            textRelatorio.ScrollBars = ScrollBars.Both;
            textRelatorio.ReadOnly = true;
            textRelatorio.Font = new System.Drawing.Font("Consolas", 9);
            textRelatorio.BackColor = System.Drawing.Color.White;

            // Botões
            buttonExportar = new Button();
            buttonExportar.Text = "📄 Exportar para Arquivo";
            buttonExportar.Location = new System.Drawing.Point(12, 545);
            buttonExportar.Size = new System.Drawing.Size(150, 30);
            buttonExportar.Click += ButtonExportar_Click;

            buttonFechar = new Button();
            buttonFechar.Text = "Fechar";
            buttonFechar.Location = new System.Drawing.Point(597, 545);
            buttonFechar.Size = new System.Drawing.Size(75, 30);
            buttonFechar.DialogResult = DialogResult.OK;

            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                textRelatorio, buttonExportar, buttonFechar
            });

            this.AcceptButton = buttonFechar;
        }

        private void ButtonExportar_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "Arquivos de Texto (*.txt)|*.txt|Todos os Arquivos (*.*)|*.*";
                saveDialog.FileName = $"Relatorio_Armaduras_{DateTime.Now:yyyyMMdd_HHmm}.txt";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    System.IO.File.WriteAllText(saveDialog.FileName, textRelatorio.Text);
                    MessageBox.Show("Relatório exportado com sucesso!", "Sucesso",
                                   MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao exportar relatório: " + ex.Message, "Erro",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}