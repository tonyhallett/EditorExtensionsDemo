namespace EditorExtensionsDemo
{
    internal class ClassificationEditorFormatDefinitionClassificationTypesNotInRegistryViewModel
    {
        public string TypesNotInRegistry { get; }
        public string Name { get; }

        public ClassificationEditorFormatDefinitionClassificationTypesNotInRegistryViewModel(
            ClassificationEditorFormatDefinitionClassificationTypesNotInRegistry classificationEditorFormatDefinitionClassificationTypesNotInRegistry
        )
        {
            this.TypesNotInRegistry = String.Join(", ", classificationEditorFormatDefinitionClassificationTypesNotInRegistry.TypesNotInRegistry);
            this.Name = ((IEditorFormatMetadata)classificationEditorFormatDefinitionClassificationTypesNotInRegistry.EditorFormatDefinition.Metadata).Name;
        }
    }
}
