using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparisonAssistant
{
    public class Storage1C
    {
        public string PathBin1C { get; set; }
        public string PathStorage { get; set; }
        public string StorageUser { get; set; }
        public string StoragePassword { get; set; }

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
