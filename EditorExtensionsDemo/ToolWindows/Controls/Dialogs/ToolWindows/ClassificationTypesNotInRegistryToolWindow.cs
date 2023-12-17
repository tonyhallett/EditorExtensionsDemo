using Microsoft.VisualStudio.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace EditorExtensionsDemo.ToolWindows.Controls.Dialogs
{
    public class ClassificationTypesNotInRegistryToolWindow : BaseToolWindow<ClassificationTypesNotInRegistryToolWindow>
    {
        public override string GetTitle(int toolWindowId) => "Classification Types Not In Registry";

        public override Type PaneType => typeof(Pane);

        public override async Task<FrameworkElement> CreateAsync(int toolWindowId, CancellationToken cancellationToken)
        {
            return new ClassificationTypesNotInRegistryWindowControl();
        }

        [Guid("e1c48431-d15b-4fc7-9680-9555b7f38426")]
        internal class Pane : ToolkitToolWindowPane
        {
            public Pane()
            {
                BitmapImageMoniker = KnownMonikers.ToolWindow;
            }
        }
    }
}
