using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using HangmanGameWPF.Services;

namespace HangmanGameWPF
{
    public partial class ScoreWindow : Window
    {
        public ScoreWindow()
        {
            InitializeComponent();
            LoadScore();
        }

        private void LoadScore()
        {
            try
            {
                var dummy = new DummyGameCallback();
                var ch = ServiceClientFactory.CreateGameClient(dummy);
                var result = ch.GetUserScore(SessionManager.UserId);
                ServiceClientFactory.CloseChannel(ch);

                if (result.Success && result.UserScore != null)
                {
                    var score = result.UserScore;
                    TxtTotal.Text = score.GamesPlayed.ToString();
                    TxtWon.Text = score.GamesWon.ToString();
                    TxtLost.Text = score.GamesLost.ToString();
                    TxtPoints.Text = score.TotalPoints.ToString();

                    var entries = new List<ScoreEntry>();

                    if (score.WonGames != null)
                        foreach (var g in score.WonGames)
                            entries.Add(new ScoreEntry(g.Word, g.OpponentName, "GANADO",
                                g.PointsEarned, g.GameDate.ToString("dd/MM/yy")));

                    if (score.LostGames != null)
                        foreach (var g in score.LostGames)
                            entries.Add(new ScoreEntry(g.Word, g.OpponentName, "PERDIDO",
                                g.PointsEarned, g.GameDate.ToString("dd/MM/yy")));

                    if (score.Penalties != null)
                        foreach (var p in score.Penalties)
                            entries.Add(new ScoreEntry(p.Word, p.Reason, "PENALIZADO",
                                -p.PointsDeducted, p.PenaltyDate.ToString("dd/MM/yy")));

                    ScoreGrid.ItemsSource = entries;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Score] Load error: {ex.Message}");
                ScoreGrid.ItemsSource = new List<ScoreEntry>();
            }
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();
        private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();
        private void BtnMinimize_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;
        private void BtnBack_Click(object sender, RoutedEventArgs e) => Close();
    }

    public class ScoreEntry
    {
        public string Palabra { get; }
        public string Rival { get; }
        public string Resultado { get; }
        public int Puntos { get; }
        public string Fecha { get; }

        public ScoreEntry(string palabra, string rival, string resultado, int puntos, string fecha)
        {
            Palabra = palabra; Rival = rival; Resultado = resultado; Puntos = puntos; Fecha = fecha;
        }
    }
}
