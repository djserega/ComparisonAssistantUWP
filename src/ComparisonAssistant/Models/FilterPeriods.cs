using ComparisonAssistant.Additions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparisonAssistant.Models
{
    public class FilterPeriods
    {
        public List<FilterPeriod> Periods { get; } = InitializePeriods();

        private static List<FilterPeriod> InitializePeriods()
        {
            DateTime currentDay = DateTime.Now.Date;

            return new List<FilterPeriod>()
            {
                new FilterPeriod("Сегодня", currentDay),
                new FilterPeriod("Со вчера", currentDay.AddDays(-1)),
                new FilterPeriod("Последние 7 дней", currentDay.AddDays(-7)),
                new FilterPeriod("Последние 2 недели", currentDay.AddDays(-14)),
                new FilterPeriod("Последний месяц", currentDay.AddMonths(-1)),
                new FilterPeriod("Произвольный")
            };
        }

    }

    public class FilterPeriod
    {
        public FilterPeriod(string description, DateTime? dateStart = null, DateTime? dateEnd = null)
        {
            Description = description;
            DateStart = dateStart ?? DateTime.MinValue;
            DateEnd = dateEnd ?? DateTime.Now.Date.EndDay();
        }

        public string Description { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }

        public override string ToString()
        {
            return Description;
        }
    }

}
