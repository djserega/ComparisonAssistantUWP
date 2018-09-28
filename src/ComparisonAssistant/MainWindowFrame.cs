using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace ComparisonAssistant
{
    internal class MainWindowFrame : NotifyPropertyChangedClass
    {
        internal object[] Parameters { get; set; }
        internal Type[] TypesParameters { get; set; }

        internal int CountParameter { get => Parameters.Count(); }
    }
}
