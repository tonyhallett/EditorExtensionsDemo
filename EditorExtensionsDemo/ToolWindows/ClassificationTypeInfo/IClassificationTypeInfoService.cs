using System.Collections.Generic;

namespace EditorExtensionsDemo
{
    internal interface IClassificationTypeInfoService
    {
        List<ClassificationTypeInfo> ClassificationTypeInfo { get; }
        void GetClassificationTypeInfo();
        event EventHandler ClassificationTypesChanged;
    }
}
