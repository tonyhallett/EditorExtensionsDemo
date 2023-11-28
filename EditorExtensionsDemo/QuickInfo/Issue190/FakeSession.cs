using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System.Collections.Generic;

namespace EditorExtensionsDemo
{
    public class FakeSession : IAsyncQuickInfoSession
    {
        private readonly SnapshotPoint triggerPoint;

        public FakeSession(SnapshotPoint triggerPoint)
        {
            this.triggerPoint = triggerPoint;
        }
        public ITrackingSpan ApplicableToSpan => throw new NotImplementedException();

        public IEnumerable<object> Content => throw new NotImplementedException();

        public bool HasInteractiveContent => throw new NotImplementedException();

        public QuickInfoSessionOptions Options => throw new NotImplementedException();

        public QuickInfoSessionState State => throw new NotImplementedException();

        public ITextView TextView => throw new NotImplementedException();

        public PropertyCollection Properties => throw new NotImplementedException();

        public event EventHandler<QuickInfoSessionStateChangedEventArgs> StateChanged;

        public Task DismissAsync()
        {
            throw new NotImplementedException();
        }

        public ITrackingPoint GetTriggerPoint(ITextBuffer textBuffer)
        {
            throw new NotImplementedException();
        }

        public SnapshotPoint? GetTriggerPoint(ITextSnapshot snapshot)
        {
            return triggerPoint;
        }
    }



}
