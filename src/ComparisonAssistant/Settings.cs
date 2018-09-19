using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Windows.Storage;
using Windows.UI.Xaml;

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

        public StagePanel StageFilterPanel { get; set; } = StagePanel.Open;
        public GridLength WidthFilterPanel
        {
            get
            {
                switch (StageFilterPanel)
                {
                    case StagePanel.Open:
                        return new GridLength(315);
                    case StagePanel.Minimize:
                        return new GridLength(40);
                    case StagePanel.Close:
                        return new GridLength(0);
                    default:
                        return new GridLength(315);
                }
            }
        }
        public Visibility VisibilityFilterPanel { get => StageFilterPanel == StagePanel.Open ? Visibility.Visible : Visibility.Collapsed; }
        public Visibility VisibilityFilterPanelCompact { get => StageFilterPanel != StagePanel.Open ? Visibility.Visible : Visibility.Collapsed; }

        public StagePanel StageSettingsPanel { get; set; } = StagePanel.Close;
        public GridLength WidthSettingsPanel
        {
            get
            {
                switch (StageSettingsPanel)
                {
                    case StagePanel.Open:
                        return new GridLength(315);
                    case StagePanel.Minimize:
                        return new GridLength(40);
                    case StagePanel.Close:
                        return new GridLength(0);
                    default:
                        return new GridLength(315);
                }
            }
        }
        public GridLength WidthSettingsPanelBorder
        {
            get
            {
                switch (StageSettingsPanel)
                {
                    case StagePanel.Open:
                        return new GridLength(10);
                    case StagePanel.Minimize:
                        return new GridLength(0);
                    case StagePanel.Close:
                        return new GridLength(0);
                    default:
                        return new GridLength(10);
                }
            }
        }
        public Visibility VisibilitySettingsPanel { get => StageSettingsPanel == StagePanel.Open ? Visibility.Visible : Visibility.Collapsed; }

        private void SetValueLocalSettings(string key, object value)
        {
            if (_localSettings.Values.ContainsKey(key))
                _localSettings.Values[key] = value;
            else
                _localSettings.Values.Add(key, value);
        }
        private object GetValueLocalSettings(string key)
        {
            try
            {
                return (_localSettings.Values.ContainsKey(key)) ? _localSettings.Values[key] : null;
            }
            catch (Exception ex)
            {
                Dialogs.ShowPopups(ex.Message);
                return null;
            }
        }
    }
}
