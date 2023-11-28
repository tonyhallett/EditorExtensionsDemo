using Microsoft.VisualStudio.Text.Editor;
using System.Collections;
using System.Collections.Generic;

namespace EditorExtensionsDemo.QuickInfo.Models
{
    internal class CodeModel
    {
        public CodeModel(
            string code,
            string contentType,
            IEnumerable<string> textViewRoles,
            Action<IWpfTextView> initializeTextView,
            bool readOnly
        )
        {
            this.Code = code;
            ContentType = contentType;
            TextViewRoles = textViewRoles;
            InitializeTextView = initializeTextView;
            ReadOnly = readOnly;
        }
        public CodeModel(
            string code, 
            string contentType, 
            IEnumerable<string> textViewRoles, 
            Dictionary<Guid, uint[]> allowedCommands, 
            Action<IWpfTextView> initializeTextView,
            bool readOnly
        )
        {
            this.Code = code;
            ContentType = contentType;
            TextViewRoles = textViewRoles;
            AllowedCommands = allowedCommands;
            InitializeTextView = initializeTextView;
            ReadOnly = readOnly;
        }

 
        public string Code { get; }
        public string ContentType { get; internal set; }
        public IEnumerable<string> TextViewRoles { get; }
        public Dictionary<Guid, uint[]> AllowedCommands { get; }
        public Action<IWpfTextView> InitializeTextView { get; }
        public bool ReadOnly { get; }
    }
}
