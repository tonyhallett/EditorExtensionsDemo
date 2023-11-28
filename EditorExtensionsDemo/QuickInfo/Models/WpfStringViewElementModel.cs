namespace EditorExtensionsDemo.QuickInfo
{
    class WpfStringViewElementModel
    {
        private readonly string text;

        public WpfStringViewElementModel(string text)
        {
            this.text = text;
        }
        public override string ToString()
        {
            return text;
        }
    }
}
