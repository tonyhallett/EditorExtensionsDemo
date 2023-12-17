using System.Windows;
using System.Windows.Media.TextFormatting;

namespace EditorExtensionsDemo
{
    public class DefaultTextParagraphProperties : TextParagraphProperties
    {
        public override FlowDirection FlowDirection => FlowDirection.LeftToRight;

        public override TextAlignment TextAlignment => TextAlignment.Left;

        public override double LineHeight => 20;

        public override bool FirstLineInParagraph => true;

        public override TextRunProperties DefaultTextRunProperties { get; } = new DefaultTextRunProperties();

        public override TextWrapping TextWrapping => TextWrapping.Wrap;

        public override TextMarkerProperties TextMarkerProperties => null;

        public override double Indent => 0;
    }
}
