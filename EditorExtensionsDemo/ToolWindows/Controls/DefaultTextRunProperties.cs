using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

namespace EditorExtensionsDemo
{
    public class DefaultTextRunProperties : TextRunProperties
    {
        public override Typeface Typeface { get; } = new Typeface("Consolas");

        public override double FontRenderingEmSize => 16;

        public override double FontHintingEmSize => 16;

        public override TextDecorationCollection TextDecorations { get; } = new TextDecorationCollection();

        public override Brush ForegroundBrush { get; } = Brushes.Black;

        public override Brush BackgroundBrush { get; } = Brushes.White;

        public override CultureInfo CultureInfo { get; } = CultureInfo.CurrentCulture;

        public override TextEffectCollection TextEffects { get; } = new TextEffectCollection();
    }
}
