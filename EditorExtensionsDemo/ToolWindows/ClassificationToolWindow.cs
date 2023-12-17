using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using static Microsoft.VisualStudio.VSConstants;

namespace EditorExtensionsDemo
{
    public class ClassificationToolWindow : BaseToolWindow<ClassificationToolWindow>
    {
        public override string GetTitle(int toolWindowId) => "Classification";

        public override Type PaneType => typeof(Pane);

        public override async Task<FrameworkElement> CreateAsync(int toolWindowId, CancellationToken cancellationToken)
        {
            var classificationViewModel = await VS.GetMefServiceAsync<ClassificationViewModel>();
            var view = new ClassificationToolWindowControl
            {
                DataContext = classificationViewModel
            };
            return view;
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
