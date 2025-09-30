namespace Rebar_Revit
{
    partial class FormularioPrincipal
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
            components = new System.ComponentModel.Container();
            VisualizadorArmaduraViga.InformacaoArmaduraViga informacaoArmaduraViga1 = new VisualizadorArmaduraViga.InformacaoArmaduraViga();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormularioPrincipal));
            panelEsquerdo = new Panel();
            groupSelecaoViga = new GroupBox();
            labelSelecionarViga = new Label();
            comboVigasDisponiveis = new ComboBox();
            labelFiltrarPor = new Label();
            radioFiltrarPorDescricao = new RadioButton();
            radioFiltrarPorDesignacao = new RadioButton();
            buttonAtualizarLista = new Button();
            checkUsarVigaSelecionada = new CheckBox();
            groupVisualizacao = new GroupBox();
            visualizador = new VisualizadorArmaduraViga();
            labelDimensoes = new Label();
            labelAltura = new Label();
            lblAlturaValor = new Label();
            labelLargura = new Label();
            lblLarguraValor = new Label();
            labelInfoViga = new Label();
            selectionTimer = new System.Windows.Forms.Timer(components);
            panelDireito = new Panel();
            groupArmaduraLongitudinal = new GroupBox();
            groupArmaduraSuperior = new GroupBox();
            checkCombinacaoSuperior = new CheckBox();
            labelQuant1Superior = new Label();
            numQuant1Superior = new NumericUpDown();
            labelDiam1Superior = new Label();
            comboDiam1Superior = new ComboBox();
            labelQuant2Superior = new Label();
            numQuant2Superior = new NumericUpDown();
            labelDiam2Superior = new Label();
            comboDiam2Superior = new ComboBox();
            groupArmaduraInferior = new GroupBox();
            checkCombinacaoInferior = new CheckBox();
            labelQuant1Inferior = new Label();
            numQuant1Inferior = new NumericUpDown();
            labelDiam1Inferior = new Label();
            comboDiam1Inferior = new ComboBox();
            labelQuant2Inferior = new Label();
            numQuant2Inferior = new NumericUpDown();
            labelDiam2Inferior = new Label();
            comboDiam2Inferior = new ComboBox();
            groupArmaduraLateral = new GroupBox();
            checkArmaduraLateral = new CheckBox();
            checkCombinacaoLateral = new CheckBox();
            labelQuant1Lateral = new Label();
            numQuant1Lateral = new NumericUpDown();
            labelDiam1Lateral = new Label();
            comboDiam1Lateral = new ComboBox();
            labelQuant2Lateral = new Label();
            numQuant2Lateral = new NumericUpDown();
            labelDiam2Lateral = new Label();
            comboDiam2Lateral = new ComboBox();
            groupEstribos = new GroupBox();
            checkCombinacaoEstribos = new CheckBox();
            labelDiam1Estribo = new Label();
            comboDiam1Estribo = new ComboBox();
            labelEspac1Estribo = new Label();
            numEspac1Estribo = new NumericUpDown();
            labelDiam2Estribo = new Label();
            comboDiam2Estribo = new ComboBox();
            groupParametros = new GroupBox();
            labelCobrimento = new Label();
            numCobrimento = new NumericUpDown();
            labelAmarracao = new Label();
            numMultAmarracao = new NumericUpDown();
            buttonExecutar = new Button();
            buttonCancelar = new Button();
            progressBar = new ProgressBar();
            panelEsquerdo.SuspendLayout();
            groupSelecaoViga.SuspendLayout();
            groupVisualizacao.SuspendLayout();
            panelDireito.SuspendLayout();
            groupArmaduraLongitudinal.SuspendLayout();
            groupArmaduraSuperior.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numQuant1Superior).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numQuant2Superior).BeginInit();
            groupArmaduraInferior.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numQuant1Inferior).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numQuant2Inferior).BeginInit();
            groupArmaduraLateral.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numQuant1Lateral).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numQuant2Lateral).BeginInit();
            groupEstribos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numEspac1Estribo).BeginInit();
            groupParametros.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numCobrimento).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numMultAmarracao).BeginInit();
            SuspendLayout();
            // 
            // panelEsquerdo
            // 
            panelEsquerdo.Controls.Add(groupSelecaoViga);
            panelEsquerdo.Controls.Add(groupVisualizacao);
            panelEsquerdo.Dock = DockStyle.Left;
            panelEsquerdo.Location = new Point(0, 0);
            panelEsquerdo.Margin = new Padding(3, 4, 3, 4);
            panelEsquerdo.Name = "panelEsquerdo";
            panelEsquerdo.Padding = new Padding(10, 12, 10, 12);
            panelEsquerdo.Size = new Size(500, 813);
            panelEsquerdo.TabIndex = 0;
            // 
            // groupSelecaoViga
            // 
            groupSelecaoViga.Controls.Add(labelSelecionarViga);
            groupSelecaoViga.Controls.Add(comboVigasDisponiveis);
            groupSelecaoViga.Controls.Add(labelFiltrarPor);
            groupSelecaoViga.Controls.Add(radioFiltrarPorDescricao);
            groupSelecaoViga.Controls.Add(radioFiltrarPorDesignacao);
            groupSelecaoViga.Controls.Add(buttonAtualizarLista);
            groupSelecaoViga.Controls.Add(checkUsarVigaSelecionada);
            groupSelecaoViga.Dock = DockStyle.Top;
            groupSelecaoViga.Location = new Point(10, 12);
            groupSelecaoViga.Margin = new Padding(3, 4, 3, 4);
            groupSelecaoViga.Name = "groupSelecaoViga";
            groupSelecaoViga.Padding = new Padding(3, 4, 3, 4);
            groupSelecaoViga.Size = new Size(480, 153);
            groupSelecaoViga.TabIndex = 0;
            groupSelecaoViga.TabStop = false;
            groupSelecaoViga.Text = "Selecione a Viga a Detalhar";
            // 
            // labelSelecionarViga
            // 
            labelSelecionarViga.AutoSize = true;
            labelSelecionarViga.Location = new Point(15, 31);
            labelSelecionarViga.Name = "labelSelecionarViga";
            labelSelecionarViga.Size = new Size(115, 20);
            labelSelecionarViga.TabIndex = 0;
            labelSelecionarViga.Text = "Selecionar Viga:";
            // 
            // comboVigasDisponiveis
            // 
            comboVigasDisponiveis.DropDownStyle = ComboBoxStyle.DropDownList;
            comboVigasDisponiveis.FormattingEnabled = true;
            comboVigasDisponiveis.Location = new Point(15, 56);
            comboVigasDisponiveis.Margin = new Padding(3, 4, 3, 4);
            comboVigasDisponiveis.Name = "comboVigasDisponiveis";
            comboVigasDisponiveis.Size = new Size(350, 28);
            comboVigasDisponiveis.TabIndex = 1;
            // 
            // labelFiltrarPor
            // 
            labelFiltrarPor.AutoSize = true;
            labelFiltrarPor.Location = new Point(15, 101);
            labelFiltrarPor.Name = "labelFiltrarPor";
            labelFiltrarPor.Size = new Size(77, 20);
            labelFiltrarPor.TabIndex = 2;
            labelFiltrarPor.Text = "Filtrar por:";
            // 
            // radioFiltrarPorDescricao
            // 
            radioFiltrarPorDescricao.AutoSize = true;
            radioFiltrarPorDescricao.Checked = true;
            radioFiltrarPorDescricao.Location = new Point(100, 101);
            radioFiltrarPorDescricao.Margin = new Padding(3, 4, 3, 4);
            radioFiltrarPorDescricao.Name = "radioFiltrarPorDescricao";
            radioFiltrarPorDescricao.Size = new Size(61, 24);
            radioFiltrarPorDescricao.TabIndex = 3;
            radioFiltrarPorDescricao.TabStop = true;
            radioFiltrarPorDescricao.Text = "Type";
            radioFiltrarPorDescricao.UseVisualStyleBackColor = true;
            // 
            // radioFiltrarPorDesignacao
            // 
            radioFiltrarPorDesignacao.AutoSize = true;
            radioFiltrarPorDesignacao.Location = new Point(185, 101);
            radioFiltrarPorDesignacao.Margin = new Padding(3, 4, 3, 4);
            radioFiltrarPorDesignacao.Name = "radioFiltrarPorDesignacao";
            radioFiltrarPorDesignacao.Size = new Size(108, 24);
            radioFiltrarPorDesignacao.TabIndex = 4;
            radioFiltrarPorDesignacao.Text = "Designação";
            radioFiltrarPorDesignacao.UseVisualStyleBackColor = true;
            // 
            // buttonAtualizarLista
            // 
            buttonAtualizarLista.Location = new Point(375, 56);
            buttonAtualizarLista.Margin = new Padding(3, 4, 3, 4);
            buttonAtualizarLista.Name = "buttonAtualizarLista";
            buttonAtualizarLista.Size = new Size(90, 31);
            buttonAtualizarLista.TabIndex = 5;
            buttonAtualizarLista.Text = "Atualizar";
            buttonAtualizarLista.UseVisualStyleBackColor = true;
            // 
            // checkUsarVigaSelecionada
            // 
            checkUsarVigaSelecionada.AutoSize = true;
            checkUsarVigaSelecionada.Location = new Point(190, 27);
            checkUsarVigaSelecionada.Name = "checkUsarVigaSelecionada";
            checkUsarVigaSelecionada.Size = new Size(175, 24);
            checkUsarVigaSelecionada.TabIndex = 6;
            checkUsarVigaSelecionada.Text = "Usar viga selecionada";
            checkUsarVigaSelecionada.UseVisualStyleBackColor = true;
            checkUsarVigaSelecionada.CheckedChanged += CheckUsarVigaSelecionada_CheckedChanged;
            // 
            // groupVisualizacao
            // 
            groupVisualizacao.Controls.Add(visualizador);
            groupVisualizacao.Controls.Add(labelDimensoes);
            groupVisualizacao.Controls.Add(labelAltura);
            groupVisualizacao.Controls.Add(lblAlturaValor);
            groupVisualizacao.Controls.Add(labelLargura);
            groupVisualizacao.Controls.Add(lblLarguraValor);
            groupVisualizacao.Controls.Add(labelInfoViga);
            groupVisualizacao.Dock = DockStyle.Fill;
            groupVisualizacao.Location = new Point(10, 12);
            groupVisualizacao.Margin = new Padding(3, 4, 3, 4);
            groupVisualizacao.Name = "groupVisualizacao";
            groupVisualizacao.Padding = new Padding(3, 4, 3, 4);
            groupVisualizacao.Size = new Size(480, 789);
            groupVisualizacao.TabIndex = 1;
            groupVisualizacao.TabStop = false;
            groupVisualizacao.Text = "Pré-visualização (secção)";
            // 
            // visualizador
            // 
            visualizador.BackColor = Color.White;
            visualizador.BorderStyle = BorderStyle.FixedSingle;
            informacaoArmaduraViga1.Altura = 500D;
            informacaoArmaduraViga1.AmarracaoAutomatica = true;
            informacaoArmaduraViga1.Designacao = "";
            informacaoArmaduraViga1.Largura = 300D;
            informacaoArmaduraViga1.MultiplicadorAmarracao = 50D;
            informacaoArmaduraViga1.Recobrimento = 25D;
            informacaoArmaduraViga1.TipoFamilia = "";
            visualizador.InformacaoViga = informacaoArmaduraViga1;
            visualizador.Location = new Point(15, 230);
            visualizador.Margin = new Padding(3, 4, 3, 4);
            visualizador.ModoEdicao = false;
            visualizador.MostrarCorteTransversal = true;
            visualizador.Name = "visualizador";
            visualizador.Size = new Size(450, 461);
            visualizador.TabIndex = 8;
            visualizador.Load += visualizador_Load;
            // 
            // labelDimensoes
            // 
            labelDimensoes.AutoSize = true;
            labelDimensoes.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelDimensoes.Location = new Point(18, 167);
            labelDimensoes.Name = "labelDimensoes";
            labelDimensoes.Size = new Size(143, 18);
            labelDimensoes.TabIndex = 0;
            labelDimensoes.Text = "Dimensões (mm):";
            // 
            // labelAltura
            // 
            labelAltura.AutoSize = true;
            labelAltura.Location = new Point(350, 168);
            labelAltura.Name = "labelAltura";
            labelAltura.Size = new Size(52, 20);
            labelAltura.TabIndex = 3;
            labelAltura.Text = "Altura:";
            // 
            // lblAlturaValor
            // 
            lblAlturaValor.AutoSize = true;
            lblAlturaValor.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblAlturaValor.Location = new Point(408, 170);
            lblAlturaValor.Name = "lblAlturaValor";
            lblAlturaValor.Size = new Size(44, 18);
            lblAlturaValor.TabIndex = 4;
            lblAlturaValor.Text = "0000";
            // 
            // labelLargura
            // 
            labelLargura.AutoSize = true;
            labelLargura.Location = new Point(185, 167);
            labelLargura.Name = "labelLargura";
            labelLargura.Size = new Size(62, 20);
            labelLargura.TabIndex = 5;
            labelLargura.Text = "Largura:";
            // 
            // lblLarguraValor
            // 
            lblLarguraValor.AutoSize = true;
            lblLarguraValor.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblLarguraValor.Location = new Point(253, 169);
            lblLarguraValor.Name = "lblLarguraValor";
            lblLarguraValor.Size = new Size(44, 18);
            lblLarguraValor.TabIndex = 6;
            lblLarguraValor.Text = "0000";
            // 
            // labelInfoViga
            // 
            labelInfoViga.BackColor = Color.WhiteSmoke;
            labelInfoViga.BorderStyle = BorderStyle.FixedSingle;
            labelInfoViga.Location = new Point(15, 269);
            labelInfoViga.Name = "labelInfoViga";
            labelInfoViga.Size = new Size(450, 422);
            labelInfoViga.TabIndex = 7;
            labelInfoViga.Text = "Pré-visualização 2D da secção da viga com armaduras";
            labelInfoViga.TextAlign = ContentAlignment.MiddleCenter;
            labelInfoViga.Visible = false;
            labelInfoViga.Click += labelInfoViga_Click;
            // 
            // panelDireito
            // 
            panelDireito.Controls.Add(groupArmaduraLongitudinal);
            panelDireito.Controls.Add(groupEstribos);
            panelDireito.Controls.Add(groupParametros);
            panelDireito.Controls.Add(buttonExecutar);
            panelDireito.Controls.Add(buttonCancelar);
            panelDireito.Controls.Add(progressBar);
            panelDireito.Dock = DockStyle.Fill;
            panelDireito.Location = new Point(500, 0);
            panelDireito.Margin = new Padding(3, 4, 3, 4);
            panelDireito.Name = "panelDireito";
            panelDireito.Padding = new Padding(10, 12, 10, 12);
            panelDireito.Size = new Size(400, 813);
            panelDireito.TabIndex = 1;
            // 
            // groupArmaduraLongitudinal
            // 
            groupArmaduraLongitudinal.Controls.Add(groupArmaduraSuperior);
            groupArmaduraLongitudinal.Controls.Add(groupArmaduraInferior);
            groupArmaduraLongitudinal.Controls.Add(groupArmaduraLateral);
            groupArmaduraLongitudinal.Dock = DockStyle.Top;
            groupArmaduraLongitudinal.Location = new Point(10, 240);
            groupArmaduraLongitudinal.Margin = new Padding(3, 4, 3, 4);
            groupArmaduraLongitudinal.Name = "groupArmaduraLongitudinal";
            groupArmaduraLongitudinal.Padding = new Padding(3, 4, 3, 4);
            groupArmaduraLongitudinal.Size = new Size(380, 474);
            groupArmaduraLongitudinal.TabIndex = 0;
            groupArmaduraLongitudinal.TabStop = false;
            groupArmaduraLongitudinal.Text = "Armadura Longitudinal";
            // 
            // groupArmaduraSuperior
            // 
            groupArmaduraSuperior.Controls.Add(checkCombinacaoSuperior);
            groupArmaduraSuperior.Controls.Add(labelQuant1Superior);
            groupArmaduraSuperior.Controls.Add(numQuant1Superior);
            groupArmaduraSuperior.Controls.Add(labelDiam1Superior);
            groupArmaduraSuperior.Controls.Add(comboDiam1Superior);
            groupArmaduraSuperior.Controls.Add(labelQuant2Superior);
            groupArmaduraSuperior.Controls.Add(numQuant2Superior);
            groupArmaduraSuperior.Controls.Add(labelDiam2Superior);
            groupArmaduraSuperior.Controls.Add(comboDiam2Superior);
            groupArmaduraSuperior.Location = new Point(15, 31);
            groupArmaduraSuperior.Margin = new Padding(3, 4, 3, 4);
            groupArmaduraSuperior.Name = "groupArmaduraSuperior";
            groupArmaduraSuperior.Padding = new Padding(3, 4, 3, 4);
            groupArmaduraSuperior.Size = new Size(350, 126);
            groupArmaduraSuperior.TabIndex = 0;
            groupArmaduraSuperior.TabStop = false;
            groupArmaduraSuperior.Text = "Armadura Superior";
            // 
            // checkCombinacaoSuperior
            // 
            checkCombinacaoSuperior.AutoSize = true;
            checkCombinacaoSuperior.Location = new Point(225, 27);
            checkCombinacaoSuperior.Name = "checkCombinacaoSuperior";
            checkCombinacaoSuperior.Size = new Size(115, 24);
            checkCombinacaoSuperior.TabIndex = 4;
            checkCombinacaoSuperior.Text = "Combinação";
            checkCombinacaoSuperior.UseVisualStyleBackColor = true;
            // 
            // labelQuant1Superior
            // 
            labelQuant1Superior.AutoSize = true;
            labelQuant1Superior.Enabled = false;
            labelQuant1Superior.Location = new Point(15, 54);
            labelQuant1Superior.Name = "labelQuant1Superior";
            labelQuant1Superior.Size = new Size(49, 20);
            labelQuant1Superior.TabIndex = 5;
            labelQuant1Superior.Text = "Qtd 1:";
            // 
            // numQuant1Superior
            // 
            numQuant1Superior.Enabled = false;
            numQuant1Superior.Location = new Point(15, 82);
            numQuant1Superior.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            numQuant1Superior.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numQuant1Superior.Name = "numQuant1Superior";
            numQuant1Superior.Size = new Size(50, 27);
            numQuant1Superior.TabIndex = 6;
            numQuant1Superior.Value = new decimal(new int[] { 2, 0, 0, 0 });
            // 
            // labelDiam1Superior
            // 
            labelDiam1Superior.AutoSize = true;
            labelDiam1Superior.Enabled = false;
            labelDiam1Superior.Location = new Point(76, 54);
            labelDiam1Superior.Name = "labelDiam1Superior";
            labelDiam1Superior.Size = new Size(71, 20);
            labelDiam1Superior.TabIndex = 7;
            labelDiam1Superior.Text = "Ø1 (mm):";
            // 
            // comboDiam1Superior
            // 
            comboDiam1Superior.DropDownStyle = ComboBoxStyle.DropDownList;
            comboDiam1Superior.Enabled = false;
            comboDiam1Superior.Location = new Point(76, 82);
            comboDiam1Superior.Name = "comboDiam1Superior";
            comboDiam1Superior.Size = new Size(60, 28);
            comboDiam1Superior.TabIndex = 8;
            // 
            // labelQuant2Superior
            // 
            labelQuant2Superior.AutoSize = true;
            labelQuant2Superior.Enabled = false;
            labelQuant2Superior.Location = new Point(200, 54);
            labelQuant2Superior.Name = "labelQuant2Superior";
            labelQuant2Superior.Size = new Size(49, 20);
            labelQuant2Superior.TabIndex = 9;
            labelQuant2Superior.Text = "Qtd 2:";
            // 
            // numQuant2Superior
            // 
            numQuant2Superior.Enabled = false;
            numQuant2Superior.Location = new Point(199, 83);
            numQuant2Superior.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            numQuant2Superior.Name = "numQuant2Superior";
            numQuant2Superior.Size = new Size(50, 27);
            numQuant2Superior.TabIndex = 10;
            numQuant2Superior.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // labelDiam2Superior
            // 
            labelDiam2Superior.AutoSize = true;
            labelDiam2Superior.Enabled = false;
            labelDiam2Superior.Location = new Point(260, 54);
            labelDiam2Superior.Name = "labelDiam2Superior";
            labelDiam2Superior.Size = new Size(71, 20);
            labelDiam2Superior.TabIndex = 11;
            labelDiam2Superior.Text = "Ø2 (mm):";
            // 
            // comboDiam2Superior
            // 
            comboDiam2Superior.DropDownStyle = ComboBoxStyle.DropDownList;
            comboDiam2Superior.Enabled = false;
            comboDiam2Superior.Location = new Point(265, 82);
            comboDiam2Superior.Name = "comboDiam2Superior";
            comboDiam2Superior.Size = new Size(60, 28);
            comboDiam2Superior.TabIndex = 12;
            // 
            // groupArmaduraInferior
            // 
            groupArmaduraInferior.Controls.Add(checkCombinacaoInferior);
            groupArmaduraInferior.Controls.Add(labelQuant1Inferior);
            groupArmaduraInferior.Controls.Add(numQuant1Inferior);
            groupArmaduraInferior.Controls.Add(labelDiam1Inferior);
            groupArmaduraInferior.Controls.Add(comboDiam1Inferior);
            groupArmaduraInferior.Controls.Add(labelQuant2Inferior);
            groupArmaduraInferior.Controls.Add(numQuant2Inferior);
            groupArmaduraInferior.Controls.Add(labelDiam2Inferior);
            groupArmaduraInferior.Controls.Add(comboDiam2Inferior);
            groupArmaduraInferior.Location = new Point(15, 162);
            groupArmaduraInferior.Margin = new Padding(3, 4, 3, 4);
            groupArmaduraInferior.Name = "groupArmaduraInferior";
            groupArmaduraInferior.Padding = new Padding(3, 4, 3, 4);
            groupArmaduraInferior.Size = new Size(350, 138);
            groupArmaduraInferior.TabIndex = 1;
            groupArmaduraInferior.TabStop = false;
            groupArmaduraInferior.Text = "Armadura Inferior";
            // 
            // checkCombinacaoInferior
            // 
            checkCombinacaoInferior.AutoSize = true;
            checkCombinacaoInferior.Location = new Point(225, 27);
            checkCombinacaoInferior.Name = "checkCombinacaoInferior";
            checkCombinacaoInferior.Size = new Size(115, 24);
            checkCombinacaoInferior.TabIndex = 4;
            checkCombinacaoInferior.Text = "Combinação";
            checkCombinacaoInferior.UseVisualStyleBackColor = true;
            // 
            // labelQuant1Inferior
            // 
            labelQuant1Inferior.AutoSize = true;
            labelQuant1Inferior.Enabled = false;
            labelQuant1Inferior.Location = new Point(15, 62);
            labelQuant1Inferior.Name = "labelQuant1Inferior";
            labelQuant1Inferior.Size = new Size(49, 20);
            labelQuant1Inferior.TabIndex = 5;
            labelQuant1Inferior.Text = "Qtd 1:";
            // 
            // numQuant1Inferior
            // 
            numQuant1Inferior.Enabled = false;
            numQuant1Inferior.Location = new Point(14, 90);
            numQuant1Inferior.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            numQuant1Inferior.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numQuant1Inferior.Name = "numQuant1Inferior";
            numQuant1Inferior.Size = new Size(50, 27);
            numQuant1Inferior.TabIndex = 6;
            numQuant1Inferior.Value = new decimal(new int[] { 2, 0, 0, 0 });
            // 
            // labelDiam1Inferior
            // 
            labelDiam1Inferior.AutoSize = true;
            labelDiam1Inferior.Enabled = false;
            labelDiam1Inferior.Location = new Point(75, 62);
            labelDiam1Inferior.Name = "labelDiam1Inferior";
            labelDiam1Inferior.Size = new Size(71, 20);
            labelDiam1Inferior.TabIndex = 7;
            labelDiam1Inferior.Text = "Ø1 (mm):";
            // 
            // comboDiam1Inferior
            // 
            comboDiam1Inferior.DropDownStyle = ComboBoxStyle.DropDownList;
            comboDiam1Inferior.Enabled = false;
            comboDiam1Inferior.Location = new Point(75, 89);
            comboDiam1Inferior.Name = "comboDiam1Inferior";
            comboDiam1Inferior.Size = new Size(60, 28);
            comboDiam1Inferior.TabIndex = 8;
            // 
            // labelQuant2Inferior
            // 
            labelQuant2Inferior.AutoSize = true;
            labelQuant2Inferior.Enabled = false;
            labelQuant2Inferior.Location = new Point(200, 62);
            labelQuant2Inferior.Name = "labelQuant2Inferior";
            labelQuant2Inferior.Size = new Size(49, 20);
            labelQuant2Inferior.TabIndex = 9;
            labelQuant2Inferior.Text = "Qtd 2:";
            // 
            // numQuant2Inferior
            // 
            numQuant2Inferior.Enabled = false;
            numQuant2Inferior.Location = new Point(200, 90);
            numQuant2Inferior.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            numQuant2Inferior.Name = "numQuant2Inferior";
            numQuant2Inferior.Size = new Size(50, 27);
            numQuant2Inferior.TabIndex = 10;
            numQuant2Inferior.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // labelDiam2Inferior
            // 
            labelDiam2Inferior.AutoSize = true;
            labelDiam2Inferior.Enabled = false;
            labelDiam2Inferior.Location = new Point(260, 62);
            labelDiam2Inferior.Name = "labelDiam2Inferior";
            labelDiam2Inferior.Size = new Size(71, 20);
            labelDiam2Inferior.TabIndex = 11;
            labelDiam2Inferior.Text = "Ø2 (mm):";
            // 
            // comboDiam2Inferior
            // 
            comboDiam2Inferior.DropDownStyle = ComboBoxStyle.DropDownList;
            comboDiam2Inferior.Enabled = false;
            comboDiam2Inferior.Location = new Point(271, 87);
            comboDiam2Inferior.Name = "comboDiam2Inferior";
            comboDiam2Inferior.Size = new Size(60, 28);
            comboDiam2Inferior.TabIndex = 12;
            // 
            // groupArmaduraLateral
            // 
            groupArmaduraLateral.Controls.Add(checkArmaduraLateral);
            groupArmaduraLateral.Controls.Add(checkCombinacaoLateral);
            groupArmaduraLateral.Controls.Add(labelQuant1Lateral);
            groupArmaduraLateral.Controls.Add(numQuant1Lateral);
            groupArmaduraLateral.Controls.Add(labelDiam1Lateral);
            groupArmaduraLateral.Controls.Add(comboDiam1Lateral);
            groupArmaduraLateral.Controls.Add(labelQuant2Lateral);
            groupArmaduraLateral.Controls.Add(numQuant2Lateral);
            groupArmaduraLateral.Controls.Add(labelDiam2Lateral);
            groupArmaduraLateral.Controls.Add(comboDiam2Lateral);
            groupArmaduraLateral.Location = new Point(15, 306);
            groupArmaduraLateral.Margin = new Padding(3, 4, 3, 4);
            groupArmaduraLateral.Name = "groupArmaduraLateral";
            groupArmaduraLateral.Padding = new Padding(3, 4, 3, 4);
            groupArmaduraLateral.Size = new Size(350, 154);
            groupArmaduraLateral.TabIndex = 2;
            groupArmaduraLateral.TabStop = false;
            groupArmaduraLateral.Text = "Armadura Alma";
            groupArmaduraLateral.Enter += groupArmaduraLateral_Enter;
            // 
            // checkArmaduraLateral
            // 
            checkArmaduraLateral.AutoSize = true;
            checkArmaduraLateral.Location = new Point(6, 27);
            checkArmaduraLateral.Name = "checkArmaduraLateral";
            checkArmaduraLateral.Size = new Size(70, 24);
            checkArmaduraLateral.TabIndex = 14;
            checkArmaduraLateral.Text = "Ativar";
            checkArmaduraLateral.UseVisualStyleBackColor = true;
            // 
            // checkCombinacaoLateral
            // 
            checkCombinacaoLateral.AutoSize = true;
            checkCombinacaoLateral.Enabled = false;
            checkCombinacaoLateral.Location = new Point(225, 27);
            checkCombinacaoLateral.Name = "checkCombinacaoLateral";
            checkCombinacaoLateral.Size = new Size(115, 24);
            checkCombinacaoLateral.TabIndex = 5;
            checkCombinacaoLateral.Text = "Combinação";
            checkCombinacaoLateral.UseVisualStyleBackColor = true;
            // 
            // labelQuant1Lateral
            // 
            labelQuant1Lateral.AutoSize = true;
            labelQuant1Lateral.Enabled = false;
            labelQuant1Lateral.Location = new Point(14, 66);
            labelQuant1Lateral.Name = "labelQuant1Lateral";
            labelQuant1Lateral.Size = new Size(49, 20);
            labelQuant1Lateral.TabIndex = 6;
            labelQuant1Lateral.Text = "Qtd 1:";
            // 
            // numQuant1Lateral
            // 
            numQuant1Lateral.Enabled = false;
            numQuant1Lateral.Location = new Point(15, 99);
            numQuant1Lateral.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            numQuant1Lateral.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numQuant1Lateral.Name = "numQuant1Lateral";
            numQuant1Lateral.Size = new Size(50, 27);
            numQuant1Lateral.TabIndex = 7;
            numQuant1Lateral.Value = new decimal(new int[] { 2, 0, 0, 0 });
            // 
            // labelDiam1Lateral
            // 
            labelDiam1Lateral.AutoSize = true;
            labelDiam1Lateral.Enabled = false;
            labelDiam1Lateral.Location = new Point(83, 66);
            labelDiam1Lateral.Name = "labelDiam1Lateral";
            labelDiam1Lateral.Size = new Size(71, 20);
            labelDiam1Lateral.TabIndex = 8;
            labelDiam1Lateral.Text = "Ø1 (mm):";
            // 
            // comboDiam1Lateral
            // 
            comboDiam1Lateral.DropDownStyle = ComboBoxStyle.DropDownList;
            comboDiam1Lateral.Enabled = false;
            comboDiam1Lateral.Location = new Point(75, 99);
            comboDiam1Lateral.Name = "comboDiam1Lateral";
            comboDiam1Lateral.Size = new Size(60, 28);
            comboDiam1Lateral.TabIndex = 9;
            // 
            // labelQuant2Lateral
            // 
            labelQuant2Lateral.AutoSize = true;
            labelQuant2Lateral.Enabled = false;
            labelQuant2Lateral.Location = new Point(201, 66);
            labelQuant2Lateral.Name = "labelQuant2Lateral";
            labelQuant2Lateral.Size = new Size(49, 20);
            labelQuant2Lateral.TabIndex = 10;
            labelQuant2Lateral.Text = "Qtd 2:";
            // 
            // numQuant2Lateral
            // 
            numQuant2Lateral.Enabled = false;
            numQuant2Lateral.Location = new Point(199, 99);
            numQuant2Lateral.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            numQuant2Lateral.Name = "numQuant2Lateral";
            numQuant2Lateral.Size = new Size(50, 27);
            numQuant2Lateral.TabIndex = 11;
            numQuant2Lateral.Value = new decimal(new int[] { 2, 0, 0, 0 });
            // 
            // labelDiam2Lateral
            // 
            labelDiam2Lateral.AutoSize = true;
            labelDiam2Lateral.Enabled = false;
            labelDiam2Lateral.Location = new Point(265, 66);
            labelDiam2Lateral.Name = "labelDiam2Lateral";
            labelDiam2Lateral.Size = new Size(71, 20);
            labelDiam2Lateral.TabIndex = 12;
            labelDiam2Lateral.Text = "Ø2 (mm):";
            // 
            // comboDiam2Lateral
            // 
            comboDiam2Lateral.DropDownStyle = ComboBoxStyle.DropDownList;
            comboDiam2Lateral.Enabled = false;
            comboDiam2Lateral.Location = new Point(271, 99);
            comboDiam2Lateral.Name = "comboDiam2Lateral";
            comboDiam2Lateral.Size = new Size(60, 28);
            comboDiam2Lateral.TabIndex = 13;
            // 
            // groupEstribos
            // 
            groupEstribos.Controls.Add(checkCombinacaoEstribos);
            groupEstribos.Controls.Add(labelDiam1Estribo);
            groupEstribos.Controls.Add(comboDiam1Estribo);
            groupEstribos.Controls.Add(labelEspac1Estribo);
            groupEstribos.Controls.Add(numEspac1Estribo);
            groupEstribos.Controls.Add(labelDiam2Estribo);
            groupEstribos.Controls.Add(comboDiam2Estribo);
            groupEstribos.Dock = DockStyle.Top;
            groupEstribos.Location = new Point(10, 113);
            groupEstribos.Margin = new Padding(3, 4, 3, 4);
            groupEstribos.Name = "groupEstribos";
            groupEstribos.Padding = new Padding(3, 4, 3, 4);
            groupEstribos.Size = new Size(380, 127);
            groupEstribos.TabIndex = 1;
            groupEstribos.TabStop = false;
            groupEstribos.Text = "Estribos";
            // 
            // checkCombinacaoEstribos
            // 
            checkCombinacaoEstribos.AutoSize = true;
            checkCombinacaoEstribos.Location = new Point(240, 27);
            checkCombinacaoEstribos.Name = "checkCombinacaoEstribos";
            checkCombinacaoEstribos.Size = new Size(115, 24);
            checkCombinacaoEstribos.TabIndex = 5;
            checkCombinacaoEstribos.Text = "Combinação";
            checkCombinacaoEstribos.UseVisualStyleBackColor = true;
            // 
            // labelDiam1Estribo
            // 
            labelDiam1Estribo.AutoSize = true;
            labelDiam1Estribo.Enabled = false;
            labelDiam1Estribo.Location = new Point(9, 54);
            labelDiam1Estribo.Name = "labelDiam1Estribo";
            labelDiam1Estribo.Size = new Size(71, 20);
            labelDiam1Estribo.TabIndex = 6;
            labelDiam1Estribo.Text = "Ø1 (mm):";
            // 
            // comboDiam1Estribo
            // 
            comboDiam1Estribo.DropDownStyle = ComboBoxStyle.DropDownList;
            comboDiam1Estribo.Enabled = false;
            comboDiam1Estribo.Location = new Point(15, 78);
            comboDiam1Estribo.Name = "comboDiam1Estribo";
            comboDiam1Estribo.Size = new Size(60, 28);
            comboDiam1Estribo.TabIndex = 7;
            // 
            // labelEspac1Estribo
            // 
            labelEspac1Estribo.AutoSize = true;
            labelEspac1Estribo.Enabled = false;
            labelEspac1Estribo.Location = new Point(272, 54);
            labelEspac1Estribo.Name = "labelEspac1Estribo";
            labelEspac1Estribo.Size = new Size(78, 20);
            labelEspac1Estribo.TabIndex = 8;
            labelEspac1Estribo.Text = "Esp. (mm):";
            // 
            // numEspac1Estribo
            // 
            numEspac1Estribo.Enabled = false;
            numEspac1Estribo.Increment = new decimal(new int[] { 25, 0, 0, 0 });
            numEspac1Estribo.Location = new Point(285, 79);
            numEspac1Estribo.Maximum = new decimal(new int[] { 500, 0, 0, 0 });
            numEspac1Estribo.Minimum = new decimal(new int[] { 50, 0, 0, 0 });
            numEspac1Estribo.Name = "numEspac1Estribo";
            numEspac1Estribo.Size = new Size(70, 27);
            numEspac1Estribo.TabIndex = 9;
            numEspac1Estribo.Value = new decimal(new int[] { 125, 0, 0, 0 });
            // 
            // labelDiam2Estribo
            // 
            labelDiam2Estribo.AutoSize = true;
            labelDiam2Estribo.Enabled = false;
            labelDiam2Estribo.Location = new Point(91, 54);
            labelDiam2Estribo.Name = "labelDiam2Estribo";
            labelDiam2Estribo.Size = new Size(71, 20);
            labelDiam2Estribo.TabIndex = 10;
            labelDiam2Estribo.Text = "Ø2 (mm):";
            // 
            // comboDiam2Estribo
            // 
            comboDiam2Estribo.DropDownStyle = ComboBoxStyle.DropDownList;
            comboDiam2Estribo.Enabled = false;
            comboDiam2Estribo.Location = new Point(98, 78);
            comboDiam2Estribo.Name = "comboDiam2Estribo";
            comboDiam2Estribo.Size = new Size(60, 28);
            comboDiam2Estribo.TabIndex = 11;
            // 
            // groupParametros
            // 
            groupParametros.Controls.Add(labelCobrimento);
            groupParametros.Controls.Add(numCobrimento);
            groupParametros.Controls.Add(labelAmarracao);
            groupParametros.Controls.Add(numMultAmarracao);
            groupParametros.Dock = DockStyle.Top;
            groupParametros.Location = new Point(10, 12);
            groupParametros.Margin = new Padding(3, 4, 3, 4);
            groupParametros.Name = "groupParametros";
            groupParametros.Padding = new Padding(3, 4, 3, 4);
            groupParametros.Size = new Size(380, 101);
            groupParametros.TabIndex = 2;
            groupParametros.TabStop = false;
            groupParametros.Text = "Parâmetros";
            // 
            // labelCobrimento
            // 
            labelCobrimento.AutoSize = true;
            labelCobrimento.Location = new Point(15, 31);
            labelCobrimento.Name = "labelCobrimento";
            labelCobrimento.Size = new Size(146, 20);
            labelCobrimento.TabIndex = 0;
            labelCobrimento.Text = "Recobrimento (mm):";
            // 
            // numCobrimento
            // 
            numCobrimento.Increment = new decimal(new int[] { 5, 0, 0, 0 });
            numCobrimento.Location = new Point(15, 56);
            numCobrimento.Margin = new Padding(3, 4, 3, 4);
            numCobrimento.Minimum = new decimal(new int[] { 15, 0, 0, 0 });
            numCobrimento.Name = "numCobrimento";
            numCobrimento.Size = new Size(70, 27);
            numCobrimento.TabIndex = 1;
            numCobrimento.Value = new decimal(new int[] { 30, 0, 0, 0 });
            // 
            // labelAmarracao
            // 
            labelAmarracao.AutoSize = true;
            labelAmarracao.Location = new Point(162, 31);
            labelAmarracao.Name = "labelAmarracao";
            labelAmarracao.Size = new Size(178, 20);
            labelAmarracao.TabIndex = 2;
            labelAmarracao.Text = "Multiplicador Amarração:";
            // 
            // numMultAmarracao
            // 
            numMultAmarracao.Increment = new decimal(new int[] { 5, 0, 0, 0 });
            numMultAmarracao.Location = new Point(162, 56);
            numMultAmarracao.Margin = new Padding(3, 4, 3, 4);
            numMultAmarracao.Minimum = new decimal(new int[] { 30, 0, 0, 0 });
            numMultAmarracao.Name = "numMultAmarracao";
            numMultAmarracao.Size = new Size(70, 27);
            numMultAmarracao.TabIndex = 3;
            numMultAmarracao.Value = new decimal(new int[] { 50, 0, 0, 0 });
            // 
            // buttonExecutar
            // 
            buttonExecutar.BackColor = Color.LightGreen;
            buttonExecutar.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            buttonExecutar.Location = new Point(25, 722);
            buttonExecutar.Margin = new Padding(3, 4, 3, 4);
            buttonExecutar.Name = "buttonExecutar";
            buttonExecutar.Size = new Size(160, 44);
            buttonExecutar.TabIndex = 4;
            buttonExecutar.Text = "Criar Armadura";
            buttonExecutar.UseVisualStyleBackColor = false;
            // 
            // buttonCancelar
            // 
            buttonCancelar.Location = new Point(265, 722);
            buttonCancelar.Margin = new Padding(3, 4, 3, 4);
            buttonCancelar.Name = "buttonCancelar";
            buttonCancelar.Size = new Size(100, 44);
            buttonCancelar.TabIndex = 5;
            buttonCancelar.Text = "Cancelar";
            buttonCancelar.UseVisualStyleBackColor = true;
            // 
            // progressBar
            // 
            progressBar.Location = new Point(25, 774);
            progressBar.Margin = new Padding(3, 4, 3, 4);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(350, 25);
            progressBar.TabIndex = 6;
            progressBar.Visible = false;
            // 
            // FormularioPrincipal
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(900, 813);
            Controls.Add(panelDireito);
            Controls.Add(panelEsquerdo);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(3, 4, 3, 4);
            MaximizeBox = false;
            Name = "FormularioPrincipal";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Armaduras em Vigas - v0.2_Alpha";
            panelEsquerdo.ResumeLayout(false);
            groupSelecaoViga.ResumeLayout(false);
            groupSelecaoViga.PerformLayout();
            groupVisualizacao.ResumeLayout(false);
            groupVisualizacao.PerformLayout();
            panelDireito.ResumeLayout(false);
            groupArmaduraLongitudinal.ResumeLayout(false);
            groupArmaduraSuperior.ResumeLayout(false);
            groupArmaduraSuperior.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numQuant1Superior).EndInit();
            ((System.ComponentModel.ISupportInitialize)numQuant2Superior).EndInit();
            groupArmaduraInferior.ResumeLayout(false);
            groupArmaduraInferior.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numQuant1Inferior).EndInit();
            ((System.ComponentModel.ISupportInitialize)numQuant2Inferior).EndInit();
            groupArmaduraLateral.ResumeLayout(false);
            groupArmaduraLateral.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numQuant1Lateral).EndInit();
            ((System.ComponentModel.ISupportInitialize)numQuant2Lateral).EndInit();
            groupEstribos.ResumeLayout(false);
            groupEstribos.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numEspac1Estribo).EndInit();
            groupParametros.ResumeLayout(false);
            groupParametros.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numCobrimento).EndInit();
            ((System.ComponentModel.ISupportInitialize)numMultAmarracao).EndInit();
            ResumeLayout(false);
        }

        #endregion

        // PAINEL ESQUERDO - SELEÇÃO E VISUALIZAÇÃO
        private Panel panelEsquerdo;
        private GroupBox groupSelecaoViga;
        private Label labelSelecionarViga;
        private ComboBox comboVigasDisponiveis;
        private Label labelFiltrarPor;
        private RadioButton radioFiltrarPorDescricao;
        private RadioButton radioFiltrarPorDesignacao;
        private Button buttonAtualizarLista;

        // NEW: Controls to allow using current Revit selection
        private CheckBox checkUsarVigaSelecionada;
        private System.Windows.Forms.Timer selectionTimer;

        // PAINEL DIREITO - CONFIGURAÇÃO DE ARMADURA
        private Panel panelDireito;

        private GroupBox groupArmaduraLongitudinal;
        private GroupBox groupArmaduraSuperior;
        // Novos controles para combinações com interface melhorada
        private CheckBox checkCombinacaoSuperior;
        private Label labelQuant1Superior;
        private NumericUpDown numQuant1Superior;
        private Label labelDiam1Superior;
        private ComboBox comboDiam1Superior;
        private Label labelQuant2Superior;
        private NumericUpDown numQuant2Superior;
        private Label labelDiam2Superior;
        private ComboBox comboDiam2Superior;

        private GroupBox groupArmaduraInferior;
        // Novos controles para combinações com interface melhorada
        private CheckBox checkCombinacaoInferior;
        private Label labelQuant1Inferior;
        private NumericUpDown numQuant1Inferior;
        private Label labelDiam1Inferior;
        private ComboBox comboDiam1Inferior;
        private Label labelQuant2Inferior;
        private NumericUpDown numQuant2Inferior;
        private Label labelDiam2Inferior;
        private ComboBox comboDiam2Inferior;

        private GroupBox groupArmaduraLateral;
        // Novos controles para combinações com interface melhorada
        private CheckBox checkArmaduraLateral;
        private CheckBox checkCombinacaoLateral;
        private Label labelQuant1Lateral;
        private NumericUpDown numQuant1Lateral;
        private Label labelDiam1Lateral;
        private ComboBox comboDiam1Lateral;
        private Label labelQuant2Lateral;
        private NumericUpDown numQuant2Lateral;
        private Label labelDiam2Lateral;
        private ComboBox comboDiam2Lateral;

        private GroupBox groupEstribos;
        // Novos controles para combinações de estribos com interface melhorada
        private CheckBox checkCombinacaoEstribos;
        private Label labelDiam1Estribo;
        private ComboBox comboDiam1Estribo;
        private Label labelEspac1Estribo;
        private NumericUpDown numEspac1Estribo;
        private Label labelDiam2Estribo;
        private ComboBox comboDiam2Estribo;

        private GroupBox groupParametros;
        private Label labelCobrimento;
        private NumericUpDown numCobrimento;
        private Label labelAmarracao;
        private NumericUpDown numMultAmarracao;

        // BOTÕES DE AÇÃO
        private Button buttonExecutar;
        private Button buttonCancelar;
        private ProgressBar progressBar;
        private GroupBox groupVisualizacao;
        private Label labelDimensoes;
        private Label labelAltura;
        private Label lblAlturaValor;
        private Label labelLargura;
        private Label lblLarguraValor;
        private VisualizadorArmaduraViga visualizador;
        private Label labelInfoViga;
    }
}