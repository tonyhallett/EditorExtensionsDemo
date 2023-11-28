using Microsoft.VisualStudio.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace EditorExtensionsDemo
{
    public class ClassificationToolWindow : BaseToolWindow<ClassificationToolWindow>
    {
        public override string GetTitle(int toolWindowId) => "ClassificationToolWindow";

        public override Type PaneType => typeof(Pane);

        public override Task<FrameworkElement> CreateAsync(int toolWindowId, CancellationToken cancellationToken)
        {
            return Task.FromResult<FrameworkElement>(new ClassificationToolWindowControl());
        }

        [Guid("1d44db34-7806-4dca-94f5-f939434419e7")]
        internal class Pane : ToolkitToolWindowPane
        {
            public Pane()
            {
                BitmapImageMoniker = KnownMonikers.ToolWindow;
            }
        }
    }
}
