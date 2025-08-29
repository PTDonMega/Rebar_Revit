using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

namespace MacroArmaduraAvancado
{
    /// <summary>
    /// Detector avançado de elementos com análise de posicionamento - Versão Completa
    /// </summary>
    public class DetectorElementosAvancado
    {
        private Document doc;

        public DetectorElementosAvancado(Document documento)
        {
            doc = documento;
        }

        /// <summary>
        /// Detecta elementos estruturais do tipo especificado
        /// </summary>
        public List<Element> DetectarElementos(TipoElementoEstruturalEnum tipo)
        {
            List<Element> elementos = new List<Element>();
            FilteredElementCollector collector = new FilteredElementCollector(doc);

            try
            {
                switch (tipo)
                {
                    case TipoElementoEstruturalEnum.Pilares:
                        elementos = collector
                            .OfCategory(BuiltInCategory.OST_StructuralColumns)
                            .OfClass(typeof(FamilyInstance))
                            .Where(e => EElementoEstruturalValidoPilar(e))
                            .ToList();

                        // Incluir pilares indiretos se detectados
                        var pilaresIndiretos = DetectarPilaresIndiretos();
                        foreach (var pilarIndireto in pilaresIndiretos)
                        {
                            if (!elementos.Contains(pilarIndireto))
                            {
                                elementos.Add(pilarIndireto);
                            }
                        }
                        break;

                    case TipoElementoEstruturalEnum.Vigas:
                        elementos = collector
                            .OfCategory(BuiltInCategory.OST_StructuralFraming)
                            .OfClass(typeof(FamilyInstance))
                            .Where(e => EElementoEstruturalValidoViga(e))
                            .ToList();
                        break;

                    case TipoElementoEstruturalEnum.Fundacoes:
                        elementos = collector
                            .OfCategory(BuiltInCategory.OST_StructuralFoundation)
                            .OfClass(typeof(FamilyInstance))
                            .Where(e => EElementoEstruturalValidoFundacao(e))
                            .ToList();
                        break;

                    case TipoElementoEstruturalEnum.Lajes:
                        elementos = collector
                            .OfCategory(BuiltInCategory.OST_Floors)
                            .OfClass(typeof(Floor))
                            .Where(e => EElementoEstruturalValidoLaje(e))
                            .Cast<Element>()
                            .ToList();
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro na detecção de elementos do tipo {tipo}: {ex.Message}");
            }

            return elementos;
        }

        /// <summary>
        /// Analisa elementos e classifica por posicionamento
        /// </summary>
        public InformacaoAnaliseElementos AnalisarElementos(List<Element> elementos)
        {
            InformacaoAnaliseElementos analise = new InformacaoAnaliseElementos();
            analise.TotalElementos = elementos.Count;

            if (elementos.Count == 0) return analise;

            try
            {
                // Obter todos os níveis do projecto ordenados por elevação
                var todosNiveis = ObterNiveisOrdenados();

                if (todosNiveis.Count == 0)
                {
                    // Se não há níveis definidos, todos são considerados intermédios
                    analise.ElementosIntermedios = elementos.Count;
                    return analise;
                }

                Level nivelMaisBaixo = todosNiveis.First();
                Level nivelMaisAlto = todosNiveis.Last();
                double tolerancia = 0.1; // Tolerância em pés para comparação de elevações

                foreach (Element elemento in elementos)
                {
                    try
                    {
                        Level nivelElemento = ObterNivelElemento(elemento);

                        if (nivelElemento != null)
                        {
                            string nomeNivel = nivelElemento.Name;
                            if (!analise.NiveisDetectados.Contains(nomeNivel))
                            {
                                analise.NiveisDetectados.Add(nomeNivel);
                            }

                            // Classificar posição do elemento
                            if (Math.Abs(nivelElemento.Elevation - nivelMaisBaixo.Elevation) < tolerancia)
                            {
                                analise.ElementosFundacao++;
                            }
                            else if (Math.Abs(nivelElemento.Elevation - nivelMaisAlto.Elevation) < tolerancia)
                            {
                                analise.ElementosUltimoPiso++;
                            }
                            else
                            {
                                analise.ElementosIntermedios++;
                            }
                        }
                        else
                        {
                            // Elemento sem nível definido - considerar intermédio
                            analise.ElementosIntermedios++;
                        }
                    }
                    catch
                    {
                        // Em caso de erro com elemento individual, considerar intermédio
                        analise.ElementosIntermedios++;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro na análise de elementos: {ex.Message}");
            }

            return analise;
        }

        /// <summary>
        /// Verifica se um elemento é estrutural válido (método geral)
        /// </summary>
        public bool EElementoEstruturalValido(Element elemento)
        {
            if (elemento == null) return false;

            try
            {
                Category categoria = elemento.Category;
                if (categoria == null) return false;

                BuiltInCategory catBuiltIn = (BuiltInCategory)categoria.Id.IntegerValue;

                return catBuiltIn == BuiltInCategory.OST_StructuralColumns ||
                       catBuiltIn == BuiltInCategory.OST_StructuralFraming ||
                       catBuiltIn == BuiltInCategory.OST_StructuralFoundation ||
                       catBuiltIn == BuiltInCategory.OST_Floors;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Validação específica para pilares
        /// </summary>
        private bool EElementoEstruturalValidoPilar(Element elemento)
        {
            if (!(elemento is FamilyInstance pilar)) return false;

            try
            {
                // Verificar se tem parâmetros de dimensões
                var paramLargura = pilar.get_Parameter(BuiltInParameter.FAMILY_WIDTH_PARAM);
                var paramProfundidade = pilar.get_Parameter(BuiltInParameter.FAMILY_HEIGHT_PARAM);
                var paramAltura = pilar.get_Parameter(BuiltInParameter.FAMILY_HEIGHT_PARAM);

                if (paramLargura == null || paramProfundidade == null) return false;

                double largura = paramLargura.AsDouble();
                double profundidade = paramProfundidade.AsDouble();

                // Verificar dimensões mínimas (10cm convertido para pés)
                double dimensaoMinima = 0.1 / 0.3048;

                return largura > dimensaoMinima && profundidade > dimensaoMinima;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Validação específica para vigas
        /// </summary>
        private bool EElementoEstruturalValidoViga(Element elemento)
        {
            if (!(elemento is FamilyInstance viga)) return false;

            try
            {
                // Verificar se tem comprimento
                var paramComprimento = viga.get_Parameter(BuiltInParameter.INSTANCE_LENGTH_PARAM);
                if (paramComprimento == null) return false;

                double comprimento = paramComprimento.AsDouble();
                double comprimentoMinimo = 0.5 / 0.3048; // 50cm mínimo

                return comprimento > comprimentoMinimo;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Validação específica para fundações
        /// </summary>
        private bool EElementoEstruturalValidoFundacao(Element elemento)
        {
            if (!(elemento is FamilyInstance fundacao)) return false;

            try
            {
                // Verificar se está em nível baixo (próximo ao terreno)
                Level nivelElemento = ObterNivelElemento(fundacao);
                if (nivelElemento != null)
                {
                    var todosNiveis = ObterNiveisOrdenados();
                    if (todosNiveis.Count > 0)
                    {
                        Level nivelMaisBaixo = todosNiveis.First();
                        // Se está no nível mais baixo ou próximo dele, é uma fundação válida
                        return Math.Abs(nivelElemento.Elevation - nivelMaisBaixo.Elevation) < 1.0;
                    }
                }

                return true; // Se não conseguir determinar o nível, aceitar como válido
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Validação específica para lajes
        /// </summary>
        private bool EElementoEstruturalValidoLaje(Element elemento)
        {
            if (!(elemento is Floor laje)) return false;

            try
            {
                // Verificar se tem área mínima
                var paramArea = laje.get_Parameter(BuiltInParameter.HOST_AREA_COMPUTED);
                if (paramArea != null)
                {
                    double area = paramArea.AsDouble();
                    double areaMinima = 1.0; // 1 pé quadrado mínimo
                    return area > areaMinima;
                }

                return true; // Se não conseguir obter a área, aceitar como válido
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Determina tipo de amarração automático baseado na posição do elemento
        /// </summary>
        public TipoAmarracaoEnum DeterminarTipoAmarracaoAutomatico(Element elemento)
        {
            try
            {
                Level nivelElemento = ObterNivelElemento(elemento);
                if (nivelElemento == null) return TipoAmarracaoEnum.Reta;

                var todosNiveis = ObterNiveisOrdenados();
                if (todosNiveis.Count < 2) return TipoAmarracaoEnum.Reta;

                Level nivelMaisBaixo = todosNiveis.First();
                Level nivelMaisAlto = todosNiveis.Last();
                double tolerancia = 0.1;

                // Determinar tipo de amarração baseado na posição
                if (Math.Abs(nivelElemento.Elevation - nivelMaisBaixo.Elevation) < tolerancia)
                {
                    return TipoAmarracaoEnum.Dobrada90; // Fundação
                }
                else if (Math.Abs(nivelElemento.Elevation - nivelMaisAlto.Elevation) < tolerancia)
                {
                    return TipoAmarracaoEnum.Dobrada90; // Último piso
                }
                else
                {
                    return TipoAmarracaoEnum.Reta; // Pisos intermédios
                }
            }
            catch
            {
                return TipoAmarracaoEnum.Reta; // Valor seguro por defeito
            }
        }

        /// <summary>
        /// Detecta pilares indiretos apoiados em lajes estruturais
        /// </summary>
        public List<Element> DetectarPilaresIndiretos()
        {
            List<Element> pilaresIndiretos = new List<Element>();

            try
            {
                // Detectar todos os pilares
                FilteredElementCollector collectorPilares = new FilteredElementCollector(doc);
                var pilares = collectorPilares
                    .OfCategory(BuiltInCategory.OST_StructuralColumns)
                    .OfClass(typeof(FamilyInstance))
                    .Cast<FamilyInstance>()
                    .ToList();

                // Detectar todas as lajes estruturais
                FilteredElementCollector collectorLajes = new FilteredElementCollector(doc);
                var lajes = collectorLajes
                    .OfCategory(BuiltInCategory.OST_Floors)
                    .OfClass(typeof(Floor))
                    .Cast<Floor>()
                    .ToList();

                foreach (FamilyInstance pilar in pilares)
                {
                    if (EPilarIndireto(pilar, lajes))
                    {
                        pilaresIndiretos.Add(pilar);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro na detecção de pilares indiretos: {ex.Message}");
            }

            return pilaresIndiretos;
        }

        /// <summary>
        /// Verifica se um pilar é indireto (apoiado em laje)
        /// </summary>
        private bool EPilarIndireto(FamilyInstance pilar, List<Floor> lajes)
        {
            try
            {
                LocationPoint locPoint = pilar.Location as LocationPoint;
                if (locPoint == null) return false;

                XYZ pontoBase = locPoint.Point;
                XYZ pontoTeste = new XYZ(pontoBase.X, pontoBase.Y, pontoBase.Z - 0.1);

                foreach (Floor laje in lajes)
                {
                    if (PontoEstaEmLaje(pontoTeste, laje))
                    {
                        return true;
                    }
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Verifica se um ponto está em uma laje
        /// </summary>
        private bool PontoEstaEmLaje(XYZ ponto, Floor laje)
        {
            try
            {
                GeometryElement geoElement = laje.get_Geometry(new Options());
                if (geoElement == null) return false;

                foreach (GeometryObject geoObj in geoElement)
                {
                    if (geoObj is Solid solid && solid.Volume > 0)
                    {
                        // Verificar se o ponto está dentro do sólido usando intersecção de raios
                        return VerificarPontoDentroSolido(ponto, solid);
                    }
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Verifica se um ponto está dentro de um sólido usando método de intersecção de raios
        /// </summary>
        private bool VerificarPontoDentroSolido(XYZ ponto, Solid solido)
        {
            try
            {
                // Usar método de ray casting para verificar se o ponto está dentro
                XYZ direcao = new XYZ(1, 0, 0); // Direção horizontal
                XYZ pontoOrigem = new XYZ(ponto.X - 100, ponto.Y, ponto.Z); // Ponto distante

                Line linha = Line.CreateBound(pontoOrigem, new XYZ(ponto.X + 100, ponto.Y, ponto.Z));
                SolidCurveIntersection intersecao = solido.IntersectWithCurve(linha, new SolidCurveIntersectionOptions());

                if (intersecao != null)
                {
                    // Contar intersecções antes do ponto
                    int interseccoesAntes = 0;
                    for (int i = 0; i < intersecao.SegmentCount; i++)
                    {
                        Curve segmento = intersecao.GetCurveSegment(i);
                        XYZ pontoIntersecao = segmento.GetEndPoint(0);

                        if (pontoIntersecao.X < ponto.X)
                        {
                            interseccoesAntes++;
                        }
                    }

                    // Se número ímpar de intersecções, o ponto está dentro
                    return (interseccoesAntes % 2) == 1;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Analisa o tipo de suporte de um pilar
        /// </summary>
        public string AnalisarTipoSuportePilar(Element pilar)
        {
            if (!(pilar is FamilyInstance instanciaPilar))
                return "Elemento inválido";

            try
            {
                LocationPoint locPoint = instanciaPilar.Location as LocationPoint;
                if (locPoint == null) return "Localização inválida";

                XYZ pontoBase = locPoint.Point;
                XYZ pontoTeste = new XYZ(pontoBase.X, pontoBase.Y, pontoBase.Z - 0.1);

                // Verificar apoio em fundação
                string tipoFundacao = VerificarApoioFundacao(pontoTeste);
                if (tipoFundacao != "Não encontrado")
                {
                    return tipoFundacao;
                }

                // Verificar apoio em laje
                string tipoLaje = VerificarApoioLaje(pontoTeste);
                if (tipoLaje != "Não encontrado")
                {
                    return tipoLaje;
                }

                // Verificar apoio em viga
                string tipoViga = VerificarApoioViga(pontoBase);
                if (tipoViga != "Não encontrado")
                {
                    return tipoViga;
                }

                return "Suporte não identificado";
            }
            catch (Exception ex)
            {
                return $"Erro na análise: {ex.Message}";
            }
        }

        /// <summary>
        /// Verifica apoio em fundação
        /// </summary>
        private string VerificarApoioFundacao(XYZ pontoTeste)
        {
            try
            {
                FilteredElementCollector collectorFundacoes = new FilteredElementCollector(doc);
                var fundacoes = collectorFundacoes
                    .OfCategory(BuiltInCategory.OST_StructuralFoundation)
                    .OfClass(typeof(FamilyInstance))
                    .Cast<FamilyInstance>();

                foreach (FamilyInstance fundacao in fundacoes)
                {
                    BoundingBoxXYZ bbox = fundacao.get_BoundingBox(null);
                    if (bbox != null &&
                        pontoTeste.X >= bbox.Min.X && pontoTeste.X <= bbox.Max.X &&
                        pontoTeste.Y >= bbox.Min.Y && pontoTeste.Y <= bbox.Max.Y &&
                        pontoTeste.Z >= bbox.Min.Z && pontoTeste.Z <= bbox.Max.Z)
                    {
                        return "Fundação Directa";
                    }
                }

                return "Não encontrado";
            }
            catch
            {
                return "Não encontrado";
            }
        }

        /// <summary>
        /// Verifica apoio em laje
        /// </summary>
        private string VerificarApoioLaje(XYZ pontoTeste)
        {
            try
            {
                FilteredElementCollector collectorLajes = new FilteredElementCollector(doc);
                var lajes = collectorLajes
                    .OfCategory(BuiltInCategory.OST_Floors)
                    .OfClass(typeof(Floor))
                    .Cast<Floor>();

                foreach (Floor laje in lajes)
                {
                    if (PontoEstaEmLaje(pontoTeste, laje))
                    {
                        return "Laje Estrutural";
                    }
                }

                return "Não encontrado";
            }
            catch
            {
                return "Não encontrado";
            }
        }

        /// <summary>
        /// Verifica apoio em viga
        /// </summary>
        private string VerificarApoioViga(XYZ pontoBase)
        {
            try
            {
                FilteredElementCollector collectorVigas = new FilteredElementCollector(doc);
                var vigas = collectorVigas
                    .OfCategory(BuiltInCategory.OST_StructuralFraming)
                    .OfClass(typeof(FamilyInstance))
                    .Cast<FamilyInstance>();

                foreach (FamilyInstance viga in vigas)
                {
                    LocationCurve locCurve = viga.Location as LocationCurve;
                    if (locCurve != null)
                    {
                        Curve curve = locCurve.Curve;
                        IntersectionResult resultado = curve.Project(pontoBase);

                        if (resultado.Distance < 0.3) // 30cm de tolerância
                        {
                            return "Viga de Suporte";
                        }
                    }
                }

                return "Não encontrado";
            }
            catch
            {
                return "Não encontrado";
            }
        }

        // Métodos auxiliares privados

        /// <summary>
        /// Obtém todos os níveis ordenados por elevação
        /// </summary>
        private List<Level> ObterNiveisOrdenados()
        {
            try
            {
                FilteredElementCollector collectorNiveis = new FilteredElementCollector(doc);
                return collectorNiveis
                    .OfClass(typeof(Level))
                    .Cast<Level>()
                    .OrderBy(l => l.Elevation)
                    .ToList();
            }
            catch
            {
                return new List<Level>();
            }
        }

        /// <summary>
        /// Obtém o nível de um elemento
        /// </summary>
        private Level ObterNivelElemento(Element elemento)
        {
            try
            {
                Parameter paramNivel = elemento.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM) ??
                                      elemento.get_Parameter(BuiltInParameter.SCHEDULE_LEVEL_PARAM) ??
                                      elemento.get_Parameter(BuiltInParameter.LEVEL_PARAM);

                if (paramNivel != null)
                {
                    ElementId nivelId = paramNivel.AsElementId();
                    return doc.GetElement(nivelId) as Level;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Filtra elementos por designação específica
        /// </summary>
        public List<Element> FiltrarPorDesignacao(List<Element> elementos, string designacao)
        {
            if (string.IsNullOrEmpty(designacao) || designacao == "Todas as designações")
            {
                return elementos;
            }

            try
            {
                return elementos.Where(e =>
                {
                    string designacaoElemento = e.get_Parameter(BuiltInParameter.ELEM_FAMILY_PARAM)?.AsValueString() ??
                                               e.get_Parameter(BuiltInParameter.ELEM_TYPE_PARAM)?.AsValueString() ??
                                               "Sem designação";

                    return designacaoElemento.Equals(designacao, StringComparison.OrdinalIgnoreCase);
                }).ToList();
            }
            catch
            {
                return elementos;
            }
        }

        /// <summary>
        /// Filtra elementos por níveis específicos
        /// </summary>
        public List<Element> FiltrarPorNiveis(List<Element> elementos, List<string> nomesNiveis)
        {
            if (nomesNiveis == null || nomesNiveis.Count == 0)
            {
                return elementos;
            }

            try
            {
                return elementos.Where(e =>
                {
                    Level nivelElemento = ObterNivelElemento(e);
                    return nivelElemento != null && nomesNiveis.Contains(nivelElemento.Name);
                }).ToList();
            }
            catch
            {
                return elementos;
            }
        }

        /// <summary>
        /// Obtém todas as designações únicas de uma lista de elementos
        /// </summary>
        public List<string> ObterDesignacoes(List<Element> elementos)
        {
            HashSet<string> designacoes = new HashSet<string>();

            try
            {
                foreach (Element elemento in elementos)
                {
                    string designacao = elemento.get_Parameter(BuiltInParameter.ELEM_FAMILY_PARAM)?.AsValueString() ??
                                       elemento.get_Parameter(BuiltInParameter.ELEM_TYPE_PARAM)?.AsValueString() ??
                                       "Sem designação";

                    designacoes.Add(designacao);
                }
            }
            catch
            {
                // Em caso de erro, retornar lista com designação genérica
                designacoes.Add("Sem designação");
            }

            return designacoes.OrderBy(d => d).ToList();
        }

        /// <summary>
        /// Obtém todos os nomes de níveis de uma lista de elementos
        /// </summary>
        public List<string> ObterNomesNiveis(List<Element> elementos)
        {
            HashSet<string> nomesNiveis = new HashSet<string>();

            try
            {
                foreach (Element elemento in elementos)
                {
                    Level nivelElemento = ObterNivelElemento(elemento);
                    if (nivelElemento != null)
                    {
                        nomesNiveis.Add(nivelElemento.Name);
                    }
                }
            }
            catch
            {
                // Em caso de erro, não adicionar nada
            }

            return nomesNiveis.OrderBy(n => n).ToList();
        }
    }

    /// <summary>
    /// Informações de análise dos elementos estruturais
    /// </summary>
    public class InformacaoAnaliseElementos
    {
        public int TotalElementos { get; set; } = 0;
        public int ElementosFundacao { get; set; } = 0;
        public int ElementosUltimoPiso { get; set; } = 0;
        public int ElementosIntermedios { get; set; } = 0;
        public List<string> NiveisDetectados { get; set; } = new List<string>();

        /// <summary>
        /// Gera relatório textual da análise
        /// </summary>
        public string GerarRelatorio()
        {
            var relatorio = new System.Text.StringBuilder();

            relatorio.AppendLine($"Total de elementos: {TotalElementos}");
            relatorio.AppendLine($"Elementos de fundação: {ElementosFundacao}");
            relatorio.AppendLine($"Elementos de último piso: {ElementosUltimoPiso}");
            relatorio.AppendLine($"Elementos intermédios: {ElementosIntermedios}");
            relatorio.AppendLine($"Níveis detectados: {string.Join(", ", NiveisDetectados)}");

            return relatorio.ToString();
        }

        /// <summary>
        /// Valida se a análise tem dados consistentes
        /// </summary>
        public bool EValidaAnalise()
        {
            return TotalElementos == (ElementosFundacao + ElementosUltimoPiso + ElementosIntermedios) &&
                   TotalElementos >= 0;
        }
    }
}