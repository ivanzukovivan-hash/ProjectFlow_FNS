using System.Windows;
using ProjectFlow_FNS.Services;

namespace ProjectFlow_FNS.Views
{
    public partial class LoginWindow : Window
    {
        private readonly AuthService _auth = new AuthService();

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(TxtLogin.Text) || string.IsNullOrWhiteSpace(TxtPassword.Password))
                {
                    ShowError("Заполните все поля!");
                    return;
                }

                var user = _auth.Login(TxtLogin.Text, TxtPassword.Password);
                if (user != null)
                {
                    var main = new MainWindow(user);
                    main.Show();
                    Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
                    Application.Current.MainWindow = main;
                    this.Close();
                }
                else
                {
                    ShowError("Неверный логин или пароль!");
                }
            }
            catch (System.Exception ex)
            {
                ShowError("Ошибка: " + ex.Message);
            }
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            new RegisterWindow().Show();
            this.Close();
        }

        private void BtnForgot_Click(object sender, RoutedEventArgs e)
        {
            new ForgotPasswordWindow().Show();
            this.Close();
        }

        private void ShowError(string msg)
        {
            TxtError.Text = msg;
            TxtError.Visibility = Visibility.Visible;
        }
    }
}