﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparisonAssistant
{
    public class Storage1C
    {
        private static ValueStorage1CEvents _valueStorage1CEvents;

        public Storage1C(ValueStorage1CEvents valueStorage1CEvents)
        {
            _valueStorage1CEvents = valueStorage1CEvents;
        }

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

        internal string CheckConnection()
        {
            StringBuilder stringBuilder = new StringBuilder();

            if (string.IsNullOrEmpty(PathBin1C))
                stringBuilder.AppendLine("  - Не выбран каталог 1С");
            if (string.IsNullOrEmpty(PathStorage))
                stringBuilder.AppendLine("  - Не выбран каталог хранилища");
            if (string.IsNullOrEmpty(StorageUser))
                stringBuilder.AppendLine("  - Не заполнен пользователь хранилища.");

            string result = stringBuilder.ToString();

            if (!string.IsNullOrEmpty(result))
                return result;

            return string.Empty;
        }
    }
}
