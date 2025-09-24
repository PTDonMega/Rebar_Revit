using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using Rebar_Revit;
using Rebar_Revit;

namespace Rebar_Revit
{
    /// <summary>
    /// Configura��o e execu��o de armadura especializada para Vigas
    /// </summary>
    public class ArmConfigExec
    {
        // Configura��o principal
        public List<ArmVar> Varoes { get; set; } = new List<ArmVar>();
        public List<ArmStirrup> Estribos { get; set; } = new List<ArmStirrup>();
        public TipoDistribuicaoArmaduraEnum TipoDistribuicao { get; set; } = TipoDistribuicaoArmaduraEnum.MistaComMaioresNasBordas;
        public bool AmarracaoAuto { get; set; } = true;
        public double MultAmarracao { get; set; } = 50;
        public string TipoAmarracao { get; set; } = "Autom�tico";
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
                throw new Exception($"Erro na coloca��o de armadura: {ex.Message}");
            }
        }

        private bool ColocarArmaduraViga(Element viga)
        {
            FamilyInstance inst = viga as FamilyInstance;
            if (inst == null)
            {
                throw new Exception("Elemento n�o � uma FamilyInstance v�lida");
            }

            try
            {
                // Informa��es da viga para debug
                string familyName = inst.Symbol?.FamilyName ?? "N/A";
                string typeName = inst.Symbol?.Name ?? "N/A";

                // Obter propriedades da viga
                var propriedades = ObterPropriedadesViga(inst);
                if (propriedades == null)
                {
                    throw new Exception($"N�o foi poss�vel obter as propriedades geom�tricas da viga {familyName} - {typeName}");
                }

                double recobrimento = Uteis.MilimetrosParaFeet(Defs?.RecobrimentoVigas ?? 25); // Converter mm para p�s

                // Verificar se existem configura��es de armadura
                if (Varoes.Count == 0 && Estribos.Count == 0)
                {
                    throw new Exception("Nenhuma configura��o de armadura definida");
                }

                // Verificar se existem tipos de armadura dispon�veis no projeto
                var tiposDisponiveis = ObterTiposArmaduraDisponiveis();
                if (tiposDisponiveis.Count == 0)
                {
                    throw new Exception("Nenhum tipo de armadura (RebarBarType) encontrado no projeto. Carregue um template de armaduras ou crie tipos de armadura no projeto.");
                }

                // Valida��o dimensional cr�tica
                if (!ValidarDimensoesCriticas(propriedades, recobrimento))
                {
                    string dimensoesInfo = $"Dimens�es: L={Uteis.FeetParaMilimetros(propriedades.Comprimento):F0}mm, " +
                                          $"B={Uteis.FeetParaMilimetros(propriedades.Largura):F0}mm, " +
                                          $"H={Uteis.FeetParaMilimetros(propriedades.Altura):F0}mm, " +
                                          $"Recobrimento={Uteis.FeetParaMilimetros(recobrimento):F0}mm";
                    throw new Exception($"Dimens�es insuficientes para coloca��o de armadura. {dimensoesInfo}");
                }

                bool sucessoLongitudinal = true;
                bool sucessoEstribos = true;

                // Criar armadura longitudinal (superior, inferior, lateral)
                if (Varoes.Count > 0)
                {
                    try
                    {
                        sucessoLongitudinal = CriarArmaduraVigaLongitudinal(inst, propriedades, recobrimento);
                        if (!sucessoLongitudinal)
                        {
                            throw new Exception("Falha na cria��o de armadura longitudinal");
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
                        sucessoEstribos = CriarEstribosViga(inst, propriedades, recobrimento);
                        if (!sucessoEstribos)
                        {
                            throw new Exception("Falha na cria��o de estribos");
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

        private bool ValidarDimensoesCriticas(PropriedadesViga props, double recobrimento)
        {
            double larguraMinima = Uteis.MilimetrosParaFeet(50); // 5cm em p�s
            double alturaMinima = Uteis.MilimetrosParaFeet(100); // 10cm em p�s
            double comprimentoMinimo = Uteis.MilimetrosParaFeet(300); // 30cm em p�s

            if (props.Largura < larguraMinima ||
                props.Altura < alturaMinima ||
                props.Comprimento < comprimentoMinimo)
            {
                return false;
            }

            double larguraUtil = props.Largura - 2 * recobrimento;
            double alturaUtil = props.Altura - 2 * recobrimento;

            if (larguraUtil <= Uteis.MilimetrosParaFeet(20) || alturaUtil <= Uteis.MilimetrosParaFeet(20))
            {
                return false;
            }

            if (props.PontoInicial == null || props.PontoFinal == null ||
                Uteis.PontosSaoQuaseIguais(props.PontoInicial, props.PontoFinal))
            {
                return false;
            }

            return true;
        }

        public PropriedadesViga ObterPropriedadesViga(FamilyInstance inst)
        {
            try
            {
                LocationCurve locCurve = inst.Location as LocationCurve;
                if (locCurve == null)
                {
                    throw new Exception("Viga n�o possui LocationCurve v�lida");
                }

                var props = new PropriedadesViga();

                Curve curvaViga = locCurve.Curve;
                double comprimento = curvaViga.Length;

                double altura = 0;
                double largura = 0;

                // Tentar obter dimens�es da inst�ncia
                Parameter paramAlturaInst = inst.LookupParameter("Altura") ??
                                           inst.LookupParameter("Height") ??
                                           inst.LookupParameter("h");

                Parameter paramLarguraInst = inst.LookupParameter("Largura") ??
                                            inst.LookupParameter("Width") ??
                                            inst.LookupParameter("b");

                if (paramAlturaInst != null && paramAlturaInst.AsDouble() > 0)
                {
                    altura = paramAlturaInst.AsDouble();
                }

                if (paramLarguraInst != null && paramLarguraInst.AsDouble() > 0)
                {
                    largura = paramLarguraInst.AsDouble();
                }

                // Se n�o encontrou nas Properties da inst�ncia, buscar no tipo
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
                                              elementType.LookupParameter("h");

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
                                               elementType.LookupParameter("b");

                            if (larguraFamilia != null && larguraFamilia.AsDouble() > 0)
                            {
                                largura = larguraFamilia.AsDouble();
                            }
                        }
                    }
                }

                // Verificar se encontrou todas as dimens�es
                if (comprimento <= 0)
                {
                    throw new Exception($"Comprimento inv�lido: {Uteis.FeetParaMilimetros(comprimento):F0}mm");
                }

                if (altura <= 0)
                {
                    throw new Exception("Altura n�o encontrada nos par�metros da viga");
                }

                if (largura <= 0)
                {
                    throw new Exception("Largura n�o encontrada nos par�metros da viga");
                }

                return new PropriedadesViga
                {
                    Comprimento = comprimento,
                    Altura = altura,
                    Largura = largura,
                    CurvaEixo = curvaViga,
                    PontoInicial = curvaViga.GetEndPoint(0),
                    PontoFinal = curvaViga.GetEndPoint(1),
                    InstanciaViga = inst
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter propriedades da viga: {ex.Message}");
            }
        }

        private bool CriarArmaduraVigaLongitudinal(FamilyInstance elemento, PropriedadesViga props, double recobrimento)
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
                        if (!CriarArmaduraSuperior(elemento, props, varao, tipoVarao, recobrimento))
                            return false;
                    }
                    else if (tipoLower == "inferior")
                    {
                        if (!CriarArmaduraInferior(elemento, props, varao, tipoVarao, recobrimento))
                            return false;
                    }
                    else if (tipoLower == "lateral")
                    {
                        if (!CriarArmaduraLateral(elemento, props, varao, tipoVarao, recobrimento))
                            return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro na cria��o de armadura longitudinal: {ex.Message}");
            }
        }

        private bool CriarArmaduraSuperior(FamilyInstance elemento, PropriedadesViga props,
                                         ArmVar varao, RebarBarType tipoVarao, double recobrimento)
        {
            try
            {
                if (varao.Quantidade <= 0) return true;

                Transform transformViga = ObterTransformViga(props);
                double larguraUtil = props.Largura - 2 * recobrimento;
                double espacamento = varao.Quantidade > 1 ? larguraUtil / (varao.Quantidade - 1) : 0;

                double diamEstribo = Estribos.Count > 0 ? Uteis.MilimetrosParaFeet(Estribos[0].Diametro) : 0;
                double diamVarao = Uteis.MilimetrosParaFeet(varao.Diametro);
                double alturaZ = recobrimento + larguraUtil - diamEstribo / 2 - diamVarao / 2;
                double alturaUtil = props.Altura - 2 * recobrimento;

                for (int i = 0; i < varao.Quantidade; i++)
                {
                    double offsetY = varao.Quantidade == 1 ? 0 : -larguraUtil / 2 + (i * espacamento);

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
                                         ArmVar varao, RebarBarType tipoVarao, double recobrimento)
        {
            try
            {
                if (varao.Quantidade <= 0) return true;

                Transform transformViga = ObterTransformViga(props);
                double larguraUtil = props.Largura - 2 * recobrimento;
                double espacamento = varao.Quantidade > 1 ? larguraUtil / (varao.Quantidade - 1) : 0;

                double diamEstribo = Estribos.Count > 0 ? Uteis.MilimetrosParaFeet(Estribos[0].Diametro) : 0;
                double diamVarao = Uteis.MilimetrosParaFeet(varao.Diametro);
                double alturaZ = recobrimento + diamEstribo / 2 + diamVarao / 2;

                for (int i = 0; i < varao.Quantidade; i++)
                {
                    double offsetY = varao.Quantidade == 1 ? 0 : -larguraUtil / 2 + (i * espacamento);

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
                                        ArmVar varao, RebarBarType tipoVarao, double recobrimento)
        {
            try
            {
                if (varao.Quantidade <= 0) return true;

                Transform transformViga = ObterTransformViga(props);
                double alturaUtil = props.Altura - 2 * recobrimento;
                double diamEstribo = Estribos.Count > 0 ? Uteis.MilimetrosParaFeet(Estribos[0].Diametro) : 0;
                double diamVarao = Uteis.MilimetrosParaFeet(varao.Diametro);
                double alturaZ = recobrimento + diamEstribo / 2 + alturaUtil / 2; // Meia altura �til

                // Lado esquerdo
                double offsetYEsq = -(props.Largura / 2 - recobrimento - diamEstribo / 2 - diamVarao / 2);
                // Lado direito
                double offsetYDir = props.Largura / 2 - recobrimento - diamEstribo / 2 - diamVarao / 2;

                for (int i = 0; i < varao.Quantidade; i++)
                {
                    // Se quiser distribuir lateralmente, pode ajustar offsetYEsq/offsetYDir
                    XYZ pontoInicialEsqLocal = new XYZ(0, offsetYEsq, alturaZ);
                    XYZ pontoFinalEsqLocal = new XYZ(props.Comprimento, offsetYEsq, alturaZ);

                    XYZ pontoInicialEsq = transformViga.OfPoint(pontoInicialEsqLocal);
                    XYZ pontoFinalEsq = transformViga.OfPoint(pontoFinalEsqLocal);

                    List<XYZ> pontosEsq = CalcularPontosAmarracao(pontoInicialEsq, pontoFinalEsq, varao.Diametro, "lateral");
                    if (!CriarArmaduraIndividual(elemento, pontosEsq, tipoVarao, varao.Diametro))
                        return false;

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
            // Baseado apenas em Z global para evitar rota��es inesperadas.
            XYZ x = (props.PontoFinal - props.PontoInicial).Normalize();
            XYZ z = XYZ.BasisZ;

            // Se a viga for quase vertical, usar Y global como refer�ncia de "cima"
            if (x.IsAlmostEqualTo(z, 1e-6) || x.IsAlmostEqualTo(-z, 1e-6))
            {
                z = XYZ.BasisY;
            }

            XYZ y = z.CrossProduct(x).Normalize();
            // Recalcular Z para garantir ortonormalidade
            z = x.CrossProduct(y).Normalize();

            Transform transform = Transform.Identity;
            transform.Origin = props.PontoInicial;
            transform.BasisX = x;
            transform.BasisY = y;
            transform.BasisZ = z;

            return transform;
        }

        private List<XYZ> CalcularPontosAmarracao(XYZ pontoInicial, XYZ pontoFinal, double diametro, string posicao)
        {
            try
            {
                if (pontoInicial == null || pontoFinal == null)
                {
                    throw new Exception("Pontos inicial ou final s�o nulos");
                }

                if (pontoInicial.IsAlmostEqualTo(pontoFinal, 1e-6))
                {
                    throw new Exception("Pontos inicial e final s�o coincidentes");
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
                    if (distancia > 1e-3) // 1mm m�nimo
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
                    throw new Exception("Nenhuma curva v�lida criada");
                }

                XYZ vetorNormal = Uteis.DeterminarVetorNormalSeguro(curvas);

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
                throw new Exception($"Erro na cria��o de armadura �{diametro}mm: {ex.Message}");
            }
        }

        private List<XYZ> ValidarELimparPontos(List<XYZ> pontos)
        {
            return Uteis.ValidarELimparPontos(pontos);
        }

        private XYZ DeterminarVetorNormalSeguro(List<Curve> curvas)
        {
            return Uteis.DeterminarVetorNormalSeguro(curvas);
        }

        private List<XYZ> ConverterPontosParaGlobais(List<XYZ> pontosLocais, Transform transform)
        {
            return Uteis.ConverterPontosParaGlobais(pontosLocais, transform);
        }

        private void DefinirPropriedadesArmadura(Rebar armadura, double diametro)
        {
            try
            {
                if (armadura != null)
                {
                    string comentario = $"�{diametro}mm - Amarra��o: {MultAmarracao}�";
                    var paramComentario = armadura.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS);
                    paramComentario?.Set(comentario);
                }
            }
            catch
            {
                // Ignorar erros
            }
        }

        private bool CriarEstribosViga(FamilyInstance elemento, PropriedadesViga props, double recobrimento)
        {
            try
            {
                if (Estribos.Count == 0) return true;

                foreach (var estribo in Estribos)
                {
                    var tipoEstribo = ObterTipoArmaduraPorDiametro(estribo.Diametro);
                    if (tipoEstribo == null) continue;

                    double espacamento = Uteis.MilimetrosParaFeet(estribo.Espacamento);
                    int numeroEstribos = Math.Max(1, (int)(props.Comprimento / espacamento) - 1);

                    Transform transformViga = ObterTransformViga(props);

                    for (int i = 1; i <= numeroEstribos; i++)
                    {
                        double posicaoX = i * espacamento;

                        if (posicaoX >= props.Comprimento) break;

                        CriarEstriboVigaIndividual(elemento, posicaoX, props, tipoEstribo, recobrimento, transformViga);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro na cria��o de estribos: {ex.Message}");
            }
        }

        private bool CriarEstriboVigaIndividual(FamilyInstance elemento, double posicaoX,
                                              PropriedadesViga props, RebarBarType tipoEstribo,
                                              double recobrimento, Transform transformViga)
        {
            try
            {
                double larguraUtil = props.Largura - 2 * recobrimento;
                double alturaUtil = props.Altura - 2 * recobrimento;

                if (larguraUtil <= 0 || alturaUtil <= 0) return false;

                // Corrigir offsets para garantir que o estribo fique dentro da viga
                double y1 = -larguraUtil / 2;
                double y2 = larguraUtil / 2;
                double z1 = recobrimento;
                double z2 = recobrimento + alturaUtil;

                List<XYZ> pontosLocais = new List<XYZ>
                {
                    new XYZ(posicaoX, y1, z1),
                    new XYZ(posicaoX, y2, z1),
                    new XYZ(posicaoX, y2, z2),
                    new XYZ(posicaoX, y1, z2),
                    new XYZ(posicaoX, y1, z1) // Fechar o estribo
                };

                List<XYZ> pontosGlobais = Uteis.ConverterPontosParaGlobais(pontosLocais, transformViga);

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

                // Corrigir vetor normal: deve ser perpendicular ao plano do estribo
                XYZ vetorNormal = transformViga.BasisY;

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

            relatorio.AppendLine("=== RELAT�RIO DE PR�-VISUALIZA��O - VIGAS ===");
            relatorio.AppendLine($"Total de Vigas: {elementos.Count}");
            relatorio.AppendLine();

            relatorio.AppendLine("ARMADURA LONGITUDINAL:");
            relatorio.AppendLine($"- Quantidade total de var�es: {QtdTotalVaroes()}");

            var armaduraSuperior = Varoes.Where(v => v.TipoArmadura.ToLower() == "superior").ToList();
            var armaduraInferior = Varoes.Where(v => v.TipoArmadura.ToLower() == "inferior").ToList();
            var armaduraLateral = Varoes.Where(v => v.TipoArmadura.ToLower() == "lateral").ToList();

            if (armaduraSuperior.Any())
            {
                relatorio.AppendLine("  SUPERIOR:");
                foreach (var varao in armaduraSuperior)
                    relatorio.AppendLine($"    * {varao.Quantidade}�{varao.Diametro}mm");
            }

            if (armaduraInferior.Any())
            {
                relatorio.AppendLine("  INFERIOR:");
                foreach (var varao in armaduraInferior)
                    relatorio.AppendLine($"    * {varao.Quantidade}�{varao.Diametro}mm");
            }

            if (armaduraLateral.Any())
            {
                relatorio.AppendLine("  LATERAL:");
                foreach (var varao in armaduraLateral)
                    relatorio.AppendLine($"    * {varao.Quantidade}�{varao.Diametro}mm");
            }

            relatorio.AppendLine();
            relatorio.AppendLine("ESTRIBOS:");
            foreach (var estribo in Estribos)
            {
                string tipo = estribo.Alternado ? " (Alternado)" : " (Uniforme)";
                relatorio.AppendLine($"- �{estribo.Diametro}mm // {estribo.Espacamento}mm{tipo}");
            }

            return relatorio.ToString();
        }
    }

    /// <summary>
    /// Classe para representar um var�o individual para vigas
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

        public override string ToString() => $"{Quantidade}�{Diametro}mm ({TipoArmadura})";
    }

    /// <summary>
    /// Classe para representar configura��o de estribos
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
            $"�{Diametro}mm//{Espacamento}mm{(Alternado ? " Alt." : "")}";
    }

    /// <summary>
    /// Propriedades geom�tricas de uma viga
    /// </summary>
    public class PropriedadesViga
    {
        public double Comprimento { get; set; }
        public double Altura { get; set; }
        public double Largura { get; set; }
        public Curve CurvaEixo { get; set; }
        public XYZ PontoInicial { get; set; }
        public XYZ PontoFinal { get; set; }
        public FamilyInstance InstanciaViga { get; set; }
    }
}