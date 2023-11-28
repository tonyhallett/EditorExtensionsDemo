using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;

namespace EditorExtensionsDemo.QuickInfo
{
    //[Export(typeof(IAsyncQuickInfoSourceProvider))]
    [Name("Issue quick info source provider")]
    [ContentType("Roslyn Languages")]
    [Order]
    //https://github.com/microsoft/VSSDK-Extensibility-Samples/issues/190
    internal class DocTranslatorAsyncQuickInfoSourceProvider : IAsyncQuickInfoSourceProvider
    {
        public IAsyncQuickInfoSource TryCreateQuickInfoSource(ITextBuffer textBuffer)
        {
            return new DocTranslatorAsyncQuickInfoSource(textBuffer);
        }

        
    }
}
