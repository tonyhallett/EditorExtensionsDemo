using Glyph;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Shapes;

namespace EditorExtensionsDemo.Intra
{
    [Export(typeof(ITaggerProvider))]
    [Name("My.IntraTextAdornmentTaggerProvider")]
    [ContentType("text")]
    [TagType(typeof(IntraTextAdornmentTag))]
    internal class IntraTextAdornmentTaggerProvider : ITaggerProvider
    {
        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            //singleton ?
            
            return new IntraTextTagger() as ITagger<T>;
        }
    }

    internal class IntraTextTagger : ITagger<IntraTextAdornmentTag>
    {
        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
        private bool loggedLines;
        private List<Microsoft.VisualStudio.Text.Span> cache = new();

        // initially there is just one span with everything
        // then when make a change it will be called again only with the change !
        public IEnumerable<ITagSpan<IntraTextAdornmentTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if(spans.Count == 0)
            {
                yield return null;
            }
            var snapshot = spans[0].Snapshot;
            var lines = spans[0].Snapshot.Lines;
            foreach (var span in spans)
            {
                // NOTE - that cannot use intersect with spans from different snapshots
                var x = cache.Where(s => s.IntersectsWith(span)).ToList();
                var c = x.Count;
                var startLineNumber = span.Start.GetContainingLineNumber();
                var endLineNumber = span.End.GetContainingLineNumber();

                var lineRange = new List<ITextSnapshotLine> { snapshot.GetLineFromLineNumber(startLineNumber) };
                for(var i = startLineNumber + 1; i < endLineNumber; i++)
                {
                    lineRange.Add(snapshot.GetLineFromLineNumber(i));
                }
                
                foreach(var line in lineRange)
                {
                    var text = line.GetText();
                    var index = text.IndexOf("intra"); // would need to look for multiple occurences
                    if (index != -1)
                    {
                        Debug.WriteLine("Found intra");
                        var start = line.Start.Add(index);
                        //var snapshotSpan = new SnapshotSpan(start, 5);
                        var snapshotSpan = new SnapshotSpan(start, 0);

                        //don't want to add again - and because of length 0 !!!!!
                        cache.Add(snapshotSpan.Span);
                        var tag = new IntraTextAdornmentTag(CreateAdornment(), null, PositionAffinity.Predecessor);

                        yield return new TagSpan<IntraTextAdornmentTag>(snapshotSpan, tag);
                    }
                }
              
            }
            
        }

        private UIElement CreateAdornment()
        {
            var stackPanel = new StackPanel();
            stackPanel.Children.Add(new TextBlock { Text = "Intra" });
            stackPanel.Children.Add(new TextBlock { Text = "Yes" });
            stackPanel.Children.Add(new TextBlock { Text = "Indeed" });
            return stackPanel;
        }
    }

    [Export(typeof(ITaggerProvider))]
    [Name("My.SpaceNegotitatingTaggerProvider")]
    [ContentType("text")]
    [TagType(typeof(SpaceNegotiatingAdornmentTag))]
    internal class SpaceNegotiatingTaggerProvider : ITaggerProvider
    {
        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            //singleton ?

            return new SpaceNegotitatingTagger() as ITagger<T>;
        }
    }

    internal class SpaceNegotitatingTagger : ITagger<SpaceNegotiatingAdornmentTag>
    {
        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        // initially there is just one span with everything
        // then when make a change it will be called again only with the change !
        public IEnumerable<ITagSpan<SpaceNegotiatingAdornmentTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            var lines = spans[0].Snapshot.Lines;
            foreach (var span in spans)
            {
                var line = lines.FirstOrDefault(line => line.Extent.IntersectsWith(span));
                if (line != null)
                {
                    var text = line.GetText();
                    var index = text.IndexOf("!*#"); // would need to look for multiple occurences
                    if (index != -1)
                    {
                        var start = line.Start.Add(index);
                        var snapshotSpan = new SnapshotSpan(start, 5);
                        var tag = new SpaceNegotiatingAdornmentTag(10,10,10,10,10, PositionAffinity.Successor,new object(),this);

                        yield return new TagSpan<SpaceNegotiatingAdornmentTag>(snapshotSpan, tag);
                    }
                }
            }

        }
    }

}
