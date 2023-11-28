using Microsoft.VisualStudio.Text.Classification;
using System.Windows.Media;

namespace EditorExtensionsDemo.QuickInfo
{
    public class MyClassificationFormatDefinition : ClassificationFormatDefinition
    {
        public MyClassificationFormatDefinition()
        {
            this.TextDecorations = new System.Windows.TextDecorationCollection
            {
                System.Windows.TextDecorations.Strikethrough
            };

            this.TextEffects = new TextEffectCollection
            {
                new TextEffect
                {
                    PositionStart = 0,
                    PositionCount = 1,
                    Foreground = Brushes.Red,
                    Transform = new RotateTransform(45)
                }
            };
            this.BackgroundColor = Colors.LightBlue;
            this.ForegroundColor = Colors.Yellow;
            this.FontTypeface = new System.Windows.Media.Typeface("Arial");
            

        }
    }
}
