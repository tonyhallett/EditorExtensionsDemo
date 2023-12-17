using Microsoft.VisualStudio.Text.Classification;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.Composition;

namespace EditorExtensionsDemo
{
    [Export(typeof(ClassificationTypesNotInRegistry))]
    internal class ClassificationTypesNotInRegistry
    {
        private readonly IEnumerable<Lazy<EditorFormatDefinition, IClassificationFormatMetadata>> classificationEditorFormatDefinitions;
        private readonly IClassificationTypeRegistryService classificationTypeRegistryService;

        [ImportingConstructor]
        public ClassificationTypesNotInRegistry(
            [ImportMany]
            IEnumerable<Lazy<EditorFormatDefinition, IClassificationFormatMetadata>> classificationEditorFormatDefinitions,
            IClassificationTypeRegistryService classificationTypeRegistryService
        )
        {
            this.classificationEditorFormatDefinitions = classificationEditorFormatDefinitions;
            this.classificationTypeRegistryService = classificationTypeRegistryService;
        }

        public List<ClassificationEditorFormatDefinitionClassificationTypesNotInRegistry> GetClassificationTypesNotInRegistry()
        {
            return classificationEditorFormatDefinitions.Select(l =>
            {
                var classificationTypeNames = l.Metadata.ClassificationTypeNames;
                var typesNotInRegistry = classificationTypeNames.Where(ctn => classificationTypeRegistryService.GetClassificationType(ctn) == null).ToList();
                if (typesNotInRegistry.Any())
                {
                    return new ClassificationEditorFormatDefinitionClassificationTypesNotInRegistry(typesNotInRegistry, l);
                }
                return null;
            }).Where(ctnir => ctnir != null).ToList();

        }
    }
}
