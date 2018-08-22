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

        public List<Models.Commit> AllCommits { get; } = new List<Models.Commit>();
        public ObservableCollection<Models.Commit> Commits { get; } = new ObservableCollection<Models.Commit>();
        public List<string> Users { get; } = new List<string>();
        public List<string> UserTasks { get; } = new List<string>();

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
            Commits.Clear();
            Users.Clear();
            UserTasks.Clear();
            SelectedFilters.ClearFilter();
            _dictionaryUserTasks.Clear();


            ReaderFileLog readerLog = new ReaderFileLog() { FileName = Settings.FullNameFileLogs };
            List<Models.Commit> listCommits = await readerLog.ReadFileAsync();
            foreach (Models.Commit item in listCommits)
                AllCommits.Add(item);

            _groupedCommitByUser = listCommits.GroupBy(f => f.UserName);
            foreach (IGrouping<string, Models.Commit> itemGroup in _groupedCommitByUser)
            {
                List<Models.Commit> list = new List<Models.Commit>();

                foreach (var item in itemGroup)
                    list.Add(item);

                _dictionaryUserTasks.Add(itemGroup.Key, list);
            }


            foreach (var item in _groupedCommitByUser)
                Users.Add(item.Key);

            Users.Sort();

            Bindings.Update();
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
            SelectedFilters.SelectedTask = ""; // null;
            //ComboBoxTask.SelectedIndex = -1;

            Commits.Clear();
            UserTasks.Clear();

            if (_dictionaryUserTasks.ContainsKey(SelectedFilters.SelectedUser))
                foreach (Models.Commit item in _dictionaryUserTasks[SelectedFilters.SelectedUser])
                    if (UserTasks.FirstOrDefault(f => f == item.Task) == null)
                        UserTasks.Add(item.Task);

            UserTasks.Sort();

            ComboBoxTask.ItemsSource = UserTasks;
        }

        private void ComboBoxTask_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Commits.Clear();

            if (SelectedFilters.SelectedTask != null)
            {
                //List<Models.Commit> listCommits = new List<Models.Commit>();
                if (_dictionaryUserTasks.ContainsKey(SelectedFilters.SelectedUser))
                    foreach (Models.Commit item in _dictionaryUserTasks[SelectedFilters.SelectedUser])
                        if (SelectedFilters.SelectedTask == item.Task)
                            Commits.Add(item);

                //foreach (Models.Commit item in listCommits)
                //    Commits.Add(item);
            }

            Bindings.Update();
        }
    }
}
