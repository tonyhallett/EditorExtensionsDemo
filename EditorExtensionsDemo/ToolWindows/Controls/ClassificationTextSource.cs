using Microsoft.VisualStudio.Text.Formatting;
using System.Windows.Media.TextFormatting;

namespace EditorExtensionsDemo
{
    public class ClassificationTextSource : TextSource
    {
        private readonly TextFormattingRunProperties secondTextFormattingRunProperties;
        private TextFormattingRunProperties currentTextFormattingRunProperties;
        public ClassificationTextSource(string classification, TextFormattingRunProperties firstTextFormattingRunProperties, TextFormattingRunProperties secondTextFormattingRunProperties)
        {
            this.Classification = classification;
            this.secondTextFormattingRunProperties = secondTextFormattingRunProperties;
            currentTextFormattingRunProperties = firstTextFormattingRunProperties;
        }

        // because of this going to have to display the details filled in a popup


        public string Classification { get; }

        public override TextSpan<CultureSpecificCharacterBufferRange> GetPrecedingText(int textSourceCharacterIndexLimit)
        {
            throw new NotImplementedException();
        }

        public override int GetTextEffectCharacterIndexFromTextSourceCharacterIndex(int textSourceCharacterIndex)
        {
            return 0;
        }
        private bool providedTextRun = false;
        public override TextRun GetTextRun(int textSourceCharacterIndex)
        {
            if (providedTextRun)
            {
                return new TextEndOfLine(textSourceCharacterIndex);
            }
            providedTextRun = true;
            return new TextCharacters(this.Classification, currentTextFormattingRunProperties);
        }

        internal void Next()
        {
            providedTextRun = false;
            currentTextFormattingRunProperties = secondTextFormattingRunProperties;
        }
    }
}
