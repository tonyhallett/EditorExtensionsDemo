using Microsoft.VisualStudio.Shell.TableControl;
using System.Windows;

namespace EditorExtensionsDemo
{
    public class TableControlEventProcessor : TableControlEventProcessorBase
    {
        public override void PreprocessDragOver(ITableEntryHandle entry, DragEventArgs e)
        {
            e.Handled = true;
        }
        public override void PostprocessDragOver(ITableEntryHandle entry, DragEventArgs e)
        {
            
        }
        public override void PostprocessDrop(ITableEntryHandle entry, DragEventArgs e)
        {
        }
        public override void PreprocessDrop(ITableEntryHandle entry, DragEventArgs e)
        {
            e.Handled = true;
        }

    }
}
