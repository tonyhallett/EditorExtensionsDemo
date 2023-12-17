using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace EditorExtensionsDemo
{
    [Export(typeof(IInvokeWithDelay))]
    internal class InvokeWithDelay : IInvokeWithDelay
    {
        public void DelayedInvoke(int delay, Action action)
        {
            _ = System.Threading.Tasks.Task.Delay(delay).ContinueWith(t => action(),TaskScheduler.Default);
        }
    }
}
