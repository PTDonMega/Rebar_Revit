namespace MacroArmaduraAvancado
{
    partial class FormularioPrincipalAvancado
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
            this.groupTipoElemento = new System.Windows.Forms.GroupBox();
            this.labelInfoElementos = new System.Windows.Forms.Label();
            this.radioVigas = new System.Windows.Forms.RadioButton();
            this.groupFiltros = new System.Windows.Forms.GroupBox();
            this.labelContagem = new System.Windows.Forms.Label();
            this.checkSeleccaoActual = new System.Windows.Forms.CheckBox();
            this.listNiveis = new System.Windows.Forms.CheckedListBox();
            this.labelNiveis = new System.Windows.Forms.Label();
            this.comboDesignacao = new System.Windows.Forms.ComboBox();
            this.labelDesignacao = new System.Windows.Forms.Label();
            this.groupConfigArmadura = new System.Windows.Forms.GroupBox();
            this.checkAmarracaoAutomatica = new System.Windows.Forms.CheckBox();
            this.comboTipoDistribuicao = new System.Windows.Forms.ComboBox();
            this.labelDistribuicao = new System.Windows.Forms.Label();
            this.buttonRemoverVarao = new System.Windows.Forms.Button();
            this.buttonAdicionarVarao = new System.Windows.Forms.Button();
            this.listViewVaroes = new System.Windows.Forms.ListView();
            this.colQuantidade = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colDiametro = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colPosicao = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupVisualizador = new System.Windows.Forms.GroupBox();
            this.labelInfoVisualizador = new System.Windows.Forms.Label();
            this.buttonModoEdicao = new System.Windows.Forms.Button();
            this.buttonAlternarVista = new System.Windows.Forms.Button();
            this.numLarguraViga = new System.Windows.Forms.NumericUpDown();
            this.labelLargura = new System.Windows.Forms.Label();
            this.numAlturaViga = new System.Windows.Forms.NumericUpDown();
            this.labelAltura = new System.Windows.Forms.Label();
            this.numComprimentoViga = new System.Windows.Forms.NumericUpDown();
            this.labelComprimento = new System.Windows.Forms.Label();
            this.labelDimensoes = new System.Windows.Forms.Label();
            this.groupAmarracao = new System.Windows.Forms.GroupBox();
            this.labelAmarracaoInfo = new System.Windows.Forms.Label();
            this.comboTipoAmarracao = new System.Windows.Forms.ComboBox();
            this.labelTipoAmarracao = new System.Windows.Forms.Label();
            this.labelPhi = new System.Windows.Forms.Label();
            this.numMultiplicadorAmarracao = new System.Windows.Forms.NumericUpDown();
            this.labelMultiplicador = new System.Windows.Forms.Label();
            this.groupEstribos = new System.Windows.Forms.GroupBox();
            this.checkEstribosAutomaticos = new System.Windows.Forms.CheckBox();
            this.buttonRemoverEstribo = new System.Windows.Forms.Button();
            this.buttonAdicionarEstribo = new System.Windows.Forms.Button();
            this.listViewEstribos = new System.Windows.Forms.ListView();
            this.colDiametroEstribo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colEspacamento = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colAlternado = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.buttonCancelar = new System.Windows.Forms.Button();
            this.buttonExecutar = new System.Windows.Forms.Button();
            this.buttonPreVisualizacao = new System.Windows.Forms.Button();
            this.buttonDefinicoes = new System.Windows.Forms.Button();
            this.groupTipoElemento.SuspendLayout();
            this.groupFiltros.SuspendLayout();
            this.groupConfigArmadura.SuspendLayout();
            this.groupVisualizador.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numLarguraViga)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAlturaViga)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numComprimentoViga)).BeginInit();
            this.groupAmarracao.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMultiplicadorAmarracao)).BeginInit();
            this.groupEstribos.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupTipoElemento
            // 
            this.groupTipoElemento.Controls.Add(this.labelInfoElementos);
            this.groupTipoElemento.Controls.Add(this.radioVigas);
            this.groupTipoElemento.Location = new System.Drawing.Point(12, 12);
            this.groupTipoElemento.Name = "groupTipoElemento";
            this.groupTipoElemento.Size = new System.Drawing.Size(1160, 70);
            this.groupTipoElemento.TabIndex = 0;
            this.groupTipoElemento.TabStop = false;
            this.groupTipoElemento.Text = "Tipo de Elemento Estrutural";
            // 
            // labelInfoElementos
            // 
            this.labelInfoElementos.ForeColor = System.Drawing.Color.DarkBlue;
            this.labelInfoElementos.Location = new System.Drawing.Point(240, 25);
            this.labelInfoElementos.Name = "labelInfoElementos";
            this.labelInfoElementos.Size = new System.Drawing.Size(450, 40);
            this.labelInfoElementos.TabIndex = 1;
            this.labelInfoElementos.Text = "Armadura longitudinal (superior, inferior, lateral) + estribos automáticos";
            // 
            // radioVigas
            // 
            this.radioVigas.AutoSize = true;
            this.radioVigas.Checked = true;
            this.radioVigas.Enabled = false;
            this.radioVigas.Location = new System.Drawing.Point(20, 25);
            this.radioVigas.Name = "radioVigas";
            this.radioVigas.Size = new System.Drawing.Size(173, 19);
            this.radioVigas.TabIndex = 0;
            this.radioVigas.TabStop = true;
            this.radioVigas.Text = "Vigas (Automação Focada)";
            this.radioVigas.UseVisualStyleBackColor = true;
            // 
            // groupFiltros
            // 
            this.groupFiltros.Controls.Add(this.labelContagem);
            this.groupFiltros.Controls.Add(this.checkSeleccaoActual);
            this.groupFiltros.Controls.Add(this.listNiveis);
            this.groupFiltros.Controls.Add(this.labelNiveis);
            this.groupFiltros.Controls.Add(this.comboDesignacao);
            this.groupFiltros.Controls.Add(this.labelDesignacao);
            this.groupFiltros.Location = new System.Drawing.Point(12, 95);
            this.groupFiltros.Name = "groupFiltros";
            this.groupFiltros.Size = new System.Drawing.Size(580, 120);
            this.groupFiltros.TabIndex = 1;
            this.groupFiltros.TabStop = false;
            this.groupFiltros.Text = "Selecção de Vigas";
            // 
            // labelContagem
            // 
            this.labelContagem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelContagem.Location = new System.Drawing.Point(20, 90);
            this.labelContagem.Name = "labelContagem";
            this.labelContagem.Size = new System.Drawing.Size(300, 20);
            this.labelContagem.TabIndex = 5;
            this.labelContagem.Text = "Vigas encontradas: 0";
            // 
            // checkSeleccaoActual
            // 
            this.checkSeleccaoActual.AutoSize = true;
            this.checkSeleccaoActual.Location = new System.Drawing.Point(20, 65);
            this.checkSeleccaoActual.Name = "checkSeleccaoActual";
            this.checkSeleccaoActual.Size = new System.Drawing.Size(138, 19);
            this.checkSeleccaoActual.TabIndex = 4;
            this.checkSeleccaoActual.Text = "Usar selecção actual";
            this.checkSeleccaoActual.UseVisualStyleBackColor = true;
            this.checkSeleccaoActual.CheckedChanged += new System.EventHandler(this.CheckSeleccaoActual_CheckedChanged);
            // 
            // listNiveis
            // 
            this.listNiveis.FormattingEnabled = true;
            this.listNiveis.Location = new System.Drawing.Point(380, 28);
            this.listNiveis.Name = "listNiveis";
            this.listNiveis.Size = new System.Drawing.Size(180, 76);
            this.listNiveis.TabIndex = 3;
            this.listNiveis.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.ListNiveis_ItemCheck);
            // 
            // labelNiveis
            // 
            this.labelNiveis.AutoSize = true;
            this.labelNiveis.Location = new System.Drawing.Point(330, 30);
            this.labelNiveis.Name = "labelNiveis";
            this.labelNiveis.Size = new System.Drawing.Size(44, 15);
            this.labelNiveis.TabIndex = 2;
            this.labelNiveis.Text = "Níveis:";
            // 
            // comboDesignacao
            // 
            this.comboDesignacao.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboDesignacao.FormattingEnabled = true;
            this.comboDesignacao.Location = new System.Drawing.Point(110, 28);
            this.comboDesignacao.Name = "comboDesignacao";
            this.comboDesignacao.Size = new System.Drawing.Size(200, 23);
            this.comboDesignacao.TabIndex = 1;
            this.comboDesignacao.SelectedIndexChanged += new System.EventHandler(this.ComboDesignacao_SelectedIndexChanged);
            // 
            // labelDesignacao
            // 
            this.labelDesignacao.AutoSize = true;
            this.labelDesignacao.Location = new System.Drawing.Point(20, 30);
            this.labelDesignacao.Name = "labelDesignacao";
            this.labelDesignacao.Size = new System.Drawing.Size(80, 15);
            this.labelDesignacao.TabIndex = 0;
            this.labelDesignacao.Text = "Tipo de Viga:";
            // 
            // groupConfigArmadura
            // 
            this.groupConfigArmadura.Controls.Add(this.checkAmarracaoAutomatica);
            this.groupConfigArmadura.Controls.Add(this.comboTipoDistribuicao);
            this.groupConfigArmadura.Controls.Add(this.labelDistribuicao);
            this.groupConfigArmadura.Controls.Add(this.buttonRemoverVarao);
            this.groupConfigArmadura.Controls.Add(this.buttonAdicionarVarao);
            this.groupConfigArmadura.Controls.Add(this.listViewVaroes);
            this.groupConfigArmadura.Location = new System.Drawing.Point(12, 230);
            this.groupConfigArmadura.Name = "groupConfigArmadura";
            this.groupConfigArmadura.Size = new System.Drawing.Size(580, 200);
            this.groupConfigArmadura.TabIndex = 2;
            this.groupConfigArmadura.TabStop = false;
            this.groupConfigArmadura.Text = "Armadura Longitudinal";
            // 
            // checkAmarracaoAutomatica
            // 
            this.checkAmarracaoAutomatica.AutoSize = true;
            this.checkAmarracaoAutomatica.Checked = true;
            this.checkAmarracaoAutomatica.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkAmarracaoAutomatica.Location = new System.Drawing.Point(20, 170);
            this.checkAmarracaoAutomatica.Name = "checkAmarracaoAutomatica";
            this.checkAmarracaoAutomatica.Size = new System.Drawing.Size(154, 19);
            this.checkAmarracaoAutomatica.TabIndex = 5;
            this.checkAmarracaoAutomatica.Text = "Amarração automática";
            this.checkAmarracaoAutomatica.UseVisualStyleBackColor = true;
            this.checkAmarracaoAutomatica.CheckedChanged += new System.EventHandler(this.CheckAmarracaoAutomatica_CheckedChanged);
            // 
            // comboTipoDistribuicao
            // 
            this.comboTipoDistribuicao.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboTipoDistribuicao.FormattingEnabled = true;
            this.comboTipoDistribuicao.Items.AddRange(new object[] {
            "Automática (Regulamentar)",
            "Uniforme",
            "ConcentradaNasBordas"});
            this.comboTipoDistribuicao.Location = new System.Drawing.Point(100, 138);
            this.comboTipoDistribuicao.Name = "comboTipoDistribuicao";
            this.comboTipoDistribuicao.Size = new System.Drawing.Size(180, 23);
            this.comboTipoDistribuicao.TabIndex = 4;
            // 
            // labelDistribuicao
            // 
            this.labelDistribuicao.AutoSize = true;
            this.labelDistribuicao.Location = new System.Drawing.Point(20, 140);
            this.labelDistribuicao.Name = "labelDistribuicao";
            this.labelDistribuicao.Size = new System.Drawing.Size(74, 15);
            this.labelDistribuicao.TabIndex = 3;
            this.labelDistribuicao.Text = "Distribuição:";
            // 
            // buttonRemoverVarao
            // 
            this.buttonRemoverVarao.Location = new System.Drawing.Point(420, 65);
            this.buttonRemoverVarao.Name = "buttonRemoverVarao";
            this.buttonRemoverVarao.Size = new System.Drawing.Size(80, 25);
            this.buttonRemoverVarao.TabIndex = 2;
            this.buttonRemoverVarao.Text = "Remover";
            this.buttonRemoverVarao.UseVisualStyleBackColor = true;
            this.buttonRemoverVarao.Click += new System.EventHandler(this.ButtonRemoverVarao_Click);
            // 
            // buttonAdicionarVarao
            // 
            this.buttonAdicionarVarao.Location = new System.Drawing.Point(420, 30);
            this.buttonAdicionarVarao.Name = "buttonAdicionarVarao";
            this.buttonAdicionarVarao.Size = new System.Drawing.Size(80, 25);
            this.buttonAdicionarVarao.TabIndex = 1;
            this.buttonAdicionarVarao.Text = "Adicionar";
            this.buttonAdicionarVarao.UseVisualStyleBackColor = true;
            this.buttonAdicionarVarao.Click += new System.EventHandler(this.ButtonAdicionarVarao_Click);
            // 
            // listViewVaroes
            // 
            this.listViewVaroes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colQuantidade,
            this.colDiametro,
            this.colPosicao});
            this.listViewVaroes.FullRowSelect = true;
            this.listViewVaroes.GridLines = true;
            this.listViewVaroes.Location = new System.Drawing.Point(20, 30);
            this.listViewVaroes.Name = "listViewVaroes";
            this.listViewVaroes.Size = new System.Drawing.Size(380, 100);
            this.listViewVaroes.TabIndex = 0;
            this.listViewVaroes.UseCompatibleStateImageBehavior = false;
            this.listViewVaroes.View = System.Windows.Forms.View.Details;
            // 
            // colQuantidade
            // 
            this.colQuantidade.Text = "Quant.";
            this.colQuantidade.Width = 60;
            // 
            // colDiametro
            // 
            this.colDiametro.Text = "Ø (mm)";
            this.colDiametro.Width = 70;
            // 
            // colPosicao
            // 
            this.colPosicao.Text = "Posição";
            this.colPosicao.Width = 80;
            // 
            // groupVisualizador
            // 
            this.groupVisualizador.Controls.Add(this.labelInfoVisualizador);
            this.groupVisualizador.Controls.Add(this.buttonModoEdicao);
            this.groupVisualizador.Controls.Add(this.buttonAlternarVista);
            this.groupVisualizador.Controls.Add(this.numLarguraViga);
            this.groupVisualizador.Controls.Add(this.labelLargura);
            this.groupVisualizador.Controls.Add(this.numAlturaViga);
            this.groupVisualizador.Controls.Add(this.labelAltura);
            this.groupVisualizador.Controls.Add(this.numComprimentoViga);
            this.groupVisualizador.Controls.Add(this.labelComprimento);
            this.groupVisualizador.Controls.Add(this.labelDimensoes);
            this.groupVisualizador.Location = new System.Drawing.Point(610, 95);
            this.groupVisualizador.Name = "groupVisualizador";
            this.groupVisualizador.Size = new System.Drawing.Size(562, 485);
            this.groupVisualizador.TabIndex = 3;
            this.groupVisualizador.TabStop = false;
            this.groupVisualizador.Text = "Visualização da Viga";
            // 
            // labelInfoVisualizador
            // 
            this.labelInfoVisualizador.ForeColor = System.Drawing.Color.DarkBlue;
            this.labelInfoVisualizador.Location = new System.Drawing.Point(20, 440);
            this.labelInfoVisualizador.Name = "labelInfoVisualizador";
            this.labelInfoVisualizador.Size = new System.Drawing.Size(520, 20);
            this.labelInfoVisualizador.TabIndex = 9;
            this.labelInfoVisualizador.Text = "Visualizador será carregado dinamicamente.";
            // 
            // buttonModoEdicao
            // 
            this.buttonModoEdicao.Location = new System.Drawing.Point(430, 25);
            this.buttonModoEdicao.Name = "buttonModoEdicao";
            this.buttonModoEdicao.Size = new System.Drawing.Size(70, 25);
            this.buttonModoEdicao.TabIndex = 8;
            this.buttonModoEdicao.Text = "? Editar";
            this.buttonModoEdicao.UseVisualStyleBackColor = true;
            this.buttonModoEdicao.Click += new System.EventHandler(this.ButtonModoEdicao_Click);
            // 
            // buttonAlternarVista
            // 
            this.buttonAlternarVista.Location = new System.Drawing.Point(350, 25);
            this.buttonAlternarVista.Name = "buttonAlternarVista";
            this.buttonAlternarVista.Size = new System.Drawing.Size(70, 25);
            this.buttonAlternarVista.TabIndex = 7;
            this.buttonAlternarVista.Text = "?? Vista";
            this.buttonAlternarVista.UseVisualStyleBackColor = true;
            this.buttonAlternarVista.Click += new System.EventHandler(this.ButtonAlternarVista_Click);
            // 
            // numLarguraViga
            // 
            this.numLarguraViga.Location = new System.Drawing.Point(240, 48);
            this.numLarguraViga.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numLarguraViga.Minimum = new decimal(new int[] {
            150,
            0,
            0,
            0});
            this.numLarguraViga.Name = "numLarguraViga";
            this.numLarguraViga.Size = new System.Drawing.Size(70, 23);
            this.numLarguraViga.TabIndex = 6;
            this.numLarguraViga.Value = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.numLarguraViga.ValueChanged += new System.EventHandler(this.DimensoesViga_ValueChanged);
            // 
            // labelLargura
            // 
            this.labelLargura.AutoSize = true;
            this.labelLargura.Location = new System.Drawing.Point(220, 50);
            this.labelLargura.Name = "labelLargura";
            this.labelLargura.Size = new System.Drawing.Size(16, 15);
            this.labelLargura.TabIndex = 5;
            this.labelLargura.Text = "L:";
            // 
            // numAlturaViga
            // 
            this.numAlturaViga.Location = new System.Drawing.Point(140, 48);
            this.numAlturaViga.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.numAlturaViga.Minimum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.numAlturaViga.Name = "numAlturaViga";
            this.numAlturaViga.Size = new System.Drawing.Size(70, 23);
            this.numAlturaViga.TabIndex = 4;
            this.numAlturaViga.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.numAlturaViga.ValueChanged += new System.EventHandler(this.DimensoesViga_ValueChanged);
            // 
            // labelAltura
            // 
            this.labelAltura.AutoSize = true;
            this.labelAltura.Location = new System.Drawing.Point(120, 50);
            this.labelAltura.Name = "labelAltura";
            this.labelAltura.Size = new System.Drawing.Size(18, 15);
            this.labelAltura.TabIndex = 3;
            this.labelAltura.Text = "H:";
            // 
            // numComprimentoViga
            // 
            this.numComprimentoViga.Location = new System.Drawing.Point(40, 48);
            this.numComprimentoViga.Maximum = new decimal(new int[] {
            20000,
            0,
            0,
            0});
            this.numComprimentoViga.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numComprimentoViga.Name = "numComprimentoViga";
            this.numComprimentoViga.Size = new System.Drawing.Size(70, 23);
            this.numComprimentoViga.TabIndex = 2;
            this.numComprimentoViga.Value = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.numComprimentoViga.ValueChanged += new System.EventHandler(this.DimensoesViga_ValueChanged);
            // 
            // labelComprimento
            // 
            this.labelComprimento.AutoSize = true;
            this.labelComprimento.Location = new System.Drawing.Point(20, 50);
            this.labelComprimento.Name = "labelComprimento";
            this.labelComprimento.Size = new System.Drawing.Size(17, 15);
            this.labelComprimento.TabIndex = 1;
            this.labelComprimento.Text = "C:";
            // 
            // labelDimensoes
            // 
            this.labelDimensoes.AutoSize = true;
            this.labelDimensoes.Location = new System.Drawing.Point(20, 25);
            this.labelDimensoes.Name = "labelDimensoes";
            this.labelDimensoes.Size = new System.Drawing.Size(106, 15);
            this.labelDimensoes.TabIndex = 0;
            this.labelDimensoes.Text = "Dimensões (mm):";
            // 
            // groupAmarracao
            // 
            this.groupAmarracao.Controls.Add(this.labelAmarracaoInfo);
            this.groupAmarracao.Controls.Add(this.comboTipoAmarracao);
            this.groupAmarracao.Controls.Add(this.labelTipoAmarracao);
            this.groupAmarracao.Controls.Add(this.labelPhi);
            this.groupAmarracao.Controls.Add(this.numMultiplicadorAmarracao);
            this.groupAmarracao.Controls.Add(this.labelMultiplicador);
            this.groupAmarracao.Location = new System.Drawing.Point(12, 440);
            this.groupAmarracao.Name = "groupAmarracao";
            this.groupAmarracao.Size = new System.Drawing.Size(580, 110);
            this.groupAmarracao.TabIndex = 4;
            this.groupAmarracao.TabStop = false;
            this.groupAmarracao.Text = "Configuração de Amarração";
            // 
            // labelAmarracaoInfo
            // 
            this.labelAmarracaoInfo.ForeColor = System.Drawing.Color.DarkBlue;
            this.labelAmarracaoInfo.Location = new System.Drawing.Point(300, 63);
            this.labelAmarracaoInfo.Name = "labelAmarracaoInfo";
            this.labelAmarracaoInfo.Size = new System.Drawing.Size(250, 25);
            this.labelAmarracaoInfo.TabIndex = 5;
            this.labelAmarracaoInfo.Text = "Amarração automática ajustada para vigas";
            // 
            // comboTipoAmarracao
            // 
            this.comboTipoAmarracao.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboTipoAmarracao.FormattingEnabled = true;
            this.comboTipoAmarracao.Items.AddRange(new object[] {
            "Automático",
            "Reta",
            "Dobrada 90°",
            "Gancho 135°"});
            this.comboTipoAmarracao.Location = new System.Drawing.Point(140, 63);
            this.comboTipoAmarracao.Name = "comboTipoAmarracao";
            this.comboTipoAmarracao.Size = new System.Drawing.Size(150, 23);
            this.comboTipoAmarracao.TabIndex = 4;
            // 
            // labelTipoAmarracao
            // 
            this.labelTipoAmarracao.AutoSize = true;
            this.labelTipoAmarracao.Location = new System.Drawing.Point(20, 65);
            this.labelTipoAmarracao.Name = "labelTipoAmarracao";
            this.labelTipoAmarracao.Size = new System.Drawing.Size(114, 15);
            this.labelTipoAmarracao.TabIndex = 3;
            this.labelTipoAmarracao.Text = "Tipo de amarração:";
            // 
            // labelPhi
            // 
            this.labelPhi.AutoSize = true;
            this.labelPhi.Location = new System.Drawing.Point(220, 30);
            this.labelPhi.Name = "labelPhi";
            this.labelPhi.Size = new System.Drawing.Size(108, 15);
            this.labelPhi.TabIndex = 2;
            this.labelPhi.Text = "? (50? para vigas)";
            // 
            // numMultiplicadorAmarracao
            // 
            this.numMultiplicadorAmarracao.Location = new System.Drawing.Point(160, 28);
            this.numMultiplicadorAmarracao.Maximum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numMultiplicadorAmarracao.Minimum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.numMultiplicadorAmarracao.Name = "numMultiplicadorAmarracao";
            this.numMultiplicadorAmarracao.Size = new System.Drawing.Size(50, 23);
            this.numMultiplicadorAmarracao.TabIndex = 1;
            this.numMultiplicadorAmarracao.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numMultiplicadorAmarracao.ValueChanged += new System.EventHandler(this.NumMultiplicadorAmarracao_ValueChanged);
            // 
            // labelMultiplicador
            // 
            this.labelMultiplicador.AutoSize = true;
            this.labelMultiplicador.Location = new System.Drawing.Point(20, 30);
            this.labelMultiplicador.Name = "labelMultiplicador";
            this.labelMultiplicador.Size = new System.Drawing.Size(139, 15);
            this.labelMultiplicador.TabIndex = 0;
            this.labelMultiplicador.Text = "Comprimento amarração:";
            // 
            // groupEstribos
            // 
            this.groupEstribos.Controls.Add(this.checkEstribosAutomaticos);
            this.groupEstribos.Controls.Add(this.buttonRemoverEstribo);
            this.groupEstribos.Controls.Add(this.buttonAdicionarEstribo);
            this.groupEstribos.Controls.Add(this.listViewEstribos);
            this.groupEstribos.Location = new System.Drawing.Point(12, 560);
            this.groupEstribos.Name = "groupEstribos";
            this.groupEstribos.Size = new System.Drawing.Size(1160, 90);
            this.groupEstribos.TabIndex = 5;
            this.groupEstribos.TabStop = false;
            this.groupEstribos.Text = "Configuração de Estribos";
            // 
            // checkEstribosAutomaticos
            // 
            this.checkEstribosAutomaticos.AutoSize = true;
            this.checkEstribosAutomaticos.Checked = true;
            this.checkEstribosAutomaticos.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkEstribosAutomaticos.Location = new System.Drawing.Point(480, 30);
            this.checkEstribosAutomaticos.Name = "checkEstribosAutomaticos";
            this.checkEstribosAutomaticos.Size = new System.Drawing.Size(422, 19);
            this.checkEstribosAutomaticos.TabIndex = 3;
            this.checkEstribosAutomaticos.Text = "Distribuição automática (zonas de corte elevado nas extremidades)";
            this.checkEstribosAutomaticos.UseVisualStyleBackColor = true;
            // 
            // buttonRemoverEstribo
            // 
            this.buttonRemoverEstribo.Location = new System.Drawing.Point(380, 55);
            this.buttonRemoverEstribo.Name = "buttonRemoverEstribo";
            this.buttonRemoverEstribo.Size = new System.Drawing.Size(80, 25);
            this.buttonRemoverEstribo.TabIndex = 2;
            this.buttonRemoverEstribo.Text = "Remover";
            this.buttonRemoverEstribo.UseVisualStyleBackColor = true;
            this.buttonRemoverEstribo.Click += new System.EventHandler(this.ButtonRemoverEstribo_Click);
            // 
            // buttonAdicionarEstribo
            // 
            this.buttonAdicionarEstribo.Location = new System.Drawing.Point(380, 30);
            this.buttonAdicionarEstribo.Name = "buttonAdicionarEstribo";
            this.buttonAdicionarEstribo.Size = new System.Drawing.Size(80, 25);
            this.buttonAdicionarEstribo.TabIndex = 1;
            this.buttonAdicionarEstribo.Text = "Adicionar";
            this.buttonAdicionarEstribo.UseVisualStyleBackColor = true;
            this.buttonAdicionarEstribo.Click += new System.EventHandler(this.ButtonAdicionarEstribo_Click);
            // 
            // listViewEstribos
            // 
            this.listViewEstribos.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colDiametroEstribo,
            this.colEspacamento,
            this.colAlternado});
            this.listViewEstribos.FullRowSelect = true;
            this.listViewEstribos.GridLines = true;
            this.listViewEstribos.Location = new System.Drawing.Point(20, 30);
            this.listViewEstribos.Name = "listViewEstribos";
            this.listViewEstribos.Size = new System.Drawing.Size(350, 50);
            this.listViewEstribos.TabIndex = 0;
            this.listViewEstribos.UseCompatibleStateImageBehavior = false;
            this.listViewEstribos.View = System.Windows.Forms.View.Details;
            // 
            // colDiametroEstribo
            // 
            this.colDiametroEstribo.Text = "Ø (mm)";
            this.colDiametroEstribo.Width = 70;
            // 
            // colEspacamento
            // 
            this.colEspacamento.Text = "Espaçamento (mm)";
            this.colEspacamento.Width = 120;
            // 
            // colAlternado
            // 
            this.colAlternado.Text = "Tipo";
            this.colAlternado.Width = 100;
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(300, 680);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(530, 15);
            this.progressBar.TabIndex = 9;
            this.progressBar.Visible = false;
            // 
            // buttonCancelar
            // 
            this.buttonCancelar.Location = new System.Drawing.Point(1020, 670);
            this.buttonCancelar.Name = "buttonCancelar";
            this.buttonCancelar.Size = new System.Drawing.Size(90, 35);
            this.buttonCancelar.TabIndex = 8;
            this.buttonCancelar.Text = "? Cancelar";
            this.buttonCancelar.UseVisualStyleBackColor = true;
            this.buttonCancelar.Click += new System.EventHandler(this.ButtonCancelar_Click);
            // 
            // buttonExecutar
            // 
            this.buttonExecutar.BackColor = System.Drawing.Color.LightGreen;
            this.buttonExecutar.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonExecutar.Location = new System.Drawing.Point(850, 670);
            this.buttonExecutar.Name = "buttonExecutar";
            this.buttonExecutar.Size = new System.Drawing.Size(150, 35);
            this.buttonExecutar.TabIndex = 7;
            this.buttonExecutar.Text = "? Executar Armadura";
            this.buttonExecutar.UseVisualStyleBackColor = false;
            this.buttonExecutar.Click += new System.EventHandler(this.ButtonExecutar_Click);
            // 
            // buttonPreVisualizacao
            // 
            this.buttonPreVisualizacao.BackColor = System.Drawing.Color.LightBlue;
            this.buttonPreVisualizacao.Location = new System.Drawing.Point(150, 670);
            this.buttonPreVisualizacao.Name = "buttonPreVisualizacao";
            this.buttonPreVisualizacao.Size = new System.Drawing.Size(130, 35);
            this.buttonPreVisualizacao.TabIndex = 6;
            this.buttonPreVisualizacao.Text = "?? Pré-visualização";
            this.buttonPreVisualizacao.UseVisualStyleBackColor = false;
            this.buttonPreVisualizacao.Click += new System.EventHandler(this.ButtonPreVisualizacao_Click);
            // 
            // buttonDefinicoes
            // 
            this.buttonDefinicoes.Location = new System.Drawing.Point(12, 670);
            this.buttonDefinicoes.Name = "buttonDefinicoes";
            this.buttonDefinicoes.Size = new System.Drawing.Size(120, 35);
            this.buttonDefinicoes.TabIndex = 10;
            this.buttonDefinicoes.Text = "? Definições";
            this.buttonDefinicoes.UseVisualStyleBackColor = true;
            this.buttonDefinicoes.Click += new System.EventHandler(this.ButtonDefinicoes_Click);
            // 
            // FormularioPrincipalAvancado
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 721);
            this.Controls.Add(this.buttonDefinicoes);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.buttonCancelar);
            this.Controls.Add(this.buttonExecutar);
            this.Controls.Add(this.buttonPreVisualizacao);
            this.Controls.Add(this.groupEstribos);
            this.Controls.Add(this.groupAmarracao);
            this.Controls.Add(this.groupVisualizador);
            this.Controls.Add(this.groupConfigArmadura);
            this.Controls.Add(this.groupFiltros);
            this.Controls.Add(this.groupTipoElemento);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "FormularioPrincipalAvancado";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Automação de Armaduras - Vigas v2.0";
            this.groupTipoElemento.ResumeLayout(false);
            this.groupTipoElemento.PerformLayout();
            this.groupFiltros.ResumeLayout(false);
            this.groupFiltros.PerformLayout();
            this.groupConfigArmadura.ResumeLayout(false);
            this.groupConfigArmadura.PerformLayout();
            this.groupVisualizador.ResumeLayout(false);
            this.groupVisualizador.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numLarguraViga)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAlturaViga)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numComprimentoViga)).EndInit();
            this.groupAmarracao.ResumeLayout(false);
            this.groupAmarracao.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMultiplicadorAmarracao)).EndInit();
            this.groupEstribos.ResumeLayout(false);
            this.groupEstribos.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupTipoElemento;
        private System.Windows.Forms.Label labelInfoElementos;
        private System.Windows.Forms.RadioButton radioVigas;
        private System.Windows.Forms.GroupBox groupFiltros;
        private System.Windows.Forms.Label labelContagem;
        private System.Windows.Forms.CheckBox checkSeleccaoActual;
        private System.Windows.Forms.CheckedListBox listNiveis;
        private System.Windows.Forms.Label labelNiveis;
        private System.Windows.Forms.ComboBox comboDesignacao;
        private System.Windows.Forms.Label labelDesignacao;
        private System.Windows.Forms.GroupBox groupConfigArmadura;
        private System.Windows.Forms.CheckBox checkAmarracaoAutomatica;
        private System.Windows.Forms.ComboBox comboTipoDistribuicao;
        private System.Windows.Forms.Label labelDistribuicao;
        private System.Windows.Forms.Button buttonRemoverVarao;
        private System.Windows.Forms.Button buttonAdicionarVarao;
        private System.Windows.Forms.ListView listViewVaroes;
        private System.Windows.Forms.ColumnHeader colQuantidade;
        private System.Windows.Forms.ColumnHeader colDiametro;
        private System.Windows.Forms.ColumnHeader colPosicao;
        private System.Windows.Forms.GroupBox groupVisualizador;
        private System.Windows.Forms.Label labelInfoVisualizador;
        private System.Windows.Forms.Button buttonModoEdicao;
        private System.Windows.Forms.Button buttonAlternarVista;
        private System.Windows.Forms.NumericUpDown numLarguraViga;
        private System.Windows.Forms.Label labelLargura;
        private System.Windows.Forms.NumericUpDown numAlturaViga;
        private System.Windows.Forms.Label labelAltura;
        private System.Windows.Forms.NumericUpDown numComprimentoViga;
        private System.Windows.Forms.Label labelComprimento;
        private System.Windows.Forms.Label labelDimensoes;
        private System.Windows.Forms.GroupBox groupAmarracao;
        private System.Windows.Forms.Label labelAmarracaoInfo;
        private System.Windows.Forms.ComboBox comboTipoAmarracao;
        private System.Windows.Forms.Label labelTipoAmarracao;
        private System.Windows.Forms.Label labelPhi;
        private System.Windows.Forms.NumericUpDown numMultiplicadorAmarracao;
        private System.Windows.Forms.Label labelMultiplicador;
        private System.Windows.Forms.GroupBox groupEstribos;
        private System.Windows.Forms.CheckBox checkEstribosAutomaticos;
        private System.Windows.Forms.Button buttonRemoverEstribo;
        private System.Windows.Forms.Button buttonAdicionarEstribo;
        private System.Windows.Forms.ListView listViewEstribos;
        private System.Windows.Forms.ColumnHeader colDiametroEstribo;
        private System.Windows.Forms.ColumnHeader colEspacamento;
        private System.Windows.Forms.ColumnHeader colAlternado;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button buttonCancelar;
        private System.Windows.Forms.Button buttonExecutar;
        private System.Windows.Forms.Button buttonPreVisualizacao;
        private System.Windows.Forms.Button buttonDefinicoes;
    }
}