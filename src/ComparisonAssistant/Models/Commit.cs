using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparisonAssistant.Models
{
    public class Commit
    {
        public Commit(string userName, string comment, DateTime date, string task)
        {
            UserName = userName;
            Comment = comment;
            Date = date;
            Task = task;
        }

        public string UserName { get; set; }
        public string Task { get; set; }
        public string Comment { get; set; }
        public DateTime Date { get; set; }
        public List<File> Files { get; set; } = new List<File>();
    }
}
