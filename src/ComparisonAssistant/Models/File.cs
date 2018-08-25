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

        public string Mode { get; set; }
        public string FullName { get; set; }
        public FilePart FileParts { get; set; }

        public string TypeObjectName { get; set; }
        public string ObjectName { get; set; }

        public bool IsModule { get; set; } = false;
        public bool IsModuleManager { get; set; } = false;
        public bool IsForm { get; set; } = false;
        public bool ChangedFormObject { get; set; } = false;
        public bool ChangeObject { get; set; } = false;

        private void ParseFileParts()
        {
            for (int i = FileParts.MaxIndex; i >= 0; --i)
            {
                string part = FileParts[i];

                if (part == "Module.bsl")
                    IsModule = true;
                if (part == "ManagerModule.bsl")
                    IsModuleManager = true;
                if (part == "Forms")
                    IsForm = true;

                if (IsModule && IsForm && FileParts[--i] == "ФормаЭлемента")
                    ChangedFormObject = true;

                if (part == "Catalogs")
                {
                    TypeObjectName = "Справочники";
                    ObjectName = FileParts[i + 1];
                }

            }

            if (!IsModule && !IsModuleManager && !IsForm)
                ChangeObject = true;

        }

    }
}
