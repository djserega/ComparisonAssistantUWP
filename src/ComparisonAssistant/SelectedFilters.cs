using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparisonAssistant
{
    public class SelectedFilters : NotifyPropertyChangedClass
    {
        public string SelectedUser { get; set; } = string.Empty;
        public string SelectedTask { get; set; } = string.Empty;
        public DateTime SelectedDateStart { get; set; } = DateTime.MinValue;
        public DateTime SelectedDateEnd { get; set; } = DateTime.MaxValue;
        public DateTime DateTaskChangedMin { get; set; } = DateTime.MinValue;
        public DateTime DateTaskChangedMax { get; set; } = DateTime.MaxValue;
        public Models.Commit SelectedCommit { get; set; }
        public Models.Commit SelectedCommit2 { get; set; }

        public void ClearFilter()
        {
            SelectedUser = string.Empty;
            SelectedTask = string.Empty;
            ClearSelectedDate();
            ClearDateTaskChanged();
        }

        public void ClearSelectedDate()
        {
            SelectedDateStart = DateTime.MinValue;
            SelectedDateEnd = DateTime.MaxValue;
        }

        public void ClearDateTaskChanged()
        {
            DateTaskChangedMin = DateTime.MinValue;
            DateTaskChangedMax = DateTime.MaxValue;
        }
    }
}
