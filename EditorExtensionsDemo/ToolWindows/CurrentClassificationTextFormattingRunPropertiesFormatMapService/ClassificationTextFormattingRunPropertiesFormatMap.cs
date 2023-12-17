using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Formatting;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EditorExtensionsDemo
{
    internal class ClassificationTextFormattingRunPropertiesFormatMap
    {
        private bool dirty;
        private readonly Dictionary<string, ClassificationTextFormattingRunProperties> textFormattingPropertiesLookup = new();
        public readonly string Category;
        public IClassificationFormatMap ClassificationFormatMap { get; private set; }
        private readonly IClassificationFormatMapProvider classificationFormatMapProvider;
        private Action formatChangedHandler;

        public ClassificationTextFormattingRunPropertiesFormatMap(
            string category,
            IClassificationFormatMapProvider classificationFormatMapProvider
        )
        {
            this.Category = category;
            this.classificationFormatMapProvider = classificationFormatMapProvider;
            SetClassificationFormatMap();
        }
        
        public void AddChangeHandler(Action action)
        {
            formatChangedHandler = action;
        }

        public void RemoveChangeHandler()
        {
            formatChangedHandler = null;
        }

        #region pass throughs
        public ReadOnlyCollection<IClassificationType> CurrentPriorityOrder => ClassificationFormatMap.CurrentPriorityOrder;
        
        public TextFormattingRunProperties DefaultTextFormattingRunProperties => ClassificationFormatMap.DefaultTextProperties;
        
        public string GetEditorFormatMapKey(IClassificationType classificationType)
        {
            return ClassificationFormatMap.GetEditorFormatMapKey(classificationType);
        }
        
        public void SwapPriority(IClassificationType first, IClassificationType second)
        {
            ClassificationFormatMap.SwapPriorities(first, second);
        }

        #endregion

        private void SetClassificationFormatMap()
        {
            ClassificationFormatMap = classificationFormatMapProvider.GetClassificationFormatMap(Category);
            ClassificationFormatMap.ClassificationFormatMappingChanged += ClassificationFormatMap_ClassificationFormatMappingChanged;
        }

        private void ClassificationFormatMap_ClassificationFormatMappingChanged(object sender, EventArgs e)
        {
            dirty = true;
            formatChangedHandler?.Invoke();
            ClassificationFormatMap.ClassificationFormatMappingChanged -= ClassificationFormatMap_ClassificationFormatMappingChanged;
        }

        public ClassificationTextFormattingRunProperties GetClassificationTextFormattingRunProperties(IClassificationType classificationType)
        {
            if (dirty)
            {
                SetClassificationFormatMap();
                textFormattingPropertiesLookup.Clear();
                dirty = false;
            }

            return GetOrCreateClassificationTextFormattingRunProperties(classificationType);
        }

        private ClassificationTextFormattingRunProperties GetOrCreateClassificationTextFormattingRunProperties(IClassificationType classificationType)
        {
            if (!textFormattingPropertiesLookup.TryGetValue(classificationType.Classification, out var classificationTextFormattingRunProperties))
            {
                classificationTextFormattingRunProperties = new ClassificationTextFormattingRunProperties(
                    ClassificationFormatMap.GetExplicitTextProperties(classificationType),
                    ClassificationFormatMap.GetTextProperties(classificationType),
                    ClassificationFormatMap.DefaultTextProperties
                );
                textFormattingPropertiesLookup.Add(classificationType.Classification, classificationTextFormattingRunProperties);
            }

            return classificationTextFormattingRunProperties;
        }

    }
}
