using System.Collections.Generic;
using System.Linq;
using Microsoft.Internal.VisualStudio.Shell.TableControl;
using Microsoft.VisualStudio.Shell.TableControl;
using Microsoft.VisualStudio.Shell.TableManager;
using System.ComponentModel.Composition;

namespace EditorExtensionsDemo
{
    [Export(typeof(IWpfTableControlCreator))]
    internal class WpfTableControlCreator : IWpfTableControlCreator
    {
        private readonly ITableManagerProvider tableManagerProvider;
        private readonly IWpfTableControlProvider wpfTableControlProvider;

        [ImportingConstructor]
        public WpfTableControlCreator(
            ITableManagerProvider tableManagerProvider,
            IWpfTableControlProvider wpfTableControlProvider
        )
        {
            this.tableManagerProvider = tableManagerProvider;
            this.wpfTableControlProvider = wpfTableControlProvider;
        }

        

        public IWpfTableControl Create(string tableIdentifier, ITableDataSource tableDataSource, IEnumerable<ITableColumnDefinition> columnDefinitions)
        {
            var tableManager = tableManagerProvider.GetTableManager(tableIdentifier);
            tableManager.AddSource(tableDataSource, columnDefinitions.Select(cd => cd.Name).ToArray());

            foreach (var columnDefinition in columnDefinitions) {
                wpfTableControlProvider.AddColumnDefinition(columnDefinition);
            }

            return wpfTableControlProvider.CreateControl(
                tableManager,
                true,
                Enumerable.Empty<ColumnState>()
                //columnDefinitions.Select(cd => new ColumnState(cd.Name, true, cd is ITableColumnDefinition2 cd2 ? cd2.DefaultWidth : 0))
            );
        }
    }

}
