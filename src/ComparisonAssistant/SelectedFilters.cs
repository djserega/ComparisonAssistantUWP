using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparisonAssistant
{
    public class SelectedFilters : NotifyPropertyChangedClass
    {
        private static UpdateElementsEvents _updateElementsEvents;

        private static readonly List<Models.FilterPeriod> _staticFilterPeriods = new Models.FilterPeriods().Periods;
        private static Models.FilterPeriod _selectedPeriod = _staticFilterPeriods[0];
        private DateTime _selectedDateStart = DateTime.MinValue;
        private DateTime _selectedDateEnd = DateTime.MaxValue;
        private bool _methodChangeSelectedDate;

        public List<Models.FilterPeriod> FilterPeriods = _staticFilterPeriods;

        public SelectedFilters(UpdateElementsEvents updateElementsEvents)
        {
            _updateElementsEvents = updateElementsEvents;

        }

        public string SelectedUser { get; set; } = string.Empty;
        public string SelectedTask { get; set; } = string.Empty;
        public Models.FilterPeriod SelectedPeriod { get => _selectedPeriod; set { _selectedPeriod = value; ChangeSelectedDate(); } }
        public DateTime SelectedDateStart { get => _selectedDateStart; set { _selectedDateStart = value; SetFilterAnyPeriod(); } }
        public DateTime SelectedDateEnd { get => _selectedDateEnd; set { _selectedDateEnd = value; SetFilterAnyPeriod(); } }
        public DateTime DateTaskChangedMin { get; set; } = DateTime.MinValue;
        public DateTime DateTaskChangedMax { get; set; } = DateTime.MaxValue;
        public Models.Commit SelectedCommit { get; set; }
        public Models.Commit SelectedCommit2 { get; set; }

        public void ClearFilter()
        {
            SelectedUser = string.Empty;
            SelectedTask = string.Empty;
            //ClearSelectedDate();
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

        private void ChangeSelectedDate()
        {
            if (_updateElementsEvents.Updating)
                return;

            new Settings().SelectedFilterPeriods = _selectedPeriod.Description;

            if (!_selectedPeriod.AnyPeriod)
            {
                _methodChangeSelectedDate = true;
                SelectedDateStart = _selectedPeriod.DateStart;
                SelectedDateEnd = _selectedPeriod.DateEnd;
                _methodChangeSelectedDate = false;
            }

            _updateElementsEvents.EvokeUpdating();
        }

        public void SetFilterByString(string filterName)
        {
            if (string.IsNullOrEmpty(filterName))
                SelectedPeriod = _staticFilterPeriods.First(f => f.ByDefault);
            else
                SelectedPeriod = _staticFilterPeriods.First(f => f.Description == filterName);
        }

        private void SetFilterAnyPeriod()
        {
            if (_methodChangeSelectedDate || _updateElementsEvents.Updating)
                return;
            _selectedPeriod = _staticFilterPeriods.First(f => f.AnyPeriod);
            _updateElementsEvents.EvokeUpdating();
        }

    }
}
