using ComparisonAssistant.Additions;

using System;

using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;

using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x419

namespace ComparisonAssistant
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        #region Private events

        private static UpdateElementsEvents _updateElementsEvents = new UpdateElementsEvents();
        private static ValueStorage1CEvents _valueStorage1CEvents = new ValueStorage1CEvents();

        #endregion

        #region Private fields

        private IEnumerable<IGrouping<string, Models.Commit>> _groupedCommitByUser;
        private Dictionary<string, List<Models.Commit>> _dictionaryUserTasks = new Dictionary<string, List<Models.Commit>>();
        private ToastNotification _toast;

        #endregion

        #region Constructors & overrides methods

        public MainPage()
        {
            InitializeComponent();

            _updateElementsEvents.UpdateElementsEvent += () => { UpdateFormElements(); };
            _valueStorage1CEvents.SaveValueEvent += (string name, string value) => { Settings.SaveStorageValue(name, value); };
            _valueStorage1CEvents.LoadValueEvent += (string name) => { return Settings.LoadStorageValue(name); };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            SelectedFilters.SetFilterByString(Settings.SelectedFilterPeriods);

            UpdateDB(true);
        }

        #endregion

        #region Internal properties

        internal Settings Settings { get; set; } = new Settings();
        internal SelectedFilters SelectedFilters { get; set; } = new SelectedFilters(_updateElementsEvents);
        internal Storage1C Storage1C { get; set; } = new Storage1C(_updateElementsEvents, _valueStorage1CEvents);

        #endregion

        #region Public readonly properties

        public ObservableCollection<Models.Commit> Commits { get; } = new ObservableCollection<Models.Commit>();
        public ObservableCollection<string> Users { get; } = new ObservableCollection<string>();
        public ObservableCollection<string> UserTasks { get; } = new ObservableCollection<string>();

        #endregion

        #region Buttons

        private void ButtonUpdateDB_Click(object sender, RoutedEventArgs e)
        {
            UpdateDB();
        }

        private void ButtonUpdateListCommits_Click(object sender, RoutedEventArgs e)
        {
            FillTableCommits();
        }

        private async void ButtonGetFileNameLog_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.List,
                SuggestedStartLocation = PickerLocationId.Downloads
            };
            openPicker.FileTypeFilter.Add(".txt");

            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                StorageApplicationPermissions.FutureAccessList.AddOrReplace("FullFileNameLog", file);
                Settings.FullNameFileLogs = file.Path;
                UpdateFormElements();
            }
        }

        private async void ButtonOpenFileLog_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(Settings.FullNameFileLogs))
            {
                StorageFile storageFile = null;
                try
                {
                    storageFile = await StorageFile.GetFileFromPathAsync(Settings.FullNameFileLogs);
                }
                catch (Exception)
                {
                    Dialogs.ShowPopups("Не удалось получить доступ к файлу.\nВозможно нет доступа к файлу или файл не существует.");
                }

                if (storageFile == null)
                    return;

                await Launcher.LaunchFileAsync(storageFile);
            }
        }

        private void ButtonFilter_Click(object sender, RoutedEventArgs e)
        {
            Settings.StageFilterPanel = Settings.StageFilterPanel == StagePanel.Open
                ? StagePanel.Minimize : StagePanel.Open;

            UpdateFormElements();
        }

        private void ButtonSettings_Click(object sender, RoutedEventArgs e)
        {
            Settings.StageSettingsPanel = Settings.StageSettingsPanel == StagePanel.Open
                ? StagePanel.Close : StagePanel.Open;

            UpdateFormElements();
        }

        #endregion

        #region Handlers elements

        private void ComboBoxUser_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedFilters.SelectedTask = "";

            Commits.Clear();
            UserTasks.Clear();

            List<string> listTask = new List<string>();
            if (SelectedFilters.SelectedUser != null)
                if (_dictionaryUserTasks.ContainsKey(SelectedFilters.SelectedUser))
                    foreach (Models.Commit item in _dictionaryUserTasks[SelectedFilters.SelectedUser])
                        if (listTask.FirstOrDefault(f => f == item.Task) == null)
                            listTask.Add(item.Task);
            listTask.Sort();

            foreach (string item in listTask)
                UserTasks.Add(item);
        }

        private void ComboBoxTask_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FillTableCommits();
        }

        private void CalendarDatePickerDateStart_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            FillTableCommits();
        }

        private void CalendarDatePickerDateEnd_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            FillTableCommits();
        }

        private void CalendarViewDateTaskChanged_SelectedDatesChanged(CalendarView sender, CalendarViewSelectedDatesChangedEventArgs args)
        {
            List<DateTimeOffset> listRemovedDates = args.RemovedDates.ToList();

            for (int i = Commits.Count - 1; i >= 0; --i)
                if (listRemovedDates.FirstOrDefault(f => f.Date.StartDay() == Commits[i].Date.StartDay()) != default(DateTimeOffset))
                    Commits.RemoveAt(i);
        }

        #region MenuFlyoutItem
        private void MenuFlyoutItemSelectedDateEnd_Click(object sender, RoutedEventArgs e)
        {
            SelectedFilters.SelectedDateEnd = DateTime.Now;
            UpdateFormElements();
        }

        private void MenuFlyoutItemSelectedDateStart_Click(object sender, RoutedEventArgs e)
        {
            SelectedFilters.SelectedDateStart = DateTime.Now;
            UpdateFormElements();
        }

        #endregion

        #region Visibility FileDetailsFullname
        private void ChecboxVisibleFileDetailsFullname_Checked(object sender, RoutedEventArgs e)
        {
            ChangeVisibilityFileDetailsTextBoxFullName();
        }

        private void ChecboxVisibleFileDetailsFullname_Unchecked(object sender, RoutedEventArgs e)
        {
            ChangeVisibilityFileDetailsTextBoxFullName();
        }

        private void ChangeVisibilityFileDetailsTextBoxFullName()
        {
            StaticSettings.VisibleFullNameChangedFiles = !StaticSettings.VisibleFullNameChangedFiles;
        }
        #endregion

        #endregion

        #region Other

        private async void UpdateDB(bool onNavigatedTo = false)
        {
            PageInAppNotification.Show("Пожалуйста подождите. Выполняется чтение файла.");

            Settings.LogFileReadingIsComplete = false;

            if (!onNavigatedTo)
                UpdateFormElements();

            NotificationStartUpdateDB();

            await UpdateDBAsync();

            NotificationEndUpdateDB();

            Settings.LogFileReadingIsComplete = true;
            UpdateFormElements();

            PageInAppNotification.Dismiss();

            MainWindowFrame parameter = new MainWindowFrame()
            {
                Parameters = new object[1]
                {
                    Commits
                },
                TypesParameters = new Type[1]
                {
                    typeof(ObservableCollection<Models.Commit>)
                }
            };

            MainFrame.Navigate(typeof(Views.DataGridCommits), parameter);
        }

        private async Task UpdateDBAsync()
        {
            string selectedUser = SelectedFilters.SelectedUser;
            string selectedTask = SelectedFilters.SelectedTask;

            ClearProperties();

            ReaderFileLog readerLog = new ReaderFileLog() { FileName = Settings.FullNameFileLogs };
            List<Models.Commit> listCommits = await readerLog.ReadFileAsync();

            _groupedCommitByUser = listCommits?.GroupBy(f => f.UserName);
            if (_groupedCommitByUser != null)
            {
                List<string> listUsers = new List<string>();

                _toast.ProgressValueDoubleMax = _groupedCommitByUser.Count();

                foreach (IGrouping<string, Models.Commit> itemGroup in _groupedCommitByUser)
                {
                    _toast.ProgressValueDouble++;

                    _toast.Update(progressValueString: itemGroup.Key);

                    await Task.Delay(500);

                    List<Models.Commit> list = new List<Models.Commit>();

                    foreach (Models.Commit item in itemGroup)
                    {
                        if (DateIncludedInFilterPeriod(item.Date))
                        {
                            if (string.IsNullOrEmpty(listUsers.FirstOrDefault(f => f == itemGroup.Key)))
                                listUsers.Add(itemGroup.Key);
                            list.Add(item);
                        }
                    }

                    _dictionaryUserTasks.Add(itemGroup.Key, list);
                }

                listUsers.Sort();

                foreach (string item in listUsers)
                    Users.Add(item);
            }

            if (!string.IsNullOrEmpty(selectedUser))
                if (!string.IsNullOrEmpty(Users.FirstOrDefault(f => f == selectedUser)))
                {
                    SelectedFilters.SelectedUser = selectedUser;

                    UpdateFormElements();

                    if (!string.IsNullOrEmpty(selectedTask))
                        if (!string.IsNullOrEmpty(UserTasks.FirstOrDefault(f => f == selectedTask)))
                        {
                            SelectedFilters.SelectedTask = selectedTask;

                            UpdateFormElements();
                        }
                }
        }

        private void FillTableCommits()
        {
            Commits.Clear();
            SelectedFilters.ClearDateTaskChanged();
            CalendarViewDateTaskChanged.SelectedDates.Clear();

            if (SelectedFilters.SelectedTask != null)
                if (_dictionaryUserTasks.ContainsKey(SelectedFilters.SelectedUser))
                    foreach (Models.Commit item in _dictionaryUserTasks[SelectedFilters.SelectedUser])
                    {
                        if (SelectedFilters.SelectedTask == item.Task)
                        {
                            if (DateIncludedInFilterPeriod(item.Date))
                            {
                                Commits.Add(item);

                                if (SelectedFilters.DateTaskChangedMin > item.Date
                                    || SelectedFilters.DateTaskChangedMin == DateTime.MinValue)
                                    SelectedFilters.DateTaskChangedMin = item.Date;
                                if (SelectedFilters.DateTaskChangedMax < item.Date
                                    || SelectedFilters.DateTaskChangedMax == DateTime.MaxValue)
                                    SelectedFilters.DateTaskChangedMax = item.Date;

                                CalendarViewDateTaskChanged.SelectedDates.Add(item.Date);
                            }
                        }
                    }

            if (SelectedFilters.DateTaskChangedMax == DateTime.MaxValue)
                SetDisplayDateCalendatView(DateTime.Now);
            else
                SetDisplayDateCalendatView(SelectedFilters.DateTaskChangedMax);

        }

        private void ClearProperties()
        {
            Commits.Clear();
            Users.Clear();
            UserTasks.Clear();
            SelectedFilters.ClearFilter();
            _dictionaryUserTasks.Clear();
        }

        private void SetDisplayDateCalendatView(DateTime date)
        {
            CalendarViewDateTaskChanged.SetDisplayDate(new DateTimeOffset(date));
        }

        private bool DateIncludedInFilterPeriod(DateTime date)
        {
            return date >= SelectedFilters.SelectedDateStart.StartDay()
                && date <= SelectedFilters.SelectedDateEnd.EndDay();
        }

        private void UpdateFormElements()
        {
            try
            {
                _updateElementsEvents.Updating = true;
                Bindings.Update();
                _updateElementsEvents.Updating = false;
            }
            catch (Exception ex)
            {
                Dialogs.Show("Ошибка обновления данных.\n" + ex.Message);
            }
        }

        #endregion

        #region ToastNotification

        private void NotificationStartUpdateDB()
          => NotificationInitialize("Чтение", "Чтение файла");

        private void NotificationEndUpdateDB()
            => NotificationInitialize("Завершено", "Файл прочитан", true);

        private void NotificationInitialize(string progressStatus, string text, bool endUpdateDB = false)
        {
            _toast = new ToastNotification()
            {
                ProgressStatus = progressStatus,
                Text = text,
                ProgressValueDouble = 0
            };

            if (endUpdateDB)
            {
                _toast.ProgressValueDouble = 1;
                _toast.ProgressValueDoubleMax = 1;
            }

            _toast.InitialToast("Updating-DB", "Updating-DB");

            _toast.Show();
        }

        #endregion

        #region Storage1C

        private async void ButtonStorageCheckConnection_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await Storage1C.CheckConnection();
            }
            catch (CheckConnectionException ex)
            {
                Dialogs.ShowPopups("Не удалось подключиться к хранилищу:\n\n" + ex.Message);
            }
        }

        private async void ButtonGetPathBin1C_Click(object sender, RoutedEventArgs e)
        {
            FolderPicker folderPicker = new FolderPicker()
            {
                ViewMode = PickerViewMode.List,
                SuggestedStartLocation = PickerLocationId.ComputerFolder
            };
            folderPicker.FileTypeFilter.Add("*");

            StorageFolder catalog = await folderPicker.PickSingleFolderAsync();
            if (catalog != null)
            {
                Storage1C.PathBin1C = catalog.Path;
                UpdateFormElements();
            }
        }

        private async void ButtonGetPathStorage_Click(object sender, RoutedEventArgs e)
        {
            FolderPicker folderPicker = new FolderPicker()
            {
                ViewMode = PickerViewMode.List,
                SuggestedStartLocation = PickerLocationId.ComputerFolder
            };
            folderPicker.FileTypeFilter.Add("*");

            StorageFolder catalog = await folderPicker.PickSingleFolderAsync();
            if (catalog != null)
            {
                Storage1C.PathStorage = catalog.Path;
                UpdateFormElements();
            }
        }

        private async void ButtonGetDBPath_Click(object sender, RoutedEventArgs e)
        {
            FolderPicker folderPicker = new FolderPicker()
            {
                ViewMode = PickerViewMode.List,
                SuggestedStartLocation = PickerLocationId.ComputerFolder
            };
            folderPicker.FileTypeFilter.Add("*");

            StorageFolder catalog = await folderPicker.PickSingleFolderAsync();
            if (catalog != null)
            {
                Storage1C.DBPath = catalog.Path;
                UpdateFormElements();
            }
        }

        #endregion


    }
}
