using ComparisonAssistant.Additions;

using System;

using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

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
        private IEnumerable<IGrouping<string, Models.Commit>> _groupedCommitByUser;
        private Dictionary<string, List<Models.Commit>> _dictionaryUserTasks = new Dictionary<string, List<Models.Commit>>();

        internal Settings Settings { get; set; } = new Settings();
        internal SelectedFilters SelectedFilters { get; set; } = new SelectedFilters();

        public ObservableCollection<Models.Commit> Commits { get; } = new ObservableCollection<Models.Commit>();
        public ObservableCollection<string> Users { get; } = new ObservableCollection<string>();
        public ObservableCollection<string> UserTasks { get; } = new ObservableCollection<string>();

        public MainPage()
        {
            InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            await ButtonUpdateDB();
        }

        private async void ButtonUpdateDB_Click(object sender, RoutedEventArgs e)
        {
            await ButtonUpdateDB();
        }

        private async System.Threading.Tasks.Task ButtonUpdateDB()
        {
            ClearProperties();

            ReaderFileLog readerLog = new ReaderFileLog() { FileName = Settings.FullNameFileLogs };
            List<Models.Commit> listCommits = await readerLog.ReadFileAsync();

            _groupedCommitByUser = listCommits?.GroupBy(f => f.UserName);
            if (_groupedCommitByUser != null)
            {
                foreach (IGrouping<string, Models.Commit> itemGroup in _groupedCommitByUser)
                {
                    List<Models.Commit> list = new List<Models.Commit>();

                    foreach (var item in itemGroup)
                        list.Add(item);

                    _dictionaryUserTasks.Add(itemGroup.Key, list);
                }

                List<string> listUsers = new List<string>();
                foreach (var item in _groupedCommitByUser)
                    listUsers.Add(item.Key);
                listUsers.Sort();

                foreach (string item in listUsers)
                    Users.Add(item);
            }
        }

        private void ClearProperties()
        {
            Commits.Clear();
            Users.Clear();
            UserTasks.Clear();
            SelectedFilters.ClearFilter();
            _dictionaryUserTasks.Clear();
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
                Bindings.Update();
            }
        }

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
                            if (item.Date >= SelectedFilters.SelectedDateStart
                            && item.Date <= SelectedFilters.SelectedDateEnd.EndDay())
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

        private void CalendarDatePickerDateStart_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            FillTableCommits();
        }

        private void CalendarDatePickerDateEnd_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            FillTableCommits();
        }

        private void MenuFlyoutItemSelectedDateEnd_Click(object sender, RoutedEventArgs e)
        {
            SelectedFilters.SelectedDateEnd = DateTime.Now;
            Bindings.Update();
        }

        private void MenuFlyoutItemSelectedDateStart_Click(object sender, RoutedEventArgs e)
        {
            SelectedFilters.SelectedDateStart = DateTime.Now;
            Bindings.Update();
        }

        private void CalendarViewDateTaskChanged_SelectedDatesChanged(CalendarView sender, CalendarViewSelectedDatesChangedEventArgs args)
        {
            List<DateTimeOffset> listRemovedDates = args.RemovedDates.ToList();

            for (int i = Commits.Count - 1; i >= 0; --i)
                if (listRemovedDates.FirstOrDefault(f => f.Date.StartDay() == Commits[i].Date.StartDay()) != default(DateTimeOffset))
                    Commits.RemoveAt(i);
        }

        private void MenuFlyoutItemGoDayCalendar_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedFilters.SelectedCommit != null)
                SetDisplayDateCalendatView(SelectedFilters.SelectedCommit.Date);
        }

        private void MenuFlyoutItemOffDayCalendar_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedFilters.SelectedCommit != null)
            {
                List<DateTimeOffset> selectedDates = CalendarViewDateTaskChanged.SelectedDates.ToList();

                DateTime dayCommit = SelectedFilters.SelectedCommit.Date.StartDay();
                for (int i = selectedDates.Count - 1; i >= 0; --i)
                    if (selectedDates[i].Date == dayCommit)
                        CalendarViewDateTaskChanged.SelectedDates.RemoveAt(i);
            }
        }

        private void SetDisplayDateCalendatView(DateTime date)
        {
            CalendarViewDateTaskChanged.SetDisplayDate(new DateTimeOffset(date));
        }

        private void ButtonUpdateListCommits_Click(object sender, RoutedEventArgs e)
        {
            FillTableCommits();
        }

        private void DataGridCommits_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (SelectedFilters.SelectedCommit != null
                && SelectedFilters.SelectedCommit == SelectedFilters.SelectedCommit2)
            {
                SelectedFilters.SelectedCommit = null;
                Bindings.Update();
            }
            SelectedFilters.SelectedCommit2 = SelectedFilters.SelectedCommit;
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
                catch (Exception ex)
                {
                    Dialogs.ShowPopups("Не удалось получить доступ к файлу.\nВозможно нет доступа к файлу или файл не существует.");
                }

                if (storageFile == null)
                    return;

                await Launcher.LaunchFileAsync(storageFile);
            }
        }
    }
}
