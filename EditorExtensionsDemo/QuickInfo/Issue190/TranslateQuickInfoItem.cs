using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace EditorExtensionsDemo
{

    public class LegacyQuickInfoMetadata : IContentTypeMetadata, IOrderable
    {
        public LegacyQuickInfoMetadata(IDictionary<string, object> data)
        {
            object obj1;
            data.TryGetValue(nameof(Name), out obj1);
            object obj2;
            data.TryGetValue(nameof(ContentTypes), out obj2);
            object obj3;
            data.TryGetValue(nameof(Before), out obj3);
            object obj4;
            data.TryGetValue(nameof(After), out obj4);
            this.ContentTypes = (IEnumerable<string>)obj2;
            this.Name = (string)obj1;
            this.Before = (IEnumerable<string>)obj3;
            this.After = (IEnumerable<string>)obj4;
        }

        internal LegacyQuickInfoMetadata(
          string name,
          IEnumerable<string> contentTypes,
          IEnumerable<string> before,
          IEnumerable<string> after)
        {
            this.Name = name;
            this.ContentTypes = contentTypes;
            this.Before = before ?? Enumerable.Empty<string>();
            this.After = after ?? Enumerable.Empty<string>();
        }

        public IEnumerable<string> ContentTypes { get; }

        public string Name { get; }

        public IEnumerable<string> Before { get; }

        public IEnumerable<string> After { get; }
    }

    public static class TranslateQuickInfoItem
    {
        private class DemoIssue190Services
        {
            public DemoIssue190Services()
            {
                var componentModel = VS.GetRequiredService<SComponentModel, IComponentModel2>();
                SetRoslynQuickInfoProvider(componentModel);
                ActiveViewAccessor = VS.GetMefService<IActiveViewAccessor>();
                QuickInfoBroker = VS.GetMefService<IAsyncQuickInfoBroker>();

            }

            private void SetRoslynQuickInfoProvider(IComponentModel2 componentModel)
            {
                var lazyAsyncQuickInfoSourceProviders = componentModel.DefaultExportProvider.GetExports<IAsyncQuickInfoSourceProvider, LegacyQuickInfoMetadata>().ToList();
                RoslynQuickInfoSourceProvider = lazyAsyncQuickInfoSourceProviders.First(lazyAsyncQuickInfoSourceProvider =>
                {
                    return lazyAsyncQuickInfoSourceProvider.Metadata.Name == "RoslynQuickInfoProvider";
                }).Value;
            }

            public IActiveViewAccessor ActiveViewAccessor { get; }
            public IAsyncQuickInfoBroker QuickInfoBroker { get; }
            public IAsyncQuickInfoSourceProvider RoslynQuickInfoSourceProvider { get; private set; }
        }
        private static DemoIssue190Services servicesForIssue;
        private static DemoIssue190Services ServicesForIssue
        {
            get
            {
                if (servicesForIssue == null)
                {
                    servicesForIssue = new DemoIssue190Services();
                }
                return servicesForIssue;
            }
        }

        public static async Task InvokingRoslynProviderAsync(int position)
        {
            var (activeView, trackingPoint) = GetActiveViewAndTrackingPoint(position);
            var roslynSource = ServicesForIssue.RoslynQuickInfoSourceProvider.TryCreateQuickInfoSource(activeView.TextBuffer);
            var triggerPoint = trackingPoint.GetPoint(activeView.TextBuffer.CurrentSnapshot);
            var quickInfoItem = await roslynSource.GetQuickInfoItemAsync(new FakeSession(triggerPoint), CancellationToken.None);
            var quickInfoItemToTranslate = quickInfoItem.Item;
        }

        public static async Task BrokerGetQuickInfoItemsAsync(int position)
        {
            var (activeView, trackingPoint) = GetActiveViewAndTrackingPoint(position);
            var quickInfoItems = await ServicesForIssue.QuickInfoBroker.GetQuickInfoItemsAsync(activeView, trackingPoint, CancellationToken.None);
            var quickInfoItemsToTranslate = quickInfoItems.Items;
            // Translate !
        }

        private static (IWpfTextView, ITrackingPoint) GetActiveViewAndTrackingPoint(int position)
        {
            var activeView = ServicesForIssue.ActiveViewAccessor.ActiveView;
            var trackingPoint = activeView.TextBuffer.CurrentSnapshot.CreateTrackingPoint(position, PointTrackingMode.Positive);
            return (activeView, trackingPoint);
        }
    }



}
