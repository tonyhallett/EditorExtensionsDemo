using EditorExtensionsDemo.ToolWindows;
using Microsoft.VisualStudio.Text.Formatting;
using System.Collections.Generic;

namespace EditorExtensionsDemo
{
    internal class TreeItem : ITreeItem<TreeItem>
    {
        private readonly ITreeProperties treeProperties;

        public event EventHandler TreeItemChanged;
        public TreeItem(
            string name, 
            ClassificationTextFormattingRunProperties classificationTextFormattingRunProperties,
            ITreeProperties treeProperties
            )
        {
            Name = name;
            this.treeProperties = treeProperties;
            ShowExplicitTextProperties = treeProperties.ShowExplicitTextProperties;
            treeProperties.TreeChanged += TreeProperties_TreeChanged;
            this.OwnTextFormattingRunProperties = classificationTextFormattingRunProperties.OwnTextFormattingRunProperties;
            this.MergedTextFormattingRunProperties = classificationTextFormattingRunProperties.MergedTextFormattingRunProperties;
            this.DefaultTextFormattingRunProperties = classificationTextFormattingRunProperties.DefaultTextProperties;
        }

        private void TreeProperties_TreeChanged(object sender, EventArgs e)
        {
            ShowExplicitTextProperties = treeProperties.ShowExplicitTextProperties;
            TreeItemChanged?.Invoke(this, EventArgs.Empty);
        }

        public void AddChild(TreeItem child)
        {
            Children.Add(child);
            child.Parent = this;
        }

        public TreeItem Parent { get; set; }

        public string Name { get; }
        public bool ShowExplicitTextProperties { get; private set; }
        public TextFormattingRunProperties DefaultTextFormattingRunProperties { get; }
        public TextFormattingRunProperties OwnTextFormattingRunProperties { get; }
        public TextFormattingRunProperties MergedTextFormattingRunProperties { get; }
        public List<TreeItem> Children { get; private set; } = new List<TreeItem>();
    }
}
