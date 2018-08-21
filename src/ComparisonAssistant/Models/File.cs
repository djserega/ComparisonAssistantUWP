using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparisonAssistant.Models
{
    public class File
    {
        public File(string mode, string fullName)
        {
            Mode = mode;
            FullName = fullName;
        }

        public string Mode { get; set; }
        public string FullName { get; set; }
        public FilePart FileParts { get; set; }
    }
}
