using System.Collections.Generic;
using Microsoft.VisualStudio.Shell.TableControl;
using Microsoft.VisualStudio.Shell.TableManager;

namespace EditorExtensionsDemo
{
    interface IWpfTableControlDataContext : ITableDataSource
    {
        List<string> ColumnNames { get; }
        string TableIdentifier { get; }
        List<ITableColumnDefinition> TableColumnDefinitions { get; }
        IWpfTableControl WpfTableControl { set; }

    }

}
