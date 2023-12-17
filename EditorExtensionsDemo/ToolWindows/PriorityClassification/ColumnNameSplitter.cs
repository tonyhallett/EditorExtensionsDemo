namespace EditorExtensionsDemo
{
    public static class ColumnNameSplitter
    {
        public static string Split(string columnName)
        {
            return columnName.IndexOf(".") == -1 ? columnName : columnName.Substring(columnName.IndexOf(".") + 1);
        }
    }
}
