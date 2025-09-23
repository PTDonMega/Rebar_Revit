using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rebar_Revit
{
    public static class Uteis
    {
        #region Convers�es de Unidades

        /// <summary>
        /// Converte p�s para cent�metros.
        /// </summary>
        public static double FeetParaCentimetros(double feet)
        {
            return feet * 30.48; // 1 p� = 30.48 cm
        }

        /// <summary>
        /// Converte p�s para mil�metros.
        /// </summary>
        public static double FeetParaMilimetros(double feet)
        {
            return feet * 304.8; // 1 p� = 304.8 mm
        }

        /// <summary>
        /// Converte mil�metros para p�s.
        /// </summary>
        public static double MilimetrosParaFeet(double milimetros)
        {
            return milimetros / 304.8; // 1 p� = 304.8 mm
        }

        /// <summary>
        /// Formatar valor em mil�metros para exibi��o
        /// </summary>
        public static string FormatarMilimetros(double millimeters)
        {
            return millimeters.ToString("F0");
        }

        #endregion

        #region Propriedades de Elementos

        /// <summary>
        /// Classe para propriedades geom�tricas de vigas
        /// </summary>
        public class PropriedadesViga
        {
            public double Comprimento { get; set; }
            public double Altura { get; set; }
            public double Largura { get; set; }
            public string Descricao { get; set; }
            public string Tipo { get; set; }
            public string Designacao { get; set; }
            public string Nivel { get; set; }
            public ElementId Id { get; set; }
        }

        /// <summary>
        /// Obter propriedades de uma viga
        /// </summary>
        public static PropriedadesViga ObterPropriedadesViga(Element elemento, Document doc)
        {
            try
            {
                if (!(elemento is FamilyInstance fi)) return null;

                var props = new PropriedadesViga
                {
                    Id = elemento.Id,
                    Descricao = ObterDescricaoElemento(elemento),
                    Tipo = ObterTipoDoElemento(elemento),
                    Nivel = ObterNomeNivel(elemento, doc),
                    Designacao = ObterDesignacao(elemento)
                };

                // Tentar obter dimens�es da geometria
                BoundingBoxXYZ bbox = elemento.get_BoundingBox(null);
                if (bbox != null)
                {
                    double comp = FeetParaMilimetros(Math.Abs(bbox.Max.X - bbox.Min.X));
                    double alt = FeetParaMilimetros(Math.Abs(bbox.Max.Z - bbox.Min.Z));
                    double larg = FeetParaMilimetros(Math.Abs(bbox.Max.Y - bbox.Min.Y));

                    // Determinar qual � comprimento (maior dimens�o horizontal)
                    if (comp < larg)
                    {
                        props.Comprimento = larg;
                        props.Largura = comp;
                    }
                    else
                    {
                        props.Comprimento = comp;
                        props.Largura = larg;
                    }
                    props.Altura = alt;
                }

                // Tentar melhorar com par�metros
                TentarObterDimensoesDosParametros(fi, props);

                return props;
            }
            catch
            {
                return null;
            }
        }

        private static string ObterDesignacao(Element elemento)
        {
            try
            {
                // Par�metro de texto custom "Designacao" na inst�ncia
                var p = (elemento as Element)?.LookupParameter("Designacao");
                if (p != null)
                {
                    string val = p.AsString();
                    if (!string.IsNullOrWhiteSpace(val)) return val;
                }
                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        private static void TentarObterDimensoesDosParametros(FamilyInstance fi, PropriedadesViga props)
        {
            try
            {
                // Tentar par�metros de largura
                var paramLargura = fi.get_Parameter(BuiltInParameter.FAMILY_WIDTH_PARAM) ??
                                  fi.LookupParameter("Width") ??
                                  fi.LookupParameter("b") ??
                                  fi.LookupParameter("Largura");

                if (paramLargura != null && paramLargura.HasValue)
                {
                    props.Largura = FeetParaMilimetros(paramLargura.AsDouble());
                }

                // Tentar par�metros de altura
                var paramAltura = fi.get_Parameter(BuiltInParameter.FAMILY_HEIGHT_PARAM) ??
                                 fi.LookupParameter("Height") ??
                                 fi.LookupParameter("h") ??
                                 fi.LookupParameter("Altura");

                if (paramAltura != null && paramAltura.HasValue)
                {
                    props.Altura = FeetParaMilimetros(paramAltura.AsDouble());
                }

                // Tentar par�metros de comprimento
                var paramComprimento = fi.get_Parameter(BuiltInParameter.INSTANCE_LENGTH_PARAM) ??
                                      fi.LookupParameter("Length") ??
                                      fi.LookupParameter("Comprimento");

                if (paramComprimento != null && paramComprimento.HasValue)
                {
                    props.Comprimento = FeetParaMilimetros(paramComprimento.AsDouble());
                }
            }
            catch
            {
                // Ignorar erros
            }
        }

        /// <summary>
        /// Obter descri��o/nome de um elemento (Fam�lia + Tipo)
        /// </summary>
        public static string ObterDescricaoElemento(Element elemento)
        {
            try
            {
                // Tentar Family + Type
                if (elemento is FamilyInstance fi && fi.Symbol != null)
                {
                    string familia = fi.Symbol.FamilyName ?? "";
                    string tipo = fi.Symbol.Name ?? "";
                    
                    if (!string.IsNullOrEmpty(familia) && !string.IsNullOrEmpty(tipo))
                    {
                        return $"{familia} ({tipo})";
                    }
                    if (!string.IsNullOrEmpty(familia))
                    {
                        return familia;
                    }
                }

                // Fallback para ElementType
                ElementType elementType = elemento.Document.GetElement(elemento.GetTypeId()) as ElementType;
                if (elementType != null)
                {
                    return elementType.Name;
                }

                return "Viga sem identifica��o";
            }
            catch
            {
                return "Erro na identifica��o";
            }
        }

        /// <summary>
        /// Obter apenas o nome do Type do elemento
        /// </summary>
        public static string ObterTipoDoElemento(Element elemento)
        {
            try
            {
                if (elemento is FamilyInstance fi && fi.Symbol != null)
                {
                    return fi.Symbol.Name ?? "(Sem Tipo)";
                }

                ElementType elementType = elemento.Document.GetElement(elemento.GetTypeId()) as ElementType;
                return elementType?.Name ?? "(Sem Tipo)";
            }
            catch
            {
                return "(Sem Tipo)";
            }
        }

        /// <summary>
        /// Obter nome do n�vel de um elemento
        /// </summary>
        public static string ObterNomeNivel(Element elemento, Document doc)
        {
            try
            {
                Parameter paramNivel = elemento.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM) ??
                                      elemento.get_Parameter(BuiltInParameter.SCHEDULE_LEVEL_PARAM);

                if (paramNivel != null)
                {
                    Element nivel = doc.GetElement(paramNivel.AsElementId());
                    if (nivel != null)
                    {
                        return nivel.Name;
                    }
                }

                return "Sem N�vel";
            }
            catch
            {
                return "Sem N�vel";
            }
        }

        #endregion

        #region Detec��o de Elementos

        /// <summary>
        /// Detectar vigas no documento
        /// </summary>
        public static List<Element> DetectarVigas(Document doc)
        {
            try
            {
                var collector = new FilteredElementCollector(doc)
                    .OfCategory(BuiltInCategory.OST_StructuralFraming)
                    .WhereElementIsNotElementType();

                return collector.ToList();
            }
            catch
            {
                return new List<Element>();
            }
        }

        /// <summary>
        /// Agrupar vigas pelo Type (s�mbolo)
        /// </summary>
        public static Dictionary<string, List<Element>> AgruparVigasPorTipo(List<Element> vigas, Document doc)
        {
            var grupos = new Dictionary<string, List<Element>>();

            foreach (var viga in vigas)
            {
                string tipo = ObterTipoDoElemento(viga);
                if (!grupos.ContainsKey(tipo))
                {
                    grupos[tipo] = new List<Element>();
                }
                grupos[tipo].Add(viga);
            }

            return grupos;
        }

        /// <summary>
        /// Agrupar vigas por descri��o (mantido por compatibilidade)
        /// </summary>
        public static Dictionary<string, List<Element>> AgruparVigasPorDescricao(List<Element> vigas, Document doc)
        {
            var grupos = new Dictionary<string, List<Element>>();

            foreach (var viga in vigas)
            {
                string descricao = ObterDescricaoElemento(viga);
                
                if (!grupos.ContainsKey(descricao))
                {
                    grupos[descricao] = new List<Element>();
                }
                grupos[descricao].Add(viga);
            }

            return grupos;
        }

        /// <summary>
        /// Verificar se elemento � uma viga estrutural v�lida
        /// </summary>
        public static bool EVigaEstruturalValida(Element elemento)
        {
            try
            {
                if (elemento == null) return false;
                if (elemento.Category?.Id.IntegerValue != (int)BuiltInCategory.OST_StructuralFraming) return false;
                if (!(elemento is FamilyInstance)) return false;

                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Valida��es

        /// <summary>
        /// Validar se lista de elementos tem vigas v�lidas
        /// </summary>
        public static List<Element> FiltrarVigasValidas(List<Element> elementos)
        {
            return elementos.Where(EVigaEstruturalValida).ToList();
        }

        /// <summary>
        /// Obter lista de di�metros padr�o
        /// </summary>
        public static double[] ObterDiametrosPadrao()
        {
            return new double[] { 8, 10, 12, 16, 20, 25, 32 };
        }

        #endregion

        #region Geometria e Pontos (existentes)

        /// <summary>
        /// Verifica se dois pontos s�o quase iguais.
        /// </summary>
        public static bool PontosSaoQuaseIguais(XYZ ponto1, XYZ ponto2, double tolerancia = 1e-6)
        {
            return ponto1.IsAlmostEqualTo(ponto2, tolerancia);
        }

        /// <summary>
        /// Valida se um ponto XYZ � v�lido.
        /// </summary>
        public static bool PontoEhValido(XYZ ponto)
        {
            return ponto != null &&
                   !double.IsNaN(ponto.X) && !double.IsNaN(ponto.Y) && !double.IsNaN(ponto.Z) &&
                   !double.IsInfinity(ponto.X) && !double.IsInfinity(ponto.Y) && !double.IsInfinity(ponto.Z);
        }

        /// <summary>
        /// Limpa e valida uma lista de pontos XYZ.
        /// </summary>
        public static List<XYZ> ValidarELimparPontos(List<XYZ> pontos, double tolerancia = 1e-3)
        {
            List<XYZ> pontosLimpos = new List<XYZ>();

            foreach (XYZ ponto in pontos)
            {
                if (PontoEhValido(ponto) &&
                    (pontosLimpos.Count == 0 || !PontosSaoQuaseIguais(ponto, pontosLimpos.Last(), tolerancia)))
                {
                    pontosLimpos.Add(ponto);
                }
            }

            return pontosLimpos;
        }

        /// <summary>
        /// Obt�m o vetor normal seguro para curvas.
        /// </summary>
        public static XYZ DeterminarVetorNormalSeguro(List<Curve> curvas)
        {
            if (curvas.Count > 0)
            {
                XYZ direcaoCurva = (curvas[0].GetEndPoint(1) - curvas[0].GetEndPoint(0)).Normalize();

                XYZ vetorTeste1 = XYZ.BasisZ.CrossProduct(direcaoCurva);
                if (!vetorTeste1.IsAlmostEqualTo(XYZ.Zero, 1e-6))
                {
                    return vetorTeste1.Normalize();
                }

                XYZ vetorTeste2 = XYZ.BasisY.CrossProduct(direcaoCurva);
                if (!vetorTeste2.IsAlmostEqualTo(XYZ.Zero, 1e-6))
                {
                    return vetorTeste2.Normalize();
                }
            }

            return XYZ.BasisX;
        }

        /// <summary>
        /// Converte uma lista de pontos locais para globais usando uma transforma��o.
        /// </summary>
        public static List<XYZ> ConverterPontosParaGlobais(List<XYZ> pontosLocais, Transform transform)
        {
            return pontosLocais.Select(p => transform.OfPoint(p)).ToList();
        }

        #endregion

        #region Designacao de Vigas

        /// <summary>
        /// Obter valor do par�metro "Designacao" de uma viga
        /// </summary>
        public static string ObterDesignacaoViga(Element elemento, Document doc)
        {
            try
            {
                var param = elemento.LookupParameter("Designacao");
                if (param != null)
                {
                    string val = param.AsString();
                    if (!string.IsNullOrWhiteSpace(val)) return val;
                }
                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        #endregion
    }
}