using System.Linq;
using Microsoft.VisualStudio.Shell.TableControl;
using System.Windows;
using System.Windows.Controls;
using System.Reflection;

namespace EditorExtensionsDemo
{
    internal class ReflectablePriorityClassificationTableEntry: ReflectableTableEntry<PriorityClassification>, IHavePriorityClassification
    {
        public ReflectablePriorityClassificationTableEntry(PriorityClassification entry, int index, object identity) : base(entry, index, identity)
        {
            PriorityClassification = entry;
        }
        

        public PriorityClassification PriorityClassification { get; }
    }
    public class ReflectableTableEntry<T> : WpfTableEntryBase, IIndexedTableEntry
    {
        private static readonly PropertyInfo[] propertyInfos;
        internal readonly T entry;
        protected Type[] stringTypes = new Type[] { typeof(int), typeof(string),typeof(Enum)};

        static ReflectableTableEntry()
        {
            propertyInfos = typeof(T).GetProperties();
        }

        public ReflectableTableEntry(T entry, int index, object identity)
        {
            this.entry = entry;
            this.Index = index;
            this.Identity = identity;
        }

        public int Index { get; }
        public override object Identity { get; }
        private PropertyInfo GetProperty(string columnName)
        {
            var pName = ColumnNameSplitter.Split(columnName);
            return propertyInfos.First(pd => pd.Name == pName);
        }
        public override bool TryCreateStringContent(string columnName, bool truncatedText, bool singleColumnView, out string content)
        {
            var propertyInfo = GetProperty(columnName);
            content = GetStringContent(propertyInfo.GetValue(entry), columnName);
            return true;
            
        }

        protected virtual string GetStringContent(object value,string propertyName)
        {
            return value.ToString();
        }

        public override bool TryCreateColumnContent(string columnName, bool singleColumnView, out FrameworkElement content)
        {
            var propertyInfo = GetProperty(columnName);
            if (stringTypes.Contains(propertyInfo.PropertyType))
            {
                content = null;
                return false;
            }
            content = new ContentPresenter { Content = propertyInfo.GetValue(entry) };
            return true;
        }
    }
}
