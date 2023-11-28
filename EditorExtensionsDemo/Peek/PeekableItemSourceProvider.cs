using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Adornments;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace EditorExtensionsDemo.Peek {
    [Export(typeof(IPeekResultPresenter))]
    [Name("My peek result presenter")]
    internal class PeekResultPresenter : IPeekResultPresenter
    {
        public IPeekResultPresentation TryCreatePeekResultPresentation(IPeekResult result)
        {
            if(result is PeekResult peekResult)
            {
                return new PeekResultPresentation(peekResult);
            }
            return null;
        }
    }
    public class PeekResultScrollState : IPeekResultScrollState
    {
        public void Dispose()
        {
        }

        public void RestoreScrollState(IPeekResultPresentation presentation)
        {
        }
    }
    public class PeekResultPresentation : IPeekResultPresentation
    {
        public double ZoomLevel { get; set; } = 1;

        public bool IsDirty { get; private set; }

        public bool IsReadOnly { get; private set; } = true;

        private PeekResult PeekResult { get; }

        public event EventHandler<RecreateContentEventArgs> RecreateContent;
        public event EventHandler IsDirtyChanged;
        public event EventHandler IsReadOnlyChanged;

        public PeekResultPresentation(PeekResult peekResult)
        {
            _ = Task.Delay(5000).ContinueWith((_) =>
            {
                IsDirty = true;
                IsReadOnly = false;
                IsDirtyChanged?.Invoke(this, EventArgs.Empty);
                IsReadOnlyChanged?.Invoke(this, EventArgs.Empty);
            });
            this.PeekResult = peekResult;
        }

        public bool CanSave(out string defaultPath)
        {
            Debug.WriteLine("CanSave");
            defaultPath = null;
            return false;
        }

        public IPeekResultScrollState CaptureScrollState()
        {
            Debug.WriteLine("CaptureScrollState");
            return new PeekResultScrollState();
        }

        public void Close()
        {
            Debug.WriteLine("Close");
        }
        //scrollState is null
        public UIElement Create(IPeekSession session, IPeekResultScrollState scrollState)
        {
            Debug.WriteLine("Create");
            var stackPanel = new StackPanel();
            
            stackPanel.Children.Add( new TextBlock()
            {
                Text = PeekResult.DisplayInfo.Title
            });

            var changeDirtyButton = new Button()
            {
                Content = "Change IsDirty"
            };
            changeDirtyButton.Click += ChangeDirtyButton_Click;
            stackPanel.Children.Add(changeDirtyButton);

            var changeIsReadOnlyButton = new Button()
            {
                Content = "Change read only"
            };
            changeIsReadOnlyButton.Click += ChangeIsReadOnlyButton_Click;
            stackPanel.Children.Add(changeIsReadOnlyButton);

            return stackPanel;
        }

        private void ChangeIsReadOnlyButton_Click(object sender, RoutedEventArgs e)
        {
            this.IsReadOnly = !IsReadOnly;
            IsReadOnlyChanged?.Invoke(this, EventArgs.Empty);
        }

        private void ChangeDirtyButton_Click(object sender, RoutedEventArgs e)
        {
            this.IsDirty = !IsDirty;
            IsDirtyChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Dispose()
        {
            Debug.WriteLine("Dispose");
        }

        public void ScrollIntoView(IPeekResultScrollState scrollState)
        {
            Debug.WriteLine("ScrollIntoView");
        }

        public void SetKeyboardFocus()
        {
            Debug.WriteLine("SetKeyboardFoxus");
        }

        public bool TryOpen(IPeekResult otherResult)
        {
            Debug.WriteLine("TryOpen");
            return false;
        }

        public bool TryPrepareToClose()
        {
            Debug.WriteLine("TryPrepareToClose");
            return true;
        }

        public bool TrySave(bool saveAs)
        {
            Debug.WriteLine("TrySaveAs");
            return false;
        }
    }

    //[Export(typeof(IPeekableItemSourceProvider))]
    [Name("MyPeekableItemSourceProvider")]
    [ContentType("text")]
    internal class PeekableItemSourceProvider : IPeekableItemSourceProvider
    {
        public IPeekableItemSource TryCreatePeekableItemSource(ITextBuffer textBuffer)
        {
            return new PeekableItemSource(textBuffer);
        }
    }

    public class PeekableItemSource : IPeekableItemSource
    {
        private ITextBuffer textBuffer;
        public PeekableItemSource(ITextBuffer textBuffer)
        {
            this.textBuffer = textBuffer;
        }
        public void AugmentPeekSession(IPeekSession session, IList<IPeekableItem> peekableItems)
        {
            //session.GetTriggerPoint(textBuffer)
            peekableItems.Add(new PeekableItem());
            peekableItems.Add(new PeekableItem2());
        }

        public void Dispose()
        {
        }
    }

    public class PeekRelationship : IPeekRelationship
    {
        public string Name => "relationship";

        public string DisplayName => "Relationship";
    }


    public class PeekableItem2 : IPeekableItem
    {
        public string DisplayName => "My peekable item 2";

        public IEnumerable<IPeekRelationship> Relationships => throw new Exception("Not implemented");
        public IPeekResultSource GetOrCreateResultSource(string relationshipName)
        {
            return new PeekResultSource2();
        }
    }


    public class PeekResultSource2 : IPeekResultSource
    {
        // Is being called for IsDefinedBy 
        public void FindResults(string relationshipName, IPeekResultCollection resultCollection, CancellationToken cancellationToken, IFindPeekResultsCallback callback)
        {
            new Community.VisualStudio.Toolkit.MessageBox().Show("Second source called");
            resultCollection.Add(new PeekResult("From source 2", false));
        }
    }

    public class PeekableItem : IPeekableItem
    {
        public string DisplayName => "My peekable item";
        
        //public IEnumerable<IPeekRelationship> Relationships => new List<IPeekRelationship>
        //{
        //    new PeekRelationship()
        //};
        public IEnumerable<IPeekRelationship> Relationships => throw new Exception("Not implemented");

        public IPeekResultSource GetOrCreateResultSource(string relationshipName)
        {
            return new PeekResultSource();
        }
    }



    public class PeekResultSource : IPeekResultSource
    {
        // Is being called for IsDefinedBy 
        public void FindResults(string relationshipName, IPeekResultCollection resultCollection, CancellationToken cancellationToken, IFindPeekResultsCallback callback)
        {
            callback.ReportProgress(10);
            resultCollection.Add(new PeekResult("CanNavigateTo", true));
            _ = Task.Delay(500).ContinueWith((_) =>
            {
                callback.ReportProgress(50);
                //callback.ReportFailure(new Exception("Failure !"));
            });
            _ = Task.Delay(3000).ContinueWith((_) =>
            {
                resultCollection.Add(new PeekResult("CannotNavigateTo", false));
                //callback.ReportProgress(100);
            });

            
        }
    }

    public class PeekResult : IPeekResult
    {
        public PeekResult(string title,bool canNavigateTo)
        {
            CanNavigateTo = canNavigateTo;
            DisplayInfo = new PeekResultDisplayInfo("MyLabel", "MyLabelTooltip", title, "MyTitleTooltip");
            PostNavigationCallback = (result, document, view) =>
            {
                // think that need to call it yourself in NavigateTo
            };
        }
        public IPeekResultDisplayInfo DisplayInfo { get; private set; }

        public bool CanNavigateTo { get; private set; }

        public Action<IPeekResult, object, object> PostNavigationCallback { get; private set; }

        public event EventHandler Disposed;

        public void Dispose()
        {
        }

        public void NavigateTo(object data)
        {
            //data is PeekResultScrollState
        }
    }



}
