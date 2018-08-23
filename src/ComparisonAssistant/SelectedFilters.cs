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

        public void ClearFilter()
        {
            SelectedUser = string.Empty;
            SelectedTask = string.Empty;
        }

    }
}
