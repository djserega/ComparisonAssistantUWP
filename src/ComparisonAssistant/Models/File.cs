using ComparisonAssistant.Additions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparisonAssistant.Models
{
    public class File
    {
        public File(string mode, string fullName)
        {
            Mode = mode;
            FullName = fullName;
            FileParts = new FilePart(fullName);

            ParseFileParts();
        }

        public string Mode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public FilePart FileParts { get; set; }

        public string TypeObjectName { get; set; } = string.Empty;
        public string ObjectName { get; set; } = string.Empty;

        public bool IsModule { get; set; } = false;
        public bool IsModuleManager { get; set; } = false;
        public bool IsForm { get; set; } = false;
        public bool ChangedObjectForm { get; set; } = false;
        public bool ChangeModuleForm { get; set; } = false;
        public bool ChangeObject { get; set; } = false;

        private void ParseFileParts()
        {
            Dictionary<string, string> dictionaryTranslate = GetDictionaryTranslateObject();

            for (int i = FileParts.MaxIndex; i >= 0; --i)
            {
                string part = FileParts[i];

                if (part == "Module.bsl" || part == "ObjectModule.bsl")
                    IsModule = true;
                if (part == "ManagerModule.bsl")
                    IsModuleManager = true;
                if (part == "Forms")
                    IsForm = true;

                if (dictionaryTranslate.ContainsKey(part))
                {
                    TypeObjectName = dictionaryTranslate[part];
                    ObjectName = FileParts[i + 1];
                }

            }

            if (!IsModule && !IsModuleManager && !IsForm)
                ChangeObject = true;
            if (!IsModule && !IsModuleManager && IsForm)
                ChangedObjectForm = true;
            if (IsModule && IsForm)
                ChangeModuleForm = true;

            if (ObjectName.EndsWith(".xml"))
                ObjectName = ObjectName.RemoveEndText(".xml");
        }

        internal Dictionary<string, string> GetDictionaryTranslateObject()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>() {
                    { "Configurations", "Конфигурация"},
                    { "Languages", "Язык"},
                    { "Subsystems", "Подсистема"},
                    { "StyleItems", "Элемент стиля"},
                    { "CommonPictures", "Общая картинка"},
                    { "Interfaces", "Интерфейс"},
                    { "SessionParameters", "Параметр сеанса"},
                    { "Roles", "Роль"},
                    { "CommonTemplates", "Общий макет"},
                    { "FilterCriterias", "Критерий отбора"},
                    { "CommonModules", "Общий модуль"},
                    { "CommonAttributes", "Общий реквизит"},
                    { "ExchangePlanes", "План обмена"},
                    { "XDTOPackages", "XDTO-пакет"},
                    { "WebServices", "Web-сервис"},
                    { "EventSubscriptions", "Подписка на событие"},
                    { "ScheduledJobs", "Регламентное задание"},
                    { "FunctionalOptions", "Функциональная опция"},
                    { "FunctionalOptionsParameters", "Параметр функциональных опций"},
                    { "CommonCommands", "Общая команда"},
                    { "CommandGroupes", "Группа команд"},
                    { "Constants", "Константа"},
                    { "CommonForms", "Общая форма"},
                    { "Catalogs", "Справочник"},
                    { "Documents", "Документ"},
                    { "DocumentNumerators", "Нумератор документов"},
                    { "Sequences", "Последовательность"},
                    { "DocumentJournals", "Журнал документов"},
                    { "Enums", "Перечисление"},
                    { "Reports", "Отчет"},
                    { "DataProcessors", "Обработка"},
                    { "InformationRegisters", "Регистр сведений"},
                    { "AccumulationRegisters", "Регистр накопления"},
                    { "ChartOfCharacteristicTypes", "План видов характеристик"},
                    { "BusinessProcess", "Бизнес процесс"},
                    { "Tasks", "Задачи"},
                    { "ExternalDataSources", "Внешний источники данных"}
                };
            return dict;
        }
    }
}
