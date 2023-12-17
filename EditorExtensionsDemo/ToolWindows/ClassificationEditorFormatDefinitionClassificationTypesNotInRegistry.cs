using Microsoft.VisualStudio.Text.Classification;
using System.Collections.Generic;

namespace EditorExtensionsDemo
{
    internal class ClassificationEditorFormatDefinitionClassificationTypesNotInRegistry
    {
        public ClassificationEditorFormatDefinitionClassificationTypesNotInRegistry(
            List<string> typesNotInRegistry, 
            Lazy<EditorFormatDefinition, IClassificationFormatMetadata> editorFormatDefinition
        )
        {
            TypesNotInRegistry = typesNotInRegistry;
            EditorFormatDefinition = editorFormatDefinition;
        }

        public List<string> TypesNotInRegistry { get; }
        public Lazy<EditorFormatDefinition, IClassificationFormatMetadata> EditorFormatDefinition { get; }
    }
}
