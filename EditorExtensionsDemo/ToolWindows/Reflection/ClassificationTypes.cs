using Microsoft.VisualStudio.Text.Classification;
using System.Collections.Generic;

namespace EditorExtensionsDemo
{
    public class ClassificationTypes
    {
        public Dictionary<string,IClassificationType> NonTransient { get; set; }
        public Dictionary<string, IClassificationType> Transient { get; set; }
    }
}
