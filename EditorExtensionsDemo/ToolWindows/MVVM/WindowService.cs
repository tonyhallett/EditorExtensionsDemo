using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Windows;
using Microsoft.VisualStudio.PlatformUI;

namespace EditorExtensionsDemo
{
    [Export(typeof(IWindowService))]
    internal class WindowService : IWindowService
    {
        public WindowService() { }
        public bool? ShowDialogWindowModal(object dataContext)
        {
            return InstantiateDialogWindowAndSetContext(dataContext).ShowModal();

        }

        public void ShowDialogWindow(object dataContext)
        {
            InstantiateDialogWindowAndSetContext(dataContext).Show();
        }

        private DialogWindow InstantiateDialogWindowAndSetContext(object dataContext)
        {
            var typePrefix = dataContext.GetType().Name.Replace("ViewModel", "");
            var executingAssembly = Assembly.GetExecutingAssembly();
            var dialogTypeName = typePrefix + "DialogWindow";
            var dialogWindowType = executingAssembly.GetType(dialogTypeName) ?? executingAssembly.GetTypes().FirstOrDefault(t => t.Name == dialogTypeName);
            var dialogWindow = Activator.CreateInstance(dialogWindowType) as DialogWindow;
            dialogWindow.DataContext = dataContext;
            return dialogWindow;
        }

        public async Task ShowToolWindowAsync(object dataContext)
        {
            var namePrefix = dataContext.GetType().Name.Replace("ViewModel", "");
            var toolWindowTypeName = namePrefix + "ToolWindow";
            var toolWindowType = typeof(BaseToolWindow<>).MakeGenericType(GetType(toolWindowTypeName));

            var toolWindowPaneTask = toolWindowType.GetMethod("ShowAsync", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[] { 0, true}) as System.Threading.Tasks.Task<ToolWindowPane>;
            var toolWindowPane = await toolWindowPaneTask;
            var content = toolWindowPane.Content as FrameworkElement;
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            content.DataContext = dataContext;
            
        }

        private Type GetType(string typeName)
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            var type = executingAssembly.GetType(typeName) ?? executingAssembly.GetTypes().FirstOrDefault(t => t.Name == typeName);
            return type;
        }
    }
}
