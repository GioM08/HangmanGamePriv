using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using HangmanGameWPF.Services;

namespace HangmanGameWPF
{
    public partial class EditProfileWindow : Window
    {
        public EditProfileWindow()
        {
            InitializeComponent();
            TxtUsername.Text = SessionManager.FullName ?? string.Empty;
            TxtEmail.Text = SessionManager.Email ?? string.Empty;
            TxtUsername.Focus();

            LoadCurrentProfile();
        }

        private void LoadCurrentProfile()
        {
            try
            {
                var client = ServiceClientFactory.CreateUserClient();
                var result = client.GetUserProfile(SessionManager.UserId);
                ServiceClientFactory.CloseChannel(client);

                if (result.Success && result.User != null)
                {
                    TxtPhone.Text = result.User.PhoneNumber ?? string.Empty;
                    DpBirthDate.SelectedDate = result.User.BirthDate;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[EditProfile] Load error: {ex.Message}");
            }
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();
        private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();
        private void BtnCancel_Click(object sender, RoutedEventArgs e) => Close();

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            TxtMessage.Foreground = new SolidColorBrush(Color.FromRgb(0xFF, 0x44, 0x44));
            TxtMessage.Text = "";

            string fullName = TxtUsername.Text.Trim();
            string phone = TxtPhone.Text.Trim();
            DateTime? birthDate = DpBirthDate.SelectedDate;

            if (string.IsNullOrWhiteSpace(fullName) || fullName.Length < 3)
            { TxtMessage.Text = "! El nombre debe tener al menos 3 caracteres."; return; }
            if (!birthDate.HasValue || birthDate.Value >= DateTime.Today)
            { TxtMessage.Text = "! Ingrese una fecha de nacimiento valida."; return; }
            if (string.IsNullOrWhiteSpace(phone))
            { TxtMessage.Text = "! El telefono no puede estar vacio."; return; }

            try
            {
                var client = ServiceClientFactory.CreateUserClient();
                var result = client.UpdateUserProfile(new UpdateUserProfileDto
                {
                    UserId = SessionManager.UserId,
                    FullName = fullName,
                    BirthDate = birthDate.Value,
                    PhoneNumber = phone
                });
                ServiceClientFactory.CloseChannel(client);

                if (result.Success)
                {
                    if (result.User != null)
                        SessionManager.SetUser(result.User.UserId, result.User.FullName,
                            result.User.Email, result.User.GlobalScore);

                    TxtMessage.Foreground = new SolidColorBrush(Color.FromRgb(0x00, 0xFF, 0x41));
                    TxtMessage.Text = "> Cambios guardados correctamente.";
                }
                else
                {
                    TxtMessage.Text = $"! {result.Message}";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[EditProfile] Save error: {ex.Message}");
                TxtMessage.Text = "! Error al conectar con el servidor.";
            }
        }
    }
}
