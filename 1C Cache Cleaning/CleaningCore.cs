﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace _1C_Cache_Cleaning
{
    class CleaningCore
    {
        // Counter of cache size
        private double cacheSize = 0L;


        // Start of cache cleaning
        // Find all cache directories and clean it
        public void StartCleaning()
        {
            cacheSize = 0;

            // Get Local and Roaming dirs paths 
            string AppDataLocalGlobal = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string AppDataRoamingGlobal = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            // Define list of 1C cache directories 
            string[] AppDataSubFoldersNames = {
                @"\1C\1cv8\",
                @"\1C\1cv8t\",
                @"\1C\1Cv82\",
                @"\1C\1Cv82t\",
                @"\1C\1Cv83\"
            };

            // Local
            foreach (string CurrentLocalPath in AppDataSubFoldersNames)
            {
                if (Directory.Exists(AppDataLocalGlobal + CurrentLocalPath))
                {
                    string[] LocalSubDirs = Directory.GetDirectories(AppDataLocalGlobal + CurrentLocalPath, "*", SearchOption.TopDirectoryOnly);
                    DirsDeleting(LocalSubDirs);
                }
            }

            // Roaming
            foreach (string CurrentLocalPath in AppDataSubFoldersNames)
            {
                if (Directory.Exists(AppDataRoamingGlobal + CurrentLocalPath))
                {
                    string[] LocalSubDirs = Directory.GetDirectories(AppDataRoamingGlobal + CurrentLocalPath, "*", SearchOption.TopDirectoryOnly);
                    DirsDeleting(LocalSubDirs);
                }
            }

            // Final message about cleaning status
            StringBuilder sb = new StringBuilder();
            sb.Append("Очистка кэша 1С завершена.\n\n");
            sb.Append("Очищено ");
            sb.Append(ConvertCacheSize());

            MessageBox.Show(sb.ToString(), "Завершено", MessageBoxButton.OK, MessageBoxImage.Information);

        }

        // Cleaning 
        // Foreach all cache dirictories
        private void DirsDeleting(string[] TargetPaths)
        {
            // Foreach all subfolders
            foreach (string CachePath in TargetPaths)
            {
                string ReplacedString = CachePath.Replace(@"\\", @"\");
                string[] CuttedString = ReplacedString.Split('\\');

                // If dir name length = 26
                if (CuttedString[CuttedString.Length - 1].Length == 36)
                {
                    try
                    {
                        string[] allFiles = Directory.GetFiles(CachePath, "*.*", SearchOption.AllDirectories);

                        foreach (string fileName in allFiles)
                        {
                            FileInfo info = new FileInfo(fileName);
                            cacheSize += info.Length;
                        }

                        Directory.Delete(CachePath, true);
                    }
                    catch
                    {
                        MessageBox.Show("Не все папки с кэшем особождены от процессов 1С.\n\nПопробуйте:\n• запустить очистку в агрессивном режиме\n• завершить все процессы 1С через диспетчер задач\n• запустить очистку после перезагрузки ПК.\n\nПроизойдёт очситка только незанятых папок", "Очистка не будет полной", MessageBoxButton.OK, MessageBoxImage.Exclamation);

                        // Break
                        return;
                    }
                }
            }
        }

        // Convert cache size 
        private string ConvertCacheSize()
        {
            if ((cacheSize > (1024 * 1024 * 1024)))
            {
                double temp = Convert.ToDouble(cacheSize) / (1024 * 1024 * 1024);
                return temp.ToString("0.00") + " GB";
            }
            else if ((cacheSize < (1024 * 1024 * 1024)) && (cacheSize > (1024 * 1024)))
            {
                double temp = Convert.ToDouble(cacheSize) / (1024 * 1024);
                return temp.ToString("0.00") + " MB";
            }
            else if (cacheSize < 1024 * 1024 && cacheSize > 1024)
            {
                double temp = Convert.ToDouble(cacheSize) / (1024);
                return temp.ToString("0.00") + " KB";
            }
            else if (cacheSize < 1024 && cacheSize > 0)
            {
                return cacheSize.ToString() + " B";
            }
            else
            {
                return "0 B";
            }
        }
    }
}
