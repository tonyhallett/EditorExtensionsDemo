using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel.Composition;
using EditorExtensionsDemo.ToolWindows;

namespace EditorExtensionsDemo
{

    [Export(typeof(IClassificationFormatCategoryViewModel))]
    internal class ClassificationFormatCategoryViewModel : ObservableObject, IClassificationFormatCategoryViewModel
    {
        private readonly ICategoryReflector categoryReflector;
        private readonly ICurrentClassificationTextFormattingRunPropertiesFormatMapService classificationFormatMapService;

        [ImportingConstructor]
        public ClassificationFormatCategoryViewModel(
            ICategoryReflector categoryReflector,
            ICurrentClassificationTextFormattingRunPropertiesFormatMapService classificationFormatMapService,
            IActiveViewCaretClassification activeViewCaretClassification
        )
        {
            this.categoryReflector = categoryReflector;
            this.classificationFormatMapService = classificationFormatMapService;
            //todo name change and xaml
            CreateClassificationFormatMapCommand = new RelayCommand<string>(
                CreateClassificationFormatMapForCategory,
                CanCreateClassificationFormatMap
            );
            SetCategories();
            activeViewCaretClassification.TextViewOpened += ActiveViewCaretClassification_TextViewOpened;
        }

        private void ActiveViewCaretClassification_TextViewOpened(object sender, EventArgs e)
        {
            SetCategories();
        }

        private string selectedCategory;
        public string SelectedCategory
        {
            get => selectedCategory;
            set
            {
                var changed = SetProperty(ref selectedCategory, value);
                if (changed)
                {
                    classificationFormatMapService.SetCategory(selectedCategory);
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

        public RelayCommand<string> CreateClassificationFormatMapCommand { get; }

        public ObservableCollection<string> Categories { get; set; } = new ObservableCollection<string>();



        private bool CanCreateClassificationFormatMap(string categoryInput)
        {
            return !string.IsNullOrWhiteSpace(categoryInput) && !Categories.Contains(categoryInput);
        }

        public void CreateTextClassificationFormatMap()
        {
            CreateClassificationFormatMapForCategory("text");
        }

        private void CreateClassificationFormatMapForCategory(string category)
        {
            classificationFormatMapService.SetCategory(category);
            AddCategoryIfDoesNotExist(category);
            SelectedCategory = category;
            CategoryInput = string.Empty;
        }

        private void AddCategoryIfDoesNotExist(string category)
        {
            if (!Categories.Contains(category))
            {
                Categories.Add(category);
            }
        }

        private void SetCategories()
        {
            var doNotShowGuidCategory = true;
            var categories = categoryReflector.GetCategories();

            foreach (var category in categories)
            {
                var add = !doNotShowGuidCategory || !guidCategoryFilter(category);
                if (add)
                {
                    AddCategoryIfDoesNotExist(category);
                }
            }

            static bool guidCategoryFilter(string category) => category.Length == 73;
        }

    }
}
