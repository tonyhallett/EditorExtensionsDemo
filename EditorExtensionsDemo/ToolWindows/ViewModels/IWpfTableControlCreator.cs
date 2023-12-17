using System.Collections.Generic;
using Microsoft.VisualStudio.Shell.TableControl;
using Microsoft.VisualStudio.Shell.TableManager;

namespace EditorExtensionsDemo
{
    interface IWpfTableControlCreator
    {
        IWpfTableControl Create(string tableIdentifier, ITableDataSource tableDataSource, IEnumerable<ITableColumnDefinition> columnDefinitions);
    }

}
