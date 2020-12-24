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
    public partial class WelcomePage : UserControl
    {
        private MainScreen ParentPage;

        public WelcomePage(MainScreen parentPageIn)
        {
            InitializeComponent();

            ParentPage = parentPageIn;
        }

        void CreateNote()
        {
            ParentPage.ShowNewNoteDialog(this);
        }

        void ImportNote()
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    Note NewNote = new Note();

                    using (StreamReader Reader = new StreamReader(ofd.FileName))
                    {
                        NewNote.Name = ofd.SafeFileName;
                        NewNote.Contents = Reader.ReadToEnd().Trim();
                        MainScreen.TheNotes.Notes.Add(NewNote);
                        ParentPage.ShowNewPage(ParentPage.CurrentScreen, new NoteEditor(NewNote, ParentPage, ParentPage.AddNote(NewNote)));
                    }
                }
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {
            CreateNote();
        }

        private void label3_Click(object sender, EventArgs e)
        {
            ImportNote();
        }
    }
}
