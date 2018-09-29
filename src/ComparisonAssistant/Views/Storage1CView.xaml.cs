using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
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
    public sealed partial class Storage1CView : Page
    {
        private static UpdateElementsEvents _updateElementsEvents = new UpdateElementsEvents();
        private static ValueStorage1CEvents _valueStorage1CEvents = new ValueStorage1CEvents();

        public Storage1CView()
        {
            InitializeComponent();

            _updateElementsEvents.UpdateElementsEvent += () => { UpdateFormElements(); };
            _valueStorage1CEvents.SaveValueEvent += (string name, string value) => { Settings.SaveStorageValue(name, value); };
            _valueStorage1CEvents.LoadValueEvent += (string name) => { return Settings.LoadStorageValue(name); };

        }

        internal Settings Settings { get; set; } = new Settings();
        internal Storage1C Storage1C { get; set; } = new Storage1C(_updateElementsEvents, _valueStorage1CEvents);

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
    }
}
