using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparisonAssistant.Models
{
    public class Commit
    {
        public string UserName { get; set; }
        public string Task { get; set; }
        public string Comment { get; set; }
        public DateTime Date { get; set; }
        public List<File> Files { get; set; }
    }
}
