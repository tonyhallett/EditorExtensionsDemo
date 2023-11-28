using Microsoft.VisualStudio.Text.Formatting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

namespace EditorExtensionsDemo
{
    public class ClassificationTextSource : TextSource
    {
        private readonly TextFormattingRunProperties mergedTextFormattingRunProperties;
        private readonly TextFormattingRunProperties ownTextFormattingRunProperties;
        private TextFormattingRunProperties currentTextFormattingRunProperties;
        public ClassificationTextSource(string classification, TextFormattingRunProperties mergedTextFormattingRunProperties, TextFormattingRunProperties ownTextFormattingRunProperties)
        {
            this.Classification = classification;
            this.mergedTextFormattingRunProperties = mergedTextFormattingRunProperties;
            this.ownTextFormattingRunProperties = EnsureFilled(ownTextFormattingRunProperties);
            currentTextFormattingRunProperties = this.ownTextFormattingRunProperties;
        }

        // because of this going to have to display the details filled in a popup
        public static TextFormattingRunProperties EnsureFilled(TextFormattingRunProperties textFormattingRunProperties)
        {
            if (textFormattingRunProperties.Typeface == null)
            {
                return textFormattingRunProperties.SetTypeface(new Typeface("Consolas"));
            }
            return textFormattingRunProperties;
        }

        public string Classification { get; }

        public override TextSpan<CultureSpecificCharacterBufferRange> GetPrecedingText(int textSourceCharacterIndexLimit)
        {
            throw new NotImplementedException();
        }

        public override int GetTextEffectCharacterIndexFromTextSourceCharacterIndex(int textSourceCharacterIndex)
        {
            return 0;
        }
        private bool providedTextRun = false;
        public override TextRun GetTextRun(int textSourceCharacterIndex)
        {
            if (providedTextRun)
            {
                return new TextEndOfLine(textSourceCharacterIndex);
            }
            providedTextRun = true;
            return new TextCharacters(this.Classification, currentTextFormattingRunProperties);
        }

        internal void Next()
        {
            providedTextRun = false;
            currentTextFormattingRunProperties = mergedTextFormattingRunProperties;
        }
    }

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

    internal class ClassificationTreeItem : FrameworkElement
    {
        private readonly TextFormatter textFormatter;

        public ClassificationTreeItem()
        {
            this.textFormatter = TextFormatter.Create();
            
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            return new Size(300, 50);
        }
        protected override void OnRender(DrawingContext drawingContext)
        {
            var treeItem = this.DataContext as TreeItem;
            var classificationTextSource = new ClassificationTextSource(treeItem.Name, treeItem.MergedTextFormattingRunProperties, treeItem.OwnTextFormattingRunProperties);
            var textLine = textFormatter.FormatLine(classificationTextSource, 0, this.ActualWidth, new DefaultTextParagraphProperties(), null);
            classificationTextSource.Next();
            var textLine2 = textFormatter.FormatLine(classificationTextSource, 0, this.ActualWidth, new DefaultTextParagraphProperties(), null);
            textLine.Draw(drawingContext, new Point(0, 0), InvertAxes.None);
            textLine2.Draw(drawingContext, new Point(0, 30), InvertAxes.None);
        }
    }
}
