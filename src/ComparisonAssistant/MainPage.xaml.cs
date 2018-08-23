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

            _groupedCommitByUser = listCommits.GroupBy(f => f.UserName);
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
            Commits.Clear();

            FillTableCommits();
        }

        private void FillTableCommits()
        {
            if (SelectedFilters.SelectedTask != null)
                if (_dictionaryUserTasks.ContainsKey(SelectedFilters.SelectedUser))
                    foreach (Models.Commit item in _dictionaryUserTasks[SelectedFilters.SelectedUser])
                    {
                        if (SelectedFilters.SelectedTask != item.Task)
                            continue;



                        Commits.Add(item);
                    }
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
    }
}
