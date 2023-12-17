namespace EditorExtensionsDemo
{
    internal interface IWindowService
    {
        bool? ShowDialogWindowModal(object dataContext);
        void ShowDialogWindow(object dataContext);
        Task ShowToolWindowAsync(object dataContext);
    }
}
