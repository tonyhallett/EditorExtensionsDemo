using System.ComponentModel;

namespace EditorExtensionsDemo
{
    public interface IEditorFormatMetadata
    {
        string Name { get; }

        [DefaultValue(false)]
        bool UserVisible { get; }

        [DefaultValue(0)]
        int Priority { get; }
    }
}
