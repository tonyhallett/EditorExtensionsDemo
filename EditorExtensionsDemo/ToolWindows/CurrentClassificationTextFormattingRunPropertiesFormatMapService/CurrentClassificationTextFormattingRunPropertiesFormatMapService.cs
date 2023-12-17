using Microsoft.VisualStudio.Text.Classification;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace EditorExtensionsDemo
{
    [Export(typeof(ICurrentClassificationTextFormattingRunPropertiesFormatMapService))]
    internal class CurrentClassificationTextFormattingRunPropertiesFormatMapService : ICurrentClassificationTextFormattingRunPropertiesFormatMapService, IClassificationFormatMapProvider
    {
        private ClassificationTextFormattingRunPropertiesFormatMap currentClassificationTextFormattingRunPropertiesFormatMap;
        private readonly IClassificationFormatMapService classificationFormatMapService;

        private Dictionary<string, ClassificationTextFormattingRunPropertiesFormatMap> ClassificationTextFormattingRunPropertiesFormatMaps { get; } = new Dictionary<string, ClassificationTextFormattingRunPropertiesFormatMap>();

        [ImportingConstructor]
        public CurrentClassificationTextFormattingRunPropertiesFormatMapService(IClassificationFormatMapService classificationFormatMapService)
        {
            this.classificationFormatMapService = classificationFormatMapService;
        }

        public ClassificationTextFormattingRunPropertiesFormatMap CurrentClassificationTextFormattingRunPropertiesFormatMap { 
            get => currentClassificationTextFormattingRunPropertiesFormatMap;
            private set
            {
                currentClassificationTextFormattingRunPropertiesFormatMap = value;
                OnCurrentClassificationTextFormattingRunPropertiesFormatMapChanged();
            }
        }

        public event EventHandler CurrentClassificationTextFormattingRunPropertiesFormatMapChanged;

        public void SetCategory(string category)
        {
            if (currentClassificationTextFormattingRunPropertiesFormatMap?.Category != category)
            {
                if (!ClassificationTextFormattingRunPropertiesFormatMaps.ContainsKey(category))
                {
                    CreateClassificationTextFormattingRunPropertiesFormatMap(category);
                }
                else
                {
                    var classificationFormatMap = ClassificationTextFormattingRunPropertiesFormatMaps[category];

                    currentClassificationTextFormattingRunPropertiesFormatMap?.RemoveChangeHandler();
                    CurrentClassificationTextFormattingRunPropertiesFormatMap = classificationFormatMap;
                    classificationFormatMap.AddChangeHandler(OnCurrentClassificationTextFormattingRunPropertiesFormatMapChanged);
                }
            }
        }

        private void CreateClassificationTextFormattingRunPropertiesFormatMap(string category)
        {
            currentClassificationTextFormattingRunPropertiesFormatMap?.RemoveChangeHandler();
            CurrentClassificationTextFormattingRunPropertiesFormatMap = new ClassificationTextFormattingRunPropertiesFormatMap(
                category,
                this
            );
            currentClassificationTextFormattingRunPropertiesFormatMap.AddChangeHandler(OnCurrentClassificationTextFormattingRunPropertiesFormatMapChanged);
            ClassificationTextFormattingRunPropertiesFormatMaps.Add(category, currentClassificationTextFormattingRunPropertiesFormatMap);
        }

        private void OnCurrentClassificationTextFormattingRunPropertiesFormatMapChanged()
        {
            CurrentClassificationTextFormattingRunPropertiesFormatMapChanged?.Invoke(this, new EventArgs());
        }

        public IClassificationFormatMap GetClassificationFormatMap(string category)
        {
            return classificationFormatMapService.GetClassificationFormatMap(category);
        }
    }
}
