using CommunityToolkit.Mvvm.ComponentModel;
using EditorExtensionsDemo.ToolWindows;
using System.ComponentModel.Composition;

namespace EditorExtensionsDemo
{
    [Export(typeof(CaretClassificationViewModel))]
    internal class CaretClassificationViewModel : ObservableObject
    {
        private string caretClassification;
        public string CaretClassification
        {
            get => caretClassification;
            set => SetProperty(ref caretClassification, value);
        }

        [ImportingConstructor]
        public CaretClassificationViewModel(IActiveViewCaretClassification caretListener)
        {
            caretListener.ClassificationChanged += (s, e) =>
            {
                CaretClassification = caretListener.Classification;
            };
            CaretClassification = caretListener.Classification;
        }
    }
}
