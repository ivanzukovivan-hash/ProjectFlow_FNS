using System.Windows;
using System.Windows.Controls;
using ProjectFlow_FNS.Services;

namespace ProjectFlow_FNS.Views
{
    public partial class RegisterWindow : Window
    {
        private readonly AuthService _auth = new AuthService();

        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(TxtLogin.Text) ||
                    string.IsNullOrWhiteSpace(TxtFullName.Text) ||
                    string.IsNullOrWhiteSpace(TxtPassword.Password) ||
                    string.IsNullOrWhiteSpace(TxtQuestion.Text) ||
                    string.IsNullOrWhiteSpace(TxtAnswer.Text))
                {
                    TxtMsg.Text = "Заполните все поля!";
                    TxtMsg.Foreground = System.Windows.Media.Brushes.Red;
                    return;
                }

                string role = (CbRole.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Исполнитель";
                bool ok = _auth.Register(TxtLogin.Text, TxtPassword.Password, TxtFullName.Text,
                                         TxtEmail.Text, role, TxtQuestion.Text, TxtAnswer.Text);
                if (ok)
                {
                    MessageBox.Show("Регистрация успешна! Войдите в систему.", "Успех",
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                    new LoginWindow().Show();
                    this.Close();
                }
                else
                {
                    TxtMsg.Text = "Ошибка регистрации (возможно, логин занят).";
                    TxtMsg.Foreground = System.Windows.Media.Brushes.Red;
                }
            }
            catch (System.Exception ex)
            {
                TxtMsg.Text = "Ошибка: " + ex.Message;
                TxtMsg.Foreground = System.Windows.Media.Brushes.Red;
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            new LoginWindow().Show();
            this.Close();
        }
    }
}