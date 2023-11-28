namespace EditorExtensionsDemo
{
    public interface IClassificationFormatMetadata : IEditorFormatMetadata, IOrderable
    {
        string[] ClassificationTypeNames { get; }
    }
}
