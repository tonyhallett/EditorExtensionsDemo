using Microsoft.VisualStudio.Text.Formatting;

namespace EditorExtensionsDemo
{
    public class ClassificationTextFormattingRunProperties
    {
        public ClassificationTextFormattingRunProperties(
            TextFormattingRunProperties ownTextFormattingRunProperties,
            TextFormattingRunProperties mergedTextFormattingRunProperties,
            TextFormattingRunProperties defaultTextProperties
        )
        {
            OwnTextFormattingRunProperties = ownTextFormattingRunProperties;
            MergedTextFormattingRunProperties = mergedTextFormattingRunProperties;
            DefaultTextProperties = defaultTextProperties;
        }

        public TextFormattingRunProperties OwnTextFormattingRunProperties { get; }
        public TextFormattingRunProperties MergedTextFormattingRunProperties { get; }
        public TextFormattingRunProperties DefaultTextProperties { get; }
    }
}
