using EditorExtensionsDemo.Classification;
using EnvDTE;
using Microsoft.CodeAnalysis.Host;
using Microsoft.VisualStudio.Core.Imaging;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Adornments;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Threading;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using System.Windows.Threading;

namespace EditorExtensionsDemo.QuickInfo
{
    internal class OverridesCommentClassificationFormatDefinition : ClassificationFormatDefinition
    {
        public OverridesCommentClassificationFormatDefinition()
        {
            //this.ForegroundColor = Colors.Pink; - For Comment - this will get overridden by DataStorage
            //this.BackgroundColor = Colors.LightBlue;

            //  this.ForegroundOpacity = 0.1;
            this.IsBold = true;
            this.IsItalic = true;
            this.TextDecorations = new System.Windows.TextDecorationCollection { System.Windows.TextDecorations.OverLine };
            
        }
    }


    [Export(typeof(IAsyncQuickInfoSourceProvider))]
    [Name("My demo quick info source provider")]
    [ContentType("text")]
    [Order]
    internal class AsyncQuickInfoSourceProvider : IAsyncQuickInfoSourceProvider
    {

        [Export(typeof(EditorFormatDefinition))]
        [Name("Comment")]
        [UserVisible(true)]
        [Priority(1)]
        internal EditorFormatDefinition CommentFormatDefinition { get; set; } = new OverridesCommentClassificationFormatDefinition();


        public const string MarkerTagTypeForClassifiedTextRunBackground = "MarkerTagTypeForClassifiedTextRunBackground";

        [Export(typeof(EditorFormatDefinition))] // needs to go on here to be picked up by the editor
        [Name(MarkerTagTypeForClassifiedTextRunBackground)]
        [UserVisible(true)]
        public EditorFormatDefinition ClassifiedTextRunBackgroundEditorFormatDefinition { get; set; } = new BackgroundEditorFormatDefinition(Colors.Orange);

        internal const string MyCustomClassificationType = "MyCustomClassificationType";
        [Export(typeof(EditorFormatDefinition))]
        [Name(MyCustomClassificationType)]
        internal EditorFormatDefinition MyCustomClassificationFormatDefinition { get; set; } = new MyClassificationFormatDefinition();

        [Export(typeof(ClassificationTypeDefinition))]
        [ClassificationType(ClassificationTypeNames =MyCustomClassificationType)]
        [Name(MyCustomClassificationType)]
        [Order]
        //UserVisible ???????
        internal ClassificationTypeDefinition MyCustomClassificationTypeDefinition { get;}


        [Import(typeof(ITextBufferFactoryService))]
        internal ITextBufferFactoryService TextBufferFactoryService { get; set; }

        [Import(typeof(IContentTypeRegistryService))]
        internal IContentTypeRegistryService ContentTypeRegistryService { get; set; }
        public IAsyncQuickInfoSource TryCreateQuickInfoSource(ITextBuffer textBuffer)
        {
            return new AsyncQuickInfoSource(textBuffer, TextBufferFactoryService, ContentTypeRegistryService);
        }

        public const string CustomContentType = "custom";
        [Export]
        [Name(CustomContentType)]
        //[BaseDefinition("text")]
        internal static ContentTypeDefinition CustomContentTypeDefinition = null;
    }

    internal class AsyncQuickInfoSource : IAsyncQuickInfoSource
    {
        private readonly ITextBuffer textBuffer;
        private readonly ITextBufferFactoryService textBufferFactoryService;
        private readonly IContentTypeRegistryService contentTypeRegistryService;

        public AsyncQuickInfoSource(
            ITextBuffer textBuffer, 
            ITextBufferFactoryService textBufferFactoryService,
            IContentTypeRegistryService contentTypeRegistryService)
        {
            this.textBuffer = textBuffer;
            this.textBufferFactoryService = textBufferFactoryService;
            this.contentTypeRegistryService = contentTypeRegistryService;
        }
        public void Dispose()
        {
        }

        public Task<QuickInfoItem> GetQuickInfoItemAsync(IAsyncQuickInfoSession session, CancellationToken cancellationToken)
        {
            var trackingPoint = session.GetTriggerPoint(textBuffer);
            var line = session.TextView.GetTextViewLineContainingBufferPosition(trackingPoint.GetPoint(textBuffer.CurrentSnapshot));
            var text = line.Extent.GetText();
            QuickInfoItem quickInfoItem = null;
            if(text.Contains("Show me"))
            {
                var code = ClassificationTagger.GetCode(false);
                
                // Note get exception The calling thread must be STA if create a UIElement
                quickInfoItem = new QuickInfoItem(textBuffer.CurrentSnapshot.CreateTrackingSpan(line.Extent, SpanTrackingMode.EdgeInclusive),

                    new ContainerElement(
                        ContainerElementStyle.Stacked,
                        new WpfStringViewElementModel("Hello world"),
                        new ImageElement(KnownMonikers.Comment.ToImageId()),

                        new ClassifiedTextElement(
                            new ClassifiedTextRun(
                                PredefinedClassificationTypeNames.Keyword, "Keyword", ClassifiedTextRunStyle.Italic, AsyncQuickInfoSourceProvider.MarkerTagTypeForClassifiedTextRunBackground)
                            ),
                            new ClassifiedTextElement(
                                new ClassifiedTextRun(
                                PredefinedClassificationTypeNames.Comment, "//Comment"
                            )
                        ),
                        new ClassifiedTextElement(
                            new ClassifiedTextRun(
                                PredefinedClassificationTypeNames.Identifier, "Identifier", () =>
                                {
                                    new Community.VisualStudio.Toolkit.MessageBox().Show("Clicked 1");
                                }, "Hyperlink tooltip"
                            ),
                            new ClassifiedTextRun(
                                PredefinedClassificationTypeNames.Text, "text", () =>
                                {
                                    new Community.VisualStudio.Toolkit.MessageBox().Show("Clicked 2");
                                }
                            )
                        ),
                        new ClassifiedTextElement(
                                new ClassifiedTextRun(
                                AsyncQuickInfoSourceProvider.MyCustomClassificationType, "Custom classification type"
                            )
                        ),
                        new ClassifiedTextElement(
                                new ClassifiedTextRun(
                                AsyncQuickInfoSourceProvider.MyCustomClassificationType, "Custom classification type with styles",
                                ClassifiedTextRunStyle.UseClassificationFont | ClassifiedTextRunStyle.UseClassificationStyle
                            )
                        ),
                        new ThematicBreakElement(),
                        new ViewElementFactoryModel(123),

                        new Models.CodeModel(
                            code,
                            "Roslyn Languages",
                            Hoster.ReadOnlyAllPredefinedTextViewRoles,
                            (view) => {
                                //
                            },
                            true
                        ),
                        new Models.CodeModel(
                            code,
                            "Roslyn Languages",
                            new string[] {
                                PredefinedTextViewRoles.Document,
                                PredefinedTextViewRoles.Editable,
                                PredefinedTextViewRoles.Structured,
                                PredefinedTextViewRoles.Interactive,
                                PredefinedTextViewRoles.Analyzable,
                                PredefinedTextViewRoles.Zoomable,
                                PredefinedTextViewRoles.PrimaryDocument,
                                PredefinedTextViewRoles.ChangePreview
                            },
                            (view) => {
                                //
                            },
                            false
                        ),
                        textBufferFactoryService.CreateTextBuffer(code,contentTypeRegistryService.GetContentType("CSharp")),
                        //"Roslyn Languages"
                        textBufferFactoryService.CreateTextBuffer(ClassificationTagger.GetCode(true), contentTypeRegistryService.GetContentType(AsyncQuickInfoSourceProvider.CustomContentType))
                    )
                );
            }
            return Task.FromResult(quickInfoItem);
        }
    }
}
