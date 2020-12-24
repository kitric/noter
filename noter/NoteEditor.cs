using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Noter
{
    public partial class NoteEditor : UserControl
    {
        Note EditingNote;

        MainScreen MainScreen;

        NoteButton NotesButton;

        public NoteEditor(Note note, MainScreen ms, NoteButton notesbtn)
        {
            InitializeComponent();

            NotesButton = notesbtn;

            MainScreen = ms;

            EditingNote = note;
            Title.Text = EditingNote.Name;
            TextField.Text = EditingNote.Contents;
            TextField.AcceptsTab = true;
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            Save();

            MainScreen.ShowNewPage(this, new WelcomePage(MainScreen));
        }

        public void Save()
        {
            EditingNote.Name = Title.Text.Trim();
            EditingNote.Contents = TextField.Text.Trim();
            NotesButton.UpdateButton();
        }

        private void TextField_TextChanged(object sender, EventArgs e)
        {
        }

        void DeleteNote()
        {
            switch (MessageBox.Show("Are you sure you want to delete this note?", "Confirm", MessageBoxButtons.YesNo))
            {
                case DialogResult.Yes:
                    MainScreen.TheNotes.Notes.Remove(EditingNote);
                    MainScreen.MoveAllNotesBack(MainScreen.GetLevel(NotesButton));
                    MainScreen.ShowNewPage(this, new WelcomePage(MainScreen));
                    break;
                case DialogResult.No:
                    break;
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
            DeleteNote();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.S))
            {
                Save();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void NoteEditor_KeyDown(object sender, KeyEventArgs e)
        {
        }

        private void Title_KeyDown(object sender, KeyEventArgs e)
        {
        }

        private void label2_Click(object sender, EventArgs e)
        {
            Export();
        }

        void Export()
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                sfd.FilterIndex = 0;

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    using (StreamWriter writer = new StreamWriter(sfd.FileName))
                    {
                        writer.WriteLine(TextField.Text.Trim());
                        MessageBox.Show("File Exported.", "Export Complete");
                    }
                }
            }
        }
    }
}
