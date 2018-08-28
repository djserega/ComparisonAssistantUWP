using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparisonAssistant.Additions
{
    public static class AdditionsDateTime
    {
        public static DateTime StartDay(this DateTime date) => date.Date;

        public static DateTime EndDay(this DateTime date) => date == DateTime.MaxValue ? date : date.StartDay().AddDays(1).AddTicks(-1);

        public static DateTime StartWeek(this DateTime date, bool atStartTheDay = true, DayOfWeek dayOfWeek = DayOfWeek.Monday)
        {
            if (atStartTheDay)
                date = date.StartDay();

            while (date.DayOfWeek != dayOfWeek)
                date = date.AddDays(-1);

            return date;
        }
    }
}
