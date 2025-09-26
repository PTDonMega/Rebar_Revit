using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Rebar_Revit; // Import the namespace for the form
using System.Windows.Forms;
using System;

namespace Rebar_Revit
{
    [Transaction(TransactionMode.Manual)]
    public class CommandMain : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Launch the main form for defining attributes as modeless so user can interact with Revit
            var doc = commandData.Application.ActiveUIDocument.Document;
            var uidoc = commandData.Application.ActiveUIDocument;
            FormularioPrincipal form = new FormularioPrincipal(doc, uidoc);

            try
            {
                // Use Revit main window as owner to keep proper Z-order
                var owner = new Win32WindowWrapper(commandData.Application.MainWindowHandle);
                form.Show(owner);
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = "Erro ao abrir o formulário: " + ex.Message;
                return Result.Failed;
            }
        }
    }
}
