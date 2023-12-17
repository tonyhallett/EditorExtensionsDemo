using Microsoft.VisualStudio.Text.Classification;

namespace EditorExtensionsDemo
{
    internal interface IClassificationFormatMapProvider
    {
        IClassificationFormatMap GetClassificationFormatMap(string category);
    }
}
