using Microsoft.VisualStudio.Text.Classification;
using System.Collections.Generic;

namespace EditorExtensionsDemo
{
    public class MyClassificationFormatMap
    {
        private bool dirty;
        private readonly Dictionary<string, ClassificationTextFormattingRunProperties> textFormattingPropertiesLookup = new();
        private readonly string category;
        private IClassificationFormatMap classificationFormatMap;
        private readonly IClassificationTypeRegistryService classificationTypeRegistryService;
        private readonly IClassificationFormatMapService classificationFormatMapService;
        private Action formatChangedHandler;

        public MyClassificationFormatMap(
            string category,
            IClassificationTypeRegistryService classificationTypeRegistryService,
            IClassificationFormatMapService classificationFormatMapService
        )
        {
            this.category = category;
            this.classificationTypeRegistryService = classificationTypeRegistryService;
            this.classificationFormatMapService = classificationFormatMapService;
            SetClassificationFormatMap();
        }
        
        public void AddHandler(Action action)
        {
            formatChangedHandler = action;
        }

        public void RemoveHandler()
        {
            formatChangedHandler = null;
        }

        private void SetClassificationFormatMap()
        {
            classificationFormatMap = classificationFormatMapService.GetClassificationFormatMap(category);
            classificationFormatMap.ClassificationFormatMappingChanged += ClassificationFormatMap_ClassificationFormatMappingChanged;
        }

        private void ClassificationFormatMap_ClassificationFormatMappingChanged(object sender, EventArgs e)
        {
            formatChangedHandler?.Invoke();
            dirty = true;
            classificationFormatMap.ClassificationFormatMappingChanged -= ClassificationFormatMap_ClassificationFormatMappingChanged;
        }

        public ClassificationTextFormattingRunProperties GetClassificationTextFormattingRunProperties(string classification)
        {
            if (dirty)
            {
                SetClassificationFormatMap();
                textFormattingPropertiesLookup.Clear();
                dirty = false;
            }
            if(textFormattingPropertiesLookup.TryGetValue(classification, out var classificationTextFormatinngRunProperties)){
                return classificationTextFormatinngRunProperties;
            }
            var ct = classificationTypeRegistryService.GetClassificationType(classification);
            var classificationTextFormattingRunProperties =  new ClassificationTextFormattingRunProperties(
                classificationFormatMap.GetExplicitTextProperties(ct),
                classificationFormatMap.GetTextProperties(ct)
            );
            textFormattingPropertiesLookup.Add(classification, classificationTextFormattingRunProperties);
            return classificationTextFormattingRunProperties;
        }

        

    }
}
