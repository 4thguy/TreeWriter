﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF.Commands
{
    public class CloseEditor :ICommand
    {
        Document Document;
        DocumentEditor Editor;
        public bool Cancel = false;
        bool AppClosing = false;

        public CloseEditor(Document Document, DocumentEditor Editor, bool AppClosing)
        {
            this.Document = Document;
            this.Editor = Editor;
            this.AppClosing = AppClosing;
        }

        public void Execute(Model Model, Main View)
        {
            if (!Document.NeedChangesSaved)
            {
                Document.OpenEditors.Remove(Editor);
                if (Document.OpenEditors.Count == 0 && !AppClosing) Model.CloseDocument(Document);
                Cancel = false;
            }
            else
            {
                // More than one open editor for this document - go ahead and close it.
                if (Document.OpenEditors.Count > 1)
                {
                    Document.OpenEditors.Remove(Editor);
                    Cancel = false;
                }
                else
                {
                    var prompt = System.Windows.Forms.MessageBox.Show(String.Format("Save changes to {0}?", Document.FileName),
                        "Unsaved changes!", System.Windows.Forms.MessageBoxButtons.YesNoCancel);
                    if (prompt == System.Windows.Forms.DialogResult.Yes)
                    {
                        Model.SaveDocument(Document);
                        if (!AppClosing) Model.CloseDocument(Document);
                        Cancel = false;
                    }
                    else if (prompt == System.Windows.Forms.DialogResult.No)
                    {
                        if (!AppClosing) Model.CloseDocument(Document);
                        Cancel = false;
                    }
                    else if (prompt == System.Windows.Forms.DialogResult.Cancel)
                    {
                        Cancel = true;
                    }
                    else
                        throw new InvalidProgramException();
                }
            }
        }
    }
}
