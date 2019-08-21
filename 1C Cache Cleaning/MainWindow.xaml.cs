﻿using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace _1C_Cache_Cleaning
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Counter of errors
        private static int errorsCount = 0;
        // Counter of cache size
        private static double cacheSize = 0L;
        // Mouse hover counter
        private static int mouseCount = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        public object MessageBoxIcon { get; private set; }

        // Start button handler
        private void CacheCleaningButton_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Calling for cleaning
            StartCleaning();
        }

        // Start button handler with killing 1C processes
        private void CacheCleaningButtonAggressive_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                // Kill all processes
                foreach (Process process in Process.GetProcessesByName("1cv8"))
                {
                    process.Kill();
                    process.WaitForExit();
                }
                foreach (Process process in Process.GetProcessesByName("1cv8c"))
                {
                    process.Kill();
                    process.WaitForExit();
                }
                // Calling for cleaning
                Process[] proc1cv8 = Process.GetProcessesByName("1cv8");
                Process[] proc1cv8c = Process.GetProcessesByName("1cv8c");
                if (proc1cv8.Length == 0 && proc1cv8c.Length == 0)
                {
                    StartCleaning();
                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.ToString());
            }
        }

        // Cleaning 
        private void StartCleaning()
        {
            cacheSize = 0;

            string AppDataLocalGlobal = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string AppDataRoamingGlobal = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            string[] AppDataSubFoldersNames = {
                @"\1C\1cv8\",
                @"\1C\1cv8t\",
                @"\1C\1Cv82\",
                @"\1C\1Cv82t\",
                @"\1C\1Cv83\"
            };

            // Status of cleaning
            int statusLocal = 0;
            int statusRoaming = 0;

            // Local
            foreach (string CurrentLocalPath in AppDataSubFoldersNames)
            {
                if (Directory.Exists(AppDataLocalGlobal + CurrentLocalPath) && errorsCount < 1)
                {
                    string[] LocalSubDirs = Directory.GetDirectories(AppDataLocalGlobal + CurrentLocalPath, "*", SearchOption.TopDirectoryOnly);
                    Cleaning(LocalSubDirs, out statusLocal);
                }
            }

            // Roaming
            foreach (string CurrentLocalPath in AppDataSubFoldersNames)
            {
                if (Directory.Exists(AppDataRoamingGlobal + CurrentLocalPath) && errorsCount < 1)
                {
                    string[] LocalSubDirs = Directory.GetDirectories(AppDataRoamingGlobal + CurrentLocalPath, "*", SearchOption.TopDirectoryOnly);
                    Cleaning(LocalSubDirs, out statusLocal);
                }
            }

            // Reset errors count
            errorsCount = 0;

            if (statusLocal == 0 && statusRoaming == 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Очистка кэша 1С завершена.\n\n");
                sb.Append("Очищено ");
                sb.Append(ConvertCacheSize());

                MessageBox.Show(sb.ToString(), "Завершено", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // Cleaning of cache
        private void Cleaning(string[] TargetPaths, out int status)
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
                        // Errors count increment
                        errorsCount += 1;

                        // Returned status 1
                        status = 0;

                        cacheSize = 0;

                        // Break
                        return;
                    }                    
                }
            }
            status = 0;
        }

        // Convert cache size 
        private string ConvertCacheSize()
        {
            if ((cacheSize > (1024 * 1024 * 1024 )))
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

        // Open Logic Flow web site
        private void LabelLF_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Process.Start("https://logicflow.ru");
        }

        // LogicFlow Logo Hover Action
        private void LogicFlow_Site_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (mouseCount == 0)
            {
                LogicFlow_Site.Opacity = 1;
                mouseCount += 1;
            }
        }

        private void LogicFlow_Site_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            LogicFlow_Site.Opacity = 0.4;
            mouseCount = 0;
        }

        // Open GitHub web site
        private void LabelGitHub_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Process.Start("https://github.com/dtinside/1C_Cache_Cleaning");
        }

        // Open GitHub Releases site
        private void LabelGitHubReleases_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Process.Start("https://github.com/dtinside/1C_Cache_Cleaning/releases");
        }



        // Start button hover action
        private void startButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (mouseCount == 0) {
                buttonStart.Source = new BitmapImage(new Uri(@"\Images\1CCC_Start_Hover.bmp", UriKind.Relative));
                LabelCacheButtonTitle.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 123, 255));
                mouseCount += 1;
            }
        }

        // Start button leave action
        private void startButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            buttonStart.Source = new BitmapImage(new Uri(@"\Images\1CCC_Start_Normal.bmp", UriKind.Relative));
            LabelCacheButtonTitle.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(97, 107, 117));
            mouseCount = 0;
        }

        // Aggressive start button hover action
        private void startButtonAgg_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (mouseCount == 0)
            {
                buttonStartAggressive.Source = new BitmapImage(new Uri(@"\Images\1CCC_StartAgg_Hover.bmp", UriKind.Relative));
                LabelCacheButtonAggTitle.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(204, 37, 37));
                mouseCount += 1;
            }
        }

        // Aggressive start button leave action
        private void startButtonAgg_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            buttonStartAggressive.Source = new BitmapImage(new Uri(@"\Images\1CCC_StartAgg_Normal.bmp", UriKind.Relative));
            LabelCacheButtonAggTitle.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(117, 97, 97));
            mouseCount = 0;
        }
    }
}
