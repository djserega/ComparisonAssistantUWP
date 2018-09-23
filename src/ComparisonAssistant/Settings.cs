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
        #region Saved settings
        private ApplicationDataContainer _localSettings = ApplicationData.Current.LocalSettings;
        private const string _keyFullNameFileLogs = "FullNameFileLogs";
        private const string _keyPrefixSiteCommits = "PrefixSiteCommits";
        private const string _keySelectedFilterPeriods = "SelectedFilterPeriods";

        private const string _keyStorage1CPathBin1C = "Storage1CPathBin1C";
        private const string _keyStorage1CPathStorage = "Storage1CPathStorage";
        private const string _keyStorage1CStorageUser = "Storage1CStorageUser";
        private const string _keyStorage1CDBType = "Storage1CDBType";
        private const string _keyStorage1CDBPath = "Storage1CDBPath";
        private const string _keyStorage1CDBServer = "Storage1CDBServer";
        private const string _keyStorage1CDBName = "Storage1CDBName";

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

        public void SaveStorageValue(string name, string value)
        {
            switch (name)
            {
                case "Storage1CPathBin1C":
                    SetValueLocalSettings(_keyStorage1CPathBin1C, value);
                    break;
                case "Storage1CPathStorage":
                    SetValueLocalSettings(_keyStorage1CPathStorage, value);
                    break;
                case "Storage1CStorageUser":
                    SetValueLocalSettings(_keyStorage1CStorageUser, value);
                    break;
                case "Storage1CDBType":
                    SetValueLocalSettings(_keyStorage1CDBType, value);
                    break;
                case "Storage1CDBPath":
                    SetValueLocalSettings(_keyStorage1CDBPath, value);
                    break;
                case "Storage1CDBServer":
                    SetValueLocalSettings(_keyStorage1CDBServer, value);
                    break;
                case "Storage1CDBName":
                    SetValueLocalSettings(_keyStorage1CDBName, value);
                    break;
            }
        }
        public string LoadStorageValue(string name)
        {
            switch (name)
            {
                case "Storage1CPathBin1C":
                    return GetValueLocalSettings(_keyStorage1CPathBin1C) as string;
                case "Storage1CPathStorage":
                    return GetValueLocalSettings(_keyStorage1CPathStorage) as string;
                case "Storage1CStorageUser":
                    return GetValueLocalSettings(_keyStorage1CStorageUser) as string;
                case "Storage1CDBType":
                    return GetValueLocalSettings(_keyStorage1CDBType) as string;
                case "Storage1CDBPath":
                    return GetValueLocalSettings(_keyStorage1CDBPath) as string;
                case "Storage1CDBServer":
                    return GetValueLocalSettings(_keyStorage1CDBServer) as string;
                case "Storage1CDBName":
                    return GetValueLocalSettings(_keyStorage1CDBName) as string;
                default:
                    return string.Empty;  
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
        #endregion

        #region Other settings
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

        public GridLength HeightButtonPanel
        {
            get
            {
                switch (StageFilterPanel)
                {
                    case StagePanel.Open:
                        return new GridLength(30);
                    case StagePanel.Minimize:
                        return new GridLength(70);
                    case StagePanel.Close:
                        return new GridLength(70);
                    default:
                        return new GridLength(30);
                }
            }
        }
        #endregion
    }
}
