using Microsoft.VisualStudio.Text.Classification;

namespace EditorExtensionsDemo
{
    internal class ClassificationTypeInfo
    {
        public ClassificationTypeInfo(IClassificationType classificationType, bool isTransient)
        {
            ClassificationType = classificationType;
            IsTransient = isTransient;
        }

        public IClassificationType ClassificationType { get; }
        public bool IsTransient { get; }
        public bool HasClassificationTypeDefinition { get; set; }
        public DefinitionFrom DefinitionFrom { get; internal set; }
    }
}
