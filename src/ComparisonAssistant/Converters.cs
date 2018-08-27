using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace ComparisonAssistant
{
    public sealed class TimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
            => new DateTimeOffset(((DateTime)value).ToUniversalTime());

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => ((DateTimeOffset)value).DateTime;
    }
}
