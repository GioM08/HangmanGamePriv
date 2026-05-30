using System;
using System.Windows;
using System.Windows.Input;
using HangmanGameWPF.Localization;
using HangmanGameWPF.Services;

namespace HangmanGameWPF
{
    public partial class LeaderboardWindow : Window
    {
        public LeaderboardWindow()
        {
            InitializeComponent();
            LoadLeaderboard();
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            => DragMove();

        private void BtnClose_Click(object sender, RoutedEventArgs e)
            => Close();

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
            => WindowState = WindowState.Minimized;

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadLeaderboard();
        }

        private void LoadLeaderboard()
        {
            ILeaderboardService client = null;

            try
            {
                client = ServiceClientFactory.CreateLeaderboardClient();

                LeaderboardOperationResultDto result =
                    client.GetTopScoreLeaderboard(SessionManager.UserId);

                if (result == null)
                {
                    SetStatus(ClientLocalizer.Get("LEADERBOARD_NO_RESPONSE"));
                    return;
                }

                if (!result.Success)
                {
                    SetStatus(result.Message);
                    return;
                }

                LstTopPlayers.ItemsSource = result.TopPlayers;

                if (result.CurrentPlayerRank != null)
                {
                    TxtCurrentPlayer.Text =
                        string.Format(
                            ClientLocalizer.Get("LEADERBOARD_PLAYER_FORMAT"),
                            result.CurrentPlayerRank.Position,
                            result.CurrentPlayerRank.FullName,
                            result.CurrentPlayerRank.GlobalScore);
                }
                else
                {
                    TxtCurrentPlayer.Text = ClientLocalizer.Get("LEADERBOARD_NOT_FOUND");
                }

                SetStatus(result.Message);
            }
            catch (Exception ex)
            {
                SetStatus(string.Format(ClientLocalizer.Get("LEADERBOARD_LOAD_ERROR"), ex.Message));
            }
            finally
            {
                ServiceClientFactory.CloseChannel(client);
            }
        }

        private void SetStatus(string message)
        {
            TxtStatus.Text = message;
        }
    }
}
