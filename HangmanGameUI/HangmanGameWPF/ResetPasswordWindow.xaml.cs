using System;
using System.Windows;
using System.Windows.Input;
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
            TxtEmailInfo.Text = string.Format("Codigo enviado a: {0}", email);
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
                TxtStatus.Text = "! ERROR: Ingrese el codigo.";
                return;
            }

            if (string.IsNullOrWhiteSpace(newPassword))
            {
                TxtStatus.Text = "! ERROR: Ingrese la nueva contrasena.";
                return;
            }

            if (string.IsNullOrWhiteSpace(confirmPassword))
            {
                TxtStatus.Text = "! ERROR: Confirme la nueva contrasena.";
                return;
            }

            if (newPassword != confirmPassword)
            {
                TxtStatus.Text = "! ERROR: Las contrasenas no coinciden.";
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

                TxtStatus.Text = result == null ? "Sin respuesta del servidor." : result.Message;

                if (result != null && result.Success)
                {
                    MessageBox.Show("Contrasena actualizada correctamente.", "Recuperacion");
                    Close();
                }
            }
            catch (Exception ex)
            {
                TxtStatus.Text = "Error cambiando contrasena: " + ex.Message;
            }
            finally
            {
                ServiceClientFactory.CloseChannel(client);
            }
        }
    }
}
