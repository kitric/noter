using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Noter
{
    public partial class CreateNoteDialog : UserControl
    {
        MainScreen MainScreen;

        public CreateNoteDialog(MainScreen ms)
        {
            InitializeComponent();

            NameField.AcceptsReturn = true;

            NameField.Focus();
            MainScreen = ms;
        }

        void Create()
        {
            if (NameField.Text.Trim() != "" && NameField.Text.Length <= 75)
            {
                Note NewNote = new Note()
                {
                    Name = NameField.Text.Trim(),
                    Contents = "Edit your new note..."
                };

                MainScreen.TheNotes.Notes.Add(NewNote);
                MainScreen.ShowNewPage(this, new NoteEditor(NewNote, MainScreen, MainScreen.AddNote(NewNote)));
            }
            else
            {
                if (NameField.Text.Length > 30)
                {
                    MessageBox.Show("Please enter a valid note name...\nName must be less than 75 characters long.");
                }
                else if (NameField.Text.Trim() == "")
                {
                    MessageBox.Show("Please enter a valid note name...\nName field must not be left empty.");
                }
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {
            Create();
        }

        void Cancel()
        {
            MainScreen.ShowNewPage(this, new WelcomePage(MainScreen));
        }

        private void cancel_Click(object sender, EventArgs e)
        {
            Cancel();
        }

        private void NameField_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Create();

                e.SuppressKeyPress = false;
                e.Handled = true;
            }
        }
    }
}
