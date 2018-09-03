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
        private readonly string _keyPrefixSiteCommits = "PrefixSiteCommits";

        public Settings()
        {
            if (!_localSettings.Values.ContainsKey(_keyFullNameFileLogs))
                SetValueLocalSettings(_keyFullNameFileLogs, string.Empty);
            if (!_localSettings.Values.ContainsKey(_keyPrefixSiteCommits))
                SetValueLocalSettings(_keyPrefixSiteCommits, string.Empty);
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
        public bool LogFileReadingIsComplete { get; set; }
        public string PrefixSiteCommits
        {
            get
            {
                object value = GetValueLocalSettings(_keyPrefixSiteCommits);
                return value == null ? string.Empty : (string)value;
            }
            set
            {
                SetValueLocalSettings(_keyPrefixSiteCommits, value);
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
