using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Formatting;

namespace EditorExtensionsDemo
{
    internal class PriorityClassification
    {
        public TextFormattingRunProperties MergedFormattingRunProperties { get; }
        public bool IsPriority { get; }
        public bool IsNull { get; }
        public string Classification { get; }
        public string BaseTypes { get; } = "";
        public bool IsTransient { get; }

        
        public bool HasClassificationTypeDefinition { get; }
        public DefinitionFrom DefinitionFrom { get; } = DefinitionFrom.None;
        public bool HasClassificationFormatMetadata { get; }
        public bool UserVisible { get; }
        public int Priority { get; } = 0;
        
        public string Before { get; } = "";
        public string After { get; } = "";
        public IClassificationType ClassificationType { get; }
        
        public PriorityClassification(
            ClassificationTypeInfo classificationTypeInfo, 
            TextFormattingRunProperties mergedFormattingRunProperties,
            IClassificationFormatMetadata classificationFormatMetadata,
            bool isPriority
            )
        {
            IsPriority = isPriority;
            if (classificationTypeInfo == null)
            {
                Classification = "null";
                IsNull = true;
            }
            else
            {
                ClassificationType = classificationTypeInfo.ClassificationType;
                Classification = ClassificationType.Classification;
                BaseTypes = String.Join(", ", ClassificationType.BaseTypes);

                DefinitionFrom = classificationTypeInfo.DefinitionFrom;
                HasClassificationTypeDefinition = classificationTypeInfo.HasClassificationTypeDefinition;
                IsTransient = classificationTypeInfo.IsTransient;
            }
            
            MergedFormattingRunProperties = mergedFormattingRunProperties;
            
            if (classificationFormatMetadata == null) return;

            HasClassificationFormatMetadata = true;
            // DisplayName....
            UserVisible = classificationFormatMetadata.UserVisible;
            Priority = classificationFormatMetadata.Priority;
            Before = BeforeOrAfter(classificationFormatMetadata.Before);
            After = BeforeOrAfter(classificationFormatMetadata.After);
        }

        public static PriorityClassification Null(TextFormattingRunProperties defaultFormattingRunProperties)
        {
            return new PriorityClassification(null, defaultFormattingRunProperties, null,true);
        }

        public bool IsPriorityType()
        {
            return IsPriority && !IsNull;
        }

        private string BeforeOrAfter(IEnumerable<string> cts)
        {
            return cts == null ? "" : String.Join(", ", cts.Where(ct => ct != null));
        }
    }
}
