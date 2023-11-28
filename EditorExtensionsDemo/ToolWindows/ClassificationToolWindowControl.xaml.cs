using EditorExtensionsDemo.QuickInfo;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Text.Classification;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;

namespace EditorExtensionsDemo
{
    public partial class ClassificationToolWindowControl : UserControl
    {
        private readonly IComponentModel2 mef;
        private readonly IClassificationFormatMapService classificationFormatMapService;
        private readonly IClassificationTypeRegistryService classificationTypeRegistryService;
        private readonly List<IClassificationFormatMetadata> classificationFormatDefinitionMetadata;
        private readonly List<IClassificationTypeDefinitionMetadata> classificationTypeDefinitionMetadata;
        private readonly IClassificationFormatMap textClassificationFormatMap;

        private void Other()
        {
            var allClassificationTypes = classificationTypeDefinitionMetadata.SelectMany(m => m.BaseDefinition != null ? m.BaseDefinition.Concat(new string[] { m.Name }) : new string[] { m.Name }).Distinct();
            
            // handle the event

            //this is where the nulls come from
            var numNull = textClassificationFormatMap.CurrentPriorityOrder.Count(ct => ct == null);
            var typesNotInRegistry = classificationFormatDefinitionMetadata.
                SelectMany(cfm => cfm.ClassificationTypeNames).
                // Note that the registry dictionary look up is OrdinalIgnoreCase
                Where(ctn => classificationTypeRegistryService.GetClassificationType(ctn) == null).ToList();
            foreach (var typeNotInRegistry in typesNotInRegistry)
            {
                Debug.WriteLine(typeNotInRegistry);
            }
            var priorityOrder = textClassificationFormatMap.CurrentPriorityOrder.Where(ct => ct != null).ToList();


            var editorFormatDefinitionMetadata = mef.DefaultExportProvider.GetExports<EditorFormatDefinition, IEditorFormatMetadata>().Select(l =>
            {
                return l.Metadata;
            }).ToList();

            var notPriority = classificationTypeDefinitionMetadata.Where(ctm => !priorityOrder.Any(ct => ct.Classification == ctm.Name)).ToList();
            notPriority.ForEach(np =>
            {
                var hasEditorFormat = editorFormatDefinitionMetadata.Any(efm => efm.Name.ToLower() == np.Name.ToLower());
                if (!hasEditorFormat)
                {
                    Debug.WriteLine($"{np.Name} - {np.BaseDefinition?.First()}");
                }
            });

            var transientEditorFormatDefinitionNames = editorFormatDefinitionMetadata.Where(m => m.Name.Contains("(TRANSIENT)")).Select(m => m.Name).ToList();

            foreach (var ctn in allClassificationTypes)
            {
                var definitionFrom = DefinitionFrom.None;
                var ct = classificationTypeRegistryService.GetClassificationType(ctn);
                if (ct != null)
                {
                    var editorFormatMapKey = textClassificationFormatMap.GetEditorFormatMapKey(ct);

                    var classificationFormatDefinition = classificationFormatDefinitionMetadata.Any(cfm => ((IEditorFormatMetadata)cfm).Name.ToLower() == editorFormatMapKey.ToLower());
                    if (!classificationFormatDefinition)
                    {
                        var hasEditorFormatMetadata = editorFormatDefinitionMetadata.Any(efm => efm.Name.ToLower() == ctn.ToLower());
                        if (hasEditorFormatMetadata)
                        {
                            definitionFrom = DefinitionFrom.EditorFormatDefinition;
                        }
                    }
                    else
                    {
                        definitionFrom = DefinitionFrom.ClassificationFormatDefinition;
                    }

                }
                Debug.WriteLine($"{ctn} - {definitionFrom}");

            }
            // Is 1
            //var maxBaseDefinitions = classificationTypeMetadata.Max(m => m.BaseDefinition?.Count() ?? 0);

            // Is 1
            // var maxClassificationTypeNames = classificationMetadata.Max(c => c.ClassificationTypeNames.Length);

        }
        public ClassificationToolWindowControl()
        {
            this.DataContext = new ClassificationViewModel();

            InitializeComponent();

        }


        private void LogTree(List<TreeItem> treeItems)
        {
            treeItems.ForEach(ti => LogTreeItem(ti, 0));

            static void LogTreeItem(TreeItem treeItem, int level)
            {
                Debug.WriteLine($"{new string(' ', level * 2)}{treeItem.Name}");
                foreach (var child in treeItem.Children)
                {
                    LogTreeItem(child, level + 1);
                }
            }
        }

    }

    internal enum DefinitionFrom
    {
        ClassificationFormatDefinition,
        EditorFormatDefinition,
        None
    }
}
