using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Adornments;
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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace EditorExtensionsDemo.Error
{

    [Export(typeof(IViewTaggerProvider))]
    [TagType(typeof(ErrorTag))]
    //[Name("My.ErrorTaggerProvider")]
    [ContentType("text")]
    internal class ErrorTaggerProvider : IViewTaggerProvider
    {
        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
        {
            return new ErrorTagger() as ITagger<T>;
        }
    }
    

    class ErrorEditorFormatDefinition : EditorFormatDefinition
    {

        public ErrorEditorFormatDefinition(Color errorColor, string displayName = null) : this(displayName)
        {
            this.ForegroundColor = errorColor;

        }
        public ErrorEditorFormatDefinition(Brush brush, string displayName = null) : this(displayName)
        {
            this.ForegroundBrush = brush;
        }
        private ErrorEditorFormatDefinition(string displayName)
        {
            if (displayName != null)
            {
                this.DisplayName = displayName;
            }
            this.BackgroundCustomizable = false;
        }
    }
    internal class ErrorTagger : ITagger<IErrorTag>
    {
        public const string MethodNotCoveredErrorType = "MethodNotCovered";
        public const string MethodPartiallyCoveredErrorType = "MethodPartiallyCovered";
        public const string LineNotCoveredErrorType = "LineNotCovered";
        public const string LinePartiallyCoveredErrorType = "LinePartiallyCovered";

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        [Export(typeof(ErrorTypeDefinition))]
        [Name(MethodNotCoveredErrorType)]
        public ErrorTypeDefinition MethodNotCoveredErrorTypeDefinition { get; }

        [Export(typeof(ErrorTypeDefinition))]
        [Name(MethodPartiallyCoveredErrorType)]
        public ErrorTypeDefinition MethodPartiallyErrorTypeDefinition { get; }
        
        [Export(typeof(ErrorTypeDefinition))]
        [Name(LineNotCoveredErrorType)]
        public ErrorTypeDefinition LineNotCoveredErrorTypeDefinition { get; }
        
        [Export(typeof(ErrorTypeDefinition))]
        [Name(LinePartiallyCoveredErrorType)]
        public ErrorTypeDefinition LinePartiallyErrorTypeDefinition { get; }


        [Export(typeof(EditorFormatDefinition))]
        [Name(MethodNotCoveredErrorType)]
        [UserVisible(true)]
        public EditorFormatDefinition MethodNotCoveredErrorFormatDefinition { get; } = new ErrorEditorFormatDefinition(new SolidColorBrush(Colors.Pink));

        [Export(typeof(EditorFormatDefinition))]
        [Name(MethodPartiallyCoveredErrorType)]
        public EditorFormatDefinition MethodPartiallyCoveredErrorFormatDefinition { get; } = new ErrorEditorFormatDefinition(new LinearGradientBrush()
        {
            StartPoint = new System.Windows.Point(0, 0),
            EndPoint = new System.Windows.Point(1, 0),
            GradientStops = new GradientStopCollection
            {
                new GradientStop(Colors.Yellow, 0.0),
                new GradientStop(Colors.Red, 0.25),
                new GradientStop(Colors.Blue, 0.75),
                new GradientStop(Colors.LimeGreen, 1.0)
            }
        },"Call me what you want");
        
        [Name(LineNotCoveredErrorType)]
        [Export(typeof(EditorFormatDefinition))]
        public EditorFormatDefinition LineNotCoveredErrorFormatDefinition { get; } = new ErrorEditorFormatDefinition(Colors.Brown);
        [Name(LinePartiallyCoveredErrorType)]
        public EditorFormatDefinition LinePartiallyCoveredErrorFormatDefinition { get; } = new ErrorEditorFormatDefinition(Colors.Cyan);

        public IEnumerable<ITagSpan<IErrorTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            return new List<ITagSpan<IErrorTag>>
            {
                new TagSpan<IErrorTag>(new SnapshotSpan(spans[0].Snapshot, new Span(0, spans[0].Snapshot.Length)), new ErrorTag(MethodPartiallyCoveredErrorType, "Method partially covered")),
            };
        }
    }




    [Export(typeof(ISuggestedActionsSourceProvider))]
    [Name("My.SuggestedActionSourceProvider")] // required
    [ContentType("any")] // required
    //[Order]
    internal class MySuggestedActionSourceProvider : ISuggestedActionsSourceProvider
    {
        public ISuggestedActionsSource CreateSuggestedActionsSource(ITextView textView, ITextBuffer textBuffer)
        {
            return new MySuggestedActionSource();
        }
    }

    // there is ISuggestedAction2 and ISuggestedAction3
    internal class MySuggestedAction : ISuggestedAction
    {
        public string DisplayText => "MyDisplayText";
        //menuItem.DataContext = !(groupHeader is string str) || string.IsNullOrWhiteSpace(str) ? groupHeader : (object) (str + ":");

        // -----------------------------------------------------------------------------
        public ImageMoniker IconMoniker => KnownMonikers.CodeCoverageDisabled;

        // only used if supply above
        public string IconAutomationText => null;// there is a default.  But should supply

        // ------------------------------------------------------------------------

        public string InputGestureText => null;// for the created MenuItem

        

        public void Dispose()
        {
            
        }

        public bool HasActionSets => false; //todo
        public Task<IEnumerable<SuggestedActionSet>> GetActionSetsAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public bool HasPreview => true;//todo

        public Task<object> GetPreviewAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult<object>(new TextBlock { Text = "A preview"});
        }

        public void Invoke(CancellationToken cancellationToken)
        {
            new MessageBox().Show("Hello");
        }

        public bool TryGetTelemetryId(out Guid telemetryId)
        {
            telemetryId = Guid.Empty;
            return false;
        }
    }

    internal class MySuggestedActionSets : ISuggestedAction
    {
        public string DisplayText => "WithActionSets";
        //menuItem.DataContext = !(groupHeader is string str) || string.IsNullOrWhiteSpace(str) ? groupHeader : (object) (str + ":");

        // -----------------------------------------------------------------------------
        public ImageMoniker IconMoniker => KnownMonikers.RawCodeCoverageDataFile;

        // only used if supply above
        public string IconAutomationText => null;// there is a default.  But should supply

        // ------------------------------------------------------------------------

        public string InputGestureText => null;// for the created MenuItem



        public void Dispose()
        {

        }

        public bool HasActionSets => true;
        public Task<IEnumerable<SuggestedActionSet>> GetActionSetsAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(new List<SuggestedActionSet>
            {
                new SuggestedActionSet("My category", new List<ISuggestedAction>
                {
                    new MySuggestedAction()
                },"My nested", SuggestedActionSetPriority.Low,null),
                new SuggestedActionSet("My category", new List<ISuggestedAction>
                {
                    new MySuggestedAction()
                },"My nested", SuggestedActionSetPriority.Low,null)
            } as IEnumerable<SuggestedActionSet>);
        }

        public bool HasPreview => true;//todo

        public Task<object> GetPreviewAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult<object>(new TextBlock { Text = "A preview with action sets" });
        }

        public void Invoke(CancellationToken cancellationToken)
        {
            new MessageBox().Show("Hello");
        }

        public bool TryGetTelemetryId(out Guid telemetryId)
        {
            telemetryId = Guid.Empty;
            return false;
        }
    }

    internal class MySuggestedActionSetsWithFlavor : ISuggestedActionWithFlavors
    {
        public string DisplayText => "WithActionSets";
        //menuItem.DataContext = !(groupHeader is string str) || string.IsNullOrWhiteSpace(str) ? groupHeader : (object) (str + ":");

        // -----------------------------------------------------------------------------
        public ImageMoniker IconMoniker => KnownMonikers.RawCodeCoverageDataFile;

        // only used if supply above
        public string IconAutomationText => null;// there is a default.  But should supply

        // ------------------------------------------------------------------------

        public string InputGestureText => null;// for the created MenuItem



        public void Dispose()
        {

        }

        public bool HasActionSets => true;
        public Task<IEnumerable<SuggestedActionSet>> GetActionSetsAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(new List<SuggestedActionSet>
            {
                new SuggestedActionSet("My category", new List<ISuggestedAction>
                {
                    new MySuggestedAction()
                },"My nested", SuggestedActionSetPriority.Low,null),
                new SuggestedActionSet("My category", new List<ISuggestedAction>
                {
                    new MySuggestedAction()
                },"My nested", SuggestedActionSetPriority.Low,null)
            } as IEnumerable<SuggestedActionSet>);
        }

        public bool HasPreview => true;//todo

        public Task<object> GetPreviewAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult<object>(new TextBlock { Text = "A preview with action sets" });
        }

        public void Invoke(CancellationToken cancellationToken)
        {
            new MessageBox().Show("Hello");
        }

        public bool TryGetTelemetryId(out Guid telemetryId)
        {
            telemetryId = Guid.Empty;
            return false;
        }
    }


    internal class MySuggestedActionSource : ISuggestedActionsSource
    {
        public event EventHandler<EventArgs> SuggestedActionsChanged;

        public void Dispose()
        {
            
        }

        public IEnumerable<SuggestedActionSet> GetSuggestedActions(ISuggestedActionCategorySet requestedActionCategories, SnapshotSpan range, CancellationToken cancellationToken)
        {
            // will be called with Any if press the lighbulb not triggered from errors
            if(requestedActionCategories.Any(x=>x.ToLower() == PredefinedSuggestedActionCategoryNames.CodeFix.ToLower()))
            {
                return new List<SuggestedActionSet>
                {
                    // category is used when invoke action
                    // this.session.InvokeAction(categoryName, action, topLevelMenuItemIndex); for telemetry
                    // ILightBulbSession.ApplicableCategories
                    //this.LightBulbIconContentControl.Content = (object) this.presenterStyleFactory.IconProvider.GetUIElement(session?.ApplicableCategories, (ILightBulbSession) session, UIElementType.Small);
                    new SuggestedActionSet("My category", new List<ISuggestedAction>
                    {
                        new MySuggestedAction(),
                        new MySuggestedActionSetsWithFlavor(),
                        new MySuggestedAction()
                    },"My title", SuggestedActionSetPriority.Low,null),
                    new SuggestedActionSet("My Category",new List<ISuggestedAction>
                    {
                        new MySuggestedActionSets()
                    },"Nested title",SuggestedActionSetPriority.Low,null),
                    

                };
            }
            return Enumerable.Empty<SuggestedActionSet>();
        }

        public Task<bool> HasSuggestedActionsAsync(ISuggestedActionCategorySet requestedActionCategories, SnapshotSpan range, CancellationToken cancellationToken)
        {
            // of course would check the range
            if (requestedActionCategories.Contains(PredefinedSuggestedActionCategoryNames.CodeFix))
            {
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public bool TryGetTelemetryId(out Guid telemetryId)
        {
            telemetryId = Guid.Empty;
            return false;
        }
    }


}
