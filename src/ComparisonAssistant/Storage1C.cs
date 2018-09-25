using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.System;
using Windows.UI.Xaml;

namespace ComparisonAssistant
{
    public class Storage1C
    {
        private UpdateElementsEvents _updateElementsEvents;
        private ValueStorage1CEvents _valueStorage1CEvents;
        private string _fileNameBatFileTestConnection;

        public Storage1C(UpdateElementsEvents updateElementsEvents, ValueStorage1CEvents valueStorage1CEvents)
        {
            _updateElementsEvents = updateElementsEvents;
            _valueStorage1CEvents = valueStorage1CEvents;
        }

        public string FileNameBatFileTestConnection { get => _fileNameBatFileTestConnection; set { _fileNameBatFileTestConnection = value; _updateElementsEvents.EvokeUpdating(); } }

        public string PathBin1C
        {
            get => _valueStorage1CEvents.LoadValue("Storage1CPathBin1C");
            set => _valueStorage1CEvents.SaveValue("Storage1CPathBin1C", value);
        }
        public string PathStorage
        {
            get => _valueStorage1CEvents.LoadValue("Storage1CPathStorage");
            set => _valueStorage1CEvents.SaveValue("Storage1CPathStorage", value);
        }
        public string StorageUser
        {
            get => _valueStorage1CEvents.LoadValue("Storage1CStorageUser");
            set => _valueStorage1CEvents.SaveValue("Storage1CStorageUser", value);
        }
        public string StoragePassword { get; set; }

        public bool DBTypeServer { get => DBType == "DBServer"; set { DBType = "DBServer"; _updateElementsEvents.EvokeUpdating(); } }
        public bool DBTypeFile { get => DBType == "DBFile"; set { DBType = "DBFile"; _updateElementsEvents.EvokeUpdating(); } }

        public Visibility VisibilityDBTypeServer { get => DBTypeServer ? Visibility.Visible : Visibility.Collapsed; }
        public Visibility VisibilityDBTypeFile { get => DBTypeFile ? Visibility.Visible : Visibility.Collapsed; }

        public string DBType
        {
            get => _valueStorage1CEvents.LoadValue("Storage1CDBType");
            set => _valueStorage1CEvents.SaveValue("Storage1CDBType", value);
        }
        public string DBPath
        {
            get => _valueStorage1CEvents.LoadValue("Storage1CDBPath");
            set => _valueStorage1CEvents.SaveValue("Storage1CDBPath", value);
        }
        public string DBServer
        {
            get => _valueStorage1CEvents.LoadValue("Storage1CDBServer");
            set => _valueStorage1CEvents.SaveValue("Storage1CDBServer", value);
        }
        public string DBName
        {
            get => _valueStorage1CEvents.LoadValue("Storage1CDBName");
            set => _valueStorage1CEvents.SaveValue("Storage1CDBName", value);
        }

        internal async Task CheckConnection()
        {
            StringBuilder stringBuilder = new StringBuilder();

            #region Check parameters
            if (string.IsNullOrEmpty(PathBin1C))
                stringBuilder.AppendLine("  - Не выбран каталог 1С");
            if (string.IsNullOrEmpty(PathStorage))
                stringBuilder.AppendLine("  - Не выбран каталог хранилища");
            if (string.IsNullOrEmpty(StorageUser))
                stringBuilder.AppendLine("  - Не заполнен пользователь хранилища.");

            if (DBType == "DBServer")
            {
                if (string.IsNullOrEmpty(DBServer))
                    stringBuilder.AppendLine("  - Не указан сервер");
                if (string.IsNullOrEmpty(DBName))
                    stringBuilder.AppendLine("  - Не указано имя базы");
            }
            else if (DBType == "DBFile")
            {
                if (string.IsNullOrEmpty(DBPath))
                    stringBuilder.AppendLine("  - Не выбран каталог базы");
            }
            else
                stringBuilder.AppendLine("  - Не выбран тип подключения");

            string result = stringBuilder.ToString();

            if (!string.IsNullOrEmpty(result))
                throw new CheckConnectionException(result);
            #endregion

            stringBuilder.Append($"\"{PathBin1C}\\1cv8.exe\"");
            stringBuilder.Append(" ");
            stringBuilder.Append("designer");
            stringBuilder.Append(" ");

            if (DBType == "DBServer")
                stringBuilder.Append($"/s {DBServer}\\{DBName}");
            else
                stringBuilder.Append($"/f \"{DBPath}\"");
            stringBuilder.Append(" ");

            stringBuilder.Append("/configurationrepositoryN");
            stringBuilder.Append(" ");
            stringBuilder.Append($"\"{StorageUser}\"");
            stringBuilder.Append(" ");
            if (!string.IsNullOrEmpty(StoragePassword))
            {
                stringBuilder.Append($"\"{StoragePassword}");
                stringBuilder.Append(" ");
            }

            string commandLineString = stringBuilder.ToString();

            StorageFile storageFile = await WriteTextToTempFile("TestConnectionToStorage1C.bat", commandLineString);

            if (storageFile != null)
            {
                if (!await Launcher.LaunchFileAsync(storageFile))
                {
                    Dialogs.ShowPopups("Не удалось открыть выполнить bat-файл запуска конфигуратора.\nРасположение файла прописано в поле 'Путь к файлу'.");

                    FileNameBatFileTestConnection = storageFile.Path;
                }
            }
        }

        private async Task<StorageFile> WriteTextToTempFile(string fileName, string text)
        {
            StorageFolder storageFolder = ApplicationData.Current.TemporaryFolder;
            StorageFile storageFile = await storageFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(storageFile, text);

            return storageFile;
        }

    }
}
