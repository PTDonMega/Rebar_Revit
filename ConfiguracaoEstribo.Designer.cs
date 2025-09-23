namespace Rebar_Revit
{
    partial class ConfiguracaoEstribo
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
            labelDiametro = new Label();
            comboDiametro = new ComboBox();
            labelEspacamento = new Label();
            numEspacamento = new NumericUpDown();
            labelConfiguracao = new Label();
            checkAlternado = new CheckBox();
            labelInfo = new Label();
            buttonOK = new Button();
            buttonCancelar = new Button();
            ((System.ComponentModel.ISupportInitialize)numEspacamento).BeginInit();
            SuspendLayout();
            // 
            // labelDiametro
            // 
            labelDiametro.AutoSize = true;
            labelDiametro.Location = new Point(23, 40);
            labelDiametro.Name = "labelDiametro";
            labelDiametro.Size = new Size(115, 20);
            labelDiametro.TabIndex = 0;
            labelDiametro.Text = "Diâmetro (mm):";
            // 
            // comboDiametro
            // 
            comboDiametro.DropDownStyle = ComboBoxStyle.DropDownList;
            comboDiametro.FormattingEnabled = true;
            comboDiametro.Items.AddRange(new object[] { "6", "8", "10", "12" });
            comboDiametro.Location = new Point(168, 37);
            comboDiametro.Margin = new Padding(3, 4, 3, 4);
            comboDiametro.Name = "comboDiametro";
            comboDiametro.Size = new Size(91, 28);
            comboDiametro.TabIndex = 1;
            // 
            // labelEspacamento
            // 
            labelEspacamento.AutoSize = true;
            labelEspacamento.Location = new Point(23, 93);
            labelEspacamento.Name = "labelEspacamento";
            labelEspacamento.Size = new Size(141, 20);
            labelEspacamento.TabIndex = 2;
            labelEspacamento.Text = "Espaçamento (mm):";
            // 
            // numEspacamento
            // 
            numEspacamento.Increment = new decimal(new int[] { 25, 0, 0, 0 });
            numEspacamento.Location = new Point(168, 91);
            numEspacamento.Margin = new Padding(3, 4, 3, 4);
            numEspacamento.Maximum = new decimal(new int[] { 500, 0, 0, 0 });
            numEspacamento.Minimum = new decimal(new int[] { 50, 0, 0, 0 });
            numEspacamento.Name = "numEspacamento";
            numEspacamento.Size = new Size(91, 27);
            numEspacamento.TabIndex = 3;
            numEspacamento.Value = new decimal(new int[] { 150, 0, 0, 0 });
            // 
            // labelConfiguracao
            // 
            labelConfiguracao.AutoSize = true;
            labelConfiguracao.Location = new Point(23, 147);
            labelConfiguracao.Name = "labelConfiguracao";
            labelConfiguracao.Size = new Size(101, 20);
            labelConfiguracao.TabIndex = 4;
            labelConfiguracao.Text = "Configuração:";
            // 
            // checkAlternado
            // 
            checkAlternado.AutoSize = true;
            checkAlternado.Location = new Point(149, 147);
            checkAlternado.Margin = new Padding(3, 4, 3, 4);
            checkAlternado.Name = "checkAlternado";
            checkAlternado.Size = new Size(247, 24);
            checkAlternado.TabIndex = 5;
            checkAlternado.Text = "Padrão alternado (zonas críticas)";
            checkAlternado.UseVisualStyleBackColor = true;
            // 
            // labelInfo
            // 
            labelInfo.ForeColor = Color.DarkBlue;
            labelInfo.Location = new Point(23, 187);
            labelInfo.Name = "labelInfo";
            labelInfo.Size = new Size(343, 47);
            labelInfo.TabIndex = 6;
            labelInfo.Text = "Sugestão: 8mm/150mm para vigas até 40cm\r\n10mm/125mm para vigas maiores";
            // 
            // buttonOK
            // 
            buttonOK.DialogResult = DialogResult.OK;
            buttonOK.Location = new Point(149, 247);
            buttonOK.Margin = new Padding(3, 4, 3, 4);
            buttonOK.Name = "buttonOK";
            buttonOK.Size = new Size(86, 33);
            buttonOK.TabIndex = 7;
            buttonOK.Text = "OK";
            buttonOK.UseVisualStyleBackColor = true;
            buttonOK.Click += ButtonOK_Click;
            // 
            // buttonCancelar
            // 
            buttonCancelar.DialogResult = DialogResult.Cancel;
            buttonCancelar.Location = new Point(251, 247);
            buttonCancelar.Margin = new Padding(3, 4, 3, 4);
            buttonCancelar.Name = "buttonCancelar";
            buttonCancelar.Size = new Size(86, 33);
            buttonCancelar.TabIndex = 8;
            buttonCancelar.Text = "Cancelar";
            buttonCancelar.UseVisualStyleBackColor = true;
            // 
            // ConfiguracaoEstribo
            // 
            AcceptButton = buttonOK;
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = buttonCancelar;
            ClientSize = new Size(382, 295);
            Controls.Add(buttonCancelar);
            Controls.Add(buttonOK);
            Controls.Add(labelInfo);
            Controls.Add(checkAlternado);
            Controls.Add(labelConfiguracao);
            Controls.Add(numEspacamento);
            Controls.Add(labelEspacamento);
            Controls.Add(comboDiametro);
            Controls.Add(labelDiametro);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(3, 4, 3, 4);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ConfiguracaoEstribo";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Configuração de Estribo";
            ((System.ComponentModel.ISupportInitialize)numEspacamento).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelDiametro;
        private System.Windows.Forms.ComboBox comboDiametro;
        private System.Windows.Forms.Label labelEspacamento;
        private System.Windows.Forms.NumericUpDown numEspacamento;
        private System.Windows.Forms.Label labelConfiguracao;
        private System.Windows.Forms.CheckBox checkAlternado;
        private System.Windows.Forms.Label labelInfo;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancelar;
    }
}