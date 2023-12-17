using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell.Settings;
using Microsoft.VisualStudio.Settings;
using System.Text.Json;

namespace EditorExtensionsDemo
{
    [Export(typeof(ISettingsManager))]
    internal class SettingsManager : ISettingsManager
    {
        private readonly ShellSettingsManager shellSettingsManager;
        private readonly WritableSettingsStore settingsStore;

        public SettingsManager()
        {
            var vsSettingsManager = VS.GetRequiredService<SVsSettingsManager, IVsSettingsManager>();
            shellSettingsManager = new ShellSettingsManager(vsSettingsManager);
            settingsStore = shellSettingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);
        }

        public T GetData<T>(string collectionPath, string propertyName)
        {
            if(settingsStore.CollectionExists(collectionPath) == false)
            {
                return default;
            }
            var asString = settingsStore.GetString(collectionPath, propertyName);
            if(asString == null)
            {
                return default;
            }
            return JsonSerializer.Deserialize<T>(asString);
        }

        public void SetData(string collectionPath, string propertyName, object value)
        {
            var serializedValue = JsonSerializer.Serialize(value);
            if(settingsStore.CollectionExists(collectionPath) == false)
            {
                settingsStore.CreateCollection(collectionPath);
            }
            settingsStore.SetString(collectionPath, propertyName, serializedValue);
        }
    }

}
