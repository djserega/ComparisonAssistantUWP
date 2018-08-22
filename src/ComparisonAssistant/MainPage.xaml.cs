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
        private List<Models.Commit> _allCommits = new List<Models.Commit>();
        private List<Models.Commit> _userCommits = new List<Models.Commit>();
        private List<string> _users = new List<string>();
        private List<string> _userTasks = new List<string>();
        private IEnumerable<IGrouping<string, Models.Commit>> _groupedCommitByUser;

        internal Settings Settings { get; set; } = new Settings();
        public List<Models.Commit> AllCommits { get => _allCommits; }
        public List<Models.Commit> Commits { get => _userCommits; }
        public List<string> Users { get => _users; }
        public List<string> UserTasks { get => _userTasks; }

        public string SelectedUser { get; set; }

        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        private async void ButtonUpdateDB_Click(object sender, RoutedEventArgs e)
        {
            _userCommits.Clear();
            _users.Clear();
            _userTasks.Clear();
            

            ReaderFileLog readerLog = new ReaderFileLog() { FileName = Settings.FullNameFileLogs };
            List<Models.Commit> listCommits = await readerLog.ReadFileAsync();
            foreach (Models.Commit item in listCommits)
                _allCommits.Add(item);

            _groupedCommitByUser = listCommits.GroupBy(f => f.UserName);

            foreach (var item in _groupedCommitByUser)
                _users.Add(item.Key);

            _users.Sort();
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
            foreach (IGrouping<string, Models.Commit> itemGroup in _groupedCommitByUser.Where(f => f.Key == SelectedUser))
            {
                foreach (var item in itemGroup.GroupBy(f => f.Task))
                {
                    _userTasks.Add(item.Key);
                }
            }
            _userTasks.Sort();
        }
    }
}
