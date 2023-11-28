using Microsoft.VisualStudio.Text.Formatting;
using System.Collections.Generic;

namespace EditorExtensionsDemo
{
    public class TreeItem
    {
        public TreeItem(
            string name, 
            ClassificationTextFormattingRunProperties classificationTextFormattingRunProperties,
            bool showExplicitTextProperties
            )
        {
            Name = name;
            ShowExplicitTextProperties = showExplicitTextProperties;
            this.OwnTextFormattingRunProperties = classificationTextFormattingRunProperties.OwnTextFormattingRunProperties;
            this.MergedTextFormattingRunProperties = classificationTextFormattingRunProperties.MergedTextFormattingRunProperties;
            this.DefaultTextFormattingRunProperties = classificationTextFormattingRunProperties.DefaultTextProperties;
        }

        public void AddChild(TreeItem child)
        {
            Children.Add(child);
            child.Parent = this;
        }

        internal void Sort()
        {
            this.Children.Sort((a, b) => a.Name.CompareTo(b.Name));
        }

        public TreeItem Parent { get; set; }

        public string Name { get; }
        public bool ShowExplicitTextProperties { get; }
        public TextFormattingRunProperties DefaultTextFormattingRunProperties { get; }
        public TextFormattingRunProperties OwnTextFormattingRunProperties { get; }
        public TextFormattingRunProperties MergedTextFormattingRunProperties { get; }
        public List<TreeItem> Children { get; private set; } = new List<TreeItem>();
    }
}
