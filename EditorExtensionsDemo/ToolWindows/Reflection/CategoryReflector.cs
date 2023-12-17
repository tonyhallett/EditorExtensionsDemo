using Microsoft.VisualStudio.Text.Classification;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;

namespace EditorExtensionsDemo
{
    [Export(typeof(ICategoryReflector))]
    internal class CategoryReflector : ICategoryReflector
    {
        private readonly FieldInfo classificationFormatMapsFieldInfo;
        private readonly IClassificationFormatMapService classificationFormatMapService;

        [ImportingConstructor]
        public CategoryReflector(IClassificationFormatMapService classificationFormatMapService)
        {
            this.classificationFormatMapService = classificationFormatMapService;
            classificationFormatMapsFieldInfo = classificationFormatMapService.GetType().GetField("_classificationFormatMaps", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public List<string> GetCategories()
        {
            if (classificationFormatMapsFieldInfo == null)
            {
                return new List<string>();
            }
            var lookup = classificationFormatMapsFieldInfo.GetValue(classificationFormatMapService) as Dictionary<string, IClassificationFormatMap>;
            return lookup.Keys.ToList();

        }
    }
}
