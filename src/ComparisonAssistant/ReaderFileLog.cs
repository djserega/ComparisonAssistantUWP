using ComparisonAssistant.Additions;
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
        private readonly string _patternFindTaskName = "DEV-[0-9]*";
        private readonly List<string> _removePrefixFileName = new Models.NonusePrefixFileName().ListText;
        private readonly List<string> _listSkipFiles = new Models.SkipFileNames().ListFileName;

        internal string FileName { get; set; }

        internal async Task<List<Models.Commit>> ReadFileAsync()
        {
            if (string.IsNullOrEmpty(FileName))
                return null;

            StorageFile storageFile = null;
            try
            {
                storageFile = await StorageFile.GetFileFromPathAsync(FileName);
            }
            catch (Exception)
            {
                Dialogs.ShowPopups("Ошибка чтения файла логов.\nВозможно нет доступа к файлу или файл не существует.");
            }

            if (storageFile == null)
                return null;

            IList<string> dataFile = null;

            try
            {
                dataFile = await FileIO.ReadLinesAsync(storageFile);
            }
            catch (Exception ex)
            {
                Dialogs.ShowPopups("Ошибка чтения файла. Файл должен быть в кодировке UTF-8.\n" + ex.Message);
            }

            if (dataFile == null)
                return null;

            List<Models.Commit> listTasks = new List<Models.Commit>();

            List<Models.Commit> lastCommitTasks = new List<Models.Commit>();

            try
            {
                string fileName;
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
                            fileName = GetNameObject(file[1]);

                            if (_listSkipFiles.FirstOrDefault(f => f == fileName) == null)
                                for (int i = 0; i < lastCommitTasks.Count; i++)
                                {
                                    try
                                    {
                                        lastCommitTasks[i].Files.Add(new Models.File(file[0], fileName));
                                    }
                                    catch (Exception)
                                    {
                                    }
                                }
                        }
                        #endregion
                    }
                }

            }
            catch (Exception ex)
            {
                Dialogs.ShowPopups("Ошибка чтения данных файла.\n" + ex.Message);
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
            foreach (string prefix in _removePrefixFileName)
            {
                fileName = fileName.RemoveStartText(prefix);
            }

            //foreach (KeyValuePair<string, string> item in _translateObject)
            //{
            //    fileName = fileName.ReplaseStartText(item, true);
            //    fileName = fileName.ReplaseStartText(item);
            //}

            return fileName;
        }

    }
}
