using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rebar_Revit
{
    /// <summary>
    /// Configuração e execução de armadura especializada para Vigas
    /// </summary>
    public class ArmConfigExec
    {
        // Configuração principal
        public List<ArmVar> Varoes { get; set; } = new List<ArmVar>();
        public List<ArmStirrup> Estribos { get; set; } = new List<ArmStirrup>();
        public TipoDistribuicaoArmaduraEnum TipoDistribuicao { get; set; } = TipoDistribuicaoArmaduraEnum.MistaComMaioresNasBordas;
        public bool AmarracaoAuto { get; set; } = true;
        public double MultAmarracao { get; set; } = 50; // Padrão para vigas
        public string TipoAmarracao { get; set; } = "Automático";
        public TipoElementoEstruturalEnum TipoElemento { get; set; }
        public DefinicoesProjectoAvancadas Defs { get; set; }

        private Document doc;
        private CalculadorAmarracao calcAmarracao;
        private DetectorElementosAvancado detector;

        public ArmConfigExec(Document documento)
        {
            doc = documento;
            calcAmarracao = new CalculadorAmarracao();
            detector = new DetectorElementosAvancado(documento);
            Defs = new DefinicoesProjectoAvancadas();
        }

        public int QtdTotalVaroes() => Varoes.Sum(v => v.Quantidade);

        public bool ColocarArmadura(Element elemento)
        {
            try
            {
                if (TipoElemento == TipoElementoEstruturalEnum.Vigas)
                {
                    return ColocarArmaduraViga(elemento);
                }
                
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro na colocação de armadura: {ex.Message}");
            }
        }

        private bool ColocarArmaduraViga(Element viga)
        {
            FamilyInstance inst = viga as FamilyInstance;
            if (inst == null) 
            {
                throw new Exception("Elemento não é uma FamilyInstance válida");
            }

            try
            {
                // Informações da viga para debug
                string familyName = inst.Symbol?.FamilyName ?? "N/A";
                string typeName = inst.Symbol?.Name ?? "N/A";
                
                // Obter propriedades da viga
                var propriedades = ObterPropriedadesViga(inst);
                if (propriedades == null) 
                {
                    throw new Exception($"Não foi possível obter as propriedades geométricas da viga {familyName} - {typeName}");
                }

                double cobertura = (Defs?.CoberturaVigas ?? 25) / 304.8; // Converter mm para pés

                // Verificar se existem configurações de armadura
                if (Varoes.Count == 0 && Estribos.Count == 0)
                {
                    throw new Exception("Nenhuma configuração de armadura definida");
                }

                // Verificar se existem tipos de armadura disponíveis no projeto
                var tiposDisponiveis = ObterTiposArmaduraDisponiveis();
                if (tiposDisponiveis.Count == 0)
                {
                    throw new Exception("Nenhum tipo de armadura (RebarBarType) encontrado no projeto. Carregue um template de armaduras ou crie tipos de armadura no projeto.");
                }

                // Validação dimensional crítica
                if (!ValidarDimensoesCriticas(propriedades, cobertura))
                {
                    string dimensoesInfo = $"Dimensões: L={propriedades.Comprimento * 304.8:F0}mm, " +
                                          $"B={propriedades.Largura * 304.8:F0}mm, " +
                                          $"H={propriedades.Altura * 304.8:F0}mm, " +
                                          $"Cobertura={cobertura * 304.8:F0}mm";
                    throw new Exception($"Dimensões insuficientes para colocação de armadura. {dimensoesInfo}");
                }

                bool sucessoLongitudinal = true;
                bool sucessoEstribos = true;

                // Criar armadura longitudinal (superior, inferior, lateral)
                if (Varoes.Count > 0)
                {
                    try
                    {
                        sucessoLongitudinal = CriarArmaduraVigaLongitudinal(inst, propriedades, cobertura);
                        if (!sucessoLongitudinal)
                        {
                            throw new Exception("Falha na criação de armadura longitudinal");
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Erro na armadura longitudinal: {ex.Message}");
                    }
                }

                // Criar estribos da viga
                if (Estribos.Count > 0)
                {
                    try
                    {
                        sucessoEstribos = CriarEstribosViga(inst, propriedades, cobertura);
                        if (!sucessoEstribos)
                        {
                            throw new Exception("Falha na criação de estribos");
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Erro nos estribos: {ex.Message}");
                    }
                }

                return sucessoLongitudinal && sucessoEstribos;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro na viga {viga.Id} ({inst.Symbol?.FamilyName} - {inst.Symbol?.Name}): {ex.Message}");
            }
        }

        private bool ValidarDimensoesCriticas(PropriedadesViga props, double cobertura)
        {
            if (props == null) return false;
            
            double larguraMinima = 50 / 304.8; // 5cm em pés
            double alturaMinima = 100 / 304.8; // 10cm em pés
            double comprimentoMinimo = 300 / 304.8; // 30cm em pés
            
            if (props.Largura < larguraMinima ||
                props.Altura < alturaMinima ||
                props.Comprimento < comprimentoMinimo)
            {
                return false;
            }

            double larguraUtil = props.Largura - 2 * cobertura;
            double alturaUtil = props.Altura - 2 * cobertura;

            if (larguraUtil <= 20 / 304.8 || alturaUtil <= 20 / 304.8) 
            {
                return false;
            }

            if (props.PontoInicial == null || props.PontoFinal == null ||
                props.PontoInicial.IsAlmostEqualTo(props.PontoFinal))
            {
                return false;
            }

            return true;
        }

        private PropriedadesViga ObterPropriedadesViga(FamilyInstance inst)
        {
            try
            {
                LocationCurve locCurve = inst.Location as LocationCurve;
                if (locCurve == null) 
                {
                    throw new Exception("Viga não possui LocationCurve válida");
                }

                Curve curvaViga = locCurve.Curve;
                double comprimento = curvaViga.Length;

                double altura = 0;
                double largura = 0;

                // Tentar obter dimensões da instância
                Parameter paramAlturaInst = inst.LookupParameter("Altura") ??
                                           inst.LookupParameter("Height");

                Parameter paramLarguraInst = inst.LookupParameter("Largura") ??
                                            inst.LookupParameter("Width");

                if (paramAlturaInst != null && paramAlturaInst.AsDouble() > 0)
                {
                    altura = paramAlturaInst.AsDouble();
                }

                if (paramLarguraInst != null && paramLarguraInst.AsDouble() > 0)
                {
                    largura = paramLarguraInst.AsDouble();
                }

                // Se não encontrou nas Properties da instância, buscar no tipo
                if (altura <= 0 || largura <= 0)
                {
                    ElementType elementType = doc.GetElement(inst.GetTypeId()) as ElementType;
                    if (elementType != null)
                    {
                        if (altura <= 0)
                        {
                            var alturaFamilia = elementType.LookupParameter("By") ??
                                              elementType.LookupParameter("Altura") ??
                                              elementType.LookupParameter("Height") ??
                                              elementType.get_Parameter(BuiltInParameter.FAMILY_HEIGHT_PARAM);
                            
                            if (alturaFamilia != null && alturaFamilia.AsDouble() > 0)
                            {
                                altura = alturaFamilia.AsDouble();
                            }
                        }

                        if (largura <= 0)
                        {
                            var larguraFamilia = elementType.LookupParameter("Bx") ??
                                               elementType.LookupParameter("Largura") ??
                                               elementType.LookupParameter("Width") ??
                                               elementType.get_Parameter(BuiltInParameter.FAMILY_WIDTH_PARAM);
                            
                            if (larguraFamilia != null && larguraFamilia.AsDouble() > 0)
                            {
                                largura = larguraFamilia.AsDouble();
                            }
                        }
                    }
                }

                // Verificar se encontrou todas as dimensões
                if (comprimento <= 0)
                {
                    throw new Exception($"Comprimento inválido: {comprimento * 304.8:F0}mm");
                }

                if (altura <= 0)
                {
                    throw new Exception("Altura não encontrada nos parâmetros da viga");
                }

                if (largura <= 0)
                {
                    throw new Exception("Largura não encontrada nos parâmetros da viga");
                }

                return new PropriedadesViga
                {
                    Comprimento = comprimento,
                    Altura = altura,
                    Largura = largura,
                    CurvaEixo = curvaViga,
                    PontoInicial = curvaViga.GetEndPoint(0),
                    PontoFinal = curvaViga.GetEndPoint(1)
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter propriedades da viga: {ex.Message}");
            }
        }

        private bool CriarArmaduraVigaLongitudinal(FamilyInstance elemento, PropriedadesViga props, double cobertura)
        {
            try
            {
                var tiposArmadura = ObterTiposArmaduraDisponiveis();
                if (tiposArmadura.Count == 0) return false;

                foreach (var varao in Varoes)
                {
                    var tipoVarao = tiposArmadura.FirstOrDefault(t =>
                        t.Name.Contains(varao.Diametro.ToString("F0"))) ?? tiposArmadura.First();

                    string tipoLower = varao.TipoArmadura.ToLower();
                    
                    if (tipoLower == "superior")
                    {
                        if (!CriarArmaduraSuperior(elemento, props, varao, tipoVarao, cobertura))
                            return false;
                    }
                    else if (tipoLower == "inferior")
                    {
                        if (!CriarArmaduraInferior(elemento, props, varao, tipoVarao, cobertura))
                            return false;
                    }
                    else if (tipoLower == "lateral")
                    {
                        if (!CriarArmaduraLateral(elemento, props, varao, tipoVarao, cobertura))
                            return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro na criação de armadura longitudinal: {ex.Message}");
            }
        }

        private bool CriarArmaduraSuperior(FamilyInstance elemento, PropriedadesViga props, 
                                         ArmVar varao, RebarBarType tipoVarao, double cobertura)
        {
            try
            {
                if (varao.Quantidade <= 0) return true;

                Transform transformViga = ObterTransformViga(props);
                double larguraUtil = props.Largura - 2 * cobertura;
                double espacamento = varao.Quantidade > 1 ? larguraUtil / (varao.Quantidade - 1) : 0;

                for (int i = 0; i < varao.Quantidade; i++)
                {
                    double offsetY = varao.Quantidade == 1 ? 0 : -larguraUtil / 2 + (i * espacamento);
                    double alturaZ = props.Altura - cobertura;

                    XYZ pontoInicialLocal = new XYZ(0, offsetY, alturaZ);
                    XYZ pontoFinalLocal = new XYZ(props.Comprimento, offsetY, alturaZ);

                    XYZ pontoInicial = transformViga.OfPoint(pontoInicialLocal);
                    XYZ pontoFinal = transformViga.OfPoint(pontoFinalLocal);

                    List<XYZ> pontosComAmarracao = CalcularPontosAmarracao(pontoInicial, pontoFinal, varao.Diametro, "superior");

                    if (!CriarArmaduraIndividual(elemento, pontosComAmarracao, tipoVarao, varao.Diametro))
                        return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro na armadura superior: {ex.Message}");
            }
        }

        private bool CriarArmaduraInferior(FamilyInstance elemento, PropriedadesViga props,
                                         ArmVar varao, RebarBarType tipoVarao, double cobertura)
        {
            try
            {
                if (varao.Quantidade <= 0) return true;

                Transform transformViga = ObterTransformViga(props);
                double larguraUtil = props.Largura - 2 * cobertura;
                double espacamento = varao.Quantidade > 1 ? larguraUtil / (varao.Quantidade - 1) : 0;

                for (int i = 0; i < varao.Quantidade; i++)
                {
                    double offsetY = varao.Quantidade == 1 ? 0 : -larguraUtil / 2 + (i * espacamento);
                    double alturaZ = cobertura;

                    XYZ pontoInicialLocal = new XYZ(0, offsetY, alturaZ);
                    XYZ pontoFinalLocal = new XYZ(props.Comprimento, offsetY, alturaZ);

                    XYZ pontoInicial = transformViga.OfPoint(pontoInicialLocal);
                    XYZ pontoFinal = transformViga.OfPoint(pontoFinalLocal);

                    List<XYZ> pontosComAmarracao = CalcularPontosAmarracao(pontoInicial, pontoFinal, varao.Diametro, "inferior");

                    if (!CriarArmaduraIndividual(elemento, pontosComAmarracao, tipoVarao, varao.Diametro))
                        return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro na armadura inferior: {ex.Message}");
            }
        }

        private bool CriarArmaduraLateral(FamilyInstance elemento, PropriedadesViga props,
                                        ArmVar varao, RebarBarType tipoVarao, double cobertura)
        {
            try
            {
                if (varao.Quantidade <= 0) return true;

                Transform transformViga = ObterTransformViga(props);
                double alturaUtil = props.Altura - 2 * cobertura;
                double espacamento = varao.Quantidade > 1 ? alturaUtil / (varao.Quantidade - 1) : 0;

                for (int i = 0; i < varao.Quantidade; i++)
                {
                    double alturaZ = varao.Quantidade == 1 ? props.Altura / 2 : cobertura + (i * espacamento);

                    // Lado esquerdo
                    double offsetYEsq = -(props.Largura / 2 - cobertura);
                    XYZ pontoInicialEsqLocal = new XYZ(0, offsetYEsq, alturaZ);
                    XYZ pontoFinalEsqLocal = new XYZ(props.Comprimento, offsetYEsq, alturaZ);

                    XYZ pontoInicialEsq = transformViga.OfPoint(pontoInicialEsqLocal);
                    XYZ pontoFinalEsq = transformViga.OfPoint(pontoFinalEsqLocal);

                    List<XYZ> pontosEsq = CalcularPontosAmarracao(pontoInicialEsq, pontoFinalEsq, varao.Diametro, "lateral");
                    if (!CriarArmaduraIndividual(elemento, pontosEsq, tipoVarao, varao.Diametro))
                        return false;

                    // Lado direito
                    double offsetYDir = props.Largura / 2 - cobertura;
                    XYZ pontoInicialDirLocal = new XYZ(0, offsetYDir, alturaZ);
                    XYZ pontoFinalDirLocal = new XYZ(props.Comprimento, offsetYDir, alturaZ);

                    XYZ pontoInicialDir = transformViga.OfPoint(pontoInicialDirLocal);
                    XYZ pontoFinalDir = transformViga.OfPoint(pontoFinalDirLocal);

                    List<XYZ> pontosDir = CalcularPontosAmarracao(pontoInicialDir, pontoFinalDir, varao.Diametro, "lateral");
                    if (!CriarArmaduraIndividual(elemento, pontosDir, tipoVarao, varao.Diametro))
                        return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro na armadura lateral: {ex.Message}");
            }
        }

        private Transform ObterTransformViga(PropriedadesViga props)
        {
            try
            {
                XYZ direcaoViga = (props.PontoFinal - props.PontoInicial).Normalize();
                XYZ eixoZ = XYZ.BasisZ;
                XYZ eixoY = eixoZ.CrossProduct(direcaoViga).Normalize();
                
                if (eixoY.IsAlmostEqualTo(XYZ.Zero))
                {
                    eixoY = XYZ.BasisY;
                    eixoZ = direcaoViga.CrossProduct(eixoY).Normalize();
                }

                Transform transform = Transform.Identity;
                transform.Origin = props.PontoInicial;
                transform.BasisX = direcaoViga;
                transform.BasisY = eixoY;
                transform.BasisZ = eixoZ;

                return transform;
            }
            catch
            {
                Transform fallback = Transform.Identity;
                fallback.Origin = props.PontoInicial;
                return fallback;
            }
        }

        private List<XYZ> CalcularPontosAmarracao(XYZ pontoInicial, XYZ pontoFinal, double diametro, string posicao)
        {
            try
            {
                if (pontoInicial == null || pontoFinal == null)
                {
                    throw new Exception("Pontos inicial ou final são nulos");
                }

                if (pontoInicial.IsAlmostEqualTo(pontoFinal, 1e-6))
                {
                    throw new Exception("Pontos inicial e final são coincidentes");
                }

                List<XYZ> pontos = new List<XYZ>();

                if (AmarracaoAuto)
                {
                    double comprimentoAmarracao = (MultAmarracao * diametro) / 304.8;
                    
                    if (comprimentoAmarracao <= 0)
                    {
                        pontos.Add(pontoInicial);
                        pontos.Add(pontoFinal);
                        return pontos;
                    }

                    TipoAmarracaoEnum tipoAmarracao = TipoAmarracaoEnum.Reta;
                    
                    if (posicao == "superior")
                    {
                        tipoAmarracao = TipoAmarracaoEnum.Dobrada90;
                    }

                    try
                    {
                        pontos = calcAmarracao.CalcularPontosAncoragem(pontoInicial, pontoFinal, tipoAmarracao, comprimentoAmarracao);
                        
                        if (pontos == null || pontos.Count < 2)
                        {
                            pontos = new List<XYZ> { pontoInicial, pontoFinal };
                        }
                    }
                    catch
                    {
                        pontos = new List<XYZ> { pontoInicial, pontoFinal };
                    }
                }
                else
                {
                    pontos.Add(pontoInicial);
                    pontos.Add(pontoFinal);
                }

                return pontos;
            }
            catch
            {
                return new List<XYZ> { pontoInicial, pontoFinal };
            }
        }

        private bool CriarArmaduraIndividual(FamilyInstance elemento, List<XYZ> pontos, 
                                           RebarBarType tipoArmadura, double diametro)
        {
            try
            {
                if (pontos == null || pontos.Count < 2)
                {
                    throw new Exception("Pontos insuficientes para criar armadura");
                }

                List<XYZ> pontosValidados = ValidarELimparPontos(pontos);
                if (pontosValidados.Count < 2)
                {
                    throw new Exception("Pontos validados insuficientes");
                }

                List<Curve> curvas = new List<Curve>();
                for (int i = 0; i < pontosValidados.Count - 1; i++)
                {
                    double distancia = pontosValidados[i].DistanceTo(pontosValidados[i + 1]);
                    if (distancia > 1e-3) // 1mm mínimo
                    {
                        if (!pontosValidados[i].IsAlmostEqualTo(pontosValidados[i + 1], 1e-3))
                        {
                            Line linha = Line.CreateBound(pontosValidados[i], pontosValidados[i + 1]);
                            curvas.Add(linha);
                        }
                    }
                }

                if (curvas.Count == 0)
                {
                    throw new Exception("Nenhuma curva válida criada");
                }

                XYZ vetorNormal = DeterminarVetorNormalSeguro(curvas, elemento);

                // Tentar sem ganchos primeiro
                Rebar armadura = Rebar.CreateFromCurves(
                    doc, 
                    RebarStyle.Standard, 
                    tipoArmadura,
                    null, // sem gancho
                    null, // sem gancho
                    elemento,
                    vetorNormal, 
                    curvas,
                    RebarHookOrientation.Right, 
                    RebarHookOrientation.Right,
                    true, 
                    true);

                if (armadura != null)
                {
                    DefinirPropriedadesArmadura(armadura, diametro);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro na criação de armadura Ø{diametro}mm: {ex.Message}");
            }
        }

        private List<XYZ> ValidarELimparPontos(List<XYZ> pontos)
        {
            List<XYZ> pontosLimpos = new List<XYZ>();
            
            foreach (XYZ ponto in pontos)
            {
                if (ponto != null && 
                    !double.IsNaN(ponto.X) && !double.IsNaN(ponto.Y) && !double.IsNaN(ponto.Z) &&
                    !double.IsInfinity(ponto.X) && !double.IsInfinity(ponto.Y) && !double.IsInfinity(ponto.Z))
                {
                    if (pontosLimpos.Count == 0 || 
                        !ponto.IsAlmostEqualTo(pontosLimpos.Last(), 1e-3))
                    {
                        pontosLimpos.Add(ponto);
                    }
                }
            }

            return pontosLimpos;
        }

        private XYZ DeterminarVetorNormalSeguro(List<Curve> curvas, FamilyInstance elemento)
        {
            try
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
            catch
            {
                return XYZ.BasisX;
            }
        }

        private void DefinirPropriedadesArmadura(Rebar armadura, double diametro)
        {
            try
            {
                if (armadura != null)
                {
                    string comentario = $"Ø{diametro}mm - Amarração: {MultAmarracao}Ø";
                    var paramComentario = armadura.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS);
                    paramComentario?.Set(comentario);
                }
            }
            catch
            {
                // Ignorar erros
            }
        }

        private bool CriarEstribosViga(FamilyInstance elemento, PropriedadesViga props, double cobertura)
        {
            try
            {
                if (Estribos.Count == 0) return true;

                foreach (var estribo in Estribos)
                {
                    var tipoEstribo = ObterTipoArmaduraPorDiametro(estribo.Diametro);
                    if (tipoEstribo == null) continue;

                    double espacamento = estribo.Espacamento / 304.8;
                    int numeroEstribos = Math.Max(1, (int)(props.Comprimento / espacamento) - 1);

                    Transform transformViga = ObterTransformViga(props);

                    for (int i = 1; i <= numeroEstribos; i++)
                    {
                        double posicaoX = i * espacamento;
                        
                        if (posicaoX >= props.Comprimento) break;

                        CriarEstriboVigaIndividual(elemento, posicaoX, props, tipoEstribo, cobertura, transformViga);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro na criação de estribos: {ex.Message}");
            }
        }

        private bool CriarEstriboVigaIndividual(FamilyInstance elemento, double posicaoX, 
                                              PropriedadesViga props, RebarBarType tipoEstribo, 
                                              double cobertura, Transform transformViga)
        {
            try
            {
                double larguraUtil = props.Largura - 2 * cobertura;
                double alturaUtil = props.Altura - 2 * cobertura;

                if (larguraUtil <= 0 || alturaUtil <= 0) return false;

                double y1 = -larguraUtil / 2;
                double y2 = larguraUtil / 2;
                double z1 = cobertura;
                double z2 = props.Altura - cobertura;

                List<XYZ> pontosLocais = new List<XYZ>
                {
                    new XYZ(posicaoX, y1, z1),
                    new XYZ(posicaoX, y2, z1),
                    new XYZ(posicaoX, y2, z2),
                    new XYZ(posicaoX, y1, z2),
                    new XYZ(posicaoX, y1, z1)
                };

                List<XYZ> pontosGlobais = pontosLocais.Select(p => transformViga.OfPoint(p)).ToList();

                List<Curve> curvasEstribo = new List<Curve>();
                for (int i = 0; i < pontosGlobais.Count - 1; i++)
                {
                    double distancia = pontosGlobais[i].DistanceTo(pontosGlobais[i + 1]);
                    if (distancia > 1e-6)
                    {
                        Line linha = Line.CreateBound(pontosGlobais[i], pontosGlobais[i + 1]);
                        curvasEstribo.Add(linha);
                    }
                }

                if (curvasEstribo.Count < 3) return false;

                XYZ vetorNormal = (props.PontoFinal - props.PontoInicial).Normalize();

                Rebar estriboCriado = Rebar.CreateFromCurves(
                    doc, 
                    RebarStyle.StirrupTie, 
                    tipoEstribo,
                    null, // sem gancho
                    null, // sem gancho
                    elemento,
                    vetorNormal, 
                    curvasEstribo,
                    RebarHookOrientation.Right, 
                    RebarHookOrientation.Right,
                    true, 
                    true);

                return estriboCriado != null;
            }
            catch
            {
                return false;
            }
        }

        private List<RebarBarType> ObterTiposArmaduraDisponiveis()
        {
            var collector = new FilteredElementCollector(doc);
            return collector.OfClass(typeof(RebarBarType))
                          .Cast<RebarBarType>()
                          .OrderBy(t => t.Name)
                          .ToList();
        }

        private RebarBarType ObterTipoArmaduraPorDiametro(double diametro)
        {
            var tipos = ObterTiposArmaduraDisponiveis();
            string diametroStr = diametro.ToString("F0");

            return tipos.FirstOrDefault(t => t.Name.Contains(diametroStr)) ??
                   tipos.FirstOrDefault();
        }

        public string GerarRelatorioPreVisualizacao(List<Element> elementos)
        {
            var relatorio = new System.Text.StringBuilder();

            relatorio.AppendLine("=== RELATÓRIO DE PRÉ-VISUALIZAÇÃO - VIGAS ===");
            relatorio.AppendLine($"Data/Hora: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
            relatorio.AppendLine($"Total de Vigas: {elementos.Count}");
            relatorio.AppendLine();

            relatorio.AppendLine("ARMADURA LONGITUDINAL:");
            relatorio.AppendLine($"- Quantidade total de varões: {QtdTotalVaroes()}");

            var armaduraSuperior = Varoes.Where(v => v.TipoArmadura.ToLower() == "superior").ToList();
            var armaduraInferior = Varoes.Where(v => v.TipoArmadura.ToLower() == "inferior").ToList();
            var armaduraLateral = Varoes.Where(v => v.TipoArmadura.ToLower() == "lateral").ToList();

            if (armaduraSuperior.Any())
            {
                relatorio.AppendLine("  SUPERIOR:");
                foreach (var varao in armaduraSuperior)
                    relatorio.AppendLine($"    * {varao.Quantidade}Ø{varao.Diametro}mm");
            }

            if (armaduraInferior.Any())
            {
                relatorio.AppendLine("  INFERIOR:");
                foreach (var varao in armaduraInferior)
                    relatorio.AppendLine($"    * {varao.Quantidade}Ø{varao.Diametro}mm");
            }

            if (armaduraLateral.Any())
            {
                relatorio.AppendLine("  LATERAL:");
                foreach (var varao in armaduraLateral)
                    relatorio.AppendLine($"    * {varao.Quantidade}Ø{varao.Diametro}mm");
            }

            relatorio.AppendLine();
            relatorio.AppendLine("ESTRIBOS:");
            foreach (var estribo in Estribos)
            {
                string tipo = estribo.Alternado ? " (Alternado)" : " (Uniforme)";
                relatorio.AppendLine($"- Ø{estribo.Diametro}mm // {estribo.Espacamento}mm{tipo}");
            }

            return relatorio.ToString();
        }
    }

    /// <summary>
    /// Classe para representar um varão individual para vigas
    /// </summary>
    public class ArmVar
    {
        public int Quantidade { get; set; }
        public double Diametro { get; set; }
        public string TipoArmadura { get; set; } = "Superior";

        public ArmVar(int quantidade, double diametro)
        {
            Quantidade = quantidade;
            Diametro = diametro;
        }

        public override string ToString() => $"{Quantidade}Ø{Diametro}mm ({TipoArmadura})";
    }

    /// <summary>
    /// Classe para representar configuração de estribos
    /// </summary>
    public class ArmStirrup
    {
        public double Diametro { get; set; }
        public double Espacamento { get; set; }
        public bool Alternado { get; set; } = false;

        public ArmStirrup(double diametro, double espacamento)
        {
            Diametro = diametro;
            Espacamento = espacamento;
        }

        public override string ToString() =>
            $"Ø{Diametro}mm//{Espacamento}mm{(Alternado ? " Alt." : "")}";
    }

    /// <summary>
    /// Propriedades geométricas de uma viga
    /// </summary>
    public class PropriedadesViga
    {
        public double Comprimento { get; set; }
        public double Altura { get; set; }
        public double Largura { get; set; }
        public Curve CurvaEixo { get; set; }
        public XYZ PontoInicial { get; set; }
        public XYZ PontoFinal { get; set; }
    }
}