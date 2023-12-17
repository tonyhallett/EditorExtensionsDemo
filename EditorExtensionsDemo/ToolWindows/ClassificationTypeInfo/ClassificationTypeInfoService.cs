using Microsoft.VisualStudio.Text.Classification;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.Composition;
using EditorExtensionsDemo.ToolWindows;

namespace EditorExtensionsDemo
{
    [Export(typeof(IClassificationTypeInfoService))]
    internal class ClassificationTypeInfoService : IClassificationTypeInfoService
    {
        private List<ClassificationTypeInfo> classificationTypeInfo;
        private readonly IClassificationTypeRegistryService classificationTypeRegistryService;
        private readonly IClassificationTypeReflector classificationTypeReflector;
        private readonly IEnumerable<Lazy<EditorFormatDefinition, IClassificationFormatMetadata>> classificationEditorFormatDefinitions;
        private readonly IEnumerable<Lazy<EditorFormatDefinition, IEditorFormatMetadata>> editorFormatDefinitions;
        private readonly ICurrentClassificationTextFormattingRunPropertiesFormatMapService classificationFormatMapService;
        private readonly IInvokeWithDelay invokeWithDelay;
        private readonly List<IClassificationTypeDefinitionMetadata> classificationTypeDefinitionMetadata;

        [ImportingConstructor]
        public ClassificationTypeInfoService(
            IClassificationTypeRegistryService classificationTypeRegistryService,
            IClassificationTypeReflector classificationTypeReflector,
            [ImportMany]
            IEnumerable<Lazy<EditorFormatDefinition, IClassificationFormatMetadata>> classificationEditorFormatDefinitions,
            [ImportMany]
            IEnumerable<Lazy<EditorFormatDefinition, IEditorFormatMetadata>> editorFormatDefinitions,
            [ImportMany]
            IEnumerable<Lazy<ClassificationTypeDefinition, IClassificationTypeDefinitionMetadata>> classificationTypeDefinitions,
            ICurrentClassificationTextFormattingRunPropertiesFormatMapService classificationFormatMapService,
            IActiveViewCaretClassification activeViewCaretClassification, 
            IInvokeWithDelay invokeWithDelay
            )
        {
            this.classificationTypeRegistryService = classificationTypeRegistryService;
            this.classificationTypeReflector = classificationTypeReflector;
            this.classificationEditorFormatDefinitions = classificationEditorFormatDefinitions;
            this.editorFormatDefinitions = editorFormatDefinitions;
            this.classificationFormatMapService = classificationFormatMapService;
            this.invokeWithDelay = invokeWithDelay;
            classificationTypeDefinitionMetadata = classificationTypeDefinitions.Select(l =>
            {
                return l.Metadata;
            }).ToList();
            activeViewCaretClassification.TextChanges += ActiveViewCaretClassification_TextChanges;
            activeViewCaretClassification.TextViewOpened += ActiveViewCaretClassification_TextViewOpened;
            

        }

        private void ActiveViewCaretClassification_TextViewOpened(object sender, EventArgs e)
        {
            ClassificationTypesPossiblyChanged();
        }

        private void ActiveViewCaretClassification_TextChanges(object sender, EventArgs e)
        {
            ClassificationTypesPossiblyChanged();
        }

        private void ClassificationTypesPossiblyChanged()
        {
            invokeWithDelay.DelayedInvoke(1000, () =>
            {
                var before = ClassificationTypeInfo;
                GetClassificationTypeInfo();
                var after = ClassificationTypeInfo;
                if(before.Count != after.Count)
                {
                    ClassificationTypesChanged?.Invoke(this, new EventArgs());
                }
            });
        }

        public event EventHandler ClassificationTypesChanged;

        public List<ClassificationTypeInfo> ClassificationTypeInfo
        {
            get
            {
                if (classificationTypeInfo == null)
                {
                    GetClassificationTypeInfo();
                }
                return classificationTypeInfo;
            }

        }

        public void GetClassificationTypeInfo()
        {
            var classificationTypes = classificationTypeReflector.Get();
            var allTypes = classificationTypes.NonTransient.Values.Select(nonTransient => new ClassificationTypeInfo(nonTransient, false)).ToList();
            if (classificationTypes.Transient != null)
            {
                allTypes = allTypes.Concat(classificationTypes.Transient.Values.Select(transient => new ClassificationTypeInfo(transient, true))).ToList();
            }
            foreach (var ctdm in classificationTypeDefinitionMetadata)
            {
                var classificationTypeDefinitionName = ctdm.Name;
                var classificationType = classificationTypeRegistryService.GetClassificationType(classificationTypeDefinitionName);
                if (classificationType != null)
                {
                    var classificationTypeInfo = allTypes.FirstOrDefault(cti => cti.ClassificationType == classificationType);
                    if (classificationTypeInfo != null)
                    {
                        classificationTypeInfo.HasClassificationTypeDefinition = true;
                    }
                    else
                    {
                        allTypes.Add(new ClassificationTypeInfo(classificationType, false));
                    }
                }
            }
            allTypes.ForEach(ct =>
            {
                var df = GetDefinitionFrom(ct.ClassificationType);
                ct.DefinitionFrom = df;
            });
            classificationTypeInfo = allTypes;

            DefinitionFrom GetDefinitionFrom(IClassificationType classificationType)
            {
                var definitionFrom = DefinitionFrom.None;
                var key = classificationFormatMapService.CurrentClassificationTextFormattingRunPropertiesFormatMap.GetEditorFormatMapKey(classificationType);
                var fromClassificationFormat = classificationEditorFormatDefinitions.Any(cfdm => ((IEditorFormatMetadata)cfdm.Metadata).Name.ToLower() == key.ToLower());
                if (!fromClassificationFormat)
                {
                    var fromEditor = editorFormatDefinitions.Any(cfdm => ((IEditorFormatMetadata)cfdm.Metadata).Name.ToLower() == key.ToLower());
                    if (fromEditor)
                    {
                        definitionFrom = DefinitionFrom.EditorFormatDefinition;
                    }
                }
                else
                {
                    definitionFrom = DefinitionFrom.ClassificationFormatDefinition;
                }
                return definitionFrom;

            }


        }
    }
}
