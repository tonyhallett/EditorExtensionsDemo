using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Text.Classification;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace EditorExtensionsDemo
{
    // NOTE THAT CANNOT GET TO WORK WITH CommunityToolkit.Mvvm https://github.com/CommunityToolkit/dotnet/issues/750
    partial class ClassificationViewModel : ObservableObject
    {
        private Dictionary<string, MyClassificationFormatMap> ClassificationFormatMaps { get; } = new Dictionary<string, MyClassificationFormatMap>();
        private IComponentModel2 mef;
        private IClassificationFormatMapService classificationFormatMapService;
        private IClassificationTypeRegistryService classificationTypeRegistryService;
        private IEnumerable<Lazy<EditorFormatDefinition, IClassificationFormatMetadata>> classificationFormatDefinitionMetadata;
        private List<IClassificationTypeDefinitionMetadata> classificationTypeDefinitionMetadata;
        private MyClassificationFormatMap currentClassificationFormatMap;

        private bool showExplicitTextProperties;
        public bool ShowExplicitTextProperties
        {
            get => showExplicitTextProperties;
            set
            {
                var changed = SetProperty(ref showExplicitTextProperties, value);
                if (changed)
                {
                    CreateTree();
                }
            }
        }

        private string categoryInput;
        public string CategoryInput
        {
            get => categoryInput;
            set
            {
                var changed = SetProperty(ref categoryInput, value);
                if (changed)
                {
                    CreateClassificationFormatMapCommand.NotifyCanExecuteChanged();
                }
            }
        }

        
        private List<TreeItem> classificationTree;
        public List<TreeItem> ClassificationTree
        {
            get => classificationTree;
            set => SetProperty(ref classificationTree, value);
        }

        private List<PriorityClassification> priorityClassifications;
        public List<PriorityClassification> PriorityClassifications
        {
            get => priorityClassifications;
            set => SetProperty(ref priorityClassifications, value);
        }

        public ObservableCollection<string> Categories { get; set; } = new ObservableCollection<string>();
        private string selectedCategory;
        public string SelectedCategory
        {
            get => selectedCategory;
            set
            {
                var changed = SetProperty(ref selectedCategory, value);
                if (changed)
                {
                    SelectedCategoryChanged();
                }
            }
        }

        public RelayCommand<string> CreateClassificationFormatMapCommand { get; }
        public ClassificationViewModel()
        {
            CreateClassificationFormatMapCommand = new RelayCommand<string>(CreateMyClassificationFormatMap, CanGetClassificationFormatMap);
            GetMefExports();
            CategoryInput = "text";
            CreateMyClassificationFormatMap(CategoryInput);
            CreateTree();
            CreatePriorities();
        }
        private void GetMefExports()
        {
            mef = VS.GetRequiredService<SComponentModel, IComponentModel2>();
            classificationFormatMapService = mef.GetService<IClassificationFormatMapService>();
            classificationTypeRegistryService = mef.GetService<IClassificationTypeRegistryService>();

            classificationFormatDefinitionMetadata = mef.DefaultExportProvider.GetExports<EditorFormatDefinition, IClassificationFormatMetadata>();
            

            classificationTypeDefinitionMetadata = mef.DefaultExportProvider.GetExports<ClassificationTypeDefinition, IClassificationTypeDefinitionMetadata>().Select(l =>
            {
                return l.Metadata;
            }).ToList();
        }
        private void CreatePriorities()
        {
            // will need to keep in sync 

            if(priorityOrder.Count == 0)
            {
                CreateFormatPriorityOrder(Orderer.Order(this.classificationFormatDefinitionMetadata));
            }
            if(priorityOrder.Count != currentClassificationFormatMap.CurrentPriorityOrder.Count)
            {
                throw new Exception("Oh dear");
            }

            PriorityClassifications = currentClassificationFormatMap.CurrentPriorityOrder.Select((ct, index) =>
            {
                var classification = priorityOrder[index].Item1;
                if (ct != null && classification != ct.Classification)
                {
                    throw new Exception("Oh dear");
                }

                return new PriorityClassification(
                    classification,
                    ct == null ? currentClassificationFormatMap.DefaultTextFormattingRunProperties : currentClassificationFormatMap.GetClassificationTextFormattingRunProperties(classification).MergedTextFormattingRunProperties,
                    priorityOrder[index].Item2,
                    ct == null
                );
            }).ToList();
            //var priorityOrder = currentClassificationFormatMap.CurrentPriorityOrder.Where(ct => ct != null).ToList();
            //var notPriority = classificationTypeDefinitionMetadata.Where(ctm => !priorityOrder.Any(ct => ct.Classification == ctm.Name)).ToList();

        }
        private List<(string, IClassificationFormatMetadata)> priorityOrder = new List<(string, IClassificationFormatMetadata)>();
        private void CreateFormatPriorityOrder(
                  IList<Lazy<EditorFormatDefinition, IClassificationFormatMetadata>> orders)
        {
            foreach (Lazy<EditorFormatDefinition, IClassificationFormatMetadata> order in (IEnumerable<Lazy<EditorFormatDefinition, IClassificationFormatMetadata>>)orders)
            {
                string name = ((IEditorFormatMetadata)order.Metadata).Name;
                if (!string.Equals(name, "Low Priority", StringComparison.Ordinal) && !string.Equals(name, "Default Priority", StringComparison.Ordinal) && !string.Equals(name, "High Priority", StringComparison.Ordinal))
                {
                    foreach (string classificationTypeName in order.Metadata.ClassificationTypeNames)
                        this.priorityOrder.Add((classificationTypeName,order.Metadata));
                }
            }
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
                    treeItem = new TreeItem(
                        classificationType.Name, 
                        currentClassificationFormatMap.GetClassificationTextFormattingRunProperties(classificationType.Name),
                        ShowExplicitTextProperties
                    );
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
                        baseDefinitionItem = new TreeItem(
                            baseDefinition, 
                            currentClassificationFormatMap.GetClassificationTextFormattingRunProperties(baseDefinition),
                            ShowExplicitTextProperties    
                        );
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

        private void SelectedCategoryChanged()
        {
            if(currentClassificationFormatMap.Category != SelectedCategory)
            {
                var classificationFormatMap = ClassificationFormatMaps[SelectedCategory];
                
                currentClassificationFormatMap?.RemoveHandler();
                currentClassificationFormatMap = classificationFormatMap;
                classificationFormatMap.AddHandler(CurrentFormatMapChanged);
                CreateTree();
                
            }
        }

        private bool CanGetClassificationFormatMap(string categoryInput)
        {
            return !string.IsNullOrWhiteSpace(categoryInput) && !Categories.Contains(categoryInput);
        }

        private void CreateMyClassificationFormatMap(string category)
        {
            currentClassificationFormatMap?.RemoveHandler();
            currentClassificationFormatMap = new MyClassificationFormatMap(
                category, 
                classificationTypeRegistryService,
                classificationFormatMapService
            );
            currentClassificationFormatMap.AddHandler(CurrentFormatMapChanged);
            ClassificationFormatMaps.Add(category, currentClassificationFormatMap);
            CreateTree();
            Categories.Add(category);
            SelectedCategory = category;
        }

        private void CurrentFormatMapChanged()
        {
            CreateTree();
        }
    }
}
