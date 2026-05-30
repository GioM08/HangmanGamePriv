using System;
using System.Windows;
using System.Windows.Input;
using HangmanGameWPF.Localization;
using HangmanGameWPF.Services;

namespace HangmanGameWPF
{
    public partial class VerifyEmailWindow : Window
    {
        private readonly int userId;

        public VerifyEmailWindow(int userId, string email)
        {
            InitializeComponent();

            this.userId = userId;
            TxtInfo.Text = string.Format(ClientLocalizer.Get("EMAIL_VERIFICATION_SENT_TO"), email);
            TxtCode.Focus();
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            => DragMove();

        private void BtnClose_Click(object sender, RoutedEventArgs e)
            => Close();

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
            => WindowState = WindowState.Minimized;

        private void BtnVerify_Click(object sender, RoutedEventArgs e)
        {
            string code = TxtCode.Text.Trim();

            if (string.IsNullOrWhiteSpace(code))
            {
                TxtStatus.Text = ClientLocalizer.Get("ERROR_CODE_REQUIRED");
                return;
            }

            IAccountRecoveryService client = null;

            try
            {
                client = ServiceClientFactory.CreateAccountRecoveryClient();

                EmailOperationResultDto result = client.VerifyEmailCode(new VerifyEmailDto
                {
                    UserId = userId,
                    Code = code
                });

                TxtStatus.Text = result == null ? ClientLocalizer.Get("ERROR_SERVER_EMPTY") : result.Message;

                if (result != null && result.Success)
                {
                    MessageBox.Show(
                        ClientLocalizer.Get("EMAIL_VERIFIED"),
                        ClientLocalizer.Get("VERIFICATION_TITLE"));
                    Close();
                }
            }
            catch (Exception ex)
            {
                TxtStatus.Text = string.Format(ClientLocalizer.Get("ERROR_VERIFY_EMAIL"), ex.Message);
            }
            finally
            {
                ServiceClientFactory.CloseChannel(client);
            }
        }

        private void BtnResend_Click(object sender, RoutedEventArgs e)
        {
            IAccountRecoveryService client = null;

            try
            {
                client = ServiceClientFactory.CreateAccountRecoveryClient();

                EmailOperationResultDto result = client.SendEmailVerificationCode(userId);

                TxtStatus.Text = result == null ? ClientLocalizer.Get("ERROR_SERVER_EMPTY") : result.Message;
            }
            catch (Exception ex)
            {
                TxtStatus.Text = string.Format(ClientLocalizer.Get("ERROR_RESEND_CODE"), ex.Message);
            }
            finally
            {
                ServiceClientFactory.CloseChannel(client);
            }
        }
    }
}
