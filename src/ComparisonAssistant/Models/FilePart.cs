using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparisonAssistant.Models
{
    public class FilePart
    {
        public FilePart(string fileName)
        {
            Parts = fileName.Split('/');
        }

        public string[] Parts { get; set; }
        public int MaxIndex { get => Parts == null ? -1 : Parts.Count() - 1; }

        public string this[int index]
        {
            get
            {
                if (Parts == null)
                    return null;
                if (index < 0)
                    return null;
                if (index > Parts.Count())
                    return null;

                return Parts[index];
            }
        }
    }
}