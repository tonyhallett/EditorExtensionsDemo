using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Text.Classification;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace EditorExtensionsDemo
{
    partial class ClassificationViewModel : ObservableObject
    {
        private Dictionary<string, MyClassificationFormatMap> ClassificationFormatMaps { get; } = new Dictionary<string, MyClassificationFormatMap>();
        private IComponentModel2 mef;
        private IClassificationFormatMapService classificationFormatMapService;
        private IClassificationTypeRegistryService classificationTypeRegistryService;
        private List<IClassificationFormatMetadata> classificationFormatDefinitionMetadata;
        private List<IClassificationTypeDefinitionMetadata> classificationTypeDefinitionMetadata;
        private MyClassificationFormatMap currentClassificationFormatMap;

        [ObservableProperty]
        //[NotifyCanExecuteChangedFor(nameof(CreateMyClassificationFormatMapCommand))]
        [NotifyCanExecuteChangedFor("CreateMyClassificationFormatMapCommand")]
        private string categoryInput;

        [ObservableProperty]
        private List<TreeItem> classificationTree;

        public ObservableCollection<string> Categories { get; set; } = new ObservableCollection<string>();


        public ClassificationViewModel()
        {
            GetMefExports();
            CreateMyClassificationFormatMap("text");
            CreateTree();
        }
        private void GetMefExports()
        {
            mef = VS.GetRequiredService<SComponentModel, IComponentModel2>();
            classificationFormatMapService = mef.GetService<IClassificationFormatMapService>();
            classificationTypeRegistryService = mef.GetService<IClassificationTypeRegistryService>();

            classificationFormatDefinitionMetadata = mef.DefaultExportProvider.GetExports<EditorFormatDefinition, IClassificationFormatMetadata>().Select(l =>
            {
                return l.Metadata;
            }).ToList();

            classificationTypeDefinitionMetadata = mef.DefaultExportProvider.GetExports<ClassificationTypeDefinition, IClassificationTypeDefinitionMetadata>().Select(l =>
            {
                return l.Metadata;
            }).ToList();
        }

        private void CreateTree()
        {
            List<TreeItem> treeItems = new();

            foreach (var classificationType in classificationTypeDefinitionMetadata)
            {
                // It's either a base definition or it's not
                // If it's a base definition then it has already been created as a root node
                var isBaseDefinition = true;
                var treeItem = treeItems.FirstOrDefault(ti => ti.Name == classificationType.Name);
                if (treeItem == null)
                {
                    isBaseDefinition = false;
                    treeItem = new TreeItem(classificationType.Name, currentClassificationFormatMap.GetClassificationTextFormattingRunProperties(classificationType.Name));
                }

                if (classificationType.BaseDefinition == null || !classificationType.BaseDefinition.Any())
                {
                    if (!isBaseDefinition)
                    {
                        treeItems.Add(treeItem);
                    }

                }
                else
                {
                    var baseDefinition = classificationType.BaseDefinition.First();

                    var baseDefinitionItem = FindItem(baseDefinition);
                    if (baseDefinitionItem == null)
                    {
                        baseDefinitionItem = new TreeItem(baseDefinition, currentClassificationFormatMap.GetClassificationTextFormattingRunProperties(baseDefinition));
                        treeItems.Add(baseDefinitionItem);
                    }
                    baseDefinitionItem.AddChild(treeItem);

                    if (isBaseDefinition)
                    {
                        treeItems.Remove(treeItem);
                    }
                }
            }

            TreeItem FindItem(string name)
            {
                return FindItemInTree(treeItems, name);
            }

            TreeItem FindItemInTree(List<TreeItem> treeItems, string name)
            {
                foreach (var treeItem in treeItems)
                {
                    var found = FindFromItem(treeItem, name);
                    if (found != null)
                    {
                        return found;
                    }
                }
                return null;
            }

            TreeItem FindFromItem(TreeItem treeItem, string name)
            {
                if (treeItem.Name == name)
                {
                    return treeItem;
                }
                return FindItemInTree(treeItem.Children, name);
            }

            // todo - options sorting by priority
            treeItems.Sort((a, b) => a.Name.CompareTo(b.Name));
            foreach (var item in treeItems)
            {
                item.Sort();
            }
            ClassificationTree = treeItems;
        }

        private bool CanGetClassificationFormatMap(string categoryInput)
        {
            return !string.IsNullOrWhiteSpace(categoryInput) && !Categories.Contains(categoryInput);
        }

        [RelayCommand(CanExecute = nameof(CanGetClassificationFormatMap))]
        private void CreateMyClassificationFormatMap(string category)
        {
            Categories.Add(category);
            CategoryInput = category;
            currentClassificationFormatMap?.RemoveHandler();
            currentClassificationFormatMap = new MyClassificationFormatMap(
                category, 
                classificationTypeRegistryService,
                classificationFormatMapService
            );
            currentClassificationFormatMap.AddHandler(CurrentFormatMapChanged);
            ClassificationFormatMaps.Add(CategoryInput, currentClassificationFormatMap);
        }

        private void CurrentFormatMapChanged()
        {
            CreateTree();
        }








    }
}
