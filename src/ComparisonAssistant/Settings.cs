using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Windows.Storage;

namespace ComparisonAssistant
{
    public class Settings : NotifyPropertyChangedClass
    {
        private ApplicationDataContainer _localSettings = ApplicationData.Current.LocalSettings;
        private readonly string _keyFullNameFileLogs = "FullNameFileLogs";


        public Settings()
        {
            if (!_localSettings.Values.ContainsKey(_keyFullNameFileLogs))
                SetValueLocalSettings(_keyFullNameFileLogs, string.Empty);
        }

        public string FullNameFileLogs
        {
            get
            {
                object value = GetValueLocalSettings(_keyFullNameFileLogs);
                return value == null ? string.Empty : (string)value;
            }
            set
            {
                SetValueLocalSettings(_keyFullNameFileLogs, value);
            }
        }


        private void SetValueLocalSettings(string key, object value)
        {
            if (_localSettings.Values.ContainsKey(key))
                _localSettings.Values[key] = value;
            else
                _localSettings.Values.Add(key, value);
        }
        private object GetValueLocalSettings(string key)
            => (_localSettings.Values.ContainsKey(key)) ? _localSettings.Values[key] : null;
    }
}
