namespace EditorExtensionsDemo.Commands
{
    [Command("{47088d12-3fd3-4c36-b486-885b33fb16f4}", 0x0101)]
    internal sealed class ShowClassificationToolWindowCommand : BaseCommand<ShowClassificationToolWindowCommand>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            await ClassificationToolWindow.ShowAsync();
        }
    }
}
