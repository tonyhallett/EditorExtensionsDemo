using Microsoft.VisualStudio.Text.Formatting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

namespace EditorExtensionsDemo
{
    internal class ClassificationTreeItem : FrameworkElement
    {
        private readonly TextFormatter textFormatter;

        public ClassificationTreeItem()
        {
            this.textFormatter = TextFormatter.Create();
            
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var treeItem = this.DataContext as TreeItem;
            var height = treeItem.ShowExplicitTextProperties ? 50 : 30;
            return new Size(300, height);
        }
        protected override void OnRender(DrawingContext drawingContext)
        {
            var treeItem = this.DataContext as TreeItem;
            var safeOwn = EnsureFilled(treeItem.OwnTextFormattingRunProperties, treeItem.DefaultTextFormattingRunProperties.Typeface);
            var first = treeItem.ShowExplicitTextProperties ? safeOwn : treeItem.MergedTextFormattingRunProperties;
            var classificationTextSource = new ClassificationTextSource(treeItem.Name, first, treeItem.MergedTextFormattingRunProperties);
            var textLine = textFormatter.FormatLine(classificationTextSource, 0, this.ActualWidth, new DefaultTextParagraphProperties(), null);
            textLine.Draw(drawingContext, new Point(0, 0), InvertAxes.None);
            if (treeItem.ShowExplicitTextProperties)
            {
                classificationTextSource.Next();
                var textLine2 = textFormatter.FormatLine(classificationTextSource, 0, this.ActualWidth, new DefaultTextParagraphProperties(), null);
                textLine2.Draw(drawingContext, new Point(0, 30), InvertAxes.None);
            }
        }

        public static TextFormattingRunProperties EnsureFilled(TextFormattingRunProperties textFormattingRunProperties, Typeface defaultTypeface)
        {
            if (textFormattingRunProperties.Typeface == null)
            {
                return textFormattingRunProperties.SetTypeface(defaultTypeface);
            }
            return textFormattingRunProperties;
        }
    }
}
