using System;
using System.IO;
using Newtonsoft.Json;

namespace Rebar_Revit
{
    /// <summary>
    /// Gestor de definições avançadas para persistência de configurações
    /// </summary>
    public class GestorDefinicoesAvancado
    {
        private static readonly string CAMINHO_ARQUIVO = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "AFA_Armaduras",
            "definicoes_vigas.json");

        private DefinicoesProjectoAvancadas definicoes;

        public GestorDefinicoesAvancado()
        {
            definicoes = CarregarDefinicoes();
        }

        /// <summary>
        /// Obtém as definições atuais
        /// </summary>
        public DefinicoesProjectoAvancadas ObterDefinicoes()
        {
            return definicoes ?? CriarDefinicoesPadrao();
        }

        /// <summary>
        /// Salva as definições no arquivo
        /// </summary>
        public void SalvarDefinicoes(DefinicoesProjectoAvancadas novasDefinicoes)
        {
            try
            {
                definicoes = novasDefinicoes;
                
                // Criar diretório se não existir
                string diretorio = Path.GetDirectoryName(CAMINHO_ARQUIVO);
                if (!Directory.Exists(diretorio))
                {
                    Directory.CreateDirectory(diretorio);
                }

                // Serializar e salvar
                string json = JsonConvert.SerializeObject(definicoes, Formatting.Indented);
                File.WriteAllText(CAMINHO_ARQUIVO, json);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao salvar definições: {ex.Message}");
            }
        }

        /// <summary>
        /// Restaura as definições padrão
        /// </summary>
        public void RestaurarDefinicoesPadrao()
        {
            definicoes = CriarDefinicoesPadrao();
            SalvarDefinicoes(definicoes);
        }

        /// <summary>
        /// Carrega as definições do arquivo
        /// </summary>
        private DefinicoesProjectoAvancadas CarregarDefinicoes()
        {
            try
            {
                if (File.Exists(CAMINHO_ARQUIVO))
                {
                    string json = File.ReadAllText(CAMINHO_ARQUIVO);
                    var defCarregadas = JsonConvert.DeserializeObject<DefinicoesProjectoAvancadas>(json);
                    
                    // Validar definições carregadas
                    if (ValidarDefinicoes(defCarregadas))
                    {
                        return defCarregadas;
                    }
                }
            }
            catch
            {
                // Em caso de erro, usar definições padrão
            }

            return CriarDefinicoesPadrao();
        }

        /// <summary>
        /// Cria definições padrão otimizadas para vigas
        /// </summary>
        private DefinicoesProjectoAvancadas CriarDefinicoesPadrao()
        {
            return new DefinicoesProjectoAvancadas
            {
                // Coberturas específicas para vigas
                CoberturaVigas = 25,
                CoberturaPilares = 40, // Mantido para compatibilidade
                CoberturaFundacoes = 50,
                CoberturaLajes = 20,

                // Configurações de amarração para vigas
                MultiplicadorAmarracaoMinimo = 30,
                MultiplicadorAmarracaoMaximo = 80,
                AmarracaoAutomaticaGlobal = true,

                // Estribos otimizados para vigas
                EspacamentoMinimoEstribos = 75,
                EspacamentoMaximoEstribos = 300,
                ConfinamentoSismicoAutomatico = false, // Vigas normalmente não precisam
                RatioConfinamento = 0.15,

                // Configurações específicas para vigas
                ArmaduraDuplaObrigatoria = false,
                RatioArmaduraSuperior = 0.5, // 50% da armadura inferior
                ComprimentoZonaConfinamento = 500,

                // Validação
                ValidarEurocodigo = true,
                ValidarEspacamentosMinimos = true,
                GerarAvisosSobreposicao = true,

                // Produtividade
                GerarRelatorioQuantidades = true,
                CriarEtiquetasAutomaticas = false, // Simplificar para vigas
                FormatoEtiqueta = "Ø{Diametro}mm L={Comprimento}mm"
            };
        }

        /// <summary>
        /// Valida se as definições carregadas são válidas
        /// </summary>
        private bool ValidarDefinicoes(DefinicoesProjectoAvancadas def)
        {
            if (def == null) return false;

            try
            {
                // Validar coberturas
                if (def.CoberturaVigas < 15 || def.CoberturaVigas > 50) return false;
                
                // Validar multiplicadores de amarração
                if (def.MultiplicadorAmarracaoMinimo < 20 || def.MultiplicadorAmarracaoMinimo > 60) return false;
                if (def.MultiplicadorAmarracaoMaximo < 40 || def.MultiplicadorAmarracaoMaximo > 100) return false;
                if (def.MultiplicadorAmarracaoMinimo >= def.MultiplicadorAmarracaoMaximo) return false;

                // Validar espaçamentos de estribos
                if (def.EspacamentoMinimoEstribos < 50 || def.EspacamentoMinimoEstribos > 150) return false;
                if (def.EspacamentoMaximoEstribos < 200 || def.EspacamentoMaximoEstribos > 500) return false;
                if (def.EspacamentoMinimoEstribos >= def.EspacamentoMaximoEstribos) return false;

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Exporta as definições para um arquivo específico
        /// </summary>
        public void ExportarDefinicoes(string caminhoArquivo)
        {
            try
            {
                string json = JsonConvert.SerializeObject(definicoes, Formatting.Indented);
                File.WriteAllText(caminhoArquivo, json);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao exportar definições: {ex.Message}");
            }
        }

        /// <summary>
        /// Importa definições de um arquivo específico
        /// </summary>
        public void ImportarDefinicoes(string caminhoArquivo)
        {
            try
            {
                if (!File.Exists(caminhoArquivo))
                    throw new FileNotFoundException("Arquivo de definições não encontrado.");

                string json = File.ReadAllText(caminhoArquivo);
                var defImportadas = JsonConvert.DeserializeObject<DefinicoesProjectoAvancadas>(json);

                if (ValidarDefinicoes(defImportadas))
                {
                    definicoes = defImportadas;
                    SalvarDefinicoes(definicoes);
                }
                else
                {
                    throw new Exception("Arquivo de definições inválido.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao importar definições: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtém o caminho do arquivo de configurações
        /// </summary>
        public string ObterCaminhoArquivoConfiguracoes()
        {
            return CAMINHO_ARQUIVO;
        }

        /// <summary>
        /// Verifica se existe arquivo de configurações personalizado
        /// </summary>
        public bool ExisteConfiguracaoPersonalizada()
        {
            return File.Exists(CAMINHO_ARQUIVO);
        }
    }
}