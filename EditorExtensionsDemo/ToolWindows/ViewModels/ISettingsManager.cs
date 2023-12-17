namespace EditorExtensionsDemo
{
    interface ISettingsManager
    {
        T GetData<T>(string collectionPath, string propertyName);
        void SetData(string collectionPath, string propertyName, object value);
    }

}
