using Microsoft.VisualStudio.Text.Classification;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using GongSolutions.Wpf.DragDrop;
using System.Windows.Media;
using Microsoft.VisualStudio.Shell.TableControl;
using Microsoft.VisualStudio.Shell.TableManager;
using System.ComponentModel.Composition;
using EditorExtensionsDemo.ToolWindows;

namespace EditorExtensionsDemo
{
    [Export(typeof(PriorityClassificationViewModel))]
    internal class PriorityClassificationViewModel : ObservableObject, IDropTarget, IWpfTableControlDataContext, IDisposable
    {
        private readonly ICurrentClassificationTextFormattingRunPropertiesFormatMapService classificationFormatMapService;
        private readonly IClassificationTypeInfoService classificationTypeInfoService;
        private readonly IEnumerable<Lazy<EditorFormatDefinition, IClassificationFormatMetadata>> classificationEditorFormatDefinitions;
        private readonly ISettingsManager settingsManager;

        private const string collectionName = "PriorityClassificationCollection";
        private const string storePropertyName = "ColumnStates";
        private readonly Color CanDropColor = Colors.Green;
        private readonly Color CannotDropColor = Colors.Red;

        private Color dropTargetAdornerPenColor;
        public Color DropTargetAdornerPenColor
        {
            get
            {
                return dropTargetAdornerPenColor;

            }
            set => SetProperty(ref dropTargetAdornerPenColor, value);
        }

        [ImportingConstructor]
        public PriorityClassificationViewModel(
            ICurrentClassificationTextFormattingRunPropertiesFormatMapService classificationFormatMapService,
            IClassificationTypeInfoService classificationTypeInfoService,
            IActiveViewCaretClassification activeViewCaretClassification,
            [ImportMany]
            IEnumerable<Lazy<EditorFormatDefinition, IClassificationFormatMetadata>> classificationEditorFormatDefinitions,
            ISettingsManager settingsManager
        )
        {
            this.classificationFormatMapService = classificationFormatMapService;
            this.classificationTypeInfoService = classificationTypeInfoService;
            this.classificationEditorFormatDefinitions = classificationEditorFormatDefinitions;
            this.settingsManager = settingsManager;
            classificationFormatMapService.CurrentClassificationTextFormattingRunPropertiesFormatMapChanged += ClassificationFormatMapService_CurrentClassificationFormatMapChanged;
            classificationTypeInfoService.ClassificationTypesChanged += (sender, args) =>
            {
                CreatePriorities();
            };
            InitializeColumns();
            DropTargetAdornerPenColor = CannotDropColor;
        }

        private void ClassificationFormatMapService_CurrentClassificationFormatMapChanged(object sender, EventArgs e)
        {
            CreatePriorities();
        }

        #region WpfTableControl

        private readonly List<SavedColumnState> defaultSavedColumnsStates = new()
        {
            new SavedColumnState { Width = 43 },
            new SavedColumnState { Width = 275 },
            new SavedColumnState { Width = 100 },
            new SavedColumnState { Width = 64 },
            new SavedColumnState { Width = 100 },
            new SavedColumnState { Width = 100 },
            new SavedColumnState { Width = 80 },
            new SavedColumnState { Width = 186 },
            new SavedColumnState { Width = 202 },
            new SavedColumnState { Width = 100 },
            new SavedColumnState { Width = 52 }
        };
        
        private List<double> GetColumnWidths()
        {
            var columnStates = settingsManager.GetData<List<SavedColumnState>>(collectionName, storePropertyName) ?? defaultSavedColumnsStates;
            return columnStates.Select(cs => cs.Width).ToList();
        }

        public List<ITableColumnDefinition> TableColumnDefinitions { get; private set; }
        public List<string> ColumnNames { get; private set; }
        public string TableIdentifier => nameof(PriorityClassification.Classification);

        private void InitializeColumns()
        {
            var columnWidths = GetColumnWidths();
            TableColumnDefinitions = new()
            {
                new TableColumnDefinition(nameof(PriorityClassification.IsNull),columnWidths[0],false,true),
                new TableColumnDefinition(nameof(PriorityClassification.Classification),columnWidths[1]),
                new TableColumnDefinition(nameof(PriorityClassification.BaseTypes),columnWidths[2]),
                new TableColumnDefinition(nameof(PriorityClassification.IsPriority),columnWidths[3]),
                new TableColumnDefinition(nameof(PriorityClassification.Before),columnWidths[4]),
                new TableColumnDefinition(nameof(PriorityClassification.After), columnWidths[5]),
                new TableColumnDefinition(nameof(PriorityClassification.IsTransient),columnWidths[6]),
                new TableColumnDefinition(nameof(PriorityClassification.HasClassificationTypeDefinition),columnWidths[7]),
                new TableColumnDefinition(nameof(PriorityClassification.HasClassificationFormatMetadata),columnWidths[8]),
                new TableColumnDefinition(nameof(PriorityClassification.DefinitionFrom), columnWidths[9]),
                new TableColumnDefinition(nameof(PriorityClassification.Priority),columnWidths[10])
            };
            ColumnNames = TableColumnDefinitions.Select(cd => cd.Name).ToList();
        }

        private void CreatePriorities()
        {
            var classificationTypeInfo = classificationTypeInfoService.ClassificationTypeInfo;
            var currentClassificationFormatMap = classificationFormatMapService.CurrentClassificationTextFormattingRunPropertiesFormatMap;
            var allPriorityClassifications = currentClassificationFormatMap.CurrentPriorityOrder.Select((ct, index) =>
            {

                if (ct == null)
                {
                    return PriorityClassification.Null(currentClassificationFormatMap.DefaultTextFormattingRunProperties);
                }
                var ctInfo = classificationTypeInfo.FirstOrDefault(ctInfo => ctInfo.ClassificationType == ct);
                if (ctInfo == null)
                {
                    classificationTypeInfoService.GetClassificationTypeInfo();
                    // todo 
                    ctInfo = classificationTypeInfo.FirstOrDefault(ctInfo => ctInfo.ClassificationType == ct);
                }
                return new PriorityClassification(
                    ctInfo,
                    currentClassificationFormatMap.GetClassificationTextFormattingRunProperties(ct).MergedTextFormattingRunProperties,
                    GetClassificationFormatMetadata(ct.Classification),
                    true
                );
            }).ToList();

            var nonPriorityClassifications = classificationTypeInfo.Where(ctInfo => !currentClassificationFormatMap.CurrentPriorityOrder.Contains(ctInfo.ClassificationType)).Select(ctInfo =>
            {

                return new PriorityClassification(
                    ctInfo,
                    currentClassificationFormatMap.GetClassificationTextFormattingRunProperties(ctInfo.ClassificationType).MergedTextFormattingRunProperties,
                    GetClassificationFormatMetadata(ctInfo.ClassificationType.Classification),
                    false
                );
            }).ToList();
            allPriorityClassifications = allPriorityClassifications.Concat(nonPriorityClassifications).ToList();
            var entries = allPriorityClassifications.Select((pc, i) => new PriorityClassificationTableEntry(pc, i, pc.IsNull ? null : pc.Classification)).ToList();
            if(tableDataSink != null)
            {
                tableDataSink.AddEntries(entries, true);
            }
            else
            {
                entriesToAdd = entries;
            }

            IClassificationFormatMetadata GetClassificationFormatMetadata(string classification)
            {
                return classificationEditorFormatDefinitions.FirstOrDefault(cfdm => cfdm.Metadata.ClassificationTypeNames.Contains(classification))?.Metadata;
            }
        }

        private List<PriorityClassificationTableEntry> entriesToAdd;

        #region drag drop
        public void DragEnter(IDropInfo dropInfo)
        {

        }

        private (PriorityClassification source, PriorityClassification target) GetItemsAsPriorityClassification(IDropInfo dropInfo)
        {
            var sourceIndex = dropInfo.DragInfo.SourceIndex;
            var targetIndex = dropInfo.InsertIndex;
            var sourcePriorityClassification = GetClassificationFromEntryIndex(sourceIndex);
            var targetClassification = GetClassificationFromEntryIndex(targetIndex);
            return (sourcePriorityClassification, targetClassification);

            PriorityClassification GetClassificationFromEntryIndex(int index)
            {
                var entries = WpfTableControl.Entries;
                var handle = entries.ElementAt(index);
                handle.TryGetEntry(out var aEntry);
                return (aEntry as IHavePriorityClassification).PriorityClassification;
            }
        }

        public void DragOver(IDropInfo dropInfo)
        {
            var (source, target) = GetItemsAsPriorityClassification(dropInfo);
            var canDrop = false;
            if (source != target)
            {
                canDrop = source.IsPriorityType() && target.IsPriorityType();
            }

            //todo
            dropInfo.EffectText = "effect text";
            dropInfo.DestinationText = "destination text";

            dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;

            dropInfo.Effects = canDrop ? System.Windows.DragDropEffects.Move : System.Windows.DragDropEffects.None;

            DropTargetAdornerPenColor = canDrop ? CanDropColor : CannotDropColor;
        }

        public void DragLeave(IDropInfo dropInfo)
        {
        }

        public void Drop(IDropInfo dropInfo)
        {
            var (source, target) = GetItemsAsPriorityClassification(dropInfo);
            var currentClassificationFormatMap = classificationFormatMapService.CurrentClassificationTextFormattingRunPropertiesFormatMap;
            currentClassificationFormatMap.SwapPriority(source.ClassificationType, target.ClassificationType);
        }
        #endregion

        private IWpfTableControl wpfTableControl;
        public IWpfTableControl WpfTableControl {
            
            get => wpfTableControl;
            set {
                wpfTableControl = value;
                wpfTableControl.SortFunction = (a, b) =>
                {
                    a.TryGetEntry(out var aEntry);
                    b.TryGetEntry(out var bEntry);

                    var x = aEntry as IIndexedTableEntry;
                    var y = bEntry as IIndexedTableEntry;
                    return x.Index - y.Index;
                };

            }  
        }

        #region ITableDataSource
        public const string sourceTypeIdentifier = "MySourceType";
        public string SourceTypeIdentifier => sourceTypeIdentifier;
        public const string sourceIdentifier = "MySourceIdentifier";
       
        public string Identifier => sourceIdentifier;

        public string DisplayName => "MyTableDataSource";

        private ITableDataSink tableDataSink;

        public IDisposable Subscribe(ITableDataSink sink)
        {
            this.tableDataSink = sink;
            if (entriesToAdd != null)
            {
                tableDataSink.AddEntries(entriesToAdd, true);
                entriesToAdd = null;
            }
            return this;
        }
        public void Dispose()
        {
            SaveColumnStates();
        }
        private void SaveColumnStates()
        {
            settingsManager.SetData(
                collectionName, 
                storePropertyName, 
                WpfTableControl.ColumnStates.Select(cs => new SavedColumnState { Width = cs.Width }).ToList()
            );
        }
        

        #endregion

        #endregion
    }

    public class SavedColumnState
    {
        public double Width { get; set; }
    }

}
