using System.Windows;
using ProjectFlow_FNS.Views;

namespace ProjectFlow_FNS
{
    /// <summary>
    /// Точка входа приложения ProjectFlow ФНС.
    /// Запускает окно авторизации перед основным интерфейсом.
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            // Старт с окна авторизации (требование AuthModule)
            var loginWindow = new LoginWindow();
            loginWindow.Show();
        }
    }
}