using EditorExtensionsDemo.Classification;
using Microsoft;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Debugger.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Projection;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using static System.Net.WebRequestMethods;

namespace EditorExtensionsDemo
{
    public static class AllowedCommands
    {
        public static Dictionary<Guid, uint[]> Instance = new()
        {
            {
            //https://learn.microsoft.com/en-us/dotnet/api/microsoft.visualstudio.vsconstants.vsstd2kcmdid?view=visualstudiosdk-2022
            //https://learn.microsoft.com/en-us/dotnet/api/microsoft.visualstudio.vsconstants.vsstd2kcmdid

            VSConstants.VSStd2K,
            new uint[33]
            {
                1U,
                2U,
                3U,
                4U,
                6U,
                7U,//added
                9U,//added
                11U,//added
                13U,//added
                58U,
                61U,
                60U,
                71U,
                73U,
                63U,
                62U,
                64U,
                92U,
                91U,
                1003U,
                108U,
                1550U,
                69U,
                70U,
                136U,
                137U,
                98U,
                99U,
                143U,
                112U,
                108U,
                107U,
                155U
            }
            },
            //{
            //  VSConstants.VsStd14,
            //  new uint[5]{ 1U, 2U, 3U, 6U, 7U }
            //},
            //{
            //  EditorConstants.EditorCommandSet,
            //  new uint[2]{ 49U, 48U }
            //},
            //{
            //  VSConstants.GUID_VSStandardCommandSet97,
            //  new uint[11]
            //  {
            //    17U,
            //    16U,
            //    26U,
            //    43U,
            //    29U,
            //    110U,
            //    111U,
            //    331U,
            //    226U,
            //    224U,
            //    225U
            //  }
            //}
        };


    }
    public class MyDialogWindow : Window
    {
        public MyDialogWindow()
        {
            var hoster = HostEditorHelper.CreateHoster(
                ClassificationTagger.GetCode(false),
                "code",//"CSharp","Roslyn Languages"
                new string[] { 
                    PredefinedTextViewRoles.Document,
                    PredefinedTextViewRoles.Editable, 
                    PredefinedTextViewRoles.Structured,
                    PredefinedTextViewRoles.Interactive,
                    PredefinedTextViewRoles.Analyzable,
                    PredefinedTextViewRoles.Zoomable,
                    PredefinedTextViewRoles.PrimaryDocument,
                    PredefinedTextViewRoles.ChangePreview
                    //     The predefined role used for change previews (used by light bulbs, quick actions,
                    //     etc.).
                    //public const string ChangePreview = "CHANGEPREVIEW";
                },
                AllowedCommands.Instance,
                (view) =>
                {
                    //view.Options.SetOptionValue("Appearance/Category", "text");
                },
                false
            );
            this.Content = hoster.GetControl();
        }

    }


    [Command(PackageIds.MyCommand)]
    internal sealed class MyCommand : BaseCommand<MyCommand>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            var _ =  await new MyDialogWindow().ShowDialogAsync();
        }


    }



}
