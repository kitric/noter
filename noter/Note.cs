using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noter
{
    [Serializable]
    public class Note
    {
        public string Contents { get; set; }
        public string Name { get; set; }
    }
}
