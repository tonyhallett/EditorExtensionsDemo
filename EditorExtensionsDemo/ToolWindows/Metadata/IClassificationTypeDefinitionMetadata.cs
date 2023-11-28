using System.Collections.Generic;
using System.ComponentModel;

namespace EditorExtensionsDemo
{
    public interface IClassificationTypeDefinitionMetadata
    {
        string Name { get; }

        [DefaultValue(null)]
        IEnumerable<string> BaseDefinition { get; }
    }
}
