using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel.Composition;

namespace EditorExtensionsDemo
{
    [Export(typeof(ClassificationViewModel))]
    // NOTE THAT CANNOT GET TO WORK WITH CommunityToolkit.Mvvm https://github.com/CommunityToolkit/dotnet/issues/750
    internal class ClassificationViewModel : ObservableObject, ITreeProperties
    {
        public event EventHandler TreeChanged;

        private bool showExplicitTextProperties;
        public bool ShowExplicitTextProperties
        {
            get => showExplicitTextProperties;
            set
            {
                var changed = SetProperty(ref showExplicitTextProperties, value);
                if (changed)
                {
                    //TreeChanged?.Invoke(this, new EventArgs());
                }
            }
        }

        public CaretClassificationViewModel CaretClassificationViewModel { get; }
        public ClassificationTypesNotInRegistryViewModel ClassificationTypesNotInRegistryViewModel { get; }
        public IClassificationFormatCategoryViewModel ClassificationFormatCategoryViewModel { get; }
        public PriorityClassificationViewModel PriorityClassificationViewModel { get; }

        [ImportingConstructor]
        public ClassificationViewModel(
            CaretClassificationViewModel caretClassificationViewModel,
            ClassificationTypesNotInRegistryViewModel classificationTypesNotInRegistryViewModel,
            IClassificationFormatCategoryViewModel classificationFormatCategoryViewModel,
            PriorityClassificationViewModel priorityClassificationViewModel
        )
        {
            CaretClassificationViewModel = caretClassificationViewModel;
            ClassificationTypesNotInRegistryViewModel = classificationTypesNotInRegistryViewModel;
            ClassificationFormatCategoryViewModel = classificationFormatCategoryViewModel;
            PriorityClassificationViewModel = priorityClassificationViewModel;
            ClassificationFormatCategoryViewModel.CreateTextClassificationFormatMap();
        }
        

        //private void CreateTree()
        //{
        //    // again do not need to reconstruct a tree if types have not changed
        //    // do not need to do the Where each time
        //    var withSingleBaseTypes = ClassificationTypeInfo.Where(ctInfo => ctInfo.ClassificationType.BaseTypes.Count() <= 1).Select(ctinfo => ctinfo.ClassificationType);
        //    var treeBuilder = new TreeBuilder<IClassificationType, TreeItem, string>();
        //    var treeItems = treeBuilder.Build(
        //        withSingleBaseTypes,
        //        ct => (ct.Classification, ct.BaseTypes?.FirstOrDefault()?.Classification),
        //    treeItem => treeItem.Name,
        //        (ct,id) => new TreeItem(id, currentClassificationFormatMap.GetClassificationTextFormattingRunProperties(ct),
        //                this)
        //    );

        //    // todo - options sorting by priority
        //    treeItems.Sort((a, b) => a.Name.CompareTo(b.Name));
        //    foreach (var item in treeItems)
        //    {
        //        item.Children.Sort((a, b) => a.Name.CompareTo(b.Name));
        //    }
        //    ClassificationTree = treeItems;
        //}

    }
}
