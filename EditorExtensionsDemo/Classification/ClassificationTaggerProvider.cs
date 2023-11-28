using EditorExtensionsDemo.Tagging;
using Microsoft.CodeAnalysis.Classification;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace EditorExtensionsDemo.Classification
{
    public class MyTransientClassificationFormatDefinition : ClassificationFormatDefinition
    {
        public MyTransientClassificationFormatDefinition()
        {
            this.BackgroundBrush = System.Windows.Media.Brushes.Red;
        }
    }   
    
    public class MyBaseClassificationFormatDefinition : ClassificationFormatDefinition
    {
        public MyBaseClassificationFormatDefinition()
        {
            this.BackgroundBrush = System.Windows.Media.Brushes.Purple;
        }
    }

    public class ForegroundFormatDefinition : ClassificationFormatDefinition
    {
        public ForegroundFormatDefinition(Color color)
        {
            this.ForegroundBrush = new SolidColorBrush(color);
        }
        public ForegroundFormatDefinition(Brush brush)
        {
            this.ForegroundBrush = brush;
        }
    }

    

    public class ClassificationTypes {
        public IClassificationType UnorderedType { get; }
        public IClassificationType AnotherUnorderedType { get; }
        public IClassificationType OrderedType { get; }
        public ClassificationTypes(IClassificationTypeRegistryService classificationRegistry)
        {
            UnorderedType = classificationRegistry.GetClassificationType(ClassificationTaggerProvider.UnOrderedClassificationType);
            AnotherUnorderedType = classificationRegistry.GetClassificationType(ClassificationTaggerProvider.AnotherUnOrderedClassificationType);
            OrderedType = classificationRegistry.GetClassificationType(ClassificationTaggerProvider.OrderedWithBaseClassificationType);
        }
    }


    [Export(typeof(ITaggerProvider))]
    [ContentType("code")] // could change this to be specific for a made up content type that I create a host control for
    [Name("My.ClassificationTaggerProvider")]
    [TagType(typeof(ClassificationTag))]
    internal class ClassificationTaggerProvider : ITaggerProvider
    {
        #region classification type definitions
        public const string UnOrderedClassificationType = "UnOrderedClassficationType";
        [Export(typeof(ClassificationTypeDefinition))]
        [Name(UnOrderedClassificationType)]
        public ClassificationTypeDefinition UnorderedClassificationTypeDefinition { get; }

        public const string AnotherUnOrderedClassificationType = "AnotherUnOrderedClassficationType";
        [Export(typeof(ClassificationTypeDefinition))]
        [Name(AnotherUnOrderedClassificationType)]
        public ClassificationTypeDefinition AnotherUnorderedClassificationTypeDefinition { get; }



        public const string OrderedWithBaseClassificationType = "MyOrderedClassificationType";
        [Export(typeof(ClassificationTypeDefinition))]
        [Name(OrderedWithBaseClassificationType)]
        [BaseDefinition(BaseClassificationType)]
        public ClassificationTypeDefinition OrderedWithBaseClassificationTypeDefinition { get; }

        public const string BaseClassificationType = "MyBaseClassificationType";
        [Export(typeof(ClassificationTypeDefinition))]
        [Name(BaseClassificationType)]
        public ClassificationTypeDefinition BaseClassificationTypeDefinition { get; }

        #endregion
        
        #region classification format definitions
        [Export(typeof(EditorFormatDefinition))]
        //as not ordered no need for ClassificationTypeAttribute - the name needs to match the classification type name
        [Name(UnOrderedClassificationType)]
        public ForegroundFormatDefinition MyUnOrderedClassificationFormatDefinition { get; } = new ForegroundFormatDefinition(Colors.Orange);

        [Export(typeof(EditorFormatDefinition))]
        [Name(AnotherUnOrderedClassificationType)]
        public ForegroundFormatDefinition MyAnotherUnOrderedClassificationFormatDefinition { get; } = new ForegroundFormatDefinition(Colors.Cyan);

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = BaseClassificationType)]
        [Name(BaseClassificationType)]
        public MyBaseClassificationFormatDefinition MyBaseClassificationFormatDefinition { get; } = new MyBaseClassificationFormatDefinition();

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = OrderedWithBaseClassificationType)]
        [Name(OrderedWithBaseClassificationType)]
        [Order(After = ClassificationTypeNames.Comment)]
        //[Order(After = Priority.High)]
        public ForegroundFormatDefinition MyOrderedClassificationFormatDefinition { get; } = new ForegroundFormatDefinition(Colors.Blue);


        [Export(typeof(EditorFormatDefinition))]
        [Name($"{UnOrderedClassificationType} - {ClassificationTypeNames.Comment} - (TRANSIENT)")]
        public MyTransientClassificationFormatDefinition MyTransientClassificationFormatDefinition { get; } = new MyTransientClassificationFormatDefinition();
        #endregion

        [Import]
        internal IClassificationTypeRegistryService ClassificationRegistry { get; set; }

        [Import]
        internal IClassificationFormatMapService ClassificationFormatMapService { get; set; }

        private static ClassificationTypes classificationTypes;
        public static ClassificationTypes GetClassificationTypes(IClassificationTypeRegistryService classificationTypeRegistryService)
        {
            if (classificationTypes == null)
            {
                classificationTypes = new ClassificationTypes(classificationTypeRegistryService);
            }
            return classificationTypes;
            
        }

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            return new ClassificationTagger(buffer, GetClassificationTypes(ClassificationRegistry)) as ITagger<T>;
        }

    }


    internal class ClassificationTagger : RegexTagger<ClassificationTag>
    {
        public const string TransientWithComment = "TransientWithComment";
        public const string Unordered = "Unordered";
        public const string Ordered = "OrderWinsTransientWithBase";

        public static string GetCode(bool isCustom)
        {
            return @$"
//ITextBuffer => IWpfTextView
namespace DemoFCC
{{
    {(isCustom ? "// no classification applied due to content type not matching" : "")}
    // {ClassificationTagger.Ordered} 
    //{ClassificationTagger.TransientWithComment} {ClassificationTagger.Unordered}
    // Adornments do{(isCustom ? " not" : "")} show as content type is {(isCustom ? "not" : "")} text or has base type text.
    // intra
}}
";
        }

        private readonly ClassificationTypes classificationTypes;

        public ClassificationTagger(ITextBuffer textBuffer, ClassificationTypes classificationTypes) :base(textBuffer, new[] { 
            new Regex($"{TransientWithComment}", RegexOptions.Compiled | RegexOptions.CultureInvariant),
            new Regex($"{Unordered}", RegexOptions.Compiled | RegexOptions.CultureInvariant),
            new Regex($"{Ordered}", RegexOptions.Compiled | RegexOptions.CultureInvariant)
            }
        ){
            this.classificationTypes = classificationTypes;
        }

        private IClassificationType GetClassificationType(string classificationTypeName)
        {
            switch (classificationTypeName)
            {
                case TransientWithComment:
                    return classificationTypes.UnorderedType;
                case Unordered:
                    return classificationTypes.AnotherUnorderedType;
                default:
                    return classificationTypes.OrderedType;
            }
        }
        protected override ClassificationTag TryCreateTagForMatch(Match match)
        {
            return new ClassificationTag(GetClassificationType(match.Value));
        }
    }
    
}
