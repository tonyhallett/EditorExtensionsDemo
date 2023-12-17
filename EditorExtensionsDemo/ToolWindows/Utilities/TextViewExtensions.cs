using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;

namespace EditorExtensionsDemo.ToolWindows
{
    public static class TextViewExtensions
    {
        public static string GetPath(this ITextView textView)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            textView.TextBuffer.Properties.TryGetProperty(typeof(IVsTextBuffer), out IVsTextBuffer bufferAdapter);
            var persistFileFormat = bufferAdapter as IPersistFileFormat;

            if (persistFileFormat == null)
            {
                return null;
            }
            persistFileFormat.GetCurFile(out string filePath, out _);
            return filePath;
        }

        public static string SafeGetPath(this ITextView textView)
        {
            var path = textView.GetPath();
            return path ?? "";
        }
    }
}
