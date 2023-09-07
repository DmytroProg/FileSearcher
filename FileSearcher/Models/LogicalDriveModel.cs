using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSearcher.Models
{
    public class LogicalDriveModel
    {
        public bool IsChecked { get; set; }
        public string DriveName { get; set; } = null!;
        public string DriveFormat { get; set; } = null!;
    }
}
