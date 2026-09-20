using System.Windows;
using ProjectFlow_FNS.Services;

namespace ProjectFlow_FNS.Views
{
    public partial class ForgotPasswordWindow : Window
    {
        private readonly AuthService _auth = new AuthService();
        private string _currentLogin;

        public ForgotPasswordWindow()
        {
            InitializeComponent();
        }

        private void BtnFindQuestion_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtLogin.Text))
            {
                TxtMsg.Text = "Введите логин!";
                return;
            }
            string q = _auth.GetSecurityQuestion(TxtLogin.Text);
            if (!string.IsNullOrEmpty(q))
            {
                _currentLogin = TxtLogin.Text;
                TxtQuestion.Text = q;
                TxtQuestion.Visibility = Visibility.Visible;
                TxtAnswer.Visibility = Visibility.Visible;
                LblNewPass.Visibility = Visibility.Visible;
                TxtNewPassword.Visibility = Visibility.Visible;
                BtnReset.Visibility = Visibility.Visible;
                TxtMsg.Text = "";
            }
            else
            {
                TxtMsg.Text = "Пользователь не найден!";
            }
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtAnswer.Text) || string.IsNullOrWhiteSpace(TxtNewPassword.Password))
            {
                TxtMsg.Text = "Заполните все поля!";
                return;
            }
            bool ok = _auth.ResetPassword(_currentLogin, TxtAnswer.Text, TxtNewPassword.Password);
            if (ok)
            {
                MessageBox.Show("Пароль успешно изменён!", "Успех",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                new LoginWindow().Show();
                this.Close();
            }
            else
            {
                TxtMsg.Text = "Неверный ответ на контрольный вопрос!";
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            new LoginWindow().Show();
            this.Close();
        }
    }
}