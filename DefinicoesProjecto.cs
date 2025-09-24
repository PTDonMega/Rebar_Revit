namespace Rebar_Revit
{
    /// <summary>
    /// Definições avançadas do projecto
    /// </summary>
    public class DefinicoesProjecto
    {
        // Recobrimentos em mm
        public double RecobrimentoPilares { get; set; } = 40;
        public double RecobrimentoVigas { get; set; } = 25;
        public double RecobrimentoFundacoes { get; set; } = 50;
        public double RecobrimentoLajes { get; set; } = 20;

        // Configurações de amarração
        public double MultiplicadorAmarracaoMinimo { get; set; } = 30;
        public double MultiplicadorAmarracaoMaximo { get; set; } = 100;
        public bool AmarracaoAutomaticaGlobal { get; set; } = true;

        // Estribos
        public double EspacamentoMinimoEstribos { get; set; } = 50; // mm
        public double EspacamentoMaximoEstribos { get; set; } = 300; // mm
        public bool ConfinamentoSismicoAutomatico { get; set; } = true;
        public double RatioConfinamento { get; set; } = 0.2; // 20% da altura nas extremidades

        // Fundações
        public bool IncluirEsperasFundacao { get; set; } = true;
        public double ComprimentoEspera { get; set; } = 500; // mm
        public double EspacamentoMinimoMalha { get; set; } = 150; // mm
        public double EspacamentoMaximoMalha { get; set; } = 300; // mm

        // Vigas
        public bool ArmaduraDuplaObrigatoria { get; set; } = false;
        public double RatioArmaduraSuperior { get; set; } = 0.4;
        public double ComprimentoZonaConfinamento { get; set; } = 600; // mm

        // Validação
        public bool ValidarEurocodigo { get; set; } = true;
        public bool ValidarEspacamentosMinimos { get; set; } = true;
        public bool GerarAvisosSobreposicao { get; set; } = true;

        // Produtividade
        public bool GerarRelatorioQuantidades { get; set; } = true;
        public bool CriarEtiquetasAutomaticas { get; set; } = true;
        public string FormatoEtiqueta { get; set; } = "{Diametro}? L={Comprimento}mm";
    }
}
