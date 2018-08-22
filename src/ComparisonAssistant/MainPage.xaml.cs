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

        internal Settings Settings { get; set; } = new Settings();
        public List<Models.Commit> AllCommits { get; } = new List<Models.Commit>();
        public ObservableCollection<Models.Commit> Commits { get; } = new ObservableCollection<Models.Commit>();
        public List<string> Users { get; } = new List<string>();
        public List<string> UserTasks { get; } = new List<string>();

        public string SelectedUser { get; set; }
        public string SelectedTask { get; set; }

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
            SelectedUser = string.Empty;
            SelectedTask = string.Empty;


            ReaderFileLog readerLog = new ReaderFileLog() { FileName = Settings.FullNameFileLogs };
            List<Models.Commit> listCommits = await readerLog.ReadFileAsync();
            foreach (Models.Commit item in listCommits)
                AllCommits.Add(item);

            _groupedCommitByUser = listCommits.GroupBy(f => f.UserName);

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
            UserTasks.Clear();
            foreach (IGrouping<string, Models.Commit> itemGroup in _groupedCommitByUser.Where(f => f.Key == SelectedUser))
            {
                foreach (var item in itemGroup.GroupBy(f => f.Task))
                {
                    UserTasks.Add(item.Key);
                }
            }
            UserTasks.Sort();
        }

        private void ComboBoxTask_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<Models.Commit> listCommits = new List<Models.Commit>();
            foreach (IGrouping<string, Models.Commit> itemGroup in _groupedCommitByUser.Where(f => f.Key == SelectedUser))
            {
                foreach (Models.Commit item in itemGroup.Where(f => f.Task == SelectedTask))
                {
                    listCommits.Add(item);
                }
            }

            listCommits.Sort((a, b) => -a.Date.CompareTo(b.Date));

            Commits.Clear();
            foreach (Models.Commit item in listCommits)
                Commits.Add(item);

            Bindings.Update();
        }
    }
}
