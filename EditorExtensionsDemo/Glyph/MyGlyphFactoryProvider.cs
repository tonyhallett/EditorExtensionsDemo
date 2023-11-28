using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Glyph
{
    [ContentType("code")]
    [TagType(typeof(MyGlyph))]
    [Order(Before = "VsTextMarker")]
    [Name("TestGlyphFactoryProvider")]
    [Export(typeof(IGlyphFactoryProvider))]
    public class MyGlyphFactoryProvider : IGlyphFactoryProvider
    {

        public IGlyphFactory GetGlyphFactory(IWpfTextView view, IWpfTextViewMargin margin)
        {
            return new MyGlyphFactory();
        }

        private class MyGlyphFactory : IGlyphFactory
        {
            public UIElement GenerateGlyph(IWpfTextViewLine line, IGlyphTag tag)
            {
                if(tag is not MyGlyph)
                {
                    return null;
                }
                return new Ellipse
                {
                    Fill = Brushes.Red,
                    Width = 10,
                    Height = 10
                };
            }
        }
    }
}