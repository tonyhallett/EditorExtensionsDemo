using Microsoft.VisualStudio.Text.Editor;

namespace EditorExtensionsDemo
{
    public interface IActiveViewAccessor
    {
        IWpfTextView ActiveView { get; }
    }



}
