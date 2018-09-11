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
        private readonly string _keySelectedFilterPeriods = "SelectedFilterPeriods";

        public Settings()
        {
            SetDefaultValue(_keyFullNameFileLogs);
            SetDefaultValue(_keyPrefixSiteCommits);
            SetDefaultValue(_keySelectedFilterPeriods);
        }

        private void SetDefaultValue(string key)
        {
            if (!_localSettings.Values.ContainsKey(key))
                SetValueLocalSettings(key, string.Empty);
        }

        public string FullNameFileLogs
        {
            get => (string)GetValueLocalSettings(_keyFullNameFileLogs);
            set => SetValueLocalSettings(_keyFullNameFileLogs, value);
        }
        public bool LogFileReadingIsComplete { get; set; }
        public string PrefixSiteCommits
        {
            get => (string)GetValueLocalSettings(_keyPrefixSiteCommits);
            set => SetValueLocalSettings(_keyPrefixSiteCommits, value);
        }
        public string SelectedFilterPeriods
        {
            get => (string)GetValueLocalSettings(_keySelectedFilterPeriods);
            set => SetValueLocalSettings(_keySelectedFilterPeriods, value);
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
