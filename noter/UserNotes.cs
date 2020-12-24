using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noter
{
    [Serializable]
    public class UserNotes
    {
        public List<Note> Notes = new List<Note>();
    }
}
