using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace MacroArmaduraAvancado
{
    /// <summary>
    /// Controle visual para representação e edição interativa de armadura em vigas
    /// </summary>
    public class VisualizadorArmaduraViga : UserControl
    {
        public class InformacaoArmaduraViga
        {
            public double Comprimento { get; set; } = 5000; // mm
            public double Altura { get; set; } = 500; // mm
            public double Largura { get; set; } = 300; // mm
            public double Cobertura { get; set; } = 25; // mm
            public List<ArmVar> VaroesLongitudinais { get; set; } = new List<ArmVar>();
            public List<ArmStirrup> Estribos { get; set; } = new List<ArmStirrup>();
            public double MultiplicadorAmarracao { get; set; } = 50;
            public bool AmarracaoAutomatica { get; set; } = true;
        }

        public class PontoArmadura
        {
            public PointF Posicao { get; set; }
            public double Diametro { get; set; }
            public string Tipo { get; set; } // Superior, Inferior, Lateral
            public Color Cor { get; set; }
            public bool Selecionado { get; set; }
            public int IndiceOriginal { get; set; }
        }

        private InformacaoArmaduraViga informacaoViga;
        private List<PontoArmadura> pontosArmadura;
        private PontoArmadura pontoSelecionado;
        private bool modoEdicao = false;
        private bool mostrarCorteTransversal = true;
        
        // Propriedades de desenho
        private float escalaDesenho = 1.0f;
        private PointF offsetDesenho = new PointF(50, 50);
        private Rectangle areaViga;
        private Rectangle areaEstribos;

        // Cores
        private static readonly Color COR_CONCRETO = Color.LightGray;
        private static readonly Color COR_ARMADURA_SUPERIOR = Color.Red;
        private static readonly Color COR_ARMADURA_INFERIOR = Color.Blue;
        private static readonly Color COR_ARMADURA_LATERAL = Color.Green;
        private static readonly Color COR_ESTRIBO = Color.DarkOrange;
        private static readonly Color COR_SELECAO = Color.Yellow;

        public event EventHandler<ArmaduraEditadaEventArgs> ArmaduraEditada;
        public event EventHandler<PontoArmadura> PontoArmaduraSelecionado;

        public InformacaoArmaduraViga InformacaoViga
        {
            get => informacaoViga;
            set
            {
                informacaoViga = value;
                AtualizarVisualizacao();
            }
        }

        public bool ModoEdicao
        {
            get => modoEdicao;
            set
            {
                modoEdicao = value;
                this.Cursor = modoEdicao ? Cursors.Hand : Cursors.Default;
                Invalidate();
            }
        }

        public bool MostrarCorteTransversal
        {
            get => mostrarCorteTransversal;
            set
            {
                mostrarCorteTransversal = value;
                AtualizarVisualizacao();
            }
        }

        public VisualizadorArmaduraViga()
        {
            InitializeComponent();
            informacaoViga = new InformacaoArmaduraViga();
            pontosArmadura = new List<PontoArmadura>();
            
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | 
                         ControlStyles.UserPaint | 
                         ControlStyles.DoubleBuffer | 
                         ControlStyles.ResizeRedraw, true);
        }

        private void InitializeComponent()
        {
            this.Size = new Size(600, 400);
            this.BackColor = Color.White;
            this.BorderStyle = BorderStyle.FixedSingle;
            
            // Context menu para edição
            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Adicionar Armadura Superior", null, (s, e) => AdicionarArmadura("Superior"));
            contextMenu.Items.Add("Adicionar Armadura Inferior", null, (s, e) => AdicionarArmadura("Inferior"));
            contextMenu.Items.Add("Adicionar Armadura Lateral", null, (s, e) => AdicionarArmadura("Lateral"));
            contextMenu.Items.Add("-");
            contextMenu.Items.Add("Editar Selecionado", null, (s, e) => EditarArmaduraSelecionada());
            contextMenu.Items.Add("Remover Selecionado", null, (s, e) => RemoverArmaduraSelecionada());
            contextMenu.Items.Add("-");
            contextMenu.Items.Add("Alternar Vista", null, (s, e) => AlternarVista());
            
            this.ContextMenuStrip = contextMenu;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            if (informacaoViga == null) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            CalcularEscalaEPosicao();
            
            if (mostrarCorteTransversal)
            {
                DesenharCorteTransversal(g);
            }
            else
            {
                DesenharVistaLongitudinal(g);
            }
            
            DesenharLegenda(g);
            DesenharInformacoes(g);
        }

        private void CalcularEscalaEPosicao()
        {
            float margemX = 80;
            float margemY = 80;
            float areaDisponivelX = this.Width - 2 * margemX;
            float areaDisponivelY = this.Height - 2 * margemY - 100; // Espaço para informações

            if (mostrarCorteTransversal)
            {
                float escalaX = areaDisponivelX / (float)informacaoViga.Largura;
                float escalaY = areaDisponivelY / (float)informacaoViga.Altura;
                escalaDesenho = Math.Min(escalaX, escalaY) * 0.8f;
                
                float larguraDesenho = (float)informacaoViga.Largura * escalaDesenho;
                float alturaDesenho = (float)informacaoViga.Altura * escalaDesenho;
                
                offsetDesenho = new PointF(
                    (this.Width - larguraDesenho) / 2,
                    margemY + (areaDisponivelY - alturaDesenho) / 2
                );
                
                areaViga = new Rectangle(
                    (int)offsetDesenho.X,
                    (int)offsetDesenho.Y,
                    (int)larguraDesenho,
                    (int)alturaDesenho
                );
            }
            else
            {
                float escalaX = areaDisponivelX / (float)informacaoViga.Comprimento;
                float escalaY = areaDisponivelY / (float)informacaoViga.Altura;
                escalaDesenho = Math.Min(escalaX, escalaY) * 0.8f;
                
                float comprimentoDesenho = (float)informacaoViga.Comprimento * escalaDesenho;
                float alturaDesenho = (float)informacaoViga.Altura * escalaDesenho;
                
                offsetDesenho = new PointF(
                    (this.Width - comprimentoDesenho) / 2,
                    margemY + (areaDisponivelY - alturaDesenho) / 2
                );
                
                areaViga = new Rectangle(
                    (int)offsetDesenho.X,
                    (int)offsetDesenho.Y,
                    (int)comprimentoDesenho,
                    (int)alturaDesenho
                );
            }
        }

        private void DesenharCorteTransversal(Graphics g)
        {
            // Desenhar viga (concreto)
            using (var brush = new SolidBrush(COR_CONCRETO))
            using (var pen = new Pen(Color.Black, 2))
            {
                g.FillRectangle(brush, areaViga);
                g.DrawRectangle(pen, areaViga);
            }

            // Desenhar cobertura (linha tracejada)
            float cobertura = (float)informacaoViga.Cobertura * escalaDesenho;
            using (var penTracejado = new Pen(Color.Gray, 1))
            {
                penTracejado.DashStyle = DashStyle.Dash;
                Rectangle areaCoberta = new Rectangle(
                    (int)(areaViga.X + cobertura),
                    (int)(areaViga.Y + cobertura),
                    (int)(areaViga.Width - 2 * cobertura),
                    (int)(areaViga.Height - 2 * cobertura)
                );
                g.DrawRectangle(penTracejado, areaCoberta);
            }

            // Atualizar posições dos pontos de armadura
            AtualizarPontosArmaduraCorteTransversal();

            // Desenhar armadura longitudinal
            foreach (var ponto in pontosArmadura)
            {
                DesenharPontoArmadura(g, ponto);
            }

            // Desenhar estribos
            DesenharEstribosCorteTransversal(g);

            // Título
            using (var font = new Font("Arial", 12, FontStyle.Bold))
            using (var brush = new SolidBrush(Color.Black))
            {
                string titulo = "Corte Transversal da Viga";
                SizeF tamanhoTexto = g.MeasureString(titulo, font);
                g.DrawString(titulo, font, brush, 
                    (this.Width - tamanhoTexto.Width) / 2, 10);
            }
        }

        private void DesenharVistaLongitudinal(Graphics g)
        {
            // Desenhar viga (vista lateral)
            using (var brush = new SolidBrush(COR_CONCRETO))
            using (var pen = new Pen(Color.Black, 2))
            {
                g.FillRectangle(brush, areaViga);
                g.DrawRectangle(pen, areaViga);
            }

            // Desenhar linha de centro
            using (var pen = new Pen(Color.Gray, 1))
            {
                pen.DashStyle = DashStyle.Dot;
                float centroY = areaViga.Y + areaViga.Height / 2;
                g.DrawLine(pen, areaViga.X, centroY, areaViga.X + areaViga.Width, centroY);
            }

            // Desenhar armadura longitudinal simplificada
            DesenharArmaduraLongitudinalVista(g);

            // Desenhar estribos ao longo do comprimento
            DesenharEstribosVistaLongitudinal(g);

            // Título
            using (var font = new Font("Arial", 12, FontStyle.Bold))
            using (var brush = new SolidBrush(Color.Black))
            {
                string titulo = "Vista Longitudinal da Viga";
                SizeF tamanhoTexto = g.MeasureString(titulo, font);
                g.DrawString(titulo, font, brush, 
                    (this.Width - tamanhoTexto.Width) / 2, 10);
            }
        }

        private void AtualizarPontosArmaduraCorteTransversal()
        {
            pontosArmadura.Clear();
            float cobertura = (float)informacaoViga.Cobertura * escalaDesenho;

            int indice = 0;
            foreach (var varao in informacaoViga.VaroesLongitudinais)
            {
                Color cor = ObterCorTipoArmadura(varao.TipoArmadura);
                
                for (int i = 0; i < varao.Quantidade; i++)
                {
                    PointF posicao = CalcularPosicaoVarao(varao.TipoArmadura, i, varao.Quantidade, cobertura);
                    
                    pontosArmadura.Add(new PontoArmadura
                    {
                        Posicao = posicao,
                        Diametro = varao.Diametro,
                        Tipo = varao.TipoArmadura,
                        Cor = cor,
                        IndiceOriginal = indice
                    });
                }
                indice++;
            }
        }

        private PointF CalcularPosicaoVarao(string tipo, int indice, int total, float cobertura)
        {
            float x, y;

            switch (tipo.ToLower())
            {
                case "superior":
                    if (total == 1)
                    {
                        x = areaViga.X + areaViga.Width / 2;
                    }
                    else
                    {
                        float espacamento = (areaViga.Width - 2 * cobertura) / (total - 1);
                        x = areaViga.X + cobertura + indice * espacamento;
                    }
                    y = areaViga.Y + cobertura;
                    break;

                case "inferior":
                    if (total == 1)
                    {
                        x = areaViga.X + areaViga.Width / 2;
                    }
                    else
                    {
                        float espacamento = (areaViga.Width - 2 * cobertura) / (total - 1);
                        x = areaViga.X + cobertura + indice * espacamento;
                    }
                    y = areaViga.Y + areaViga.Height - cobertura;
                    break;

                case "lateral":
                    if (indice < total / 2)
                    {
                        x = areaViga.X + cobertura;
                        float alturaUtil = areaViga.Height - 2 * cobertura;
                        float espacamento = alturaUtil / (total / 2 + 1);
                        y = areaViga.Y + cobertura + (indice + 1) * espacamento;
                    }
                    else
                    {
                        x = areaViga.X + areaViga.Width - cobertura;
                        float alturaUtil = areaViga.Height - 2 * cobertura;
                        float espacamento = alturaUtil / (total / 2 + 1);
                        y = areaViga.Y + cobertura + (indice - total / 2 + 1) * espacamento;
                    }
                    break;

                default:
                    x = areaViga.X + areaViga.Width / 2;
                    y = areaViga.Y + areaViga.Height / 2;
                    break;
            }

            return new PointF(x, y);
        }

        private void DesenharPontoArmadura(Graphics g, PontoArmadura ponto)
        {
            float raio = Math.Max(3, (float)(ponto.Diametro * escalaDesenho * 0.1));
            
            Color cor = ponto.Selecionado ? COR_SELECAO : ponto.Cor;
            
            using (var brush = new SolidBrush(cor))
            using (var pen = new Pen(Color.Black, ponto.Selecionado ? 2 : 1))
            {
                RectangleF rect = new RectangleF(
                    ponto.Posicao.X - raio,
                    ponto.Posicao.Y - raio,
                    2 * raio,
                    2 * raio
                );
                
                g.FillEllipse(brush, rect);
                g.DrawEllipse(pen, rect);
                
                // Desenhar diâmetro como texto
                if (raio > 8)
                {
                    using (var font = new Font("Arial", 6, FontStyle.Bold))
                    using (var brushTexto = new SolidBrush(Color.White))
                    {
                        string texto = ponto.Diametro.ToString("F0");
                        SizeF tamanhoTexto = g.MeasureString(texto, font);
                        g.DrawString(texto, font, brushTexto,
                            ponto.Posicao.X - tamanhoTexto.Width / 2,
                            ponto.Posicao.Y - tamanhoTexto.Height / 2);
                    }
                }
            }
        }

        private void DesenharEstribosCorteTransversal(Graphics g)
        {
            if (informacaoViga.Estribos.Count == 0) return;

            var estribo = informacaoViga.Estribos.First();
            float cobertura = (float)informacaoViga.Cobertura * escalaDesenho;
            
            using (var pen = new Pen(COR_ESTRIBO, 2))
            {
                Rectangle estribosRect = new Rectangle(
                    (int)(areaViga.X + cobertura),
                    (int)(areaViga.Y + cobertura),
                    (int)(areaViga.Width - 2 * cobertura),
                    (int)(areaViga.Height - 2 * cobertura)
                );
                
                g.DrawRectangle(pen, estribosRect);
                
                // Desenhar gancho do estribo
                float tamanhoGancho = 10;
                g.DrawLine(pen, 
                    estribosRect.X, estribosRect.Y,
                    estribosRect.X + tamanhoGancho, estribosRect.Y - tamanhoGancho);
            }
        }

        private void DesenharArmaduraLongitudinalVista(Graphics g)
        {
            float cobertura = (float)informacaoViga.Cobertura * escalaDesenho;
            
            // Armadura superior
            var armaduraSuperior = informacaoViga.VaroesLongitudinais.Where(v => v.TipoArmadura.ToLower() == "superior");
            foreach (var varao in armaduraSuperior)
            {
                using (var pen = new Pen(COR_ARMADURA_SUPERIOR, 3))
                {
                    float y = areaViga.Y + cobertura;
                    g.DrawLine(pen, areaViga.X, y, areaViga.X + areaViga.Width, y);
                    
                    // Amarração
                    if (informacaoViga.AmarracaoAutomatica)
                    {
                        float comprimentoAmarracao = (float)(informacaoViga.MultiplicadorAmarracao * varao.Diametro * escalaDesenho * 0.01);
                        // Gancho à esquerda
                        g.DrawLine(pen, areaViga.X, y, areaViga.X, y + comprimentoAmarracao);
                        // Gancho à direita
                        g.DrawLine(pen, areaViga.X + areaViga.Width, y, areaViga.X + areaViga.Width, y + comprimentoAmarracao);
                    }
                }
            }
            
            // Armadura inferior
            var armaduraInferior = informacaoViga.VaroesLongitudinais.Where(v => v.TipoArmadura.ToLower() == "inferior");
            foreach (var varao in armaduraInferior)
            {
                using (var pen = new Pen(COR_ARMADURA_INFERIOR, 3))
                {
                    float y = areaViga.Y + areaViga.Height - cobertura;
                    g.DrawLine(pen, areaViga.X, y, areaViga.X + areaViga.Width, y);
                }
            }
        }

        private void DesenharEstribosVistaLongitudinal(Graphics g)
        {
            if (informacaoViga.Estribos.Count == 0) return;

            var estribo = informacaoViga.Estribos.First();
            float espacamento = (float)estribo.Espacamento * escalaDesenho * 0.001f; // mm para escala
            
            using (var pen = new Pen(COR_ESTRIBO, 1))
            {
                int numeroEstribos = (int)(areaViga.Width / espacamento);
                for (int i = 1; i < numeroEstribos; i++)
                {
                    float x = areaViga.X + i * espacamento;
                    g.DrawLine(pen, x, areaViga.Y, x, areaViga.Y + areaViga.Height);
                }
            }
        }

        private void DesenharLegenda(Graphics g)
        {
            float x = 10;
            float y = this.Height - 80;
            
            using (var font = new Font("Arial", 8))
            using (var brush = new SolidBrush(Color.Black))
            {
                g.DrawString("Legenda:", font, brush, x, y);
                y += 15;
                
                DesenharItemLegenda(g, font, x, ref y, COR_ARMADURA_SUPERIOR, "Superior");
                DesenharItemLegenda(g, font, x, ref y, COR_ARMADURA_INFERIOR, "Inferior");
                DesenharItemLegenda(g, font, x + 100, ref y, COR_ARMADURA_LATERAL, "Lateral");
                y -= 15;
                DesenharItemLegenda(g, font, x + 200, ref y, COR_ESTRIBO, "Estribos");
            }
        }

        private void DesenharItemLegenda(Graphics g, Font font, float x, ref float y, Color cor, string texto)
        {
            using (var brush = new SolidBrush(cor))
            using (var brushTexto = new SolidBrush(Color.Black))
            {
                g.FillRectangle(brush, x, y, 10, 10);
                g.DrawString(texto, font, brushTexto, x + 15, y - 2);
                y += 15;
            }
        }

        private void DesenharInformacoes(Graphics g)
        {
            float x = this.Width - 200;
            float y = this.Height - 100;
            
            using (var font = new Font("Arial", 8))
            using (var brush = new SolidBrush(Color.Black))
            {
                g.DrawString($"Dimensões:", font, brush, x, y);
                y += 12;
                g.DrawString($"C: {informacaoViga.Comprimento:F0}mm", font, brush, x, y);
                y += 12;
                g.DrawString($"H: {informacaoViga.Altura:F0}mm", font, brush, x, y);
                y += 12;
                g.DrawString($"L: {informacaoViga.Largura:F0}mm", font, brush, x, y);
                y += 12;
                g.DrawString($"Cob: {informacaoViga.Cobertura:F0}mm", font, brush, x, y);
                y += 15;
                
                g.DrawString($"Armadura:", font, brush, x, y);
                y += 12;
                g.DrawString($"Total: {informacaoViga.VaroesLongitudinais.Sum(v => v.Quantidade)} varões", font, brush, x, y);
                y += 12;
                g.DrawString($"Estribos: {informacaoViga.Estribos.Count} tipos", font, brush, x, y);
            }
        }

        private Color ObterCorTipoArmadura(string tipo)
        {
            switch (tipo.ToLower())
            {
                case "superior": return COR_ARMADURA_SUPERIOR;
                case "inferior": return COR_ARMADURA_INFERIOR;
                case "lateral": return COR_ARMADURA_LATERAL;
                default: return Color.Gray;
            }
        }

        public void AtualizarVisualizacao()
        {
            Invalidate();
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            
            if (!modoEdicao) return;
            
            pontoSelecionado = null;
            
            foreach (var ponto in pontosArmadura)
            {
                ponto.Selecionado = false;
                
                float distancia = (float)Math.Sqrt(
                    Math.Pow(e.X - ponto.Posicao.X, 2) + 
                    Math.Pow(e.Y - ponto.Posicao.Y, 2));
                
                if (distancia <= 15) // Área de clique
                {
                    ponto.Selecionado = true;
                    pontoSelecionado = ponto;
                    PontoArmaduraSelecionado?.Invoke(this, ponto);
                    break;
                }
            }
            
            Invalidate();
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            
            if (pontoSelecionado != null)
            {
                EditarArmaduraSelecionada();
            }
        }

        private void AdicionarArmadura(string tipo)
        {
            using (var form = new ConfiguracaoVarao(false))
            {
                form.Text = $"Nova Armadura {tipo}";
                if (form.ShowDialog() == DialogResult.OK)
                {
                    var novoVarao = new ArmVar(form.QuantidadeValue, form.DiametroValue)
                    {
                        TipoArmadura = tipo
                    };
                    
                    informacaoViga.VaroesLongitudinais.Add(novoVarao);
                    AtualizarVisualizacao();
                    
                    ArmaduraEditada?.Invoke(this, new ArmaduraEditadaEventArgs 
                    { 
                        TipoOperacao = "Adicionar", 
                        VaraoModificado = novoVarao 
                    });
                }
            }
        }

        private void EditarArmaduraSelecionada()
        {
            if (pontoSelecionado == null) return;
            
            var varaoOriginal = informacaoViga.VaroesLongitudinais[pontoSelecionado.IndiceOriginal];
            
            using (var form = new ConfiguracaoVarao(false))
            {
                form.Text = "Editar Armadura";
                // Pré-preencher com valores atuais seria necessário modificar ConfiguracaoVarao
                
                if (form.ShowDialog() == DialogResult.OK)
                {
                    varaoOriginal.Quantidade = form.QuantidadeValue;
                    varaoOriginal.Diametro = form.DiametroValue;
                    varaoOriginal.TipoArmadura = form.PosicaoValue;
                    
                    AtualizarVisualizacao();
                    
                    ArmaduraEditada?.Invoke(this, new ArmaduraEditadaEventArgs 
                    { 
                        TipoOperacao = "Editar", 
                        VaraoModificado = varaoOriginal 
                    });
                }
            }
        }

        private void RemoverArmaduraSelecionada()
        {
            if (pontoSelecionado == null) return;
            
            var varaoParaRemover = informacaoViga.VaroesLongitudinais[pontoSelecionado.IndiceOriginal];
            
            if (MessageBox.Show($"Remover armadura {varaoParaRemover}?", "Confirmação", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                informacaoViga.VaroesLongitudinais.RemoveAt(pontoSelecionado.IndiceOriginal);
                pontoSelecionado = null;
                AtualizarVisualizacao();
                
                ArmaduraEditada?.Invoke(this, new ArmaduraEditadaEventArgs 
                { 
                    TipoOperacao = "Remover", 
                    VaraoModificado = varaoParaRemover 
                });
            }
        }

        private void AlternarVista()
        {
            MostrarCorteTransversal = !MostrarCorteTransversal;
        }
    }

    public class ArmaduraEditadaEventArgs : EventArgs
    {
        public string TipoOperacao { get; set; } // Adicionar, Editar, Remover
        public ArmVar VaraoModificado { get; set; }
    }
}