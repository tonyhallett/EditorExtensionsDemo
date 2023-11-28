using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Formatting;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EditorExtensionsDemo
{
    public class MyClassificationFormatMap
    {
        private bool dirty;
        private readonly Dictionary<string, ClassificationTextFormattingRunProperties> textFormattingPropertiesLookup = new();
        public readonly string Category;
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
            this.Category = category;
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
            classificationFormatMap = classificationFormatMapService.GetClassificationFormatMap(Category);
            classificationFormatMap.ClassificationFormatMappingChanged += ClassificationFormatMap_ClassificationFormatMappingChanged;
        }

        private void ClassificationFormatMap_ClassificationFormatMappingChanged(object sender, EventArgs e)
        {
            formatChangedHandler?.Invoke();
            dirty = true;
            classificationFormatMap.ClassificationFormatMappingChanged -= ClassificationFormatMap_ClassificationFormatMappingChanged;
        }

        public ReadOnlyCollection<IClassificationType> CurrentPriorityOrder => classificationFormatMap.CurrentPriorityOrder;

        public TextFormattingRunProperties DefaultTextFormattingRunProperties => classificationFormatMap.DefaultTextProperties;
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
                classificationFormatMap.GetTextProperties(ct),
                classificationFormatMap.DefaultTextProperties
            );
            textFormattingPropertiesLookup.Add(classification, classificationTextFormattingRunProperties);
            return classificationTextFormattingRunProperties;
        }

        

    }
}
