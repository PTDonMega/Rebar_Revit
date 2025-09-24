using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace Rebar_Revit
{
    /// <summary>
    /// Controle visual para representação e edição interativa de armadura em vigas
    /// </summary>
    public class VisualizadorArmaduraViga : UserControl
    {
        public class InformacaoArmaduraViga
        {
            public double Comprimento { get; set; } = 5000; // mm (comprimento real da instância)
            public double Altura { get; set; } = 500; // mm (By do tipo)
            public double Largura { get; set; } = 300; // mm (Bx do tipo)
            public double Recobrimento { get; set; } = 25; // mm 
            public List<ArmVar> VaroesLongitudinais { get; set; } = new List<ArmVar>();
            public List<ArmStirrup> Estribos { get; set; } = new List<ArmStirrup>();
            public double MultiplicadorAmarracao { get; set; } = 50;
            public bool AmarracaoAutomatica { get; set; } = true;
            
            // Informações da designação para interface
            public string Designacao { get; set; } = "";
            public string TipoFamilia { get; set; } = "";
            
            /// <summary>
            /// Configura as dimensões baseadas numa viga específica do Revit
            /// </summary>
            public void ConfigurarDimensoesDeViga(Autodesk.Revit.DB.FamilyInstance viga, Autodesk.Revit.DB.Document doc)
            {
                try
                {
                    // COMPRIMENTO: usar da instância (pode variar)
                    var locCurve = viga.Location as Autodesk.Revit.DB.LocationCurve;
                    if (locCurve != null)
                    {
                        Comprimento = Uteis.FeetParaMilimetros(locCurve.Curve.Length); // pés para mm
                    }
                    
                    // Verificar se há parâmetro Length na instância
                    var paramLength = viga.LookupParameter("Length");
                    if (paramLength != null && paramLength.AsDouble() > 0)
                    {
                        double lengthFromParam = Uteis.FeetParaMilimetros(paramLength.AsDouble()); // pés para mm
                        Comprimento = Math.Max(Comprimento, lengthFromParam);
                    }

                    // ALTURA E LARGURA: priorizar tipo (Bx, By) - são fixas por designação
                    var elementType = doc.GetElement(viga.GetTypeId()) as Autodesk.Revit.DB.ElementType;
                    if (elementType != null)
                    {
                        // Altura (By)
                        var paramBy = elementType.LookupParameter("By");
                        if (paramBy != null && paramBy.AsDouble() > 0)
                        {
                            Altura = Uteis.FeetParaMilimetros(paramBy.AsDouble()); // pés para mm
                        }

                        // Largura (Bx)
                        var paramBx = elementType.LookupParameter("Bx");
                        if (paramBx != null && paramBx.AsDouble() > 0)
                        {
                            Largura = Uteis.FeetParaMilimetros(paramBx.AsDouble()); // pés para mm
                        }
                        
                        TipoFamilia = elementType.Name;
                    }

                    // Se não encontrou Bx/By, tentar Properties da instância
                    if (Altura <= 0)
                    {
                        var paramAltura = viga.LookupParameter("Altura") ?? viga.LookupParameter("Height");
                        if (paramAltura != null && paramAltura.AsDouble() > 0)
                        {
                            Altura = Uteis.FeetParaMilimetros(paramAltura.AsDouble());
                        }
                    }

                    if (Largura <= 0)
                    {
                        var paramLargura = viga.LookupParameter("Largura") ?? viga.LookupParameter("Width");
                        if (paramLargura != null && paramLargura.AsDouble() > 0)
                        {
                            Largura = Uteis.FeetParaMilimetros(paramLargura.AsDouble());
                        }
                    }

                    // Obter designação
                    var paramDesignacao = viga.LookupParameter("Designacao");
                    if (paramDesignacao != null && !string.IsNullOrEmpty(paramDesignacao.AsString()))
                    {
                        Designacao = paramDesignacao.AsString();
                    }
                    else
                    {
                        Designacao = viga.Symbol?.FamilyName ?? "N/A";
                    }
                }
                catch (Exception ex)
                {
                    // Em caso de erro, manter valores padrão
                    System.Windows.Forms.MessageBox.Show($"Erro ao configurar dimensões: {ex.Message}", "Aviso");
                }
            }
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
        private static readonly Color COR_BETAO = Color.LightGray;
        private static readonly Dictionary<int, Color> COR_DIAMETRO = new()
        {
            { 6, Color.White },
            { 8, Color.Blue },
            { 10, Color.FromArgb(92, 184, 92) }, // verde
            { 12, Color.Red },
            { 16, Color.Yellow },
            { 20, Color.Magenta },
            { 25, Color.FromArgb(255, 140, 0) }, // laranja
            { 32, Color.Cyan }
        };

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
            // Removido: DesenharInformacoes(g);
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
            // Desenhar viga
            using (var brush = new SolidBrush(COR_BETAO))
            using (var pen = new Pen(Color.Black, 2))
            {
                g.FillRectangle(brush, areaViga);
                g.DrawRectangle(pen, areaViga);
            }

            // Desenhar recobrimento (linha tracejada)
            float recobrimento = (float)informacaoViga.Recobrimento * escalaDesenho;
            using (var penTracejado = new Pen(Color.Gray, 1))
            {
                penTracejado.DashStyle = DashStyle.Dash;
                Rectangle areaCoberta = new Rectangle(
                    (int)(areaViga.X + recobrimento),
                    (int)(areaViga.Y + recobrimento),
                    (int)(areaViga.Width - 2 * recobrimento),
                    (int)(areaViga.Height - 2 * recobrimento)
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
            // Removido: informações de dimensões/armadura
        }

        private void DesenharVistaLongitudinal(Graphics g)
        {
            // Desenhar viga (vista lateral)
            using (var brush = new SolidBrush(COR_BETAO))
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
            // Removido: informações de dimensões/armadura
        }

        private void AtualizarPontosArmaduraCorteTransversal()
        {
            pontosArmadura.Clear();
            float recobrimento = (float)informacaoViga.Recobrimento * escalaDesenho;

            int indice = 0;
            foreach (var varao in informacaoViga.VaroesLongitudinais)
            {
                Color cor = COR_DIAMETRO.ContainsKey((int)varao.Diametro) ? COR_DIAMETRO[(int)varao.Diametro] : Color.Gray;
                if (varao.TipoArmadura.ToLower() == "lateral")
                {
                    // Novo: desenhar quantidade por face em cada lateral
                    for (int i = 0; i < varao.Quantidade * 2; i++)
                    {
                        PointF posicao = CalcularPosicaoVarao(varao.TipoArmadura, i, varao.Quantidade, recobrimento);
                        pontosArmadura.Add(new PontoArmadura
                        {
                            Posicao = posicao,
                            Diametro = varao.Diametro,
                            Tipo = varao.TipoArmadura,
                            Cor = cor,
                            IndiceOriginal = indice
                        });
                    }
                }
                else
                {
                    for (int i = 0; i < varao.Quantidade; i++)
                    {
                        PointF posicao = CalcularPosicaoVarao(varao.TipoArmadura, i, varao.Quantidade, recobrimento);
                        pontosArmadura.Add(new PontoArmadura
                        {
                            Posicao = posicao,
                            Diametro = varao.Diametro,
                            Tipo = varao.TipoArmadura,
                            Cor = cor,
                            IndiceOriginal = indice
                        });
                    }
                }
                indice++;
            }
        }

        private PointF CalcularPosicaoVarao(string tipo, int indice, int total, float recobrimento)
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
                        float espacamento = (areaViga.Width - 2 * recobrimento) / (total - 1);
                        x = areaViga.X + recobrimento + indice * espacamento;
                    }
                    var varaoSup = informacaoViga.VaroesLongitudinais.FirstOrDefault(v => v.TipoArmadura.ToLower() == "superior");
                    float diametroSup = varaoSup != null ? (float)varaoSup.Diametro : 0f;
                    y = areaViga.Y + recobrimento + diametroSup * escalaDesenho / 2;
                    break;

                case "inferior":
                    if (total == 1)
                    {
                        x = areaViga.X + areaViga.Width / 2;
                    }
                    else
                    {
                        float espacamento = (areaViga.Width - 2 * recobrimento) / (total - 1);
                        x = areaViga.X + recobrimento + indice * espacamento;
                    }
                    var varaoInf = informacaoViga.VaroesLongitudinais.FirstOrDefault(v => v.TipoArmadura.ToLower() == "inferior");
                    float diametroInf = varaoInf != null ? (float)varaoInf.Diametro : 0f;
                    y = areaViga.Y + areaViga.Height - recobrimento - diametroInf * escalaDesenho / 2;
                    break;

                case "lateral":
                    // Novo: quantidade por face
                    // total = quantidade por face * 2
                    int quantidadePorFace = total;
                    int quantidadeTotal = quantidadePorFace * 2;
                    float alturaUtil = areaViga.Height - 2 * recobrimento;
                    float diametroLat = (float)(informacaoViga.VaroesLongitudinais.FirstOrDefault(v => v.TipoArmadura.ToLower() == "lateral")?.Diametro ?? 0) * escalaDesenho / 2;
                    if (indice < quantidadePorFace)
                    {
                        // Face esquerda
                        float espacamentoY = alturaUtil / (quantidadePorFace + 1);
                        x = areaViga.X + recobrimento + diametroLat;
                        y = areaViga.Y + recobrimento + (indice + 1) * espacamentoY;
                    }
                    else
                    {
                        // Face direita
                        float espacamentoY = alturaUtil / (quantidadePorFace + 1);
                        x = areaViga.X + areaViga.Width - recobrimento - diametroLat;
                        y = areaViga.Y + recobrimento + (indice - quantidadePorFace + 1) * espacamentoY;
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
            
            Color cor = ponto.Cor;
            
            using (var brush = new SolidBrush(cor))
            using (var pen = new Pen(Color.Black, 1))
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
            Color corEstribo = COR_DIAMETRO.ContainsKey((int)estribo.Diametro) ? COR_DIAMETRO[(int)estribo.Diametro] : Color.Gray;
            float recobrimento = (float)informacaoViga.Recobrimento * escalaDesenho;

            using (var pen = new Pen(corEstribo, 2))
            {
                // Definir os 4 cantos internos do estribo
                PointF p1 = new PointF(areaViga.X + recobrimento, areaViga.Y + recobrimento); // superior esquerdo
                PointF p2 = new PointF(areaViga.X + areaViga.Width - recobrimento, areaViga.Y + recobrimento); // superior direito
                PointF p3 = new PointF(areaViga.X + areaViga.Width - recobrimento, areaViga.Y + areaViga.Height - recobrimento); // inferior direito
                PointF p4 = new PointF(areaViga.X + recobrimento, areaViga.Y + areaViga.Height - recobrimento); // inferior esquerdo

                // Desenhar o estribo como polígono
                g.DrawLine(pen, p1, p2);
                g.DrawLine(pen, p2, p3);
                g.DrawLine(pen, p3, p4);
                g.DrawLine(pen, p4, p1);

                // Desenhar gancho de 135° para dentro em cada canto superior
                float tamanhoDobra = 20; // pixels
                float anguloRad = 135 * (float)Math.PI / 180f;
                // Gancho no canto superior esquerdo
                g.DrawLine(pen,
                    p1.X,
                    p1.Y,
                    p1.X + (float)(Math.Cos(anguloRad) * tamanhoDobra),
                    p1.Y + (float)(Math.Sin(anguloRad) * tamanhoDobra));
                // Gancho no canto superior direito
                g.DrawLine(pen,
                    p2.X,
                    p2.Y,
                    p2.X - (float)(Math.Cos(anguloRad) * tamanhoDobra),
                    p2.Y + (float)(Math.Sin(anguloRad) * tamanhoDobra));
            }
        }

        private void DesenharArmaduraLongitudinalVista(Graphics g)
        {
            float recobrimento = (float)informacaoViga.Recobrimento * escalaDesenho;
            
            // Armadura superior
            var armaduraSuperior = informacaoViga.VaroesLongitudinais.Where(v => v.TipoArmadura.ToLower() == "superior");
            foreach (var varao in armaduraSuperior)
            {
                Color cor = COR_DIAMETRO.ContainsKey((int)varao.Diametro) ? COR_DIAMETRO[(int)varao.Diametro] : Color.Gray;
                using (var pen = new Pen(cor, 3))
                {
                    float y = areaViga.Y + recobrimento;
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
                Color cor = COR_DIAMETRO.ContainsKey((int)varao.Diametro) ? COR_DIAMETRO[(int)varao.Diametro] : Color.Gray;
                using (var pen = new Pen(cor, 3))
                {
                    float y = areaViga.Y + areaViga.Height - recobrimento;
                    g.DrawLine(pen, areaViga.X, y, areaViga.X + areaViga.Width, y);
                }
            }
        }

        private void DesenharEstribosVistaLongitudinal(Graphics g)
        {
            if (informacaoViga.Estribos.Count == 0) return;

            var estribo = informacaoViga.Estribos.First();
            Color corEstribo = COR_DIAMETRO.ContainsKey((int)estribo.Diametro) ? COR_DIAMETRO[(int)estribo.Diametro] : Color.Gray;
            float espacamento = (float)estribo.Espacamento * escalaDesenho * 0.001f; // mm para escala
            
            using (var pen = new Pen(corEstribo, 1))
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
            // Legenda dinâmica dos diâmetros presentes
            var diametrosPresentes = informacaoViga?.VaroesLongitudinais
                .Select(v => v.Diametro)
                .ToList();
            // Adicionar diâmetro dos estribos se houver
            if (informacaoViga?.Estribos != null && informacaoViga.Estribos.Count > 0)
            {
                diametrosPresentes.AddRange(informacaoViga.Estribos.Select(e => e.Diametro));
            }
            diametrosPresentes = diametrosPresentes.Distinct().OrderBy(d => d).ToList();
            if (diametrosPresentes == null || diametrosPresentes.Count == 0) return;

            float totalLegendaLargura = diametrosPresentes.Count * 70 + 60;
            float x = (this.Width - totalLegendaLargura) / 2;
            float y = this.Height - 35;
            float espacamentoX = 70;

            using (var font = new Font("Arial", 8))
            using (var brush = new SolidBrush(Color.Black))
            {
                g.DrawString("Legenda:", font, brush, x, y);
                x += 60;
                foreach (var diam in diametrosPresentes)
                {
                    Color cor = COR_DIAMETRO.ContainsKey((int)diam) ? COR_DIAMETRO[(int)diam] : Color.Gray;
                    DesenharItemLegendaDiametro(g, font, x, y, cor, $"Ø{diam}");
                    x += espacamentoX;
                }
            }
        }

        private void DesenharItemLegendaDiametro(Graphics g, Font font, float x, float y, Color cor, string texto)
        {
            using (var brush = new SolidBrush(cor))
            using (var brushTexto = new SolidBrush(Color.Black))
            {
                g.FillRectangle(brush, x, y, 12, 12);
                g.DrawRectangle(Pens.Black, x, y, 12, 12);
                g.DrawString(texto, font, brushTexto, x + 18, y - 1);
            }
        }

        private Color ObterCorTipoArmadura(string tipo, double diametro)
        {
            return COR_DIAMETRO.ContainsKey((int)diametro) ? COR_DIAMETRO[(int)diametro] : Color.Gray;
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