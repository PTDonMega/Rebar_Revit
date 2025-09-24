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
            groupVisualizacao = new GroupBox();
            visualizador = new VisualizadorArmaduraViga();
            labelDimensoes = new Label();
            labelAltura = new Label();
            lblAlturaValor = new Label();
            labelLargura = new Label();
            lblLarguraValor = new Label();
            labelInfoViga = new Label();
            panelDireito = new Panel();
            groupArmaduraLongitudinal = new GroupBox();
            groupArmaduraSuperior = new GroupBox();
            labelQuantSuperior = new Label();
            numQuantSuperior = new NumericUpDown();
            labelDiamSuperior = new Label();
            comboDiamSuperior = new ComboBox();
            groupArmaduraInferior = new GroupBox();
            labelQuantInferior = new Label();
            numQuantInferior = new NumericUpDown();
            labelDiamInferior = new Label();
            comboDiamInferior = new ComboBox();
            groupArmaduraLateral = new GroupBox();
            checkArmaduraLateral = new CheckBox();
            labelQuantLateral = new Label();
            numQuantLateral = new NumericUpDown();
            labelDiamLateral = new Label();
            comboDiamLateral = new ComboBox();
            groupEstribos = new GroupBox();
            labelDiamEstribo = new Label();
            comboDiamEstribo = new ComboBox();
            labelEspacamentoEstribo = new Label();
            numEspacamentoEstribo = new NumericUpDown();
            checkEspacamentoVariavel = new CheckBox();
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
            ((System.ComponentModel.ISupportInitialize)numQuantSuperior).BeginInit();
            groupArmaduraInferior.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numQuantInferior).BeginInit();
            groupArmaduraLateral.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numQuantLateral).BeginInit();
            groupEstribos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numEspacamentoEstribo).BeginInit();
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
            panelEsquerdo.Size = new Size(500, 759);
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
            groupSelecaoViga.Dock = DockStyle.Top;
            groupSelecaoViga.Location = new Point(10, 12);
            groupSelecaoViga.Margin = new Padding(3, 4, 3, 4);
            groupSelecaoViga.Name = "groupSelecaoViga";
            groupSelecaoViga.Padding = new Padding(3, 4, 3, 4);
            groupSelecaoViga.Size = new Size(480, 143);
            groupSelecaoViga.TabIndex = 0;
            groupSelecaoViga.TabStop = false;
            groupSelecaoViga.Text = "Seleção de Viga";
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
            labelFiltrarPor.Location = new Point(15, 100);
            labelFiltrarPor.Name = "labelFiltrarPor";
            labelFiltrarPor.Size = new Size(77, 20);
            labelFiltrarPor.TabIndex = 2;
            labelFiltrarPor.Text = "Filtrar por:";
            // 
            // radioFiltrarPorDescricao
            // 
            radioFiltrarPorDescricao.AutoSize = true;
            radioFiltrarPorDescricao.Checked = true;
            radioFiltrarPorDescricao.Location = new Point(100, 98);
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
            radioFiltrarPorDesignacao.Location = new Point(170, 98);
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
            groupVisualizacao.Size = new Size(480, 735);
            groupVisualizacao.TabIndex = 1;
            groupVisualizacao.TabStop = false;
            groupVisualizacao.Text = "Pré-visualização (secção)";
            // 
            // visualizador
            // 
            visualizador.BackColor = Color.White;
            visualizador.BorderStyle = BorderStyle.FixedSingle;
            visualizador.Location = new Point(15, 230);
            visualizador.Margin = new Padding(3, 4, 3, 4);
            visualizador.ModoEdicao = false;
            visualizador.MostrarCorteTransversal = true;
            visualizador.Name = "visualizador";
            visualizador.Size = new Size(450, 478);
            visualizador.TabIndex = 8;
            visualizador.Load += visualizador_Load;
            // 
            // labelDimensoes
            // 
            labelDimensoes.AutoSize = true;
            labelDimensoes.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelDimensoes.Location = new Point(18, 190);
            labelDimensoes.Name = "labelDimensoes";
            labelDimensoes.Size = new Size(143, 18);
            labelDimensoes.TabIndex = 0;
            labelDimensoes.Text = "Dimensões (mm):";
            // 
            // labelAltura
            // 
            labelAltura.AutoSize = true;
            labelAltura.Location = new Point(350, 190);
            labelAltura.Name = "labelAltura";
            labelAltura.Size = new Size(52, 20);
            labelAltura.TabIndex = 3;
            labelAltura.Text = "Altura:";
            // 
            // lblAlturaValor
            // 
            lblAlturaValor.AutoSize = true;
            lblAlturaValor.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblAlturaValor.Location = new Point(408, 190);
            lblAlturaValor.Name = "lblAlturaValor";
            lblAlturaValor.Size = new Size(44, 18);
            lblAlturaValor.TabIndex = 4;
            lblAlturaValor.Text = "0000";
            // 
            // labelLargura
            // 
            labelLargura.AutoSize = true;
            labelLargura.Location = new Point(185, 190);
            labelLargura.Name = "labelLargura";
            labelLargura.Size = new Size(62, 20);
            labelLargura.TabIndex = 5;
            labelLargura.Text = "Largura:";
            // 
            // lblLarguraValor
            // 
            lblLarguraValor.AutoSize = true;
            lblLarguraValor.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblLarguraValor.Location = new Point(253, 190);
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
            labelInfoViga.Size = new Size(450, 437);
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
            panelDireito.Size = new Size(400, 759);
            panelDireito.TabIndex = 1;
            // 
            // groupArmaduraLongitudinal
            // 
            groupArmaduraLongitudinal.Controls.Add(groupArmaduraSuperior);
            groupArmaduraLongitudinal.Controls.Add(groupArmaduraInferior);
            groupArmaduraLongitudinal.Controls.Add(groupArmaduraLateral);
            groupArmaduraLongitudinal.Dock = DockStyle.Top;
            groupArmaduraLongitudinal.Location = new Point(10, 262);
            groupArmaduraLongitudinal.Margin = new Padding(3, 4, 3, 4);
            groupArmaduraLongitudinal.Name = "groupArmaduraLongitudinal";
            groupArmaduraLongitudinal.Padding = new Padding(3, 4, 3, 4);
            groupArmaduraLongitudinal.Size = new Size(380, 350);
            groupArmaduraLongitudinal.TabIndex = 0;
            groupArmaduraLongitudinal.TabStop = false;
            groupArmaduraLongitudinal.Text = "Armadura Longitudinal";
            // 
            // groupArmaduraSuperior
            // 
            groupArmaduraSuperior.Controls.Add(labelQuantSuperior);
            groupArmaduraSuperior.Controls.Add(numQuantSuperior);
            groupArmaduraSuperior.Controls.Add(labelDiamSuperior);
            groupArmaduraSuperior.Controls.Add(comboDiamSuperior);
            groupArmaduraSuperior.Location = new Point(15, 31);
            groupArmaduraSuperior.Margin = new Padding(3, 4, 3, 4);
            groupArmaduraSuperior.Name = "groupArmaduraSuperior";
            groupArmaduraSuperior.Padding = new Padding(3, 4, 3, 4);
            groupArmaduraSuperior.Size = new Size(350, 94);
            groupArmaduraSuperior.TabIndex = 0;
            groupArmaduraSuperior.TabStop = false;
            groupArmaduraSuperior.Text = "Armadura Superior";
            // 
            // labelQuantSuperior
            // 
            labelQuantSuperior.AutoSize = true;
            labelQuantSuperior.Location = new Point(15, 31);
            labelQuantSuperior.Name = "labelQuantSuperior";
            labelQuantSuperior.Size = new Size(90, 20);
            labelQuantSuperior.TabIndex = 0;
            labelQuantSuperior.Text = "Quantidade:";
            // 
            // numQuantSuperior
            // 
            numQuantSuperior.Location = new Point(15, 56);
            numQuantSuperior.Margin = new Padding(3, 4, 3, 4);
            numQuantSuperior.Maximum = new decimal(new int[] { 20, 0, 0, 0 });
            numQuantSuperior.Minimum = new decimal(new int[] { 2, 0, 0, 0 });
            numQuantSuperior.Name = "numQuantSuperior";
            numQuantSuperior.Size = new Size(60, 27);
            numQuantSuperior.TabIndex = 1;
            numQuantSuperior.Value = new decimal(new int[] { 2, 0, 0, 0 });
            // 
            // labelDiamSuperior
            // 
            labelDiamSuperior.AutoSize = true;
            labelDiamSuperior.Location = new Point(100, 31);
            labelDiamSuperior.Name = "labelDiamSuperior";
            labelDiamSuperior.Size = new Size(115, 20);
            labelDiamSuperior.TabIndex = 2;
            labelDiamSuperior.Text = "Diâmetro (mm):";
            // 
            // comboDiamSuperior
            // 
            comboDiamSuperior.DropDownStyle = ComboBoxStyle.DropDownList;
            comboDiamSuperior.FormattingEnabled = true;
            comboDiamSuperior.Location = new Point(100, 56);
            comboDiamSuperior.Margin = new Padding(3, 4, 3, 4);
            comboDiamSuperior.Name = "comboDiamSuperior";
            comboDiamSuperior.Size = new Size(70, 28);
            comboDiamSuperior.TabIndex = 3;
            // 
            // groupArmaduraInferior
            // 
            groupArmaduraInferior.Controls.Add(labelQuantInferior);
            groupArmaduraInferior.Controls.Add(numQuantInferior);
            groupArmaduraInferior.Controls.Add(labelDiamInferior);
            groupArmaduraInferior.Controls.Add(comboDiamInferior);
            groupArmaduraInferior.Location = new Point(15, 138);
            groupArmaduraInferior.Margin = new Padding(3, 4, 3, 4);
            groupArmaduraInferior.Name = "groupArmaduraInferior";
            groupArmaduraInferior.Padding = new Padding(3, 4, 3, 4);
            groupArmaduraInferior.Size = new Size(350, 94);
            groupArmaduraInferior.TabIndex = 1;
            groupArmaduraInferior.TabStop = false;
            groupArmaduraInferior.Text = "Armadura Inferior";
            // 
            // labelQuantInferior
            // 
            labelQuantInferior.AutoSize = true;
            labelQuantInferior.Location = new Point(15, 31);
            labelQuantInferior.Name = "labelQuantInferior";
            labelQuantInferior.Size = new Size(90, 20);
            labelQuantInferior.TabIndex = 0;
            labelQuantInferior.Text = "Quantidade:";
            // 
            // numQuantInferior
            // 
            numQuantInferior.Location = new Point(15, 56);
            numQuantInferior.Margin = new Padding(3, 4, 3, 4);
            numQuantInferior.Maximum = new decimal(new int[] { 20, 0, 0, 0 });
            numQuantInferior.Minimum = new decimal(new int[] { 2, 0, 0, 0 });
            numQuantInferior.Name = "numQuantInferior";
            numQuantInferior.Size = new Size(60, 27);
            numQuantInferior.TabIndex = 1;
            numQuantInferior.Value = new decimal(new int[] { 3, 0, 0, 0 });
            // 
            // labelDiamInferior
            // 
            labelDiamInferior.AutoSize = true;
            labelDiamInferior.Location = new Point(100, 31);
            labelDiamInferior.Name = "labelDiamInferior";
            labelDiamInferior.Size = new Size(115, 20);
            labelDiamInferior.TabIndex = 2;
            labelDiamInferior.Text = "Diâmetro (mm):";
            // 
            // comboDiamInferior
            // 
            comboDiamInferior.DropDownStyle = ComboBoxStyle.DropDownList;
            comboDiamInferior.FormattingEnabled = true;
            comboDiamInferior.Location = new Point(100, 56);
            comboDiamInferior.Margin = new Padding(3, 4, 3, 4);
            comboDiamInferior.Name = "comboDiamInferior";
            comboDiamInferior.Size = new Size(70, 28);
            comboDiamInferior.TabIndex = 3;
            // 
            // groupArmaduraLateral
            // 
            groupArmaduraLateral.Controls.Add(checkArmaduraLateral);
            groupArmaduraLateral.Controls.Add(labelQuantLateral);
            groupArmaduraLateral.Controls.Add(numQuantLateral);
            groupArmaduraLateral.Controls.Add(labelDiamLateral);
            groupArmaduraLateral.Controls.Add(comboDiamLateral);
            groupArmaduraLateral.Location = new Point(15, 244);
            groupArmaduraLateral.Margin = new Padding(3, 4, 3, 4);
            groupArmaduraLateral.Name = "groupArmaduraLateral";
            groupArmaduraLateral.Padding = new Padding(3, 4, 3, 4);
            groupArmaduraLateral.Size = new Size(350, 94);
            groupArmaduraLateral.TabIndex = 2;
            groupArmaduraLateral.TabStop = false;
            groupArmaduraLateral.Text = "Armadura Lateral (Opcional)";
            // 
            // checkArmaduraLateral
            // 
            checkArmaduraLateral.AutoSize = true;
            checkArmaduraLateral.Location = new Point(280, 31);
            checkArmaduraLateral.Margin = new Padding(3, 4, 3, 4);
            checkArmaduraLateral.Name = "checkArmaduraLateral";
            checkArmaduraLateral.Size = new Size(70, 24);
            checkArmaduraLateral.TabIndex = 0;
            checkArmaduraLateral.Text = "Ativar";
            checkArmaduraLateral.UseVisualStyleBackColor = true;
            // 
            // labelQuantLateral
            // 
            labelQuantLateral.AutoSize = true;
            labelQuantLateral.Enabled = false;
            labelQuantLateral.Location = new Point(15, 31);
            labelQuantLateral.Name = "labelQuantLateral";
            labelQuantLateral.Size = new Size(90, 20);
            labelQuantLateral.TabIndex = 1;
            labelQuantLateral.Text = "Quantidade:";
            // 
            // numQuantLateral
            // 
            numQuantLateral.Enabled = false;
            numQuantLateral.Location = new Point(15, 56);
            numQuantLateral.Margin = new Padding(3, 4, 3, 4);
            numQuantLateral.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            numQuantLateral.Name = "numQuantLateral";
            numQuantLateral.Size = new Size(60, 27);
            numQuantLateral.TabIndex = 2;
            numQuantLateral.Value = new decimal(new int[] { 2, 0, 0, 0 });
            // 
            // labelDiamLateral
            // 
            labelDiamLateral.AutoSize = true;
            labelDiamLateral.Enabled = false;
            labelDiamLateral.Location = new Point(100, 31);
            labelDiamLateral.Name = "labelDiamLateral";
            labelDiamLateral.Size = new Size(115, 20);
            labelDiamLateral.TabIndex = 3;
            labelDiamLateral.Text = "Diâmetro (mm):";
            // 
            // comboDiamLateral
            // 
            comboDiamLateral.DropDownStyle = ComboBoxStyle.DropDownList;
            comboDiamLateral.Enabled = false;
            comboDiamLateral.FormattingEnabled = true;
            comboDiamLateral.Location = new Point(100, 56);
            comboDiamLateral.Margin = new Padding(3, 4, 3, 4);
            comboDiamLateral.Name = "comboDiamLateral";
            comboDiamLateral.Size = new Size(70, 28);
            comboDiamLateral.TabIndex = 4;
            // 
            // groupEstribos
            // 
            groupEstribos.Controls.Add(labelDiamEstribo);
            groupEstribos.Controls.Add(comboDiamEstribo);
            groupEstribos.Controls.Add(labelEspacamentoEstribo);
            groupEstribos.Controls.Add(numEspacamentoEstribo);
            groupEstribos.Controls.Add(checkEspacamentoVariavel);
            groupEstribos.Dock = DockStyle.Top;
            groupEstribos.Location = new Point(10, 137);
            groupEstribos.Margin = new Padding(3, 4, 3, 4);
            groupEstribos.Name = "groupEstribos";
            groupEstribos.Padding = new Padding(3, 4, 3, 4);
            groupEstribos.Size = new Size(380, 125);
            groupEstribos.TabIndex = 1;
            groupEstribos.TabStop = false;
            groupEstribos.Text = "Estribos";
            // 
            // labelDiamEstribo
            // 
            labelDiamEstribo.AutoSize = true;
            labelDiamEstribo.Location = new Point(15, 31);
            labelDiamEstribo.Name = "labelDiamEstribo";
            labelDiamEstribo.Size = new Size(115, 20);
            labelDiamEstribo.TabIndex = 0;
            labelDiamEstribo.Text = "Diâmetro (mm):";
            // 
            // comboDiamEstribo
            // 
            comboDiamEstribo.DropDownStyle = ComboBoxStyle.DropDownList;
            comboDiamEstribo.FormattingEnabled = true;
            comboDiamEstribo.Location = new Point(15, 56);
            comboDiamEstribo.Margin = new Padding(3, 4, 3, 4);
            comboDiamEstribo.Name = "comboDiamEstribo";
            comboDiamEstribo.Size = new Size(70, 28);
            comboDiamEstribo.TabIndex = 1;
            // 
            // labelEspacamentoEstribo
            // 
            labelEspacamentoEstribo.AutoSize = true;
            labelEspacamentoEstribo.Location = new Point(110, 31);
            labelEspacamentoEstribo.Name = "labelEspacamentoEstribo";
            labelEspacamentoEstribo.Size = new Size(141, 20);
            labelEspacamentoEstribo.TabIndex = 2;
            labelEspacamentoEstribo.Text = "Espaçamento (mm):";
            // 
            // numEspacamentoEstribo
            // 
            numEspacamentoEstribo.Increment = new decimal(new int[] { 25, 0, 0, 0 });
            numEspacamentoEstribo.Location = new Point(110, 56);
            numEspacamentoEstribo.Margin = new Padding(3, 4, 3, 4);
            numEspacamentoEstribo.Maximum = new decimal(new int[] { 500, 0, 0, 0 });
            numEspacamentoEstribo.Minimum = new decimal(new int[] { 50, 0, 0, 0 });
            numEspacamentoEstribo.Name = "numEspacamentoEstribo";
            numEspacamentoEstribo.Size = new Size(80, 27);
            numEspacamentoEstribo.TabIndex = 3;
            numEspacamentoEstribo.Value = new decimal(new int[] { 150, 0, 0, 0 });
            // 
            // checkEspacamentoVariavel
            // 
            checkEspacamentoVariavel.AutoSize = true;
            checkEspacamentoVariavel.Location = new Point(15, 94);
            checkEspacamentoVariavel.Margin = new Padding(3, 4, 3, 4);
            checkEspacamentoVariavel.Name = "checkEspacamentoVariavel";
            checkEspacamentoVariavel.Size = new Size(308, 24);
            checkEspacamentoVariavel.TabIndex = 4;
            checkEspacamentoVariavel.Text = "Espaçamento variável (menor nos apoios)";
            checkEspacamentoVariavel.UseVisualStyleBackColor = true;
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
            groupParametros.Size = new Size(380, 125);
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
            numMultAmarracao.Location = new Point(163, 56);
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
            buttonExecutar.Location = new Point(95, 638);
            buttonExecutar.Margin = new Padding(3, 4, 3, 4);
            buttonExecutar.Name = "buttonExecutar";
            buttonExecutar.Size = new Size(160, 44);
            buttonExecutar.TabIndex = 4;
            buttonExecutar.Text = "Criar Armadura";
            buttonExecutar.UseVisualStyleBackColor = false;
            // 
            // buttonCancelar
            // 
            buttonCancelar.Location = new Point(265, 638);
            buttonCancelar.Margin = new Padding(3, 4, 3, 4);
            buttonCancelar.Name = "buttonCancelar";
            buttonCancelar.Size = new Size(100, 44);
            buttonCancelar.TabIndex = 5;
            buttonCancelar.Text = "Cancelar";
            buttonCancelar.UseVisualStyleBackColor = true;
            // 
            // progressBar
            // 
            progressBar.Location = new Point(15, 700);
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
            ClientSize = new Size(900, 759);
            Controls.Add(panelDireito);
            Controls.Add(panelEsquerdo);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(3, 4, 3, 4);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormularioPrincipal";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Armaduras em Vigas - v0.1_Alpha";
            panelEsquerdo.ResumeLayout(false);
            groupSelecaoViga.ResumeLayout(false);
            groupSelecaoViga.PerformLayout();
            groupVisualizacao.ResumeLayout(false);
            groupVisualizacao.PerformLayout();
            panelDireito.ResumeLayout(false);
            groupArmaduraLongitudinal.ResumeLayout(false);
            groupArmaduraSuperior.ResumeLayout(false);
            groupArmaduraSuperior.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numQuantSuperior).EndInit();
            groupArmaduraInferior.ResumeLayout(false);
            groupArmaduraInferior.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numQuantInferior).EndInit();
            groupArmaduraLateral.ResumeLayout(false);
            groupArmaduraLateral.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numQuantLateral).EndInit();
            groupEstribos.ResumeLayout(false);
            groupEstribos.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numEspacamentoEstribo).EndInit();
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

        // PAINEL DIREITO - CONFIGURAÇÃO DE ARMADURA
        private Panel panelDireito;

        private GroupBox groupArmaduraLongitudinal;
        private GroupBox groupArmaduraSuperior;
        private Label labelQuantSuperior;
        private NumericUpDown numQuantSuperior;
        private Label labelDiamSuperior;
        private ComboBox comboDiamSuperior;

        private GroupBox groupArmaduraInferior;
        private Label labelQuantInferior;
        private NumericUpDown numQuantInferior;
        private Label labelDiamInferior;
        private ComboBox comboDiamInferior;

        private GroupBox groupArmaduraLateral;
        private CheckBox checkArmaduraLateral;
        private Label labelQuantLateral;
        private NumericUpDown numQuantLateral;
        private Label labelDiamLateral;
        private ComboBox comboDiamLateral;

        private GroupBox groupEstribos;
        private Label labelDiamEstribo;
        private ComboBox comboDiamEstribo;
        private Label labelEspacamentoEstribo;
        private NumericUpDown numEspacamentoEstribo;
        private CheckBox checkEspacamentoVariavel;

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