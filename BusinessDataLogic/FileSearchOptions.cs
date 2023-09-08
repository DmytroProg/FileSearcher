using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessDataLogic
{
    public class FileSearchOptions
    {
        public IEnumerable<string> Drives { get; set; } = null!;
        public string SelectedFolder { get; set; } = null!;
    }
}
