using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
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

            TxtEmail.Focus();
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
                TxtError.Text = "! ERROR: Complete todos los campos.";
                return;
            }

            try
            {
                var client = ServiceClientFactory.CreateUserClient();
                var result = client.Login(new LoginDto { Email = email, Password = pass });
                ServiceClientFactory.CloseChannel(client);

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
                    TxtError.Text = $"! ERROR: {result?.Message ?? "Sin respuesta del servidor"}";
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
                TxtError.Text = "! ERROR: No se pudo conectar al servidor.";
            }
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
            => new RegisterWindow { Owner = this }.ShowDialog();
    }
}
