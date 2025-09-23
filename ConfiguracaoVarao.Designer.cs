namespace Rebar_Revit
{
    partial class ConfiguracaoVarao
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            labelQuantidade = new Label();
            numQuantidade = new NumericUpDown();
            labelDiametro = new Label();
            comboDiametro = new ComboBox();
            labelPosicao = new Label();
            comboPosicao = new ComboBox();
            buttonOK = new Button();
            buttonCancelar = new Button();
            ((System.ComponentModel.ISupportInitialize)numQuantidade).BeginInit();
            SuspendLayout();
            // 
            // labelQuantidade
            // 
            labelQuantidade.AutoSize = true;
            labelQuantidade.Location = new Point(23, 40);
            labelQuantidade.Name = "labelQuantidade";
            labelQuantidade.Size = new Size(90, 20);
            labelQuantidade.TabIndex = 0;
            labelQuantidade.Text = "Quantidade:";
            // 
            // numQuantidade
            // 
            numQuantidade.Location = new Point(141, 38);
            numQuantidade.Margin = new Padding(3, 4, 3, 4);
            numQuantidade.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            numQuantidade.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numQuantidade.Name = "numQuantidade";
            numQuantidade.Size = new Size(69, 27);
            numQuantidade.TabIndex = 1;
            numQuantidade.Value = new decimal(new int[] { 3, 0, 0, 0 });
            // 
            // labelDiametro
            // 
            labelDiametro.AutoSize = true;
            labelDiametro.Location = new Point(23, 93);
            labelDiametro.Name = "labelDiametro";
            labelDiametro.Size = new Size(115, 20);
            labelDiametro.TabIndex = 2;
            labelDiametro.Text = "Diâmetro (mm):";
            // 
            // comboDiametro
            // 
            comboDiametro.DropDownStyle = ComboBoxStyle.DropDownList;
            comboDiametro.FormattingEnabled = true;
            comboDiametro.Items.AddRange(new object[] { "8", "10", "12", "16", "20", "25", "32" });
            comboDiametro.Location = new Point(141, 90);
            comboDiametro.Margin = new Padding(3, 4, 3, 4);
            comboDiametro.Name = "comboDiametro";
            comboDiametro.Size = new Size(91, 28);
            comboDiametro.TabIndex = 3;
            // 
            // labelPosicao
            // 
            labelPosicao.AutoSize = true;
            labelPosicao.Location = new Point(23, 147);
            labelPosicao.Name = "labelPosicao";
            labelPosicao.Size = new Size(62, 20);
            labelPosicao.TabIndex = 4;
            labelPosicao.Text = "Posição:";
            // 
            // comboPosicao
            // 
            comboPosicao.DropDownStyle = ComboBoxStyle.DropDownList;
            comboPosicao.FormattingEnabled = true;
            comboPosicao.Items.AddRange(new object[] { "Superior", "Inferior", "Lateral" });
            comboPosicao.Location = new Point(141, 144);
            comboPosicao.Margin = new Padding(3, 4, 3, 4);
            comboPosicao.Name = "comboPosicao";
            comboPosicao.Size = new Size(137, 28);
            comboPosicao.TabIndex = 5;
            // 
            // buttonOK
            // 
            buttonOK.DialogResult = DialogResult.OK;
            buttonOK.Location = new Point(126, 200);
            buttonOK.Margin = new Padding(3, 4, 3, 4);
            buttonOK.Name = "buttonOK";
            buttonOK.Size = new Size(86, 33);
            buttonOK.TabIndex = 6;
            buttonOK.Text = "OK";
            buttonOK.UseVisualStyleBackColor = true;
            buttonOK.Click += ButtonOK_Click;
            // 
            // buttonCancelar
            // 
            buttonCancelar.DialogResult = DialogResult.Cancel;
            buttonCancelar.Location = new Point(229, 200);
            buttonCancelar.Margin = new Padding(3, 4, 3, 4);
            buttonCancelar.Name = "buttonCancelar";
            buttonCancelar.Size = new Size(86, 33);
            buttonCancelar.TabIndex = 7;
            buttonCancelar.Text = "Cancelar";
            buttonCancelar.UseVisualStyleBackColor = true;
            // 
            // ConfiguracaoVarao
            // 
            AcceptButton = buttonOK;
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = buttonCancelar;
            ClientSize = new Size(382, 255);
            Controls.Add(buttonCancelar);
            Controls.Add(buttonOK);
            Controls.Add(comboPosicao);
            Controls.Add(labelPosicao);
            Controls.Add(comboDiametro);
            Controls.Add(labelDiametro);
            Controls.Add(numQuantidade);
            Controls.Add(labelQuantidade);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(3, 4, 3, 4);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ConfiguracaoVarao";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Configuração de Varão";
            ((System.ComponentModel.ISupportInitialize)numQuantidade).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelQuantidade;
        private System.Windows.Forms.NumericUpDown numQuantidade;
        private System.Windows.Forms.Label labelDiametro;
        private System.Windows.Forms.ComboBox comboDiametro;
        private System.Windows.Forms.Label labelPosicao;
        private System.Windows.Forms.ComboBox comboPosicao;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancelar;
    }
}