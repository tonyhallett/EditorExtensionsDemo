namespace EditorExtensionsDemo
{
    internal interface ICurrentClassificationTextFormattingRunPropertiesFormatMapService
    {
        ClassificationTextFormattingRunPropertiesFormatMap CurrentClassificationTextFormattingRunPropertiesFormatMap { get; }
        void SetCategory(string category);
        event EventHandler CurrentClassificationTextFormattingRunPropertiesFormatMapChanged;
        
    }
}
