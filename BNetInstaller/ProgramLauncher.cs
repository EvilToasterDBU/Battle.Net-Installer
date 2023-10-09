using System;
using System.Threading;
using System.Windows.Forms;
using Dark.Net;

namespace BNetInstaller
{
    public static class ProgramLauncher
    {
        private static readonly Mutex Mutex = new(true, "D4Launcher");

        [STAThread]
        static void Main()
        {
            if (Mutex.WaitOne(TimeSpan.Zero, true))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                IDarkNet darkNet = DarkNet.Instance;
                Theme processTheme = Theme.Dark;
                darkNet.SetCurrentProcessTheme(processTheme);
                Form mainForm = new Form1();
                Theme windowTheme = Theme.Dark;
                darkNet.SetWindowThemeForms(mainForm, windowTheme);
                darkNet.UserDefaultAppThemeIsDarkChanged += (_, isSystemDarkTheme) => Console.WriteLine($"System theme is {(isSystemDarkTheme ? "Dark" : "Light")}");
                darkNet.UserTaskbarThemeIsDarkChanged += (_, isTaskbarDarkTheme) => Console.WriteLine($"Taskbar theme is {(isTaskbarDarkTheme ? "Dark" : "Light")}");
                Application.Run(mainForm);
                Mutex.ReleaseMutex();
            }
            else { MessageBox.Show("D4Launcher уже запущен"); }
        }
    }
}
