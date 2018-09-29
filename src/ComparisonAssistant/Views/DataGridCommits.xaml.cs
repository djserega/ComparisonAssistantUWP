using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace ComparisonAssistant.Views
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class DataGridCommits : Page
    {
        private static UpdateElementsEvents _updateElementsEvents = new UpdateElementsEvents();
        private ChangeContentFrameEvents _changeContentFrameEvents = new ChangeContentFrameEvents();

        public DataGridCommits()
        {
            InitializeComponent();
        }

        internal Settings Settings { get; set; } = new Settings();
        internal SelectedFilters SelectedFilters { get; set; } = new SelectedFilters(_updateElementsEvents);

        public ObservableCollection<Models.Commit> Commits { get; private set; } = new ObservableCollection<Models.Commit>();

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is MainWindowFrame objParameter)
            {
                if (objParameter.CountParameter > 0)
                {
                    Commits = objParameter.Parameters[0] as ObservableCollection<Models.Commit>;
                }
            }
        }

        private void DataGridCommits_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (e.OriginalSource is TextBlock source
                && source.Parent == null)
            {
                if (SelectedFilters.SelectedCommit != null
                   && SelectedFilters.SelectedCommit == SelectedFilters.SelectedCommit2)
                {
                    SelectedFilters.SelectedCommit = null;
                    UpdateFormElements();
                }
                SelectedFilters.SelectedCommit2 = SelectedFilters.SelectedCommit;
            }
        }

        private void MenuFlyoutItemOpenSiteTasks_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedFilters.SelectedCommit != null)
            {
                if (!string.IsNullOrWhiteSpace(Settings.PrefixSiteTasks)
                    && !string.IsNullOrWhiteSpace(SelectedFilters.SelectedCommit.Task))
                {
                    string addressSite = Settings.PrefixSiteTasks + SelectedFilters.SelectedCommit.Task;

                    MainWindowFrame parameter = new MainWindowFrame()
                    {
                        Parameters = new object[1] { addressSite },
                        TypesParameters = new Type[1] { typeof(string) }
                    };
                    _changeContentFrameEvents.ChangeContent(typeof(PageBrowser), parameter);
                }
            }
        }

        private void MenuFlyoutItemOpenSiteCommit_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedFilters.SelectedCommit != null)
            {
                if (!string.IsNullOrWhiteSpace(Settings.PrefixSiteCommits)
                    && !string.IsNullOrWhiteSpace(SelectedFilters.SelectedCommit.CommitHash))
                {
                    string addressSite = Settings.PrefixSiteCommits + SelectedFilters.SelectedCommit.CommitHash;

                    MainWindowFrame parameter = new MainWindowFrame()
                    {
                        Parameters = new object[1] { addressSite },
                        TypesParameters = new Type[1] { typeof(string) }
                    };
                    _changeContentFrameEvents.ChangeContent(typeof(PageBrowser), parameter);
                }
            }
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

        private void MenuFlyoutItemCopyObjectNameToClipboard_Click(object sender, RoutedEventArgs e)
        {
            Models.File fileObject = GetFileObjectFromRoutedEventArgs(e);
            if (fileObject != null)
                SetTextToClipboard(fileObject.ObjectName);
        }

        private void MenuFlyoutItemCopyTypeObjectAndObjectNameToClipboard_Click(object sender, RoutedEventArgs e)
        {
            Models.File fileObject = GetFileObjectFromRoutedEventArgs(e);
            if (fileObject != null)
                SetTextToClipboard($"{fileObject.TypeObjectName}.{fileObject.ObjectName}");
        }

        private void SetTextToClipboard(string text)
        {
            DataPackage dataPackage = new DataPackage();
            dataPackage.SetText(text);

            Clipboard.SetContent(dataPackage);
        }

        private Models.File GetFileObjectFromRoutedEventArgs(RoutedEventArgs e)
        {
            if (e.OriginalSource is MenuFlyoutItem menuItem)
                if (menuItem.DataContext is Models.File fileObject)
                    return fileObject;
            return null;
        }

    }
}
