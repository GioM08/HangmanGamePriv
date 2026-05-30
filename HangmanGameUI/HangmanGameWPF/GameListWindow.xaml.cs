using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using HangmanGameWPF.Localization;
using HangmanGameWPF.Services;

namespace HangmanGameWPF
{
    public partial class GameListWindow : Window
    {
        private List<GameEntry> _games = new List<GameEntry>();

        public GameListWindow()
        {
            InitializeComponent();
            LoadGames();
        }

        private void LoadGames()
        {
            try
            {
                var client = ServiceClientFactory.CreateCategoryClient();
                ServiceClientFactory.CloseChannel(client);

                var gameClient = ServiceClientFactory.CreateUserClient();
                ServiceClientFactory.CloseChannel(gameClient);

                // Use the factory method to get available games via a temp channel
                var ch = CreateGameChannelTemp();
                if (ch != null)
                {
                    var result = ch.GetAvailableGames();
                    ServiceClientFactory.CloseChannel(ch);

                    _games.Clear();
                    if (result.Success && result.Games != null)
                    {
                        foreach (var g in result.Games)
                        {
                            _games.Add(new GameEntry(
                                g.GameId,
                                g.CreatorName,
                                g.CreatorEmail,
                                g.Category,
                                g.Description ?? string.Empty,
                                g.WordLength,
                                g.CreatedAt.ToString("dd/MM/yyyy")));
                        }
                    }
                }
                GameGrid.ItemsSource = _games;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[GameList] Error: {ex.Message}");
                GameGrid.ItemsSource = new List<GameEntry>();
            }
        }

        private IGameService CreateGameChannelTemp()
        {
            try
            {
                // Dummy callback handler just to open the channel for read-only operations
                var dummy = new DummyGameCallback();
                return ServiceClientFactory.CreateGameClient(dummy);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[GameList] Channel error: {ex.Message}");
                return null;
            }
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();
        private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();
        private void BtnMinimize_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        private void BtnNewGame_Click(object sender, RoutedEventArgs e)
        {
            new CreateGameWindow { Owner = this }.ShowDialog();
            LoadGames();
        }

        private void BtnPlay_Click(object sender, RoutedEventArgs e)
        {
            if (GameGrid.SelectedItem is GameEntry selected)
            {
                try
                {
                    var dummy = new DummyGameCallback();
                    var ch = ServiceClientFactory.CreateGameClient(dummy);
                    var result = ch.JoinGame(selected.GameId, SessionManager.UserId);
                    ServiceClientFactory.CloseChannel(ch);

                    if (result.Success && result.Game != null)
                    {
                        new GameWindow(result.Game, false).Show();
                        Close();
                    }
                    else
                    {
                        MessageBox.Show(result.Message ?? ClientLocalizer.Get("ERROR_JOIN_GAME_FALLBACK"), "ERROR",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[GameList] Join error: {ex.Message}");
                    MessageBox.Show(ClientLocalizer.Get("ERROR_JOIN_GAME"), "ERROR", MessageBoxButton.OK);
                }
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            Close();
        }
    }

    public class GameEntry
    {
        public int GameId { get; }
        public string Nombre { get; }
        public string Correo { get; }
        public string Categoria { get; }
        public string Descripcion { get; }
        public string Letras { get; }
        public string Fecha { get; }

        public GameEntry(int id, string nombre, string correo, string cat, string desc, int len, string fecha)
        {
            GameId = id; Nombre = nombre; Correo = correo; Categoria = cat;
            Descripcion = desc; Letras = len.ToString(); Fecha = fecha;
        }
    }

}
