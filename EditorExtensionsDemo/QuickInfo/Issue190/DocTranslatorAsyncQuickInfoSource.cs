using Microsoft.CodeAnalysis.QuickInfo;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EditorExtensionsDemo.QuickInfo
{
    public class DocTranslatorAsyncQuickInfoSource : IAsyncQuickInfoSource
    {
        private readonly ITextBuffer _subjectBuffer;

        public DocTranslatorAsyncQuickInfoSource(ITextBuffer subjectBuffer)
        {
            _subjectBuffer = subjectBuffer;
        }

        public void Dispose()
        {
        }


        public async Task<Microsoft.VisualStudio.Language.Intellisense.QuickInfoItem> GetQuickInfoItemAsync(IAsyncQuickInfoSession session, CancellationToken cancellationToken)
        {
            var triggerPoint = session.GetTriggerPoint(_subjectBuffer.CurrentSnapshot);
            var position = triggerPoint.Value;


            if (!triggerPoint.HasValue)
                return null;

            var snapshot = triggerPoint.Value.Snapshot;
            // Microsoft.CodeAnalysis.EditorFeatures.Text package needs to be installed
            var document = snapshot.GetOpenDocumentInCurrentContextWithChanges();
            
            if (document == null)
                return null;

            var service = QuickInfoService.GetService(document);
            if (service == null)
                return null;

            //Microsoft.CodeAnalysis.Features needs to be installed
            var item = await service.GetQuickInfoAsync(document, position, cancellationToken).ConfigureAwait(false);
            if (item != null)
            {
                // Translate !
            }
            return null;
        }
    }
}
