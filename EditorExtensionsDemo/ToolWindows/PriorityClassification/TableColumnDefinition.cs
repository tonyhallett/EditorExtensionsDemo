using Microsoft.VisualStudio.Shell.TableControl;
using System.Reflection;

namespace EditorExtensionsDemo
{
    public class TableColumnDefinition : TableColumnDefinitionBase
    {
        private readonly double? defaultWidth;
        private static readonly string ColumnNamePrefix;
        static TableColumnDefinition(){
            ColumnNamePrefix = Assembly.GetExecutingAssembly().GetName().Name;
        }

        public TableColumnDefinition(string name,double? defaultWidth = null,bool isSortable = false,bool isFilterable = false)
        {
            
            this.defaultWidth = defaultWidth;
            this.DisplayName = name.SplitCamelCase();
            Name = $"{ColumnNamePrefix}.{name}";
            this.IsFilterable = isFilterable;
            this.IsSortable = isSortable;
        }

        public override double DefaultWidth => defaultWidth ?? base.DefaultWidth;
        public override bool IsFilterable { get; }
        public override bool IsSortable { get; }

        public override string Name { get; }
        public override string DisplayName { get; }
    }
}
