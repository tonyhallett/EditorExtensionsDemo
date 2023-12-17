using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Shell.TableControl;
using Microsoft.VisualStudio.Shell.TableManager;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Threading;

namespace EditorExtensionsDemo
{
    [Export(typeof(ClassificationTypesNotInRegistryViewModel))]
    internal class ClassificationTypesNotInRegistryViewModel : ObservableObject, IWpfTableControlDataContext, IDisposable
    {
        private readonly ClassificationTypesNotInRegistry classificationTypesNotInRegistry;

        public RelayCommand ShowClassificationTypesNotInRegistryCommand { get; }

        private List<ClassificationEditorFormatDefinitionClassificationTypesNotInRegistryViewModel> classificationEditorFormatDefinitionClassificationTypesNotInRegistryViewModel;
        private ITableDataSink tableDataSink;

        public List<ClassificationEditorFormatDefinitionClassificationTypesNotInRegistryViewModel> ClassificationEditorFormatDefinitionClassificationTypesNotInRegistry
        {
            get
            {
                classificationEditorFormatDefinitionClassificationTypesNotInRegistryViewModel ??= classificationTypesNotInRegistry.GetClassificationTypesNotInRegistry().Select(ctnir =>
                    {
                        return new ClassificationEditorFormatDefinitionClassificationTypesNotInRegistryViewModel(ctnir);
                    }).ToList();
                return classificationEditorFormatDefinitionClassificationTypesNotInRegistryViewModel;
            }
        }

        public List<string> ColumnNames { get; private set; }

        public string TableIdentifier => "ClassificationTypesNotInRegistryViewModelSourceTypeTable";

        public List<ITableColumnDefinition> TableColumnDefinitions { get; private set; }
        private IWpfTableControl wpfTableControl;
        public IWpfTableControl WpfTableControl {
            set => wpfTableControl = value;
        }

        public string SourceTypeIdentifier => "ClassificationTypesNotInRegistryViewModelSourceType";

        public string Identifier => "ClassificationTypesNotInRegistryViewModelIdentifier";

        public string DisplayName => "ClassificationTypesNotInRegistryViewModelDisplayName";

        [ImportingConstructor]
        public ClassificationTypesNotInRegistryViewModel(
            ClassificationTypesNotInRegistry classificationTypesNotInRegistry,
            IWindowService windowService
        )
        {
            this.classificationTypesNotInRegistry = classificationTypesNotInRegistry;
            ShowClassificationTypesNotInRegistryCommand = new RelayCommand(() =>
            {
                //windowService.ShowDialogWindow(this);
                _ = windowService.ShowToolWindowAsync(this);
            });
            SetColumns();
        }

        private void SetColumns()
        {
            TableColumnDefinitions = new List<ITableColumnDefinition>
            {
                new TableColumnDefinition(nameof(ClassificationEditorFormatDefinitionClassificationTypesNotInRegistryViewModel.Name),100),
                new TableColumnDefinition(nameof(ClassificationEditorFormatDefinitionClassificationTypesNotInRegistryViewModel.TypesNotInRegistry),200),
            };
            ColumnNames = TableColumnDefinitions.Select(tableColumnDefinition => tableColumnDefinition.Name).ToList();
        }

        public IDisposable Subscribe(ITableDataSink sink)
        {
            this.tableDataSink = sink;
            var entries = ClassificationEditorFormatDefinitionClassificationTypesNotInRegistry.Select((x, i) =>
            {
                return new ReflectableTableEntry<ClassificationEditorFormatDefinitionClassificationTypesNotInRegistryViewModel>(x, i, i);
            }).ToList();
            
            sink.AddEntries(entries, true);
            
           
            return this;
        }

        public void Dispose()
        {
            //todo
        }
    }
}
