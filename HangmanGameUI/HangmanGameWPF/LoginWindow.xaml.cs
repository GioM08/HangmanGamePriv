using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using HangmanGameWPF.Localization;
using HangmanGameWPF.Services;

namespace HangmanGameWPF
{
    public partial class LoginWindow : Window
    {
        private readonly DispatcherTimer _blinkTimer;
        private bool _blinkVisible = true;

        public LoginWindow()
        {
            InitializeComponent();

            _blinkTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(600) };
            _blinkTimer.Tick += (s, e) =>
            {
                _blinkVisible = !_blinkVisible;
                BlinkText.Opacity = _blinkVisible ? 1.0 : 0.0;
            };
            _blinkTimer.Start();

            SelectCurrentLanguage();
            TxtEmail.Focus();
        }

        private void SelectCurrentLanguage()
        {
            foreach (ComboBoxItem item in CmbLanguage.Items)
            {
                if ((item.Tag as string) == ClientLanguageContext.CurrentLanguage)
                {
                    CmbLanguage.SelectedItem = item;
                    return;
                }
            }
        }

        private void CmbLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selectedItem = CmbLanguage.SelectedItem as ComboBoxItem;

            if (selectedItem == null)
            {
                return;
            }

            ClientLanguageContext.SetLanguage(selectedItem.Tag as string);
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            => DragMove();

        private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
            => WindowState = WindowState.Minimized;

        private void Input_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) TryLogin();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e) => TryLogin();

        private void TryLogin()
        {
            TxtError.Text = "";
            string email = TxtEmail.Text.Trim();
            string pass = PbPassword.Password;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(pass))
            {
                TxtError.Text = ClientLocalizer.Get("ERROR_COMPLETE_FIELDS");
                return;
            }

            IUserService client = null;

            try
            {
                client = ServiceClientFactory.CreateUserClient();
                OperationResultDto result;

                using (ServiceCallContext.CreateScope(client))
                {
                    result = client.Login(new LoginDto { Email = email, Password = pass });
                }

                if (result != null && result.Success && result.User != null)
                {
                    SessionManager.SetUser(
                        result.User.UserId,
                        result.User.FullName,
                        result.User.Email,
                        result.User.GlobalScore);

                    _blinkTimer.Stop();
                    new MainWindow().Show();
                    Close();
                }
                else
                {
                    TxtError.Text = $"! ERROR: {result?.Message ?? ClientLocalizer.Get("ERROR_SERVER_EMPTY")}";
                    PbPassword.Clear();
                    PbPassword.Focus();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Login] Error: {ex.Message}");
                MessageBox.Show(
                    $"Tipo: {ex.GetType().Name}\n\nMensaje: {ex.Message}\n\nInner: {ex.InnerException?.Message}",
                    "ERROR EXCEPCION");
                TxtError.Text = ClientLocalizer.Get("ERROR_CONNECTION");
            }
            finally
            {
                ServiceClientFactory.CloseChannel(client);
            }
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
            => new RegisterWindow { Owner = this }.ShowDialog();

        private void BtnForgotPassword_Click(object sender, RoutedEventArgs e)
            => new ForgotPasswordWindow { Owner = this }.ShowDialog();
    }
}
