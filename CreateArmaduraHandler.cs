using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Rebar_Revit
{
    // Simple DTO with execution data that will be filled by the form and consumed by the ExternalEvent handler
    public class ExternalExecutionData
    {
        private readonly object _lock = new object();
        public List<ElementId> ElementIds { get; set; } = new List<ElementId>();
        public List<ArmVar> Varoes { get; set; } = new List<ArmVar>();
        public List<ArmStirrup> Estribos { get; set; } = new List<ArmStirrup>();
        public double MultAmarracao { get; set; } = 50;
        public bool AmarracaoAuto { get; set; } = true;
        public DefinicoesProjecto Defs { get; set; } = new DefinicoesProjecto();
        public TipoElementoEstruturalEnum TipoElemento { get; set; } = TipoElementoEstruturalEnum.Vigas;

        public void Clear()
        {
            lock (_lock)
            {
                ElementIds.Clear();
                Varoes.Clear();
                Estribos.Clear();
            }
        }
    }

    public class CreateArmaduraHandler : IExternalEventHandler
    {
        public ExternalExecutionData Data { get; set; } = new ExternalExecutionData();

        public void Execute(UIApplication app)
        {
            try
            {
                UIDocument uidoc = app.ActiveUIDocument;
                if (uidoc == null)
                {
                    Autodesk.Revit.UI.TaskDialog.Show("Erro", "UIDocument não disponível.");
                    return;
                }

                Document doc = uidoc.Document;
                if (Data.ElementIds == null || Data.ElementIds.Count == 0)
                {
                    Autodesk.Revit.UI.TaskDialog.Show("Aviso", "Nenhuma viga selecionada para processar.");
                    return;
                }

                var config = new ArmConfigExec(doc)
                {
                    TipoElemento = Data.TipoElemento,
                    AmarracaoAuto = Data.AmarracaoAuto,
                    MultAmarracao = Data.MultAmarracao,
                    Defs = Data.Defs
                };

                // Populate varoes and estribos
                config.Varoes.Clear();
                foreach (var v in Data.Varoes)
                {
                    config.Varoes.Add(new ArmVar(v.Quantidade, v.Diametro) { TipoArmadura = v.TipoArmadura });
                }

                config.Estribos.Clear();
                foreach (var e in Data.Estribos)
                {
                    config.Estribos.Add(new ArmStirrup(e.Diametro, e.Espacamento) { Alternado = e.Alternado });
                }

                int total = Data.ElementIds.Count;
                int processed = 0;
                int success = 0;
                var errors = new List<string>();

                using (Transaction trans = new Transaction(doc, "Criação de Armaduras em Vigas"))
                {
                    trans.Start();

                    foreach (var id in Data.ElementIds)
                    {
                        try
                        {
                            Element el = doc.GetElement(id);
                            if (el == null)
                            {
                                errors.Add($"Elemento ID {id} não encontrado");
                                continue;
                            }

                            bool res = config.ColocarArmadura(el);
                            if (res) success++;
                            else errors.Add($"Falha na colocação em Element {id}");
                        }
                        catch (Exception ex)
                        {
                            errors.Add($"Element {id}: {ex.Message}");
                        }
                        finally
                        {
                            processed++;
                        }
                    }

                    trans.Commit();
                }

                string mensagem = $"Processo concluído!\n\nVigas processadas: {total}\nArmaduras criadas: {success}\nErros: {errors.Count}";
                if (errors.Count > 0)
                {
                    mensagem += "\n\nPrimeiros erros:";
                    for (int i = 0; i < Math.Min(errors.Count, 5); i++) mensagem += $"\n• {errors[i]}";
                }

                Autodesk.Revit.UI.TaskDialog.Show("Resultado", mensagem);
            }
            catch (Exception ex)
            {
                Autodesk.Revit.UI.TaskDialog.Show("Erro", "Erro durante execução do ExternalEvent: " + ex.Message);
            }
            finally
            {
                // Optionally clear data after execution
                // Data.Clear();
            }
        }

        public string GetName()
        {
            return "CreateArmaduraHandler";
        }
    }
}
