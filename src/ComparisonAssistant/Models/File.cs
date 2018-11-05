using ComparisonAssistant.Additions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

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
        public string Name { get; set; } = string.Empty;
        public FilePart FileParts { get; set; }

        public string TypeObjectName { get; set; } = string.Empty;
        public string ObjectName { get; set; } = string.Empty;

        public bool IsModule { get; set; } = false;
        public bool IsModuleManager { get; set; } = false;
        public bool IsForm { get; set; } = false;
        public bool IsCommand { get; set; } = false;
        public bool IsTemplate { get; set; }
        public bool ChangedObjectForm { get; set; } = false;
        public bool ChangeModuleForm { get; set; } = false;
        public bool ChangeObject { get; set; } = false;

        public Visibility VisibilityFullNameChangedFiles { get => StaticSettings.VisibilityFullNameChangedFiles; }

        private void ParseFileParts()
        {
            bool isSubsystems = false;
            string treeSubsystem = string.Empty;

            string formName = string.Empty;
            string commandName = string.Empty;
            string templateName = string.Empty;

            string previousPart = string.Empty;
            string part = string.Empty;

            for (int i = FileParts.MaxIndex; i >= 0; --i)
            {
                if (IsForm && string.IsNullOrEmpty(formName))
                    formName = previousPart.RemoveEndText(".xml");
                if (IsCommand && string.IsNullOrEmpty(commandName))
                    commandName = previousPart.RemoveEndText(".xml");

                previousPart = part;
                part = FileParts[i];

                if (part == "Module.bsl" || part == "ObjectModule.bsl" || part == "RecordSetModule.bsl")
                    IsModule = true;
                else if (part == "ManagerModule.bsl")
                    IsModuleManager = true;
                else if (part == "Forms")
                    IsForm = true;
                else if (part == "Commands")
                    IsCommand = true;
                else if (part == "Subsystems")
                {
                    isSubsystems = true;
                    if (!string.IsNullOrEmpty(previousPart))
                        treeSubsystem = previousPart.RemoveEndText(".xml") + 
                            (string.IsNullOrEmpty(treeSubsystem) ? "" : $".{treeSubsystem}");
                }
                else if (part == "Templates")
                {
                    IsTemplate = true;
                    templateName = previousPart.RemoveEndText(".xml");
                }

                if (StaticSettings.DictionaryTranslate.ContainsKey(part))
                {
                    TypeObjectName = StaticSettings.DictionaryTranslate[part];
                    ObjectName = previousPart;
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
            if (FullName == "Ext/ParentConfigurations.bin")
            {
                TypeObjectName = "Конфигурация";
                ObjectName = "Поддержка";
            }
            else if (FullName == "Ext/ManagedApplicationModule.bsl")
            {
                TypeObjectName = "Конфигурация";
                ObjectName = "Модуль управляемого приложения";
            }

            Name = TypeObjectName;
            if (!string.IsNullOrWhiteSpace(ObjectName))
            {
                if (isSubsystems)
                {
                    Name += $".{treeSubsystem}";
                }
                else
                {
                    Name += $".{ObjectName}";

                    if (IsForm && !string.IsNullOrEmpty(formName))
                        Name += $".Форма.{formName}";
                    if (IsCommand && !string.IsNullOrEmpty(commandName))
                        Name += $".Команды.{commandName}";
                    if (IsTemplate && !string.IsNullOrEmpty(templateName))
                        Name += $".Макет.{templateName}";

                    if (IsModule)
                        Name += ".Модуль";
                    if (IsModuleManager)
                        Name += ".МодульМенеджера";
                }
            }

        }
    }
}
