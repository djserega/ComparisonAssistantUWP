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
        public bool ChangedObjectForm { get; set; } = false;
        public bool ChangeModuleForm { get; set; } = false;
        public bool ChangeObject { get; set; } = false;

        public Visibility VisibilityFullNameChangedFiles { get => StaticSettings.VisibilityFullNameChangedFiles; }

        private void ParseFileParts()
        {
            for (int i = FileParts.MaxIndex; i >= 0; --i)
            {
                string part = FileParts[i];

                if (part == "Module.bsl" || part == "ObjectModule.bsl")
                    IsModule = true;
                if (part == "ManagerModule.bsl")
                    IsModuleManager = true;
                if (part == "Forms")
                    IsForm = true;

                if (StaticSettings.DictionaryTranslate.ContainsKey(part))
                {
                    TypeObjectName = StaticSettings.DictionaryTranslate[part];
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
            if (FullName == "Ext/ParentConfigurations.bin")
            {
                TypeObjectName = "Конфигурация";
                ObjectName = "Поддержка";
            }

            Name = TypeObjectName;
            if (!string.IsNullOrWhiteSpace(ObjectName))
                Name += "." + ObjectName;

        }
    }
}
