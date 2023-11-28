using Microsoft.VisualStudio.Text.Classification;
using System.Windows.Media;

namespace EditorExtensionsDemo.QuickInfo
{
    class BackgroundEditorFormatDefinition : EditorFormatDefinition
    {

        public BackgroundEditorFormatDefinition(Color color, string displayName = null) : this(displayName)
        {
            this.BackgroundColor = color;

        }
        public BackgroundEditorFormatDefinition(Brush brush, string displayName = null) : this(displayName)
        {
            this.BackgroundBrush = brush;
        }
        private BackgroundEditorFormatDefinition(string displayName)
        {
            if (displayName != null)
            {
                this.DisplayName = displayName;
            }
            this.ForegroundCustomizable = false;
        }
    }
}
