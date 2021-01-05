using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Noter
{
    public partial class MainScreen : Form
    {
        public static UserNotes TheNotes = new UserNotes();

        private int NoteX = 5;
        private int NoteY = 0;

        public UserControl CurrentScreen;

        List<NoteButton> Buttons = new List<NoteButton>();

        string NoterData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Noter\";
        string NotesLocation = "";

        public MainScreen()
        {
            InitializeComponent();
            Application.ThreadException += Application_ThreadException;

            KeyPreview = true;

            Directory.CreateDirectory(NoterData);
            NotesLocation = NoterData + "noter.dat";

            NotesContainer.VerticalScroll.Maximum = 0;
            NotesContainer.AutoScroll = false;
            NotesContainer.HorizontalScroll.Visible = false;
            NotesContainer.AutoScroll = true;

            WelcomePage Welcome = new WelcomePage(this);
            Welcome.Dock = DockStyle.Fill;
            WindowContainer.Controls.Add(Welcome);

            CurrentScreen = Welcome;
        }

        private void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message);
        }

        public void ShowNewNoteDialog(UserControl ToClose)
        {
            ToClose.Dispose();
            WindowContainer.Controls.Remove(ToClose);

            CreateNoteDialog CND = new CreateNoteDialog(this);
            CND.Dock = DockStyle.Fill;
            WindowContainer.Controls.Add(CND);

            CurrentScreen = CND;
        }

        public void ShowNewPage(UserControl ToClose, UserControl ToOpen)
        {
            ToOpen.Dock = DockStyle.Fill;

            ToClose.Dispose();
            WindowContainer.Controls.Remove(ToClose);

            ToOpen.Dock = DockStyle.Fill;
            WindowContainer.Controls.Add(ToOpen);

            CurrentScreen = ToOpen;
        }

        public NoteButton AddNote(Note note)
        {
            NotesContainer.AutoScrollPosition = new Point(0, 0);

            NoteButton button = new NoteButton(NoteX, NoteY, note, this);
            NotesContainer.Controls.Add(button.Label);

            Buttons.Add(button);

            NoteY += 35;
            NotesContainer.AutoScroll = true;

            return button;
        }

        public void ShowNotes()
        {
            foreach (var note in TheNotes.Notes)
            {
                NoteButton button = new NoteButton(NoteX, NoteY, note, this);
                NotesContainer.Controls.Add(button.Label);

                Buttons.Add(button);

                NoteY += 35;
            }
        }

        void ShowHome()
        {
            if (CurrentScreen.GetType().Name == "NoteEditor")
            {
                using (NoteEditor Temp = CurrentScreen as NoteEditor)
                {
                    Temp.Save();
                }
            }
            ShowNewPage(CurrentScreen, new WelcomePage(this));
        }

        public int GetLevel(NoteButton b)
        {
            for (int i = 0; i < Buttons.Count; i++)
            {
                if (Buttons[i] == b)
                {
                    return i;
                }
            }
            return 10000;
        }

        public void MoveAllNotesBack(int level)
        {
            NotesContainer.VerticalScroll.Value = 0;
            NotesContainer.Controls.Remove(Buttons[level].Label);
            Buttons[level].Label.Dispose();
            Buttons.Remove(Buttons[level]);

            for (int i = level; i < Buttons.Count; i++)
            {
                Point p = new Point(Buttons[i].Label.Location.X, Buttons[i].Label.Location.Y - 35);
                Buttons[i].Label.Location = p;
            }

            NoteY -= 35;
        }

        void SerializeData()
        {
            IFormatter f = new BinaryFormatter();

            using (Stream stream = new FileStream(NotesLocation, FileMode.Create, FileAccess.Write))
            {
                f.Serialize(stream, TheNotes);
            }
        }

        void DeserializeData()
        {
            if (File.Exists(NotesLocation))
            {
                IFormatter f = new BinaryFormatter();

                using (Stream stream = new FileStream(NotesLocation, FileMode.Open, FileAccess.Read))
                {
                    TheNotes = (UserNotes)f.Deserialize(stream);
                }
            }
            else
            {
                Note note = new Note()
                {
                    Name = "Welcome!",
                    Contents = $"Hello and welcome to Noter!{Environment.NewLine}{Environment.NewLine}About Noter:{Environment.NewLine}Noter is a simple note making app, it allows you to have all your notes in one area without having to worry about looking for them through the file system. The app started off as just a Notepad replica (in fact, I drafted this message in the old Noter) that was available on itch.io, but has now been taken down. The old app was pretty broken, and there was no real use for, since you couldn't set it as a default app. However, this version is much better - and cleaner too - it keeps all your important notes within one app, all easy to access.{Environment.NewLine}{Environment.NewLine}How does saving work?{Environment.NewLine}The nice thing about Noter is that it autosaves your notes, so you don't need to worry about saving them yourself!{Environment.NewLine}{Environment.NewLine}Have fun taking notes, I guess!{Environment.NewLine}- crxssed."
                };

                TheNotes.Notes.Add(note);
            }
        }

        private void MainScreen_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (CurrentScreen.GetType().Name == "NoteEditor")
            {
                using (NoteEditor Temp = CurrentScreen as NoteEditor)
                {
                    Temp.Save();
                }
            }
            SerializeData();
        }

        private void MainScreen_Load(object sender, EventArgs e)
        {
            DeserializeData();
            ShowNotes();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            ShowHome();
        }

        private void MainScreen_KeyDown(object sender, KeyEventArgs e)
        {
        }
    }

    public class NoteButton
    {
        private Note Note;

        public Label Label;

        private MainScreen MainScreen;

        public NoteButton(int x, int y, Note n, MainScreen ms)
        {
            Note = n;

            MainScreen = ms;

            Label = new Label()
            {
                Text = Note.Name,
                Location = new Point(x, y),
                TextAlign = ContentAlignment.MiddleCenter,
                Width = 199,
                Height = 30,
                Cursor = Cursors.Hand,
                AutoSize = false,
                Font = new Font("Century Gothic", 10),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(10, 10, 10),
                Visible = true
            };

            Label.Click += ViewNote;
        }

        private void ViewNote(object sender, EventArgs e)
        {
            if (MainScreen.CurrentScreen.GetType().Name == "NoteEditor")
            {
                using (NoteEditor Temp = MainScreen.CurrentScreen as NoteEditor)
                {
                    Temp.Save();
                }
            }
            MainScreen.ShowNewPage(MainScreen.CurrentScreen, new NoteEditor(Note, MainScreen, this));
        }

        public void UpdateButton()
        {
            Label.Text = Note.Name;
        }
    }
}
