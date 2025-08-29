using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using MacroArmaduraAvancado; // Import the namespace for the form
using System.Windows.Forms;

namespace AFA_Armaduras
{
    [Transaction(TransactionMode.Manual)]
    public class CommandMain : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Launch the main form for defining attributes
            var doc = commandData.Application.ActiveUIDocument.Document;
            var uidoc = commandData.Application.ActiveUIDocument;
            FormularioPrincipalAvancado form = new FormularioPrincipalAvancado(doc, uidoc);
            form.ShowDialog();
            return Result.Succeeded;
        }
    }
}
