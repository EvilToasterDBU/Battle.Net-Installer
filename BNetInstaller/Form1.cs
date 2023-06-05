﻿using System.Diagnostics;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using BNetInstaller.Constants;
using BNetInstaller.Endpoints;
using System.Drawing;
using Microsoft.WindowsAPICodePack.Taskbar;
using Dark.Net;
using Newtonsoft.Json.Linq;

namespace BNetInstaller
{
    public partial class Form1 : Form
    {

        string product = "fenris";
        string uid = "fenris";
        string locale = "";
        bool isRepair = false;
        Image lang_en = Properties.Resources.lang_en;
        Image lang_ru = Properties.Resources.lang_ru;
        private TaskbarManager taskbarManager;

        public Form1()
        {
            InitializeComponent();
            DarkNet.Instance.EffectiveCurrentProcessThemeIsDarkChanged += (_, isDarkTheme) => RenderTheme(isDarkTheme);
            RenderTheme(DarkNet.Instance.EffectiveCurrentProcessThemeIsDark);
            this.Load += Form1_Load;
            FormClosing += Form1_FormClosing;
        }
        private void RenderTheme(bool isDarkTheme)
        {
            BackColor = isDarkTheme ? Color.FromArgb(60, 63, 65) : Color.White;
            ForeColor = isDarkTheme ? Color.FromArgb(60, 63, 65) : Color.White;
        }
        private async void Form1_Load(object sender, EventArgs e)
        {
            string repositoryOwner = "EvilToasterDBU";
            string repositoryName = "D4Launcher";
            string currentVersion = "1.0.2"; // Замените на текущую версию вашего приложения
            this.Text = repositoryName + " " + currentVersion;


            await GetLatestVersionFromGitHub(repositoryOwner, repositoryName, currentVersion);

            taskbarManager = TaskbarManager.Instance;
            // Чтение настроек
            bool isRussianSelected = Properties.Settings.Default.IsRussianSelected;
            bool isEnglishSelected = Properties.Settings.Default.IsEnglishSelected;

            // Применение выбранного варианта
            if (isRussianSelected)
            {
                ruToolStripMenuItem.Checked = true;
                engToolStripMenuItem.Checked = false;
                toolStripSplitButton1.Image = lang_ru;
                locale = "ruRU";
            }
            else if (isEnglishSelected)
            {
                ruToolStripMenuItem.Checked = false;
                engToolStripMenuItem.Checked = true;
                toolStripSplitButton1.Image = lang_en;
                locale = "enUS";
            }

            if (!(ruToolStripMenuItem.Checked || engToolStripMenuItem.Checked))
            {
                ruToolStripMenuItem.Checked = true;
                toolStripSplitButton1.Image = lang_ru;
                locale = "ruRU";
            }

            checkBox_store_password.Checked = Properties.Settings.Default.CheckBox1State;
            string currentDirectory = Application.StartupPath;
            string dir = currentDirectory;
            string filePath = dir + ".build.info";
            string lastVersion = GetLastVersionFromBuildInfo(filePath);
            string url = "http://eu.patch.battle.net:1119/" + product + "/versions";
            string version = await GetVersionFromBuildInfo(url);

            checkbox_check_files.CheckedChanged += checkBox3_CheckedChanged; // Добавляем обработчик события

            engToolStripMenuItem.Click += engToolStripMenuItem_Click;
            ruToolStripMenuItem.Click += ruToolStripMenuItem_Click;
            CompareLabelsAndSetButtonAvailability();
            CompareLabelsAndSetButton1Availability();

            label_current_version.Text = lastVersion;
            label_actual_version.Text = version;
            if (!File.Exists(filePath))
            {
                // Если файл отсутствует, устанавливаем текст кнопки "Установить"
                button_update.Text = "Установить";
                button_play.Enabled = false;
                checkBox_store_password.Enabled = false;
            }
            checkBox_store_password.CheckedChanged += checkBox_store_password_CheckedChangedAsync;
            checkBox_store_password.Enabled = true;
        }


        private async Task<string> GetVersionFromBuildInfo(string url)
        {
            string version = "отсутствует";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string buildInfo = await response.Content.ReadAsStringAsync();

                        // Используем регулярное выражение для поиска числового значения
                        // в строке формата "цифры.цифры.цифры" (например, "1.6.74264")
                        Regex regex = new Regex(@"\d+\.\d+\.\d+\.\d+");

                        // Ищем совпадения в строке
                        MatchCollection matches = regex.Matches(buildInfo);

                        // Если найдено хотя бы одно совпадение
                        if (matches.Count > 0)
                        {
                            // Получаем последнее найденное совпадение
                            Match lastMatch = matches[matches.Count - 1];

                            // Получаем значение совпадения
                            version = lastMatch.Value;
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Обработка ошибки запроса URL
                // Устанавливаем значение "неизвестна"
                version = "отсутствует";
            }

            return version;
        }


        private string GetLastVersionFromBuildInfo(string filePath)
        {
            string lastVersion = "отсутствует";

            try
            {
                if (File.Exists(filePath))
                {
                    string buildInfo = File.ReadAllText(filePath);

                    // Используем регулярное выражение для поиска числового значения
                    // в строке формата "цифры.цифры.цифры" (например, "1.6.74264")
                    Regex regex = new Regex(@"\d+\.\d+\.\d+\.\d+");

                    // Ищем совпадения в строке
                    MatchCollection matches = regex.Matches(buildInfo);

                    // Если найдено хотя бы одно совпадение
                    if (matches.Count > 0)
                    {
                        // Получаем последнее найденное совпадение
                        Match lastMatch = matches[matches.Count - 1];

                        // Получаем значение совпадения
                        lastVersion = lastMatch.Value;
                    }
                }

                return lastVersion;
            }
            catch (Exception)
            {
                // Обработка ошибки запроса URL
                // Устанавливаем значение "неизвестна"
                lastVersion = "отсутствует";
            }

            return lastVersion;
        }

        private void button_play_Click(object sender, EventArgs e)
        {
            string currentDirectory = Application.StartupPath;
            string dir = currentDirectory;
            string pathToExecutable = dir + "Diablo IV_beta.exe";
            string parameter = "";
            if (ruToolStripMenuItem.Checked) { parameter = "-launch -locale ruru"; }
            else if (!ruToolStripMenuItem.Checked) { parameter = "-launch -locale enus"; }
            if (checkBox_store_password.Checked)
            {
                pathToExecutable = dir + "Diablo IV.exe";
            }

            Process process = new Process();
            process.StartInfo.FileName = pathToExecutable;
            process.StartInfo.Arguments = parameter;

            process.Start();
            if (checkBox_store_password.Checked) { Application.Exit(); }
            // Завершение выполнения программы
            //Application.Exit();
        }

        private async void button_update_ClickAsync(object sender, EventArgs e)
        {
            // Проверяем, запущена ли программа Battle.net.exe
            Process[] processes = Process.GetProcessesByName("Battle.net");
            if (processes.Length > 0)
            {
                // Программа запущена, выполняем обновление
                await Run();
            }
            else
            {
                // Программа не запущена
                statusLabel.Text = "Сначала запустите Battle.net";
                System.Media.SystemSounds.Hand.Play(); // Воспроизвести звук ошибки
            }
        }

        // Сохранение состояния checkBox1
        private async void checkBox_store_password_CheckedChangedAsync(object sender, EventArgs e)
        {
            string currentDirectory = Application.StartupPath;
            string dir = currentDirectory;
            Properties.Settings.Default.CheckBox1State = checkBox_store_password.Checked;
            if (checkBox_store_password.Checked)
            {
                button_play.Text = "Играть";
                checkBox_store_password.Enabled = false;
                checkbox_check_files.Enabled = true;
                button_play.Enabled = false;
                if (File.Exists(dir + ".build.info")) { File.Move(dir + ".build.info", dir + "_beta/.build.info"); }
                await Task.Delay(250);
                if (File.Exists(dir + "_orig/.build.info")) { File.Move(dir + "_orig/.build.info", dir + ".build.info"); }
                await Task.Delay(250);
                checkBox_store_password.Enabled = true;
                button_play.Enabled = true;
            }
            else if (!checkBox_store_password.Checked)
            {
                button_play.Text = "Логин";
                checkBox_store_password.Enabled = false;
                button_update.Enabled = false;
                checkbox_check_files.Enabled = false;
                button_play.Enabled = false;
                if (File.Exists(dir + ".build.info")) { File.Move(dir + ".build.info", dir + "_orig/.build.info"); }
                await Task.Delay(250);
                if (File.Exists(dir + "_beta/.build.info")) { File.Move(dir + "_beta/.build.info", dir + ".build.info"); }
                await Task.Delay(250);
                checkBox_store_password.Enabled = true;
                button_play.Enabled = true;
            }
            Properties.Settings.Default.Save();
        }

        private async void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!checkBox_store_password.Checked)
            {
                e.Cancel = true; // Отменяем закрытие формы
                checkBox_store_password.Checked = true;
                await Task.Delay(500);
                Properties.Settings.Default.Save();
                Close();
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            CompareLabelsAndSetButtonAvailability();
            isRepair = checkbox_check_files.Checked;
        }

        private void CompareLabelsAndSetButtonAvailability()
        {
            if (label_actual_version.Text != label_current_version.Text || checkbox_check_files.Checked)
            {
                button_update.Enabled = true;
            }
            else
            {
                button_update.Enabled = false;
            }
        }
        private void CompareLabelsAndSetButton1Availability()
        {
            if (label_actual_version.Text != label_current_version.Text || checkbox_check_files.Checked)
            {
                button_play.Enabled = false;
                checkBox_store_password.Enabled = false;
            }
            else
            {
                button_play.Enabled = true;
                checkBox_store_password.Enabled = true;
            }
        }

        private void GetLang()
        {
            if (ruToolStripMenuItem.Checked)
            {
                Properties.Settings.Default.IsRussianSelected = true;
                Properties.Settings.Default.IsEnglishSelected = false;
            }
            else if (engToolStripMenuItem.Checked)
            {
                Properties.Settings.Default.IsRussianSelected = false;
                Properties.Settings.Default.IsEnglishSelected = true;
            }

            Properties.Settings.Default.Save();
        }

        private void ruToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ruToolStripMenuItem.Checked = true;
            engToolStripMenuItem.Checked = false;
            toolStripSplitButton1.Image = lang_ru;
            locale = "ruRU";
            GetLang();
        }

        private void engToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ruToolStripMenuItem.Checked = false;
            engToolStripMenuItem.Checked = true;
            toolStripSplitButton1.Image = lang_en;
            locale = "enUS";
            GetLang();
        }

        private void ruToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (ruToolStripMenuItem.Checked)
            {
                engToolStripMenuItem.Checked = false;
                toolStripSplitButton1.Image = lang_ru;
            }
        }

        private void engToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (engToolStripMenuItem.Checked)
            {
                ruToolStripMenuItem.Checked = false;
                toolStripSplitButton1.Image = lang_en;
            }
        }

        async Task Run()
        {
            string currentDirectory = Application.StartupPath;
            string dir = currentDirectory;
            using AgentApp app = new();

            var mode = isRepair ? Mode.Repair : Mode.Install;

            //Console.WriteLine("Authenticating");
            statusLabel.Text = "Аутентификация...";
            await app.AgentEndpoint.Get();

            //Console.WriteLine($"Queuing {mode}");
            if (mode == Mode.Install)
            {
                statusLabel.Text = "Установка\\Обновление в очереди";
            }
            else if (mode == Mode.Repair)
            {
                statusLabel.Text = "Проверка файлов в очереди";
            }
            app.InstallEndpoint.Model.InstructionsDataset = new[] { "torrent", "win", product, locale.ToLowerInvariant() };
            app.InstallEndpoint.Model.InstructionsPatchUrl = $"http://us.patch.battle.net:1119/{product}";
            app.InstallEndpoint.Model.Uid = uid;
            await app.InstallEndpoint.Post();

            //Console.WriteLine($"Starting {mode}");
            if (mode == Mode.Install)
            {
                statusLabel.Text = "Установка\\Обновление";
            }
            else if (mode == Mode.Repair)
            {
                statusLabel.Text = "Проверка файлов";
            }
            app.InstallEndpoint.Product.Model.GameDir = dir;
            app.InstallEndpoint.Product.Model.Language[0] = locale;
            app.InstallEndpoint.Product.Model.SelectedAssetLocale = locale;
            app.InstallEndpoint.Product.Model.SelectedLocale = locale;
            await app.InstallEndpoint.Product.Post();

            Console.WriteLine();

            var operation = mode switch
            {
                Mode.Install => InstallProduct(app),
                Mode.Repair => RepairProduct(app),
                _ => throw new NotSupportedException(),
            };

            // process the task
            await operation;

            // send close signal
            await app.AgentEndpoint.Delete();
        }

        async Task InstallProduct(AgentApp app)
        {
            // initiate download
            app.UpdateEndpoint.Model.Uid = uid;
            await app.UpdateEndpoint.Post();

            // first try install endpoint
            if (await ProgressLoop(app.InstallEndpoint.Product))
                return;

            // then try the update endpoint instead
            if (await ProgressLoop(app.UpdateEndpoint.Product))
                return;

            // failing that another agent or the BNet app has probably taken control of the install
            //Console.WriteLine("Another application has taken over. Launch the Battle.Net app to resume installation.");
            statusLabel.Text = "Запущено другое приложение. Запустите Battle.Net, чтобы продолжить установку.";
        }

        async Task RepairProduct(AgentApp app)
        {
            // initiate repair
            app.RepairEndpoint.Model.Uid = uid;
            await app.RepairEndpoint.Post();

            // run the repair endpoint
            if (await ProgressLoop(app.RepairEndpoint.Product))
                return;

            //Console.WriteLine("Unable to repair this product.");
            statusLabel.Text = "Невозможно восстановить этот продукт.";
        }

        async Task<bool> ProgressLoop(ProductEndpoint endpoint)
        {

            while (true)
            {
                var stats = await endpoint.Get();
                await Task.Delay(250);
                // check for completion
                var complete = stats.Value<bool?>("download_complete");
                if (complete == true)
                {
                    await afterUpdateFunctions(); // Выполнение функции после завершения обновления
                    return true;
                }

                // get progress percentage and playability
                var progress = stats.Value<float?>("progress");
                var playable = stats.Value<bool?>("playable");

                if (!progress.HasValue)
                    return false;
                int progressValue = (int)Math.Round(progress.Value * 100);
                progressbar.Visible = true;
                progressbar.Value = progressValue;
                taskbarManager.SetProgressValue(progressValue, 100);
                button_update.Enabled = false;
                button_play.Enabled = false;
                checkbox_check_files.Enabled = false;
                toolStripSplitButton1.Enabled = false;
                //Console.SetCursorPosition(cursorLeft, cursorTop);
                //Print("Downloading:", options.Product);
                //Print("Language:", locale);
                //Print("Directory:", options.Directory);
                //Print("Progress:", progress.Value.ToString("P4"));
                complete.GetValueOrDefault();
                await Task.Delay(2000);

                // exit @ 100%
                if (progress == 1f)
                {
                    await afterUpdateFunctions(); // Выполнение функции после завершения обновления
                    return true;
                }

            }
        }

        private void checkbox_check_files_CheckedChanged(object sender, EventArgs e)
        {
            isRepair = checkbox_check_files.Checked;
        }

        private async Task afterUpdateFunctions()
        {
            string currentDirectory = Application.StartupPath;
            string dir = currentDirectory;
            progressbar.Visible = false;
            taskbarManager.SetProgressState(TaskbarProgressBarState.NoProgress);
            statusLabel.Text = "Готово";
            button_update.Enabled = false;
            button_play.Enabled = true;
            checkbox_check_files.Enabled = true;
            checkbox_check_files.Checked = false;
            toolStripSplitButton1.Enabled = true;
            checkBox_store_password.Enabled = true;
            // Обновление lastVersion и version
            string url = "http://eu.patch.battle.net:1119/" + product + "/versions";
            string version = await GetVersionFromBuildInfo(url);
            label_actual_version.Text = version;
            string filePath = dir + ".build.info";
            string lastVersion = GetLastVersionFromBuildInfo(filePath);
            label_current_version.Text = lastVersion;
        }

        // Класс для десериализации информации о релизе из GitHub API
        public class GitHubRelease
        {
            public string TagName { get; set; }
            // Дополнительные поля, если необходимо
        }

        private async Task<string> GetLatestVersionFromGitHub(string repositoryOwner, string repositoryName, string currentVersion)
        {
            string latestVersion = "отсутствует";

            try
            {
                string apiUrl = $"https://api.github.com/repos/{repositoryOwner}/{repositoryName}/releases/latest";

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; Trident/6.0)");

                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResponse = await response.Content.ReadAsStringAsync();
                        JObject releaseInfo = JObject.Parse(jsonResponse);

                        // Извлечение значения поля "tag_name" из JSON-ответа
                        string tagName = releaseInfo["tag_name"].ToString();

                        if (tagName != null && tagName != currentVersion)
                        {
                            latestVersion = tagName;

                            // Проверка текущей версии
                            if (currentVersion.CompareTo(latestVersion) < 0)
                            {
                                // Вывод текста в statusLabel.Text
                                statusLabel.Text = "Вышло обновление D4 Launcher " + latestVersion;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Обработка ошибки запроса URL
                latestVersion = "отсутствует";
            }

            return latestVersion;
        }

    }
}
