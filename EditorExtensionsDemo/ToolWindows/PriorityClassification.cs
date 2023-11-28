using System.Collections.Generic;
using Microsoft.VisualStudio.Text.Formatting;

namespace EditorExtensionsDemo
{
    public class PriorityClassification
    {
        public bool UserVisible { get; }
        public string Classification { get; }
        public TextFormattingRunProperties MergedFormattingRunProperties { get; }
        public int Priority { get; }
        public bool IsNull { get; }
        public string Before { get; }
        public string After { get; }
        
        public PriorityClassification(
            string classification, 
            TextFormattingRunProperties mergedFormattingRunProperties,
            IClassificationFormatMetadata classificationFormatMetadata,
            bool isNull
            )
        {
            Classification = classification;
            MergedFormattingRunProperties = mergedFormattingRunProperties;
            IsNull = isNull;
            // DisplayName....
            UserVisible = classificationFormatMetadata.UserVisible;
            Priority = classificationFormatMetadata.Priority;
            Before = BeforeOrAfter(classificationFormatMetadata.Before);
            After = BeforeOrAfter(classificationFormatMetadata.After);
        }

        private string BeforeOrAfter(IEnumerable<string> cts)
        {
            return cts == null ? "" : String.Join(",", cts);
        }
    }
}
