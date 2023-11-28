using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Glyph
{
    [Export(typeof(ITaggerProvider))]
    [ContentType("code")]
    [TagType(typeof(MyGlyph))]
    [Name("GlyphTaggerProvider")]
    class GlyphTaggerProvider : ITaggerProvider
    {
        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            return new MyGlyphTagger() as ITagger<T>;
        }
    }

    [Export(typeof(IViewTaggerProvider))]
    [ContentType("code")]
    [TagType(typeof(MyViewGlyph))]
    [Name("GlyphViewTaggerProvider")]
    class GlyphViewTaggerProvider : IViewTaggerProvider
    {
        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
        {
            return new MyGlyphTagger() as ITagger<T>;
        }
    }

    public class MyGlyphTagger : ITagger<MyGlyph>
    {
        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public IEnumerable<ITagSpan<MyGlyph>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            return new List<ITagSpan<MyGlyph>>();
        }
    }

    public class MyViewGlyphTagger : ITagger<MyViewGlyph>
    {
        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public IEnumerable<ITagSpan<MyViewGlyph>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            return new List<ITagSpan<MyViewGlyph>>();
        }
    }
}