﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparisonAssistant.Models
{
    public class SkipFileNames
    {
        public List<string> ListFileName { get; set; } = new List<string> { "VERSION", "renames.txt", "AUTHORS", ".gitignore", "README.md", "Jenkinsfile" };
    }
}
