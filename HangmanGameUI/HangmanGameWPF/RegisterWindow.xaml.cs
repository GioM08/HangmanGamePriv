using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using HangmanGameWPF.Localization;
using HangmanGameWPF.Services;

namespace HangmanGameWPF
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
            TxtFullName.Focus();
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            => DragMove();

        private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();
        private void BtnCancel_Click(object sender, RoutedEventArgs e) => Close();

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            TxtMessage.Foreground = new SolidColorBrush(Color.FromRgb(0xFF, 0x44, 0x44));
            TxtMessage.Text = "";

            string fullName = TxtFullName.Text.Trim();
            DateTime? birthDate = DpBirthDate.SelectedDate;
            string phone = TxtPhone.Text.Trim();
            string email = TxtEmail.Text.Trim();
            string pass = PbPassword.Password;
            string confirm = PbConfirm.Password;

            if (string.IsNullOrWhiteSpace(fullName))
            { TxtMessage.Text = ClientLocalizer.Get("ERROR_FULL_NAME_REQUIRED"); return; }
            if (fullName.Length < 3)
            { TxtMessage.Text = ClientLocalizer.Get("ERROR_FULL_NAME_LENGTH"); return; }
            if (!birthDate.HasValue || birthDate.Value >= DateTime.Today)
            { TxtMessage.Text = ClientLocalizer.Get("ERROR_BIRTH_DATE_INVALID"); return; }
            if (string.IsNullOrWhiteSpace(phone))
            { TxtMessage.Text = ClientLocalizer.Get("ERROR_PHONE_REQUIRED"); return; }
            if (string.IsNullOrWhiteSpace(email) || !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            { TxtMessage.Text = ClientLocalizer.Get("ERROR_EMAIL_INVALID"); return; }
            if (string.IsNullOrEmpty(pass))
            { TxtMessage.Text = ClientLocalizer.Get("ERROR_PASSWORD_REQUIRED"); return; }
            if (pass.Length < 8)
            { TxtMessage.Text = ClientLocalizer.Get("ERROR_PASSWORD_LENGTH"); return; }
            if (pass != confirm)
            {
                TxtMessage.Text = ClientLocalizer.Get("ERROR_PASSWORD_MISMATCH");
                PbConfirm.Clear();
                PbConfirm.Focus();
                return;
            }

            IUserService client = null;

            try
            {
                client = ServiceClientFactory.CreateUserClient();

                OperationResultDto result;

                using (ServiceCallContext.CreateScope(client))
                {
                    result = client.RegisterUser(new RegisterUserDto
                    {
                        FullName = fullName,
                        BirthDate = birthDate.Value,
                        PhoneNumber = phone,
                        Email = email,
                        Password = pass
                    });
                }

                if (result != null && result.Success)
                {
                    ShowRegistrationCompleted();
                }
                else
                {
                    TxtMessage.Text = string.Format("! {0}", result == null ? ClientLocalizer.Get("ERROR_SERVER_EMPTY") : result.Message);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Register] Error: {ex.Message}");
                TxtMessage.Text = ClientLocalizer.Get("ERROR_CONNECTION");
            }
            finally
            {
                ServiceClientFactory.CloseChannel(client);
            }
        }

        private void ShowRegistrationCompleted()
        {
            TxtMessage.Foreground = new SolidColorBrush(Color.FromRgb(0x00, 0xFF, 0x41));
            TxtMessage.Text = ClientLocalizer.Get("REGISTER_SUCCESS");
            TxtFullName.IsEnabled = false;
            DpBirthDate.IsEnabled = false;
            TxtPhone.IsEnabled = false;
            TxtEmail.IsEnabled = false;
            PbPassword.IsEnabled = false;
            PbConfirm.IsEnabled = false;
        }
    }
}
