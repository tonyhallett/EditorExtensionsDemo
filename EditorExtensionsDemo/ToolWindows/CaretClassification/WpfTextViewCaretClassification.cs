using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using System.Linq;

namespace EditorExtensionsDemo.ToolWindows
{
    public class WpfTextViewCaretClassification
    {
        public WpfTextViewCaretClassification(IWpfTextView textView, IClassifier classifier)
        {
            this.TextView = textView;
            Classifier = classifier;
        }

        public IWpfTextView TextView { get; }
        public IClassifier Classifier { get; }
        public string Classification { get; private set; }
        public void UpdateClassification()
        {
            var classificationSpans = Classifier.GetClassificationSpans(new SnapshotSpan(TextView.Caret.Position.BufferPosition, 1));
            var classificationSpan = classificationSpans.FirstOrDefault();
            Classification = classificationSpan?.ClassificationType.Classification;
        }
    }
}
