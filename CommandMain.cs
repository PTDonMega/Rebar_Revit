using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Rebar_Revit; // Import the namespace for the form
using System.Windows.Forms;

namespace Rebar_Revit
{
    [Transaction(TransactionMode.Manual)]
    public class CommandMain : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Launch the main form for defining attributes
            var doc = commandData.Application.ActiveUIDocument.Document;
            var uidoc = commandData.Application.ActiveUIDocument;
            FormularioPrincipal form = new FormularioPrincipal(doc, uidoc);
            form.ShowDialog();
            return Result.Succeeded;
        }
    }
}
