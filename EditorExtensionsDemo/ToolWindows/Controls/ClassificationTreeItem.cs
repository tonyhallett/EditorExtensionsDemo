using Microsoft.VisualStudio.Text.Formatting;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

namespace EditorExtensionsDemo
{
    internal class ClassificationTreeItem : FrameworkElement
    {
        private readonly TextFormatter textFormatter;
        private TreeItem treeItem;
        public ClassificationTreeItem()
        {
            this.textFormatter = TextFormatter.Create();
            this.DataContextChanged += ClassificationTreeItem_DataContextChanged;
        }

        private void ClassificationTreeItem_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            treeItem = DataContext as TreeItem;
            treeItem.TreeItemChanged += TreeItem_TreeItemChanged;
        }

        private void TreeItem_TreeItemChanged(object sender, EventArgs e)
        {
            this.InvalidateMeasure();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var height = treeItem.ShowExplicitTextProperties ? 50 : 30;
            return new Size(300, height);
        }
        protected override void OnRender(DrawingContext drawingContext)
        {
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
