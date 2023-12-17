using Microsoft.Internal.VisualStudio.Shell.TableControl;
using Microsoft.VisualStudio.Shell.TableControl;
using Microsoft.VisualStudio.Shell.TableManager;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace EditorExtensionsDemo.ToolWindows.Controls
{
    /// <summary>
    /// Interaction logic for WpfListView.xaml
    /// </summary>
    public partial class WpfListView : UserControl
    {
        private readonly ITableManagerProvider tableManagerProvider;
        private readonly IWpfTableControlProvider wpfTableControlProvider;
        public static readonly DependencyProperty TrackingListViewProperty = DependencyProperty.Register(
    nameof(TrackingListView), typeof(FrameworkElement),
    typeof(WpfListView)
    );

        public FrameworkElement TrackingListView
        {
            get => (FrameworkElement)GetValue(TrackingListViewProperty);
            set => SetValue(TrackingListViewProperty, value);
        }
        public WpfListView()
        {
            DataContextChanged += WpfListView_DataContextChanged;
            tableManagerProvider = VS.GetMefService<ITableManagerProvider>();
            wpfTableControlProvider = VS.GetMefService<IWpfTableControlProvider>();
            InitializeComponent();
        }


        private void WpfListView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var wpfTableControlDataContext = this.DataContext as IWpfTableControlDataContext;
            if(wpfTableControlDataContext == null)
            {
                return;
            }
            if(wpfTableControlDataContext.TableColumnDefinitions != null)
            {
                foreach (var columnDefinition in wpfTableControlDataContext.TableColumnDefinitions)
                {
                    wpfTableControlProvider.AddColumnDefinition(columnDefinition);
                }
            }
            var tableManager = tableManagerProvider.GetTableManager(wpfTableControlDataContext.TableIdentifier);
            tableManager.AddSource(wpfTableControlDataContext,wpfTableControlDataContext.ColumnNames);

            // todo add states to the IWpfTableControlDataContext
            var wpfTableControl =  wpfTableControlProvider.CreateControl(
                tableManager,
                true,
                Enumerable.Empty<ColumnState>()
            );
            var wpfTableControlControl = wpfTableControl.Control; 
            this.Content = wpfTableControlControl;
            TrackingListView = LogicalTreeHelper.FindLogicalNode(wpfTableControlControl, "TableControlView") as FrameworkElement;

            wpfTableControlDataContext.WpfTableControl = wpfTableControl;
        }
    }
}
