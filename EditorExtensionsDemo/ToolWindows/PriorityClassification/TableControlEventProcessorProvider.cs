using Microsoft.VisualStudio.Shell.TableControl;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Utilities;

namespace EditorExtensionsDemo
{
    [Export(typeof(ITableControlEventProcessorProvider))]
    [ManagerType(nameof(PriorityClassification.Classification))]
    [DataSourceType(PriorityClassificationViewModel.sourceTypeIdentifier)]
    [DataSource(PriorityClassificationViewModel.sourceIdentifier)]
    [Name("ClassificationTableControlEventProcessorProvider")]
    public class TableControlEventProcessorProvider : ITableControlEventProcessorProvider
    {
        public ITableControlEventProcessor GetAssociatedEventProcessor(IWpfTableControl tableControl)
        {
            return new TableControlEventProcessor();
        }
    }
}
