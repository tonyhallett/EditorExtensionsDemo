using Microsoft.VisualStudio.Text.Adornments;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Editor.OptionsExtensionMethods;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace EditorExtensionsDemo.QuickInfo
{
    [Export(typeof(IViewElementFactory))]
    [TypeConversion(from: typeof(ViewElementFactoryModel), to: typeof(System.Windows.UIElement))]
    [Order] // any metadata that is IOrderable must have a Name **********************************
    [Name("My view element factory")]
    internal class DemoViewElementFactory : IViewElementFactory
    {
        public TView CreateViewElement<TView>(ITextView textView, object model) where TView : class
        {
            var myModel = model as ViewElementFactoryModel;
            return new TextBlock() { Text = myModel.Num.ToString() } as TView;
        }
    }
}
