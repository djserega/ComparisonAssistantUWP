using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace ComparisonAssistant
{
    public class StaticSettings
    {
        internal static Dictionary<string, string> DictionaryTranslate { get; } = GetDictionaryTranslateObject();

        private static Dictionary<string, string> GetDictionaryTranslateObject()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>() {
                    { "AccumulationRegisters", "РегистрыНакопления"},
                    { "BusinessProcesses", "БизнесПроцессы"},
                    { "Catalogs", "Справочники"},
                    { "ChartsOfCharacteristicTypes", "ПланыВидовХарактеристик"},
                    { "CommandGroups", "ГруппыКоманд"},
                    { "CommonAttributes", "ОбщиеРеквизиты"},
                    { "CommonCommands", "ОбщиеКоманды"},
                    { "CommonForms", "ОбщиеФормы"},
                    { "CommonModules", "ОбщиеМодули"},
                    { "CommonPictures", "ОбщиеКартинки"},
                    { "CommonTemplates", "ОбщиеМакеты"},
                    { "Configurations", "Конфигурации"},
                    { "Constants", "Константы"},
                    { "DataProcessors", "Обработки"},
                    { "DocumentJournals", "ЖурналыДокументов"},
                    { "DocumentNumerators", "НумераторыДокументов"},
                    { "Documents", "Документы"},
                    { "Enums", "Перечисления"},
                    { "EventSubscriptions", "ПодпискиНаСобытие"},
                    { "ExchangePlans", "ПланыОбмена"},
                    { "ExternalDataSources", "ВнешниеИсточникиДанных"},
                    { "FilterCriteria", "КритерииОтбора"},
                    { "FunctionalOptions", "ФункциональныеОпции"},
                    { "FunctionalOptionsParameters", "ПараметрыФункциональныхОпций"},
                    { "InformationRegisters", "РегистрыСведений"},
                    { "Interfaces", "Интерфейсы"},
                    { "Languages", "Языки"},
                    { "Reports", "Отчеты"},
                    { "Roles", "Роли"},
                    { "ScheduledJobs", "РегламентныеЗадания"},
                    { "Sequences", "Последовательности"},
                    { "SessionParameters", "ПараметрыСеанса"},
                    { "StyleItems", "ЭлементыСтиля"},
                    { "Styles", "Стили"},
                    { "Subsystems", "Подсистемы"},
                    { "Tasks", "Задачи"},
                    { "WebServices", "Web-сервисы"},
                    { "XDTOPackages", "XDTO-пакеты"}
                };
            return dict;
        }

        public static bool VisibleFullNameChangedFiles { get; set; } = false;

        public static Visibility VisibilityFullNameChangedFiles { get => VisibleFullNameChangedFiles ? Visibility.Visible : Visibility.Collapsed; }
    }
}
