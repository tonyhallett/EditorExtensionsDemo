namespace EditorExtensionsDemo.ToolWindows
{
    interface IActiveViewCaretClassification
    {
        string Classification { get; }
        event EventHandler ClassificationChanged;
        event EventHandler TextChanges;
        event EventHandler TextViewOpened;
    }
}
