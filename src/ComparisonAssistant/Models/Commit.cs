using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparisonAssistant.Models
{
    public class Commit : NotifyPropertyChangedClass
    {
        public Commit(string userName, string comment, DateTime date, string task, string commitHashAbbreviated, string commitHash)
        {
            UserName = userName;
            Comment = comment;
            Date = date;
            Task = task;
            CommitHashAbbreviated = commitHashAbbreviated;
            CommitHash = commitHash;
        }

        public string UserName { get; set; }
        public string Task { get; set; }
        public string Comment { get; set; }
        public DateTime Date { get; set; }
        public string CommitHashAbbreviated { get; set; }
        public string CommitHash { get; set; }
        public List<File> Files { get; set; } = new List<File>();
    }
}
