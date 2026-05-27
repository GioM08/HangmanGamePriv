using System;
using System.Windows;
using System.Windows.Input;
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
                    SetStatus("No se recibio respuesta del servidor.");
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
                        $"#{result.CurrentPlayerRank.Position}  " +
                        $"{result.CurrentPlayerRank.FullName}  |  " +
                        $"Score: {result.CurrentPlayerRank.GlobalScore}";
                }
                else
                {
                    TxtCurrentPlayer.Text = "No se encontro tu posicion en el ranking.";
                }

                SetStatus(result.Message);
            }
            catch (Exception ex)
            {
                SetStatus("Error cargando leaderboard: " + ex.Message);
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
