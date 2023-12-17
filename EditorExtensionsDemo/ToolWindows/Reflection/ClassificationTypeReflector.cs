using Microsoft.VisualStudio.Text.Classification;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;

namespace EditorExtensionsDemo
{
    [Export(typeof(IClassificationTypeReflector))]
    public class ClassificationTypeReflector : IClassificationTypeReflector
    {
        private readonly IClassificationTypeRegistryService classificationTypeRegistryService;
        private const BindingFlags NonPublicInstance = BindingFlags.NonPublic | BindingFlags.Instance;
        private readonly PropertyInfo classificationTypesPropertyInfo;
        private readonly FieldInfo transientClassificationTypesFieldInfo;

        [ImportingConstructor]
        public ClassificationTypeReflector(IClassificationTypeRegistryService classificationTypeRegistryService)
        {
            this.classificationTypeRegistryService = classificationTypeRegistryService;
            var classificationTypeRegistryServiceType = classificationTypeRegistryService.GetType();
            classificationTypesPropertyInfo = classificationTypeRegistryServiceType.GetProperty("ClassificationTypes", NonPublicInstance);
            transientClassificationTypesFieldInfo = classificationTypeRegistryServiceType.GetField("_transientClassificationTypes", NonPublicInstance);
        }

        private Dictionary<string, IClassificationType> GetClassificationTypes(object classificationTypes)
        {
            var keys = classificationTypes.GetType().GetProperty("Keys").GetValue(classificationTypes) as IEnumerable<string>;
            var values = classificationTypes.GetType().GetProperty("Values").GetValue(classificationTypes) as IEnumerable<IClassificationType>;
            return keys.Zip(values, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v);
        }

        public ClassificationTypes Get()
        {
            var classificationTypes = new ClassificationTypes
            {
                NonTransient = GetClassificationTypes(
                classificationTypesPropertyInfo.GetValue(classificationTypeRegistryService)
            )
            };

            // this can be null
            var transientClassificationTypes = transientClassificationTypesFieldInfo.GetValue(classificationTypeRegistryService);
            if (transientClassificationTypes != null)
            {
                classificationTypes.Transient = GetClassificationTypes(transientClassificationTypes);
            }

            return classificationTypes;
        }
    }
}
