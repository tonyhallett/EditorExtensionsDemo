namespace EditorExtensionsDemo
{
    internal interface ITreeProperties
    {
        bool ShowExplicitTextProperties { get; }
        event EventHandler TreeChanged;
    }
}
