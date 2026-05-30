using System;
using System.Windows;
using System.Windows.Input;
using HangmanGameWPF.Localization;
using HangmanGameWPF.Services;

namespace HangmanGameWPF
{
    public partial class ResetPasswordWindow : Window
    {
        private readonly string email;

        public ResetPasswordWindow(string email)
        {
            InitializeComponent();

            this.email = email;
            TxtEmailInfo.Text = string.Format(ClientLocalizer.Get("PASSWORD_RESET_SENT_TO"), email);
            TxtCode.Focus();
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            => DragMove();

        private void BtnClose_Click(object sender, RoutedEventArgs e)
            => Close();

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
            => WindowState = WindowState.Minimized;

        private void BtnResetPassword_Click(object sender, RoutedEventArgs e)
        {
            string code = TxtCode.Text.Trim();
            string newPassword = PbNewPassword.Password;
            string confirmPassword = PbConfirmPassword.Password;

            if (string.IsNullOrWhiteSpace(code))
            {
                TxtStatus.Text = ClientLocalizer.Get("ERROR_CODE_REQUIRED");
                return;
            }

            if (string.IsNullOrWhiteSpace(newPassword))
            {
                TxtStatus.Text = ClientLocalizer.Get("ERROR_NEW_PASSWORD_REQUIRED");
                return;
            }

            if (string.IsNullOrWhiteSpace(confirmPassword))
            {
                TxtStatus.Text = ClientLocalizer.Get("ERROR_CONFIRM_PASSWORD_REQUIRED");
                return;
            }

            if (newPassword != confirmPassword)
            {
                TxtStatus.Text = ClientLocalizer.Get("ERROR_PASSWORD_MISMATCH");
                PbConfirmPassword.Clear();
                PbConfirmPassword.Focus();
                return;
            }

            IAccountRecoveryService client = null;

            try
            {
                client = ServiceClientFactory.CreateAccountRecoveryClient();

                EmailOperationResultDto result = client.ResetPassword(new ResetPasswordDto
                {
                    Email = email,
                    Code = code,
                    NewPassword = newPassword
                });

                TxtStatus.Text = result == null ? ClientLocalizer.Get("ERROR_SERVER_EMPTY") : result.Message;

                if (result != null && result.Success)
                {
                    MessageBox.Show(
                        ClientLocalizer.Get("PASSWORD_UPDATED"),
                        ClientLocalizer.Get("RECOVERY_TITLE"));
                    Close();
                }
            }
            catch (Exception ex)
            {
                TxtStatus.Text = string.Format(ClientLocalizer.Get("ERROR_RESET_PASSWORD"), ex.Message);
            }
            finally
            {
                ServiceClientFactory.CloseChannel(client);
            }
        }
    }
}
