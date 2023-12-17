using Microsoft.VisualStudio.Shell.TableControl;
using System.Windows;
using System.Windows.Controls;

namespace EditorExtensionsDemo
{
    internal class PriorityClassificationTableEntry : WpfTableEntryBase, IIndexedTableEntry, IHavePriorityClassification
    {
        
        public PriorityClassificationTableEntry(PriorityClassification priorityClassification, int index, object identity)
        {
            this.PriorityClassification = priorityClassification;
            this.Index = index;
            this.Identity = identity;
        }

        public PriorityClassification PriorityClassification { get; }

        public int Index { get; }

        public override object Identity { get; }

        public override bool TryCreateStringContent(string columnName, bool truncatedText, bool singleColumnView, out string content)
        {
            content = null;
            switch (ColumnNameSplitter.Split(columnName))
            {
                case nameof(PriorityClassification.IsNull):
                    content = PriorityClassification.IsNull.ToString();
                    break;
                case nameof(PriorityClassification.Classification):
                    content = PriorityClassification.Classification;
                    break;
                case nameof(PriorityClassification.BaseTypes):
                    content = PriorityClassification.BaseTypes;
                    break;
                case nameof(PriorityClassification.IsPriority):
                    content = PriorityClassification.IsPriority.ToString();
                    break;
                case nameof(PriorityClassification.Before):
                    content = PriorityClassification.Before;
                    break;
                case nameof(PriorityClassification.After):
                    content = PriorityClassification.After;
                    break;
                case nameof(PriorityClassification.IsTransient):
                    content = PriorityClassification.IsTransient.ToString();
                    break;
                case nameof(PriorityClassification.HasClassificationTypeDefinition):
                    content = PriorityClassification.HasClassificationTypeDefinition.ToString();
                    break;
                case nameof(PriorityClassification.HasClassificationFormatMetadata):
                    content = PriorityClassification.HasClassificationFormatMetadata.ToString();
                    break;
                case nameof(PriorityClassification.DefinitionFrom):
                    content = PriorityClassification.DefinitionFrom.ToString();
                    break;
                case nameof(PriorityClassification.Priority):
                    content = PriorityClassification.Priority.ToString();
                    break;
            }
            return true;
        }

        public override bool TryCreateColumnContent(string columnName, bool singleColumnView, out FrameworkElement content)
        {
            bool? boolValue = null;
            switch (ColumnNameSplitter.Split(columnName))
            {
                case nameof(PriorityClassification.IsNull):
                    boolValue = PriorityClassification.IsNull;
                    break;
                case nameof(PriorityClassification.IsPriority):
                    boolValue = PriorityClassification.IsPriority;
                    break;
                case nameof(PriorityClassification.IsTransient):
                    boolValue = PriorityClassification.IsTransient;
                    break;
                case nameof(PriorityClassification.HasClassificationTypeDefinition):
                    boolValue = PriorityClassification.HasClassificationTypeDefinition;
                    break;
                case nameof(PriorityClassification.HasClassificationFormatMetadata):
                    boolValue = PriorityClassification.HasClassificationFormatMetadata;
                    break;
            }
            if (boolValue.HasValue)
            {
                content = new ContentPresenter { Content = boolValue.Value };
                return true;
            }
            content = null;
            return false;
        }
        


    }
}
