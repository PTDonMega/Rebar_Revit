using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace Rebar_Revit
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public partial class Identificador : IExternalCommand
    {
        private UIApplication uiapp;
        private Autodesk.Revit.ApplicationServices.Application app;
        private UIDocument uidoc;
        private Document doc;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            uiapp = commandData.Application;
            uidoc = uiapp.ActiveUIDocument;
            doc = uidoc.Document;
            app = uiapp.Application;

            try
            {
                FormularioPrincipal formPrincipal = new FormularioPrincipal(doc, uidoc);
                formPrincipal.ShowDialog();
                
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = "Erro na execução do macro: " + ex.Message;
                return Result.Failed;
            }
        }
    }

    /// <summary>
    /// Tipo de elemento estrutural
    /// </summary>
    public enum TipoElementoEstruturalEnum
    {
        Pilares,
        Vigas,
        Fundacoes,
        Lajes
    }

    /// <summary>
    /// Tipos de distribuição de armadura
    /// </summary>
    public enum TipoDistribuicaoArmaduraEnum
    {
        Uniforme,
        ConcentradaNasBordas,
        MistaComMaioresNasBordas
    }

    /// <summary>
    /// Tipos de amarração
    /// </summary>
    public enum TipoAmarracaoEnum
    {
        Automatico,
        Reta,
        Dobrada90,
        Gancho135
    }

    /// <summary>
    /// Posição da armadura na viga
    /// </summary>
    public enum PosicaoArmaduraViga
    {
        Superior,
        Inferior,
        Lateral
    }
}