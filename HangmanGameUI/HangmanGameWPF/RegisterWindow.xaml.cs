using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
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
            { TxtMessage.Text = "! El nombre completo no puede estar vacio."; return; }
            if (fullName.Length < 3)
            { TxtMessage.Text = "! El nombre debe tener al menos 3 caracteres."; return; }
            if (!birthDate.HasValue || birthDate.Value >= DateTime.Today)
            { TxtMessage.Text = "! Ingrese una fecha de nacimiento valida."; return; }
            if (string.IsNullOrWhiteSpace(phone))
            { TxtMessage.Text = "! El telefono no puede estar vacio."; return; }
            if (string.IsNullOrWhiteSpace(email) || !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            { TxtMessage.Text = "! Ingrese un correo electronico valido."; return; }
            if (string.IsNullOrEmpty(pass))
            { TxtMessage.Text = "! Ingrese una contrasena."; return; }
            if (pass.Length < 8)
            { TxtMessage.Text = "! La contrasena debe tener al menos 8 caracteres."; return; }
            if (pass != confirm)
            {
                TxtMessage.Text = "! Las contrasenas no coinciden.";
                PbConfirm.Clear();
                PbConfirm.Focus();
                return;
            }

            try
            {
                var client = ServiceClientFactory.CreateUserClient();
                var result = client.RegisterUser(new RegisterUserDto
                {
                    FullName = fullName,
                    BirthDate = birthDate.Value,
                    PhoneNumber = phone,
                    Email = email,
                    Password = pass
                });
                ServiceClientFactory.CloseChannel(client);

                if (result.Success)
                {
                    TxtMessage.Foreground = new SolidColorBrush(Color.FromRgb(0x00, 0xFF, 0x41));
                    TxtMessage.Text = "> Cuenta creada! Ya puede iniciar sesion.";
                    TxtFullName.IsEnabled = false;
                    DpBirthDate.IsEnabled = false;
                    TxtPhone.IsEnabled = false;
                    TxtEmail.IsEnabled = false;
                    PbPassword.IsEnabled = false;
                    PbConfirm.IsEnabled = false;
                }
                else
                {
                    TxtMessage.Text = $"! {result.Message}";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Register] Error: {ex.Message}");
                TxtMessage.Text = "! Error al conectar con el servidor.";
            }
        }
    }
}
