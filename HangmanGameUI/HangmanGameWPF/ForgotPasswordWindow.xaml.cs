using System;
using System.Windows;
using System.Windows.Input;
using HangmanGameWPF.Localization;
using HangmanGameWPF.Services;

namespace HangmanGameWPF
{
    public partial class ForgotPasswordWindow : Window
    {
        public ForgotPasswordWindow()
        {
            InitializeComponent();
            TxtEmail.Focus();
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            => DragMove();

        private void BtnClose_Click(object sender, RoutedEventArgs e)
            => Close();

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
            => WindowState = WindowState.Minimized;

        private void BtnSendCode_Click(object sender, RoutedEventArgs e)
        {
            string email = TxtEmail.Text.Trim();

            if (string.IsNullOrWhiteSpace(email))
            {
                TxtStatus.Text = ClientLocalizer.Get("ERROR_EMAIL_REQUIRED");
                return;
            }

            IAccountRecoveryService client = null;

            try
            {
                client = ServiceClientFactory.CreateAccountRecoveryClient();

                EmailOperationResultDto result = client.RequestPasswordReset(new ForgotPasswordDto
                {
                    Email = email
                });

                TxtStatus.Text = result == null ? ClientLocalizer.Get("ERROR_SERVER_EMPTY") : result.Message;

                if (result != null && result.Success)
                {
                    ResetPasswordWindow resetWindow = new ResetPasswordWindow(email)
                    {
                        Owner = this
                    };

                    resetWindow.ShowDialog();
                    Close();
                }
            }
            catch (Exception ex)
            {
                TxtStatus.Text = string.Format(ClientLocalizer.Get("ERROR_REQUEST_PASSWORD_RESET"), ex.Message);
            }
            finally
            {
                ServiceClientFactory.CloseChannel(client);
            }
        }
    }
}
