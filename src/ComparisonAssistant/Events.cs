using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace ComparisonAssistant
{
    public delegate void UpdateElements();
    public class UpdateElementsEvents : EventArgs
    {
        public bool Updating { get; set; }

        public event UpdateElements UpdateElementsEvent;
        public void EvokeUpdating()
        {
            if (!Updating)
                UpdateElementsEvent?.Invoke();
        }
    }

    public delegate void SaveValueStorage1C(string name, string value);
    public delegate string LoadValueStorage1C(string name);
    public class ValueStorage1CEvents : EventArgs
    {
        public event SaveValueStorage1C SaveValueEvent;
        public void SaveValue(string name, string value)
        {
            SaveValueEvent?.Invoke(name, value);
        }

        public event LoadValueStorage1C LoadValueEvent;
        public string LoadValue(string name)
        {
            return LoadValueEvent?.Invoke(name);
        }
    }

    public delegate void ChangeContentFrame(Type newPage, MainWindowFrame mainWindowFrame);
    public class ChangeContentFrameEvents : EventArgs
    {
        public static event ChangeContentFrame ChangeContentFrameEvent;

        public Page Page { get; private set; }
        public object[] Parameters { get; private set; }
        public void ChangeContent(Type newPage, MainWindowFrame mainWindowFrame)
        {
            ChangeContentFrameEvent?.Invoke(newPage, mainWindowFrame);
        }
    }
}
