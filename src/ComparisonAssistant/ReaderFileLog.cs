using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Windows.Storage;

namespace ComparisonAssistant
{
    internal class ReaderFileLog
    {
        private readonly string _separatorCommit = " --- ";
        private string _patternFindTaskName = "DEV-[0-9]*";

        internal string FileName { get; set; }

        internal async Task<List<Models.Commit>> ReadFileAsync()
        {
            StorageFile storageFile = null;
            try
            {
                storageFile = await StorageFile.GetFileFromPathAsync(FileName);
            }
            catch (Exception ex)
            {
                Dialogs.Show(ex.Message);
            }

            if (storageFile == null)
                return null;

            IList<string> dataFile = await FileIO.ReadLinesAsync(storageFile);

            List<Models.Commit> listTasks = new List<Models.Commit>();

            List<Models.Commit> lastCommitTasks = new List<Models.Commit>();

            foreach (string row in dataFile)
            {
                bool thisCommit = new Regex(Regex.Escape(_separatorCommit)).Matches(row).Count == 2;
                if (thisCommit)
                {
                    if (lastCommitTasks.Count > 0)
                    {
                        foreach (Models.Commit item in lastCommitTasks)
                        {
                            listTasks.Add(item);
                        }
                        lastCommitTasks.Clear();
                    }

                    #region Read commit
                    string[] commitParts = row.Split(new string[] { _separatorCommit }, StringSplitOptions.RemoveEmptyEntries);

                    if (commitParts.Count() == 3)
                    {
                        DateTime dateCommit;
                        try
                        {
                            dateCommit = DateTime.Parse(commitParts[2]);
                        }
                        catch (FormatException)
                        {
                            dateCommit = DateTime.MinValue;
                        }

                        string userName = commitParts[0];
                        string comment = commitParts[1];

                        MatchCollection matches = new Regex(_patternFindTaskName).Matches(comment);
                        if (matches.Count > 0)
                        {
                            foreach (Match item in matches)
                                lastCommitTasks.Add(new Models.Commit(userName, comment, dateCommit, item.Value));
                        }
                        else
                        {
                            lastCommitTasks.Add(new Models.Commit(userName, comment, dateCommit, "---"));
                        }
                    }
                    #endregion

                }
                else
                {
                    #region Read log
                    string[] file = row.Split(new string[] { "\t" }, StringSplitOptions.None);

                    if (file.Count() == 2 || file.Count() == 3)
                    {
                        string fileName = GetNameObject(file[1]);

                        for (int i = 0; i < lastCommitTasks.Count; i++)
                        {
                            lastCommitTasks[i].Files.Add(new Models.File(file[0], file[1]));
                        }
                    }
                    #endregion
                }
            }

            if (lastCommitTasks.Count > 0)
            {
                foreach (Models.Commit item in lastCommitTasks)
                {
                    listTasks.Add(item);
                }
                lastCommitTasks.Clear();
            }

            return listTasks;
        }

        private string GetNameObject(string fileName)
        {
            //foreach (string prefix in _removePrefixFileName)
            //{
            //    fileName = fileName.RemoveStartText(prefix);
            //}

            //foreach (KeyValuePair<string, string> item in _translateObject)
            //{
            //    fileName = fileName.ReplaseStartText(item, true);
            //    fileName = fileName.ReplaseStartText(item);
            //}

            return fileName;
        }

    }
}
