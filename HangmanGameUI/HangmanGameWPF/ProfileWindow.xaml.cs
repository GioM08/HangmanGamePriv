using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using HangmanGameWPF.Localization;
using HangmanGameWPF.Services;

namespace HangmanGameWPF
{
    public partial class ProfileWindow : Window
    {
        public ProfileWindow()
        {
            InitializeComponent();
            LoadProfile();
            LoadStats();
        }

        private void LoadProfile()
        {
            try
            {
                var client = ServiceClientFactory.CreateUserClient();
                var result = client.GetUserProfile(SessionManager.UserId);
                ServiceClientFactory.CloseChannel(client);

                if (result.Success && result.User != null)
                {
                    TxtUsername.Text = result.User.FullName.ToUpper();
                    TxtEmail.Text    = result.User.Email;
                    UpdateEmailVerificationStatus(result.User.IsEmailVerified);
                    SessionManager.SetUser(result.User.UserId, result.User.FullName,
                        result.User.Email, result.User.GlobalScore);
                }
                else
                {
                    TxtUsername.Text = SessionManager.FullName?.ToUpper() ?? ClientLocalizer.Get("PLAYER_FALLBACK");
                    TxtEmail.Text    = SessionManager.Email ?? string.Empty;
                    UpdateEmailVerificationStatus(false);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Profile] Load error: {ex.Message}");
                TxtUsername.Text = SessionManager.FullName?.ToUpper() ?? ClientLocalizer.Get("PLAYER_FALLBACK");
                TxtEmail.Text    = SessionManager.Email ?? string.Empty;
                UpdateEmailVerificationStatus(false);
            }
        }

        private void LoadStats()
        {
            try
            {
                var dummy = new DummyGameCallback();
                var ch    = ServiceClientFactory.CreateGameClient(dummy);
                var result = ch.GetUserScore(SessionManager.UserId);
                ServiceClientFactory.CloseChannel(ch);

                if (result.Success && result.UserScore != null)
                {
                    var s = result.UserScore;
                    int played = s.GamesPlayed;
                    int won    = s.GamesWon;
                    int pct    = played > 0 ? (int)Math.Round((double)won / played * 100) : 0;

                    TxtGamesPlayed.Text  = played.ToString();
                    TxtGamesWon.Text     = $"{won} / {played}  ";
                    TxtWinRate.Text      = $"({pct}%)";
                    TxtProgressLabel.Text = string.Format(ClientLocalizer.Get("PROGRESS_LABEL"), pct);
                    RectProgress.Width   = (int)(360 * pct / 100.0);
                }
                else
                {
                    TxtGamesPlayed.Text  = "0";
                    TxtGamesWon.Text     = "0 / 0  ";
                    TxtWinRate.Text      = "(0%)";
                    TxtProgressLabel.Text = string.Format(ClientLocalizer.Get("PROGRESS_LABEL"), 0);
                    RectProgress.Width   = 0;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Profile] Stats error: {ex.Message}");
                TxtGamesPlayed.Text = "0";
                TxtGamesWon.Text    = "0 / 0  ";
                TxtWinRate.Text     = "(0%)";
            }
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();
        private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();
        private void BtnMinimize_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;
        private void BtnBack_Click(object sender, RoutedEventArgs e) => Close();

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            new EditProfileWindow { Owner = this }.ShowDialog();
            LoadProfile();
            LoadStats();
        }

        private void BtnVerifyEmail_Click(object sender, RoutedEventArgs e)
        {
            IAccountRecoveryService client = null;

            try
            {
                client = ServiceClientFactory.CreateAccountRecoveryClient();

                EmailOperationResultDto result =
                    client.SendEmailVerificationCode(SessionManager.UserId);

                if (result == null)
                {
                    MessageBox.Show(
                        ClientLocalizer.Get("ERROR_SERVER_EMPTY"),
                        ClientLocalizer.Get("VERIFICATION_TITLE"));
                    return;
                }

                MessageBox.Show(result.Message, ClientLocalizer.Get("VERIFICATION_TITLE"));

                if (result.Success)
                {
                    VerifyEmailWindow verifyWindow = new VerifyEmailWindow(
                        SessionManager.UserId,
                        SessionManager.Email
                    )
                    {
                        Owner = this
                    };

                    verifyWindow.ShowDialog();
                    LoadProfile();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(ClientLocalizer.Get("ERROR_SEND_CODE_PREFIX"), ex.Message),
                    ClientLocalizer.Get("VERIFICATION_TITLE"));
            }
            finally
            {
                ServiceClientFactory.CloseChannel(client);
            }
        }

        private void UpdateEmailVerificationStatus(bool isEmailVerified)
        {
            TxtEmailVerificationStatus.Text = isEmailVerified
                ? ClientLocalizer.Get("EMAIL_VERIFIED_STATUS")
                : ClientLocalizer.Get("EMAIL_PENDING_STATUS");

            BtnVerifyEmail.IsEnabled = !isEmailVerified;
            BtnVerifyEmail.Content = isEmailVerified
                ? ClientLocalizer.Get("VERIFIED_BUTTON")
                : ClientLocalizer.Get("VERIFY_BUTTON");
        }
    }
}
