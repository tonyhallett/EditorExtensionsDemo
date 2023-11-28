using EditorExtensionsDemo.QuickInfo.Models;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Text.Adornments;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;

namespace EditorExtensionsDemo.QuickInfo
{
    [Export(typeof(IViewElementFactory))]
    [Name("my ITextBuffer to UIElement")]
    [TypeConversion(typeof(CodeModel), typeof(UIElement))]
    [Order]
    internal sealed class CodeModelViewElementFactory : IViewElementFactory
    {
        public TView CreateViewElement<TView>(ITextView textView, object model) where TView : class
        {
            var codeModel = model as CodeModel;
            //var hoster = HostEditor.CreateReadOnlyHoster(codeModel!.Code, codeModel!.ContentType, codeModel!.TextViewRoles, codeModel!.ReadSpecificAllowedCommands, codeModel!.InitializeTextView);
            //Dictionary<Guid, uint[]> ReadWriteSpecificAllowedCommands = new Dictionary<Guid, uint[]>
            //{
            //  {
            //    VSConstants.VSStd2K,
            //    new uint[31]
            //    {
            //      1U,
            //      2U,
            //      3U,
            //      6U,
            //      7U,//added
            //      9U,//added
            //      11U,//added
            //      13U,//added
            //      58U,
            //      61U,
            //      60U,
            //      71U,
            //      73U,
            //      63U,
            //      62U,
            //      64U,
            //      92U,
            //      91U,
            //      1003U,
            //      1550U,
            //      69U,
            //      70U,
            //      136U,
            //      137U,
            //      98U,
            //      99U,
            //      143U,
            //      112U,
            //      108U,
            //      107U,
            //      155U
            //    }
            //  },
            //  //{
            //  //  VSConstants.VsStd14,
            //  //  new uint[5]{ 1U, 2U, 3U, 6U, 7U }
            //  //},
            //  //{
            //  //  EditorConstants.EditorCommandSet,
            //  //  new uint[2]{ 49U, 48U }
            //  //},
            //  //{
            //  //  VSConstants.GUID_VSStandardCommandSet97,
            //  //  new uint[11]
            //  //  {
            //  //    17U,
            //  //    16U,
            //  //    26U,
            //  //    43U,
            //  //    29U,
            //  //    110U,
            //  //    111U,
            //  //    331U,
            //  //    226U,
            //  //    224U,
            //  //    225U
            //  //  }
            //  //}
            //};

            var hoster = HostEditorHelper.CreateHoster(
                codeModel!.Code, 
                codeModel!.ContentType, 
                codeModel!.TextViewRoles, 
                codeModel.AllowedCommands, 
                codeModel!.InitializeTextView, 
                codeModel.ReadOnly
            );
            return hoster.GetControl() as TView;
            //return new Hosted(hoster) as TView;
        }

        
    }

    public class Hosted : UserControl
    {
        public Hosted(Hoster hoster) {
            this.Content = hoster.GetControl();
        }
    }
}
