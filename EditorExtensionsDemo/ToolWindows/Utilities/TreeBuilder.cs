using System.Collections.Generic;
using System.Linq;

namespace EditorExtensionsDemo.ToolWindows
{
    interface ITreeItem<T> where T : ITreeItem<T>
    {
        T Parent { get; set; }
        List<T> Children { get; }
    }

    internal class TreeBuilder<TSource,TTreeItem,TIdentifier> where TTreeItem : ITreeItem<TTreeItem>
    {
        public List<TTreeItem> Build(
            IEnumerable<TSource> source, 
            Func<TSource,(TIdentifier,TIdentifier)> sourceIdentifier,
            Func<TTreeItem,TIdentifier> treeIdentifier,
            Func<TSource,TIdentifier, TTreeItem> createTreeItem)
        {
            var equalityComparer = EqualityComparer<TIdentifier>.Default;
            List<TTreeItem> treeItems = new();
            foreach (var item in source)
            {
                var (identifier, parentIdentifier) = sourceIdentifier(item);
                // It's either a base definition or it's not
                // If it's a base definition then it has already been created as a root node
                var isBaseDefinition = true;
                var treeItem = treeItems.FirstOrDefault(ti => equalityComparer.Equals(treeIdentifier(ti),identifier));
                if (treeItem == null)
                {
                    isBaseDefinition = false;
                    treeItem = createTreeItem(item,identifier);
                }

                if (parentIdentifier == null)
                {
                    if (!isBaseDefinition)
                    {
                        treeItems.Add(treeItem);
                    }

                }
                else
                {
                    var baseDefinitionItem = FindItem(parentIdentifier);
                    if (baseDefinitionItem == null)
                    {
                        baseDefinitionItem = createTreeItem(item, parentIdentifier);
                        treeItems.Add(baseDefinitionItem);
                    }
                    baseDefinitionItem.Children.Add(treeItem);
                    treeItem.Parent = baseDefinitionItem;

                    if (isBaseDefinition)
                    {
                        treeItems.Remove(treeItem);
                    }
                }
            }

            return treeItems;

            TTreeItem FindItem(TIdentifier identifier)
            {
                return FindItemInTree(treeItems, identifier);
            }

            TTreeItem FindItemInTree(List<TTreeItem> treeItems, TIdentifier identifier)
            {
                foreach (var treeItem in treeItems)
                {
                    var found = FindFromItem(treeItem, identifier);
                    if (found != null)
                    {
                        return found;
                    }
                }
                return default(TTreeItem);
            }

            TTreeItem FindFromItem(TTreeItem treeItem, TIdentifier identifier)
            {
                var match = equalityComparer.Equals(treeIdentifier(treeItem), identifier);
                if (match)
                {
                    return treeItem;
                }
                return FindItemInTree(treeItem.Children, identifier);
            }
        }
    }
}
