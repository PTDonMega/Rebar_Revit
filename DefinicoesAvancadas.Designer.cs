namespace MacroArmaduraAvancado
{
    partial class DefinicoesAvancadas
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
            this.groupCoberturas = new System.Windows.Forms.GroupBox();
            this.numCoberturaVigas = new System.Windows.Forms.NumericUpDown();
            this.labelCoberturaVigas = new System.Windows.Forms.Label();
            this.groupAmarracao = new System.Windows.Forms.GroupBox();
            this.numMultiplicadorMax = new System.Windows.Forms.NumericUpDown();
            this.labelMultMax = new System.Windows.Forms.Label();
            this.numMultiplicadorMin = new System.Windows.Forms.NumericUpDown();
            this.labelMultMin = new System.Windows.Forms.Label();
            this.groupValidacao = new System.Windows.Forms.GroupBox();
            this.checkGerarRelatorio = new System.Windows.Forms.CheckBox();
            this.checkValidarEurocodigo = new System.Windows.Forms.CheckBox();
            this.buttonRestaurarPadrao = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancelar = new System.Windows.Forms.Button();
            this.groupCoberturas.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCoberturaVigas)).BeginInit();
            this.groupAmarracao.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMultiplicadorMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMultiplicadorMin)).BeginInit();
            this.groupValidacao.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupCoberturas
            // 
            this.groupCoberturas.Controls.Add(this.numCoberturaVigas);
            this.groupCoberturas.Controls.Add(this.labelCoberturaVigas);
            this.groupCoberturas.Location = new System.Drawing.Point(12, 12);
            this.groupCoberturas.Name = "groupCoberturas";
            this.groupCoberturas.Size = new System.Drawing.Size(410, 80);
            this.groupCoberturas.TabIndex = 0;
            this.groupCoberturas.TabStop = false;
            this.groupCoberturas.Text = "Coberturas (mm)";
            // 
            // numCoberturaVigas
            // 
            this.numCoberturaVigas.Location = new System.Drawing.Point(90, 28);
            this.numCoberturaVigas.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numCoberturaVigas.Minimum = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.numCoberturaVigas.Name = "numCoberturaVigas";
            this.numCoberturaVigas.Size = new System.Drawing.Size(70, 23);
            this.numCoberturaVigas.TabIndex = 1;
            this.numCoberturaVigas.Value = new decimal(new int[] {
            25,
            0,
            0,
            0});
            // 
            // labelCoberturaVigas
            // 
            this.labelCoberturaVigas.AutoSize = true;
            this.labelCoberturaVigas.Location = new System.Drawing.Point(20, 30);
            this.labelCoberturaVigas.Name = "labelCoberturaVigas";
            this.labelCoberturaVigas.Size = new System.Drawing.Size(37, 15);
            this.labelCoberturaVigas.TabIndex = 0;
            this.labelCoberturaVigas.Text = "Vigas:";
            // 
            // groupAmarracao
            // 
            this.groupAmarracao.Controls.Add(this.numMultiplicadorMax);
            this.groupAmarracao.Controls.Add(this.labelMultMax);
            this.groupAmarracao.Controls.Add(this.numMultiplicadorMin);
            this.groupAmarracao.Controls.Add(this.labelMultMin);
            this.groupAmarracao.Location = new System.Drawing.Point(12, 110);
            this.groupAmarracao.Name = "groupAmarracao";
            this.groupAmarracao.Size = new System.Drawing.Size(410, 120);
            this.groupAmarracao.TabIndex = 1;
            this.groupAmarracao.TabStop = false;
            this.groupAmarracao.Text = "Amarração";
            // 
            // numMultiplicadorMax
            // 
            this.numMultiplicadorMax.Location = new System.Drawing.Point(150, 63);
            this.numMultiplicadorMax.Minimum = new decimal(new int[] {
            40,
            0,
            0,
            0});
            this.numMultiplicadorMax.Name = "numMultiplicadorMax";
            this.numMultiplicadorMax.Size = new System.Drawing.Size(70, 23);
            this.numMultiplicadorMax.TabIndex = 3;
            this.numMultiplicadorMax.Value = new decimal(new int[] {
            80,
            0,
            0,
            0});
            // 
            // labelMultMax
            // 
            this.labelMultMax.AutoSize = true;
            this.labelMultMax.Location = new System.Drawing.Point(20, 65);
            this.labelMultMax.Name = "labelMultMax";
            this.labelMultMax.Size = new System.Drawing.Size(124, 15);
            this.labelMultMax.TabIndex = 2;
            this.labelMultMax.Text = "Multiplicador máx. (?):";
            // 
            // numMultiplicadorMin
            // 
            this.numMultiplicadorMin.Location = new System.Drawing.Point(150, 28);
            this.numMultiplicadorMin.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.numMultiplicadorMin.Minimum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numMultiplicadorMin.Name = "numMultiplicadorMin";
            this.numMultiplicadorMin.Size = new System.Drawing.Size(70, 23);
            this.numMultiplicadorMin.TabIndex = 1;
            this.numMultiplicadorMin.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // labelMultMin
            // 
            this.labelMultMin.AutoSize = true;
            this.labelMultMin.Location = new System.Drawing.Point(20, 30);
            this.labelMultMin.Name = "labelMultMin";
            this.labelMultMin.Size = new System.Drawing.Size(122, 15);
            this.labelMultMin.TabIndex = 0;
            this.labelMultMin.Text = "Multiplicador mín. (?):";
            // 
            // groupValidacao
            // 
            this.groupValidacao.Controls.Add(this.checkGerarRelatorio);
            this.groupValidacao.Controls.Add(this.checkValidarEurocodigo);
            this.groupValidacao.Location = new System.Drawing.Point(12, 250);
            this.groupValidacao.Name = "groupValidacao";
            this.groupValidacao.Size = new System.Drawing.Size(410, 80);
            this.groupValidacao.TabIndex = 2;
            this.groupValidacao.TabStop = false;
            this.groupValidacao.Text = "Validação e Relatórios";
            // 
            // checkGerarRelatorio
            // 
            this.checkGerarRelatorio.AutoSize = true;
            this.checkGerarRelatorio.Checked = true;
            this.checkGerarRelatorio.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkGerarRelatorio.Location = new System.Drawing.Point(20, 50);
            this.checkGerarRelatorio.Name = "checkGerarRelatorio";
            this.checkGerarRelatorio.Size = new System.Drawing.Size(180, 19);
            this.checkGerarRelatorio.TabIndex = 1;
            this.checkGerarRelatorio.Text = "Gerar relatório de quantidades";
            this.checkGerarRelatorio.UseVisualStyleBackColor = true;
            // 
            // checkValidarEurocodigo
            // 
            this.checkValidarEurocodigo.AutoSize = true;
            this.checkValidarEurocodigo.Checked = true;
            this.checkValidarEurocodigo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkValidarEurocodigo.Location = new System.Drawing.Point(20, 25);
            this.checkValidarEurocodigo.Name = "checkValidarEurocodigo";
            this.checkValidarEurocodigo.Size = new System.Drawing.Size(172, 19);
            this.checkValidarEurocodigo.TabIndex = 0;
            this.checkValidarEurocodigo.Text = "Validar segundo Eurocódigo 2";
            this.checkValidarEurocodigo.UseVisualStyleBackColor = true;
            // 
            // buttonRestaurarPadrao
            // 
            this.buttonRestaurarPadrao.Location = new System.Drawing.Point(12, 345);
            this.buttonRestaurarPadrao.Name = "buttonRestaurarPadrao";
            this.buttonRestaurarPadrao.Size = new System.Drawing.Size(110, 30);
            this.buttonRestaurarPadrao.TabIndex = 3;
            this.buttonRestaurarPadrao.Text = "Restaurar Padrão";
            this.buttonRestaurarPadrao.UseVisualStyleBackColor = true;
            this.buttonRestaurarPadrao.Click += new System.EventHandler(this.ButtonRestaurarPadrao_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(267, 345);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 30);
            this.buttonOK.TabIndex = 4;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.ButtonOK_Click);
            // 
            // buttonCancelar
            // 
            this.buttonCancelar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancelar.Location = new System.Drawing.Point(347, 345);
            this.buttonCancelar.Name = "buttonCancelar";
            this.buttonCancelar.Size = new System.Drawing.Size(75, 30);
            this.buttonCancelar.TabIndex = 5;
            this.buttonCancelar.Text = "Cancelar";
            this.buttonCancelar.UseVisualStyleBackColor = true;
            // 
            // DefinicoesAvancadas
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancelar;
            this.ClientSize = new System.Drawing.Size(434, 391);
            this.Controls.Add(this.buttonCancelar);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonRestaurarPadrao);
            this.Controls.Add(this.groupValidacao);
            this.Controls.Add(this.groupAmarracao);
            this.Controls.Add(this.groupCoberturas);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DefinicoesAvancadas";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Definições Avançadas";
            this.groupCoberturas.ResumeLayout(false);
            this.groupCoberturas.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCoberturaVigas)).EndInit();
            this.groupAmarracao.ResumeLayout(false);
            this.groupAmarracao.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMultiplicadorMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMultiplicadorMin)).EndInit();
            this.groupValidacao.ResumeLayout(false);
            this.groupValidacao.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupCoberturas;
        private System.Windows.Forms.NumericUpDown numCoberturaVigas;
        private System.Windows.Forms.Label labelCoberturaVigas;
        private System.Windows.Forms.GroupBox groupAmarracao;
        private System.Windows.Forms.NumericUpDown numMultiplicadorMax;
        private System.Windows.Forms.Label labelMultMax;
        private System.Windows.Forms.NumericUpDown numMultiplicadorMin;
        private System.Windows.Forms.Label labelMultMin;
        private System.Windows.Forms.GroupBox groupValidacao;
        private System.Windows.Forms.CheckBox checkGerarRelatorio;
        private System.Windows.Forms.CheckBox checkValidarEurocodigo;
        private System.Windows.Forms.Button buttonRestaurarPadrao;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancelar;
    }
}