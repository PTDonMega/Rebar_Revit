using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace Rebar_Revit
{
    /// <summary>
    /// Controlo visual: desenha e permite interacção com a armadura de vigas
    /// (corte transversal e vista longitudinal).
    /// </summary>
    public class VisualizadorArmaduraViga : UserControl
    {
        public class InformacaoArmaduraViga
        {
            public double Altura { get; set; } = 500; // mm
            public double Largura { get; set; } = 300; // mm
            public double Recobrimento { get; set; } = 30; // mm
            public List<ArmVar> VaroesLongitudinais { get; set; } = new List<ArmVar>();
            public List<ArmStirrup> Estribos { get; set; } = new List<ArmStirrup>();
            public double MultiplicadorAmarracao { get; set; } = 50;
            public bool AmarracaoAutomatica { get; set; } = true;

            // Dados para interface
            public string Designacao { get; set; } = "";
            public string TipoFamilia { get; set; } = "";

            /// <summary>
            /// Preenche Altura/Largura/Designacao a partir de uma FamilyInstance do Revit.
            /// Mantém valores padrão em caso de falha.
            /// </summary>
            public void ConfigurarDimensoesDeViga(Autodesk.Revit.DB.FamilyInstance viga, Autodesk.Revit.DB.Document doc)
            {
                try
                {
                    // Priorizar parâmetros do tipo (Bx, By)
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

                    // Se não existirem Bx/By, tentar parâmetros da instância
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

                    // Fallback: determinar dimensão a partir do bounding box
                    if (Altura <= 0 || double.IsNaN(Altura))
                    {
                        try
                        {
                            Autodesk.Revit.DB.BoundingBoxXYZ bbox = null;
                            if (elementType != null)
                                bbox = elementType.get_BoundingBox(null);
                            if (bbox == null)
                                bbox = viga.get_BoundingBox(null);

                            if (bbox != null)
                            {
                                Autodesk.Revit.DB.Transform tf = null;
                                try { tf = viga.GetTransform(); } catch { tf = null; }

                                var min = bbox.Min;
                                var max = bbox.Max;
                                var corners = new List<Autodesk.Revit.DB.XYZ>
                                {
                                    new Autodesk.Revit.DB.XYZ(min.X, min.Y, min.Z),
                                    new Autodesk.Revit.DB.XYZ(min.X, min.Y, max.Z),
                                    new Autodesk.Revit.DB.XYZ(min.X, max.Y, min.Z),
                                    new Autodesk.Revit.DB.XYZ(min.X, max.Y, max.Z),
                                    new Autodesk.Revit.DB.XYZ(max.X, min.Y, min.Z),
                                    new Autodesk.Revit.DB.XYZ(max.X, min.Y, max.Z),
                                    new Autodesk.Revit.DB.XYZ(max.X, max.Y, min.Z),
                                    new Autodesk.Revit.DB.XYZ(max.X, max.Y, max.Z),
                                };

                                List<Autodesk.Revit.DB.XYZ> localCorners = new List<Autodesk.Revit.DB.XYZ>();
                                if (tf != null)
                                {
                                    var inv = tf.Inverse;
                                    foreach (var c in corners)
                                        localCorners.Add(inv.OfPoint(c));
                                }
                                else
                                {
                                    localCorners.AddRange(corners);
                                }

                                double minY = localCorners.Min(p => p.Y);
                                double maxY = localCorners.Max(p => p.Y);
                                double minZ = localCorners.Min(p => p.Z);
                                double maxZ = localCorners.Max(p => p.Z);

                                double alturaLocal = Math.Abs(maxZ - minZ);
                                if (alturaLocal <= 0)
                                    alturaLocal = Math.Abs(maxY - minY);

                                if (alturaLocal > 0)
                                {
                                    Altura = Uteis.FeetParaMilimetros(alturaLocal);
                                }
                            }
                        }
                        catch
                        {
                            // manter valores padrão
                        }
                    }

                    // Designação
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
                    // Em erro, notificar e manter valores padrão
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

        private InformacaoDistanciasUltimas distanciasUltimas = new InformacaoDistanciasUltimas();

        public class InformacaoDistanciasUltimas
        {
            // Distâncias entre varões (mm)
            public double Superior_mm { get; set; }
            public int CountSuperior { get; set; }
            public double Inferior_mm { get; set; }
            public int CountInferior { get; set; }
            public double Lateral_mm { get; set; }
            public int CountLateral { get; set; }
        }

        // Evento: distâncias atualizadas
        public event EventHandler<InformacaoDistanciasUltimas> DistanciasAtualizadas;

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
        }

        private void CalcularEscalaEPosicao()
        {
            // Margens de desenho
            float margemX = 40;
            float margemY = 30;
            float areaDisponavelX = this.Width - 2 * margemX;
            float areaDisponavelY = this.Height - 2 * margemY - 40; // espaço para legendas

            if (mostrarCorteTransversal)
            {
                float reservedBottom = 110f; // espaço reservado para legenda
                if (reservedBottom > this.Height / 3f) reservedBottom = this.Height / 6f; // evitar reservar demasiado

                float escalaX = areaDisponavelX / (float)informacaoViga.Largura;
                float escalaY = (this.Height - 2 * margemY - reservedBottom) / (float)informacaoViga.Altura;
                escalaDesenho = Math.Min(escalaX, escalaY) * 0.92f;

                float larguraDesenho = (float)informacaoViga.Largura * escalaDesenho;
                float alturaDesenho = (float)informacaoViga.Altura * escalaDesenho;

                // Centrar horizontalmente e verticalmente na área superior
                offsetDesenho = new PointF(
                    margemX + (areaDisponavelX - larguraDesenho) / 2f,
                    margemY + ((this.Height - 2 * margemY - reservedBottom) - alturaDesenho) / 2f
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
                // Vista longitudinal: usa largura disponível como comprimento visual
                float escalaY = areaDisponavelY / (float)informacaoViga.Altura;
                escalaDesenho = escalaY * 0.92f;

                float comprimentoDesenho = areaDisponavelX * 0.92f;
                float alturaDesenho = (float)informacaoViga.Altura * escalaDesenho;

                offsetDesenho = new PointF(
                    (this.Width - comprimentoDesenho) / 2,
                    margemY + (areaDisponavelY - alturaDesenho) / 2
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
            // Viga
            using (var brush = new SolidBrush(COR_BETAO))
            using (var pen = new Pen(Color.Black, 2))
            {
                g.FillRectangle(brush, areaViga);
                g.DrawRectangle(pen, areaViga);
            }

            // Recobrimento (linha tracejada)
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

            // Atualizar posições dos pontos
            AtualizarPontosArmaduraCorteTransversal();

            // Desenhar varões
            foreach (var ponto in pontosArmadura)
            {
                DesenharPontoArmadura(g, ponto);
            }

            // Indicadores de distância entre varões
            DesenharIndicadoresEntreVaroes(g);

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

        private void DesenharIndicadoresEntreVaroes(Graphics g)
        {
            if (pontosArmadura == null || pontosArmadura.Count == 0) return;

            // Extremos dos pontos
            float left = pontosArmadura.Min(p => p.Posicao.X);
            float right = pontosArmadura.Max(p => p.Posicao.X);
            float top = pontosArmadura.Min(p => p.Posicao.Y);
            float bottom = pontosArmadura.Max(p => p.Posicao.Y);

            // Centro entre eixos
            float centerX = (left + right) / 2f;
            float centerY = (top + bottom) / 2f;

            using (var pen = new Pen(Color.DarkGray, 1))
            using (var brush = new SolidBrush(Color.Black))
            using (var font = new Font("Arial", 8))
            {
                pen.DashStyle = DashStyle.Dash;

                // Linha vertical (entre superior/inferior)
                g.DrawLine(pen, centerX, top, centerX, bottom);

                // Linha horizontal (entre laterais)
                g.DrawLine(pen, left, centerY, right, centerY);

                // Setas nas extremidades
                DrawArrowHead(g, pen, left + 4, centerY, left, centerY);
                DrawArrowHead(g, pen, left - 4, centerY, left, centerY);
                DrawArrowHead(g, pen, right - 4, centerY, right, centerY);
                DrawArrowHead(g, pen, right + 4, centerY, right, centerY);

                DrawArrowHead(g, pen, centerX, top + 4, centerX, top);
                DrawArrowHead(g, pen, centerX, bottom - 4, centerX, bottom);
             }
         }

         private void DrawArrowHead(Graphics g, Pen pen, float xTip, float yTip, float xBase, float yBase)
         {
            // Desenha ponta de seta em V
            float ang = (float)Math.Atan2(yTip - yBase, xTip - xBase);
            float head = 4f;
            float ax = xTip - head * (float)Math.Cos(ang - Math.PI / 6);
            float ay = yTip - head * (float)Math.Sin(ang - Math.PI / 6);
            float bx = xTip - head * (float)Math.Cos(ang + Math.PI / 6);
            float by = yTip - head * (float)Math.Sin(ang + Math.PI / 6);
            g.DrawLine(pen, xTip, yTip, ax, ay);
            g.DrawLine(pen, xTip, yTip, bx, by);
        }

        private void DesenharVistaLongitudinal(Graphics g)
        {
            // Viga (vista lateral)
            using (var brush = new SolidBrush(COR_BETAO))
            using (var pen = new Pen(Color.Black, 2))
            {
                g.FillRectangle(brush, areaViga);
                g.DrawRectangle(pen, areaViga);
            }

            // Linha de centro
            using (var pen = new Pen(Color.Gray, 1))
            {
                pen.DashStyle = DashStyle.Dot;
                float centroY = areaViga.Y + areaViga.Height / 2;
                g.DrawLine(pen, areaViga.X, centroY, areaViga.X + areaViga.Width, centroY);
            }

            // Armadura longitudinal simplificada
            DesenharArmaduraLongitudinalVista(g);

            // Estribos ao longo do comprimento
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
            float recobrimento = (float)informacaoViga.Recobrimento * escalaDesenho;

            int indice = 0;
            foreach (var varao in informacaoViga.VaroesLongitudinais)
            {
                Color cor = COR_DIAMETRO.ContainsKey((int)varao.Diametro) ? COR_DIAMETRO[(int)varao.Diametro] : Color.Gray;

                // Distribuição de diâmetros (suporta combinações)
                var distribuicao = varao.ObterDistribuicaoDiametros();

                if (varao.TipoArmadura.ToLower() == "lateral")
                {
                    // Quantidade por face (duas faces laterais)
                    int quantidadePorFace = varao.Quantidade;
                    for (int i = 0; i < quantidadePorFace * 2; i++)
                    {
                        double diamAtual;
                        if (distribuicao != null && distribuicao.Count > 0)
                        {
                            if (i < quantidadePorFace)
                                diamAtual = (i < distribuicao.Count) ? distribuicao[i] : varao.Diametro;
                            else
                                diamAtual = ((i - quantidadePorFace) < distribuicao.Count) ? distribuicao[i - quantidadePorFace] : varao.Diametro;
                        }
                        else
                        {
                            diamAtual = varao.Diametro;
                        }

                        PointF posicao = CalcularPosicaoVarao(varao.TipoArmadura, i, varao.Quantidade, recobrimento, (float)diamAtual);
                        Color corBar = COR_DIAMETRO.ContainsKey((int)diamAtual) ? COR_DIAMETRO[(int)diamAtual] : cor;
                        pontosArmadura.Add(new PontoArmadura
                        {
                            Posicao = posicao,
                            Diametro = diamAtual,
                            Tipo = varao.TipoArmadura,
                            Cor = corBar,
                            IndiceOriginal = indice
                        });
                    }
                }
                else
                {
                    for (int i = 0; i < varao.Quantidade; i++)
                    {
                        double diamAtual = (distribuicao != null && i < distribuicao.Count) ? distribuicao[i] : varao.Diametro;
                        PointF posicao = CalcularPosicaoVarao(varao.TipoArmadura, i, varao.Quantidade, recobrimento, (float)diamAtual);
                        Color corBar = COR_DIAMETRO.ContainsKey((int)diamAtual) ? COR_DIAMETRO[(int)diamAtual] : cor;
                        pontosArmadura.Add(new PontoArmadura
                        {
                            Posicao = posicao,
                            Diametro = diamAtual,
                            Tipo = varao.TipoArmadura,
                            Cor = corBar,
                            IndiceOriginal = indice
                        });
                    }
                }
                indice++;
            }

            // Atualizar resumo de distâncias
            CalcularDistanciasResumo();
        }

        private void CalcularDistanciasResumo()
        {
            if (areaViga.Width == 0 || escalaDesenho == 0) return;

            float centerX = areaViga.X + areaViga.Width / 2f;
            float centerY = areaViga.Y + areaViga.Height / 2f;

            var sup = pontosArmadura.Where(p => p.Tipo.ToLower() == "superior").ToList();
            var inf = pontosArmadura.Where(p => p.Tipo.ToLower() == "inferior").ToList();
            var lat = pontosArmadura.Where(p => p.Tipo.ToLower() == "lateral").ToList();

            double sup_mm = 0;
            double inf_mm = 0;
            double lat_mm = 0;

            // Superior: espaçamento médio em X
            if (sup.Count > 1)
            {
                var xs = sup.Select(p => p.Posicao.X).OrderBy(x => x).ToList();
                var diffs = new List<double>();
                for (int i = 1; i < xs.Count; i++) diffs.Add(xs[i] - xs[i - 1]);
                double meanPixels = diffs.Average();
                sup_mm = meanPixels / escalaDesenho;
            }

            // Inferior: igual ao superior
            if (inf.Count > 1)
            {
                var xs = inf.Select(p => p.Posicao.X).OrderBy(x => x).ToList();
                var diffs = new List<double>();
                for (int i = 1; i < xs.Count; i++) diffs.Add(xs[i] - xs[i - 1]);
                double meanPixels = diffs.Average();
                inf_mm = meanPixels / escalaDesenho;
            }

            // Lateral: espaçamento médio por face em Y
            if (lat.Count > 1)
            {
                var leftFace = lat.Where(p => p.Posicao.X < centerX).OrderBy(p => p.Posicao.Y).Select(p => p.Posicao.Y).ToList();
                var rightFace = lat.Where(p => p.Posicao.X >= centerX).OrderBy(p => p.Posicao.Y).Select(p => p.Posicao.Y).ToList();

                double leftMean = 0, rightMean = 0;
                if (leftFace.Count > 1)
                {
                    var diffsL = new List<double>();
                    for (int i = 1; i < leftFace.Count; i++) diffsL.Add(leftFace[i] - leftFace[i - 1]);
                    leftMean = diffsL.Average();
                }
                if (rightFace.Count > 1)
                {
                    var diffsR = new List<double>();
                    for (int i = 1; i < rightFace.Count; i++) diffsR.Add(rightFace[i] - rightFace[i - 1]);
                    rightMean = diffsR.Average();
                }

                var nonZero = new List<double>();
                if (leftMean > 0) nonZero.Add(leftMean);
                if (rightMean > 0) nonZero.Add(rightMean);
                if (nonZero.Count > 0) lat_mm = nonZero.Average() / escalaDesenho;
            }

            distanciasUltimas.Superior_mm = Math.Round(sup_mm, 0);
            distanciasUltimas.CountSuperior = sup.Count;
            distanciasUltimas.Inferior_mm = Math.Round(inf_mm, 0);
            distanciasUltimas.CountInferior = inf.Count;
            distanciasUltimas.Lateral_mm = Math.Round(lat_mm, 0);
            distanciasUltimas.CountLateral = lat.Count;

            // Notificar ouvintes
            DistanciasAtualizadas?.Invoke(this, distanciasUltimas);
        }

        private void DesenharLegenda(Graphics g)
        {
            // Legenda dos diâmetros presentes
            var diametrosPresentes = informacaoViga?.VaroesLongitudinais
                .Select(v => v.Diametro)
                .ToList() ?? new List<double>();
            // Incluir diâmetros de estribos
            if (informacaoViga?.Estribos != null && informacaoViga.Estribos.Count > 0)
            {
                foreach (var es in informacaoViga.Estribos)
                {
                    if (es.UsaCombinacao && es.Combinacao != null)
                    {
                        diametrosPresentes.AddRange(es.Combinacao.Estribos.Select(x => x.Diametro));
                    }
                    else
                    {
                        diametrosPresentes.Add(es.Diametro);
                    }
                }
            }
            diametrosPresentes = diametrosPresentes.Distinct().OrderBy(d => d).ToList();
            if (diametrosPresentes == null || diametrosPresentes.Count == 0) return;

            float totalLegendaLargura = diametrosPresentes.Count * 70 + 60;
            float legendX = (this.Width - totalLegendaLargura) / 2;
            float legendY = this.Height - 28; // margem inferior
            float espacamentoX = 70;

            using (var font = new Font("Arial", 8))
            using (var brush = new SolidBrush(Color.Black))
            {
                g.DrawString("Legenda:", font, brush, legendX, legendY);
                float x = legendX + 60;
                foreach (var diam in diametrosPresentes)
                {
                    Color cor = COR_DIAMETRO.ContainsKey((int)diam) ? COR_DIAMETRO[(int)diam] : Color.Gray;
                    DesenharItemLegendaDiametro(g, font, x, legendY, cor, $"Ø{diam}");
                    x += espacamentoX;
                }
            }

            // Caixa de distâncias abaixo da viga
            DesenharListaDistancias(g);
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

        private void DesenharListaDistancias(Graphics g)
        {
            // Caixa compacta de distâncias centrada abaixo da viga
            int boxWidth = Math.Min(420, this.Width - 40);
            int boxHeight = 80; // altura da caixa

            // Posicionar a caixa abaixo da viga
            float boxX = areaViga.X + (areaViga.Width - boxWidth) / 2f;
            if (boxX < 8) boxX = 8;
            float boxY = areaViga.Y + areaViga.Height + 12; // espaço abaixo da viga

            using (var brushBg = new SolidBrush(Color.FromArgb(240, 240, 240)))
            using (var penBorder = new Pen(Color.Gray))
            using (var fontTitle = new Font("Arial", 9, FontStyle.Bold))
            using (var fontLabel = new Font("Arial", 8, FontStyle.Regular))
            using (var fontValue = new Font("Arial", 8, FontStyle.Regular))
            using (var brush = new SolidBrush(Color.Black))
            {
                var rect = new Rectangle((int)boxX, (int)boxY, boxWidth, boxHeight);
                g.FillRectangle(brushBg, rect);
                g.DrawRectangle(penBorder, rect);

                // Título
                string title = "Distâncias ao eixo:";
                var sizeTitle = g.MeasureString(title, fontTitle);
                g.DrawString(title, fontTitle, brush, boxX + (boxWidth - sizeTitle.Width) / 2f, boxY + 6);

                // Três colunas: Superior / Inferior / Lateral
                int cols = 3;
                float colWidth = boxWidth / (float)cols;

                string valSup = $"{distanciasUltimas.Superior_mm} mm ({distanciasUltimas.CountSuperior})";
                string valInf = $"{distanciasUltimas.Inferior_mm} mm ({distanciasUltimas.CountInferior})";
                string valLat = $"{distanciasUltimas.Lateral_mm} mm ({distanciasUltimas.CountLateral})";

                float contentAreaTop = boxY + 6 + sizeTitle.Height + 6;
                float contentAreaHeight = boxY + boxHeight - contentAreaTop;
                float contentCenterY = contentAreaTop + contentAreaHeight / 2f;

                float labelOffset = -10f;
                float valueOffset = 8f;

                // Superior
                float col0X = boxX + 0 * colWidth;
                var lblSup = g.MeasureString("Superior", fontLabel);
                var mSup = g.MeasureString(valSup, fontValue);
                g.DrawString("Superior", fontLabel, brush, col0X + colWidth / 2f - lblSup.Width / 2f, contentCenterY + labelOffset);
                g.DrawString(valSup, fontValue, brush, col0X + colWidth / 2f - mSup.Width / 2f, contentCenterY + valueOffset);

                // Inferior
                float col1X = boxX + 1 * colWidth;
                var lblInf = g.MeasureString("Inferior", fontLabel);
                var mInf = g.MeasureString(valInf, fontValue);
                g.DrawString("Inferior", fontLabel, brush, col1X + colWidth / 2f - lblInf.Width / 2f, contentCenterY + labelOffset);
                g.DrawString(valInf, fontValue, brush, col1X + colWidth / 2f - mInf.Width / 2f, contentCenterY + valueOffset);

                // Lateral
                float col2X = boxX + 2 * colWidth;
                var lblLat = g.MeasureString("Lateral", fontLabel);
                var mLat = g.MeasureString(valLat, fontValue);
                g.DrawString("Lateral", fontLabel, brush, col2X + colWidth / 2f - lblLat.Width / 2f, contentCenterY + labelOffset);
                g.DrawString(valLat, fontValue, brush, col2X + colWidth / 2f - mLat.Width / 2f, contentCenterY + valueOffset);
            }
        }

         private void DesenharInformacoes(Graphics g)
         {
            // Painel lateral com resumo da configuração de armadura
            float margem = 10;
            float posX = areaViga.X + areaViga.Width + margem;
            float posY = areaViga.Y;
            float largura = 200 - margem;
            float alturaLinha = 15;

            using (var brush = new SolidBrush(Color.Black))
            using (var font = new Font("Arial", 8))
            {
                g.DrawString("Informações da Armadura", font, brush, posX, posY);
                posY += alturaLinha + 4;

                g.DrawString("Varões Longitudinais:", font, brush, posX, posY);
                posY += alturaLinha;
                foreach (var varao in informacaoViga.VaroesLongitudinais)
                {
                    string infoVarao = $"Ø{varao.Diametro} mm - {varao.Quantidade} unid.";
                    g.DrawString(infoVarao, font, brush, posX, posY);
                    posY += alturaLinha;
                }

                posY += 4;
                // Estribos (suporta combinações)
                foreach (var estribo in informacaoViga.Estribos)
                {
                    if (estribo.UsaCombinacao && estribo.Combinacao != null)
                    {
                        var partes = estribo.Combinacao.Estribos.Select(e => $"Ø{e.Diametro}mm//{e.Espacamento}mm");
                        string infoCombo = string.Join(" + ", partes);
                        g.DrawString(infoCombo, font, brush, posX, posY);
                        posY += alturaLinha;
                    }
                    else
                    {
                        string infoEstribo = $"Ø{estribo.Diametro} mm - Espaçamento: {estribo.Espacamento} mm";
                        g.DrawString(infoEstribo, font, brush, posX, posY);
                        posY += alturaLinha;
                    }
                }
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
                
                if (distancia <= 15) // área de clique
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

        private PointF CalcularPosicaoVarao(string tipo, int indice, int total, float recobrimento, float diametroAtual_mm)
        {
            // recobrimento já em pixels (recobrimento_mm * escala)
            float x = 0f, y = 0f;

            // Diâmetro do estribo (maior definido)
            float estriboDiam_mm = 0f;
            if (informacaoViga?.Estribos != null && informacaoViga.Estribos.Count > 0)
            {
                var firstEs = informacaoViga.Estribos.First();
                if (firstEs.UsaCombinacao && firstEs.Combinacao != null && firstEs.Combinacao.Estribos != null && firstEs.Combinacao.Estribos.Count > 0)
                    estriboDiam_mm = (float)firstEs.Combinacao.Estribos.Max(e => e.Diametro);
                else
                    estriboDiam_mm = (float)firstEs.Diametro;
            }

            // Maior diâmetro na face para calcular margens
            float maxDiamFace_mm = 0f;
            if (tipo.ToLower() == "superior" || tipo.ToLower() == "inferior")
            {
                foreach (var v in informacaoViga.VaroesLongitudinais.Where(v => v.TipoArmadura.ToLower() == tipo.ToLower()))
                {
                    if (v.UsaCombinacao && v.Combinacao != null && v.Combinacao.Varoes != null && v.Combinacao.Varoes.Count > 0)
                        maxDiamFace_mm = Math.Max(maxDiamFace_mm, (float)v.Combinacao.Varoes.Max(x => x.Diametro));
                    else
                        maxDiamFace_mm = Math.Max(maxDiamFace_mm, (float)v.Diametro);
                }
            }
            if (maxDiamFace_mm == 0f) maxDiamFace_mm = diametroAtual_mm;

            // Margens em pixels
            float margemEsqPixels = recobrimento + (estriboDiam_mm * escalaDesenho) + (maxDiamFace_mm / 2f) * escalaDesenho;
            float margemDirPixels = margemEsqPixels;

            switch (tipo.ToLower())
            {
                case "superior":
                    if (total == 1)
                    {
                        x = areaViga.X + areaViga.Width / 2f;
                    }
                    else
                    {
                        float availableWidth = areaViga.Width - 2f * margemEsqPixels;
                        if (availableWidth < 0f) availableWidth = Math.Max(0f, areaViga.Width - 2f * recobrimento);
                        float espacamento = (total - 1) > 0 ? (availableWidth / (total - 1)) : 0f;
                        x = areaViga.X + margemEsqPixels + indice * espacamento;
                    }

                    // Altura: recobrimento + metade do estribo + metade do varão
                    y = areaViga.Y + recobrimento + (estriboDiam_mm / 2f + diametroAtual_mm / 2f) * escalaDesenho;
                    break;

                case "inferior":
                    if (total == 1)
                    {
                        x = areaViga.X + areaViga.Width / 2f;
                    }
                    else
                    {
                        float availableWidth = areaViga.Width - 2f * margemEsqPixels;
                        if (availableWidth < 0f) availableWidth = Math.Max(0f, areaViga.Width - 2f * recobrimento);
                        float espacamento = (total - 1) > 0 ? (availableWidth / (total - 1)) : 0f;
                        x = areaViga.X + margemEsqPixels + indice * espacamento;
                    }

                    y = areaViga.Y + areaViga.Height - (recobrimento + (estriboDiam_mm / 2f + diametroAtual_mm / 2f) * escalaDesenho);
                    break;

                case "lateral":
                    int quantidadePorFace = total;
                    float alturaUtil = areaViga.Height - 2f * recobrimento;
                    float espacamentoY = quantidadePorFace > 0 ? (alturaUtil / (quantidadePorFace + 1f)) : 0f;
                    float margemLatPixels = recobrimento + (estriboDiam_mm * escalaDesenho) + (diametroAtual_mm / 2f) * escalaDesenho;

                    if (indice < quantidadePorFace)
                    {
                        x = areaViga.X + margemLatPixels;
                        y = areaViga.Y + recobrimento + (indice + 1) * espacamentoY;
                    }
                    else
                    {
                        x = areaViga.X + areaViga.Width - margemLatPixels;
                        y = areaViga.Y + recobrimento + (indice - quantidadePorFace + 1) * espacamentoY;
                    }
                    break;

                default:
                    x = areaViga.X + areaViga.Width / 2f;
                    y = areaViga.Y + areaViga.Height / 2f;
                    break;
            }

            return new PointF(x, y);
        }

        private void DesenharPontoArmadura(Graphics g, PontoArmadura ponto)
        {
            // Raio em pixels: (diâmetro_mm / 2) * escala
            float raio = Math.Max(3f, (float)(ponto.Diametro * escalaDesenho / 2f));

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

                // Mostrar diâmetro como texto se houver espaço
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

        private void DesenharArmaduraLongitudinalVista(Graphics g)
        {
            float recobrimento = (float)informacaoViga.Recobrimento * escalaDesenho;

            // Armadura superior
            var armaduraSuperior = informacaoViga.VaroesLongitudinais.Where(v => v.TipoArmadura.ToLower() == "superior");
            foreach (var varao in armaduraSuperior)
            {
                Color cor = COR_DIAMETRO.ContainsKey((int)varao.Diametro) ? COR_DIAMETRO[(int)varao.Diametro] : Color.Gray;
                // Largura do traço aproxima o diâmetro no ecrã
                float penWidthSup = Math.Max(1f, (float)(varao.Diametro * escalaDesenho));
                using (var pen = new Pen(cor, penWidthSup))
                {
                    float y = areaViga.Y + recobrimento;
                    g.DrawLine(pen, areaViga.X, y, areaViga.X + areaViga.Width, y);
                    
                    // Amarração automática: ganchos nas extremidades
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
                float penWidthInf = Math.Max(1f, (float)(varao.Diametro * escalaDesenho));
                using (var pen = new Pen(cor, penWidthInf))
                {
                    float y = areaViga.Y + areaViga.Height - recobrimento;
                    g.DrawLine(pen, areaViga.X, y, areaViga.X + areaViga.Width, y);
                }
            }
        }

        private void DesenharEstribosCorteTransversal(Graphics g)
        {
            if (informacaoViga.Estribos.Count == 0) return;

            var estribo = informacaoViga.Estribos.First();
            Color corEstribo = COR_DIAMETRO.ContainsKey((int)estribo.Diametro) ? COR_DIAMETRO[(int)estribo.Diametro] : Color.Gray;
            float recobrimento = (float)informacaoViga.Recobrimento * escalaDesenho;
            float raioCanto = 18f * escalaDesenho; // raio para cantos arredondados

            using (var pen = new Pen(corEstribo, 2))
            {
                float x = areaViga.X + recobrimento;
                float y = areaViga.Y + recobrimento;
                float largura = areaViga.Width - 2 * recobrimento;
                float altura = areaViga.Height - 2 * recobrimento;

                // Estribo com cantos arredondados
                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddArc(x, y, raioCanto, raioCanto, 180, 90); // superior esquerdo
                    path.AddArc(x + largura - raioCanto, y, raioCanto, raioCanto, 270, 90); // superior direito
                    path.AddArc(x + largura - raioCanto, y + altura - raioCanto, raioCanto, raioCanto, 0, 90); // inferior direito
                    path.AddArc(x, y + altura - raioCanto, raioCanto, raioCanto, 90, 90); // inferior esquerdo
                    path.CloseFigure();
                    g.DrawPath(pen, path);
                }
            }
        }

        private void DesenharEstribosVistaLongitudinal(Graphics g)
        {
             if (informacaoViga.Estribos.Count == 0) return;

             // Suporta múltiplas definições de estribos (simples ou combinações)
             foreach (var estribo in informacaoViga.Estribos)
             {
                 if (estribo.UsaCombinacao && estribo.Combinacao != null)
                 {
                    // Comprimento visual em mm
                    float comprimentoVisual_mm = areaViga.Width / escalaDesenho;
                    var sequencia = estribo.Combinacao.GerarSequenciaEstribos(comprimentoVisual_mm);
                     foreach (var pos in sequencia)
                     {
                         Color cor = COR_DIAMETRO.ContainsKey((int)pos.Diametro) ? COR_DIAMETRO[(int)pos.Diametro] : Color.Gray;
                         using (var pen = new Pen(cor, 1))
                         {
                             float x = areaViga.X + (float)(pos.Posicao * escalaDesenho);
                             g.DrawLine(pen, x, areaViga.Y, x, areaViga.Y + areaViga.Height);
                         }
                     }
                 }
                 else
                 {
                     // Estribo simples: usar espaçamento em mm
                     float espacamento_pixels = (float)estribo.Espacamento * escalaDesenho;
                     if (espacamento_pixels <= 0) continue;

                     Color corEstribo = COR_DIAMETRO.ContainsKey((int)estribo.Diametro) ? COR_DIAMETRO[(int)estribo.Diametro] : Color.Gray;
                     using (var pen = new Pen(corEstribo, 1))
                     {
                        float comprimentoVisual_mm2 = areaViga.Width / escalaDesenho;
                        int numeroEstribos = (int)(comprimentoVisual_mm2 / estribo.Espacamento);
                        for (int i = 1; i <= numeroEstribos; i++)
                        {
                            float x = areaViga.X + (float)(i * estribo.Espacamento * escalaDesenho);
                            if (x <= areaViga.X || x >= areaViga.X + areaViga.Width) continue;
                            g.DrawLine(pen, x, areaViga.Y, x, areaViga.Y + areaViga.Height);
                        }
                     }
                 }
             }
         }
    }

    public class ArmaduraEditadaEventArgs : EventArgs
    {
        public string TipoOperacao { get; set; } // Adicionar, Editar, Remover
        public ArmVar VaraoModificado { get; set; }
    }
}