namespace MacroArmaduraAvancado
{
    /// <summary>
    /// Gestor avançado de definições
    /// </summary>
    public class GestorDefinicoesAvancado
    {
        private DefinicoesProjectoAvancadas definicoes;

        public GestorDefinicoesAvancado()
        {
            definicoes = new DefinicoesProjectoAvancadas();
        }

        public DefinicoesProjectoAvancadas ObterDefinicoes()
        {
            return definicoes;
        }

        public void GuardarDefinicoes(DefinicoesProjectoAvancadas novasDefinicoes)
        {
            definicoes = novasDefinicoes;
        }

        public bool ValidarDefinicoes(DefinicoesProjectoAvancadas def)
        {
            if (def.CoberturaPilares < 15 || def.CoberturaPilares > 100)
                return false;
            
            if (def.EspacamentoMinimoEstribos < 25 || def.EspacamentoMinimoEstribos > 200)
                return false;
            
            if (def.MultiplicadorAmarracaoMinimo < 20 || def.MultiplicadorAmarracaoMaximo > 150)
                return false;
            
            return true;
        }
    }
}