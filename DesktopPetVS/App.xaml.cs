using System.Configuration;
using System.Data;
using System.Windows;

namespace DesktopPetVS
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 🔹 zapne globálne sledovanie kurzora
            MouseTracker.Start();

            MainWindow main = new MainWindow();
            main.Show();

            ControlWindow control = new ControlWindow(main);
            control.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // 🔹 vypne hook pri ukončení appky
            MouseTracker.Stop();

            base.OnExit(e);
        }
    }

}
