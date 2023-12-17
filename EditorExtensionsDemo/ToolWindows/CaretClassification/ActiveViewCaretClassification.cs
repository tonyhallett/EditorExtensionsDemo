using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;

namespace EditorExtensionsDemo.ToolWindows
{

    [Export(typeof(IWpfTextViewCreationListener))]
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    [Export(typeof(IActiveViewCaretClassification))]
    internal class ActiveViewCaretClassification : IWpfTextViewCreationListener, IActiveViewCaretClassification
    {
        private WpfTextViewCaretClassification withAggregateFocus;
        private readonly IClassifierAggregatorService classifierAggregatorService;
        private readonly Dictionary<ITextView, WpfTextViewCaretClassification> lookup = new();

        [ImportingConstructor]
        public ActiveViewCaretClassification(IClassifierAggregatorService classifierAggregatorService)
        {
            this.classifierAggregatorService = classifierAggregatorService;
        }
        
        public event EventHandler ClassificationChanged;
        public event EventHandler TextChanges;
        public event EventHandler TextViewOpened;

        public string Classification
        {
            get
            {
                if(withAggregateFocus != null)
                {
                    return withAggregateFocus.Classification;
                }
                return null;
            }
        }

        public void TextViewCreated(IWpfTextView textView)
        {
            //could use MouseHover
            
            textView.GotAggregateFocus += TextView_GotAggregateFocus;
            textView.LostAggregateFocus += TextView_LostAggregateFocus;
            textView.LayoutChanged += TextView_LayoutChanged;
            textView.Caret.PositionChanged += Caret_PositionChanged;
            textView.Closed += TextView_Closed;
            
            // do you need to get the classifier each time ?
            var info = new WpfTextViewCaretClassification(textView, classifierAggregatorService.GetClassifier(textView.TextBuffer));
            info.UpdateClassification();
            lookup.Add(textView, info);

            TextViewOpened?.Invoke(this, EventArgs.Empty);
        }

        private void TextView_LostAggregateFocus(object sender, EventArgs e)
        {
            GotLostAggregateFocus(sender, false);
        }

        private void TextView_GotAggregateFocus(object sender, EventArgs e)
        {
            GotLostAggregateFocus(sender, true);
        }

        private void GotLostAggregateFocus(object sender, bool got)
        {
            var textView = sender as ITextView;
            var filePath = textView.SafeGetPath();
            Log($"Got/Lost AggregateFocus: {got} {filePath}");
            if (got)
            {
                withAggregateFocus = lookup[textView];
                ClassificationChanged?.Invoke(this, EventArgs.Empty);
            }
            // not removing withAggregateFocus if lost focus
        }

        private void TextView_LayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            var changes = e.OldSnapshot.Version.Changes;
            if(changes != null)
            {
                var textView = sender as ITextView;
                var filePath = textView.SafeGetPath();
                Log($"Layout changed - {filePath}");
                var textViewCaretClassification = lookup[textView];
                textViewCaretClassification.UpdateClassification();
                ClassificationChanged?.Invoke(this, EventArgs.Empty);
                TextChanges?.Invoke(this, EventArgs.Empty);
            }
            
        }

        private void Caret_PositionChanged(object sender, CaretPositionChangedEventArgs e)
        {
            var filePath = e.TextView.SafeGetPath();
            Log($"Caret Position Changed: {filePath}");
            var info = lookup[e.TextView];
            info.UpdateClassification();
            ClassificationChanged?.Invoke(this, EventArgs.Empty);
        }

        private void TextView_Closed(object sender, EventArgs e)
        {
            var textView = sender as ITextView;
            Log($"TextView Closed - ${textView.SafeGetPath()}");
            lookup.Remove(textView);
        }

        private readonly bool shouldLog = true;
        private void Log(string message)
        {
            if (shouldLog)
            {
                Debug.WriteLine(message);
            }
        }
    }
}
