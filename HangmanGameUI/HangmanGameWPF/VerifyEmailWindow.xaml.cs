using System;
using System.Windows;
using System.Windows.Input;
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
            TxtInfo.Text = string.Format("Se envio un codigo de 6 digitos a: {0}", email);
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
                TxtStatus.Text = "! ERROR: Ingrese el codigo.";
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

                TxtStatus.Text = result == null ? "Sin respuesta del servidor." : result.Message;

                if (result != null && result.Success)
                {
                    MessageBox.Show("Correo verificado correctamente.", "Verificacion");
                    Close();
                }
            }
            catch (Exception ex)
            {
                TxtStatus.Text = "Error verificando correo: " + ex.Message;
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

                TxtStatus.Text = result == null ? "Sin respuesta del servidor." : result.Message;
            }
            catch (Exception ex)
            {
                TxtStatus.Text = "Error reenviando codigo: " + ex.Message;
            }
            finally
            {
                ServiceClientFactory.CloseChannel(client);
            }
        }
    }
}
