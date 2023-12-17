global using Community.VisualStudio.Toolkit;
global using Microsoft.VisualStudio.Shell;
global using System;
global using Task = System.Threading.Tasks.Task;
using EditorExtensionsDemo.ToolWindows.Controls.Dialogs;
using System.Runtime.InteropServices;
using System.Threading;

namespace EditorExtensionsDemo
{
    [ProvideBindingPath]
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageGuids.EditorExtensionsDemoString)]
#pragma warning disable VSSDK003 // Support async tool windows
    [ProvideToolWindow(typeof(ClassificationToolWindow.Pane))]
    [ProvideToolWindow(typeof(ClassificationTypesNotInRegistryToolWindow.Pane))]
#pragma warning restore VSSDK003 // Support async tool windows
    public sealed class EditorExtensionsDemoPackage : ToolkitPackage
    {
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await this.RegisterCommandsAsync();
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            ClassificationToolWindow.Initialize(this);
            ClassificationTypesNotInRegistryToolWindow.Initialize(this);
            //var lazyEditorFormatDefinitions = componentModel.DefaultExportProvider.GetExports<EditorFormatDefinition, IEditorFormatMetadata>().ToList();
            //lazyEditorFormatDefinitions.OrderBy(l => l.Metadata.Name).ToList().ForEach(l =>
            //{
            //    Debug.WriteLine(l.Metadata.Name);
            //});
            //var componentModel = await ServiceProvider.GetGlobalServiceAsync<SComponentModel, IComponentModel2>();

            //var contentTypeRegistryService = componentModel.GetService<IContentTypeRegistryService>();
            //contentTypeRegistryService.ContentTypes.ToList().ForEach(contentType =>
            //{
            //    Debug.WriteLine(contentType.DisplayName);
            //});

            //IActiveViewAccessor

            //var taggerProviders = componentModel.GetExtensions<ITaggerProvider>().Select(tp => (object)tp);
            //taggerProviders = taggerProviders.Concat(componentModel.GetExtensions<IViewTaggerProvider>());
            //var structureTagProviders = taggerProviders.Select(tp => tp.GetType()).Where(tpt => tpt.GetCustomAttributes<TagTypeAttribute>().Any(tta => typeof(IOutliningRegionTag).IsAssignableFrom(tta.TagTypes)));
            //foreach(var structureTagProvider in structureTagProviders)
            //{
            //    Debug.WriteLine(structureTagProvider.FullName);
            //}
            //var contentTypeRegistryService = componentModel.GetService<IContentTypeRegistryService>();
            //var roslynLanguagesContentType = contentTypeRegistryService.GetContentType("Roslyn Languages");
            //var languageserverBaseContentType = contentTypeRegistryService.GetContentType("languageserver-base");
            //var textMateStructureContentType = contentTypeRegistryService.GetContentType("code-languageserver-textmate-structure");
            //var projectionContentType = contentTypeRegistryService.GetContentType("projection");


        }
    }
}