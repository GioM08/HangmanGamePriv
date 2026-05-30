using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using HangmanGameWPF.Localization;
using HangmanGameWPF.Services;

namespace HangmanGameWPF
{
    public partial class GameWindow : Window, IGameCallback
    {
        private readonly int _gameId;
        private readonly bool _isCreator;
        private readonly GameDto _gameInfo;
        private IGameService _gameChannel;
        private readonly ChatClient _chatClient;
        private GameStateDto _lastGameState;

        // Hangman ASCII stages (0–6 incorrect attempts)
        private static readonly string[] HangmanStages =
        {
            // 0
            "  +---+\n  |   |\n      |\n      |\n      |\n      |\n=========",
            // 1
            "  +---+\n  |   |\n  O   |\n      |\n      |\n      |\n=========",
            // 2
            "  +---+\n  |   |\n  O   |\n  |   |\n      |\n      |\n=========",
            // 3
            "  +---+\n  |   |\n  O   |\n /|   |\n      |\n      |\n=========",
            // 4
            "  +---+\n  |   |\n  O   |\n /|\\  |\n      |\n      |\n=========",
            // 5
            "  +---+\n  |   |\n  O   |\n /|\\  |\n /    |\n      |\n=========",
            // 6
            "  +---+\n  |   |\n  O   |\n /|\\  |\n / \\  |\n      |\n========="
        };

        public GameWindow(GameDto game, bool isCreator)
        {
            InitializeComponent();
            _gameId    = game.GameId;
            _isCreator = isCreator;
            _gameInfo  = game;
            _chatClient = new ChatClient();

            SelectCurrentLanguage();
            TxtPlayer.Text      = SessionManager.FullName ?? ClientLocalizer.Get("PLAYER_FALLBACK");
            TxtRole.Text        = isCreator
                ? ClientLocalizer.Get("GAME_ROLE_CREATOR")
                : ClientLocalizer.Get("GAME_ROLE_CHALLENGER");
            TxtCategory.Text    = game.Category ?? string.Empty;
            TxtDescription.Text = game.Description ?? string.Empty;

            Loaded  += OnLoaded;
            Closing += OnWindowClosing;
        }

        private void SelectCurrentLanguage()
        {
            foreach (ComboBoxItem item in CmbLanguage.Items)
            {
                if ((item.Tag as string) == ClientLanguageContext.CurrentLanguage)
                {
                    CmbLanguage.SelectedItem = item;
                    return;
                }
            }
        }

        private void CmbLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selectedItem = CmbLanguage.SelectedItem as ComboBoxItem;

            if (selectedItem == null)
            {
                return;
            }

            ClientLanguageContext.SetLanguage(selectedItem.Tag as string);
            TxtRole.Text = _isCreator
                ? ClientLocalizer.Get("GAME_ROLE_CREATOR")
                : ClientLocalizer.Get("GAME_ROLE_CHALLENGER");

            if (_lastGameState != null)
            {
                UpdateLocalizedStateTexts(_lastGameState);
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            ConnectToGame();
            ConnectToChat();
        }

        private void ConnectToGame()
        {
            try
            {
                _gameChannel = ServiceClientFactory.CreateGameClient(this);

                GameOperationResultDto result;

                using (ServiceCallContext.CreateScope(_gameChannel))
                {
                    _gameChannel.RegisterForGame(_gameId, SessionManager.UserId);
                }

                using (ServiceCallContext.CreateScope(_gameChannel))
                {
                    result = _gameChannel.GetGameState(_gameId);
                }

                if (result.Success && result.GameState != null)
                    UpdateGameUI(result.GameState);
                else if (_isCreator)
                    TxtMessage.Text = ClientLocalizer.Get("GAME_CREATED_WAITING");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[GameWindow] Connect error: {ex.Message}");
                AppendChat(ClientLocalizer.Get("GAME_SYSTEM"), ClientLocalizer.Get("ERROR_CONNECTION"));
            }
        }

        private void ConnectToChat()
        {
            _chatClient.MessageReceived += OnChatMessageReceived;
            _chatClient.Connect(_gameId, SessionManager.UserId, SessionManager.FullName ?? ClientLocalizer.Get("PLAYER_FALLBACK"));
        }

        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _chatClient?.Disconnect();
            if (_gameChannel != null)
                ServiceClientFactory.CloseChannel(_gameChannel);
        }

        // ── IGameCallback implementation ────────────────────────────────────

        public void OnGameStateChanged(GameStateDto gameState)
        {
            Dispatcher.BeginInvoke(new Action(() => UpdateGameUI(gameState)));
        }

        public void OnPlayerJoined(GameDto game)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                AppendChat(
                    ClientLocalizer.Get("GAME_SYSTEM"),
                    string.Format(ClientLocalizer.Get("GAME_PLAYER_JOINED_CHAT"), game.RetadorName));
                TxtMessage.Text = "> " + string.Format(ClientLocalizer.Get("GAME_PLAYER_JOINED"), game.RetadorName);
            }));
        }

        public void OnGameEnded(GameStateDto gameState)
        {
            Dispatcher.BeginInvoke(new Action(() => UpdateGameUI(gameState)));
        }

        // ── Game UI ─────────────────────────────────────────────────────────

        private void UpdateGameUI(GameStateDto state)
        {
            _lastGameState = state;
            TxtRevealedWord.Text = state.RevealedWord ?? string.Empty;
            UpdateLocalizedStateTexts(state);

            int stageIdx = Math.Min(state.IncorrectAttempts, 6);
            TxtHangman.Text = HangmanStages[stageIdx];

            if (state.UsedLetters != null)
            {
                var wrong = state.UsedLetters
                    .Where(l => state.RevealedWord == null || !state.RevealedWord.Contains(l))
                    .OrderBy(l => l);
                TxtWrongLetters.Text = string.Join("  ", wrong);
            }

            ApplyKeyboardState(state);

            if (!string.IsNullOrEmpty(state.Message))
                TxtMessage.Text = state.Message;

            if (state.IsOver && !_gameEnded)
            {
                _gameEnded = true;
                ShowGameResult(state);
            }
        }

        private bool _gameEnded;

        private void UpdateLocalizedStateTexts(GameStateDto state)
        {
            int wordLength = state.RevealedWord?.Replace(" ", "").Length ?? 0;
            TxtWordLength.Text = string.Format(ClientLocalizer.Get("GAME_LENGTH"), wordLength);
            TxtLives.Text = string.Format(ClientLocalizer.Get("GAME_LIVES"), 6 - state.IncorrectAttempts);
        }

        // Un solo método que aplica el estado visual completo del teclado
        private void ApplyKeyboardState(GameStateDto state)
        {
            var used = state.UsedLetters ?? new List<string>();
            string revealed = state.RevealedWord ?? string.Empty;
            bool gameOver = state.IsOver;

            Style normalStyle = (Style)FindResource("KeyboardKey");
            Style hitStyle    = (Style)FindResource("KeyboardKeyHit");

            foreach (var btn in GetKeyButtons())
            {
                string letter = btn.Tag?.ToString() ?? string.Empty;
                bool isUsed    = used.Contains(letter);
                bool isCorrect = isUsed && revealed.Contains(letter);

                if (isCorrect)
                {
                    // Verde relleno, deshabilitado para no re-seleccionar
                    btn.Style     = hitStyle;
                    btn.IsEnabled = false;
                }
                else if (isUsed)
                {
                    // Incorrecto: estilo normal con IsEnabled=false dispara el trigger rojo oscuro
                    btn.Style     = normalStyle;
                    btn.IsEnabled = false;
                }
                else
                {
                    // Disponible: verde, IsEnabled controla si el retador puede hacer click
                    btn.Style = normalStyle;
                    if (_isCreator)
                    {
                        // Creador ve verde pero no puede clickear (IsHitTestVisible=false)
                        btn.IsEnabled        = true;
                        btn.IsHitTestVisible = false;
                    }
                    else
                    {
                        btn.IsEnabled        = !gameOver;
                        btn.IsHitTestVisible = true;
                    }
                }
            }
        }

        private void ShowGameResult(GameStateDto state)
        {
            if (!IsLoaded || !IsVisible) return;

            string title = state.WinnerId == SessionManager.UserId
                ? ClientLocalizer.Get("GAME_WIN_TITLE")
                : ClientLocalizer.Get("GAME_LOSE_TITLE");
            string msg    = state.Message ?? string.Empty;
            int    points = state.WinnerId == SessionManager.UserId ? 10 : 0;

            try
            {
                var dlg = new GameResultWindow(title, msg, points) { Owner = this };
                dlg.ShowDialog();
            }
            catch (InvalidOperationException) { }

            new GameListWindow().Show();
            Close();
        }

        private IEnumerable<Button> GetKeyButtons()
        {
            return new[] {
                BtnQ,BtnW,BtnE,BtnR,BtnT,BtnY,BtnU,BtnI,BtnO,BtnP,
                BtnA,BtnS,BtnD,BtnF,BtnG,BtnH,BtnJ,BtnK,BtnL,
                BtnZ,BtnX,BtnC,BtnV,BtnB,BtnN,BtnM
            };
        }

        private void KeyButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isCreator) return;
            var btn = (Button)sender;
            string letter = btn.Tag?.ToString() ?? string.Empty;
            GuessLetter(letter);
        }

        private void GuessLetter(string letter)
        {
            if (_gameChannel == null) return;

            // Deshabilitar todo el teclado durante la llamada para evitar doble click
            foreach (var btn in GetKeyButtons())
                btn.IsHitTestVisible = false;

            // Llamada WCF en hilo de fondo para no bloquear el UI thread
            System.Threading.ThreadPool.QueueUserWorkItem(_ =>
            {
                try
                {
                    GameOperationResultDto result;

                    using (ServiceCallContext.CreateScope(_gameChannel))
                    {
                        result = _gameChannel.GuessLetter(new GuessLetterDto
                        {
                            GameId = _gameId,
                            UserId = SessionManager.UserId,
                            Letter = letter
                        });
                    }

                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        if (result != null && result.Success && result.GameState != null)
                            UpdateGameUI(result.GameState);
                        else if (result != null)
                            TxtMessage.Text = result.Message ?? ClientLocalizer.Get("ERROR_PROCESSING");
                    }));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[GameWindow] GuessLetter error: {ex.Message}");
                }
            });
        }

        // ── Chat ─────────────────────────────────────────────────────────────

        private void OnChatMessageReceived(string msg)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (msg.StartsWith("MSG:"))
                {
                    var parts = msg.Split(new[] { ':' }, 5);
                    string sender = parts.Length >= 3 ? parts[2] : "?";
                    string text = parts.Length >= 5 ? parts[4] : msg;
                    AppendChat(sender, text);
                }
                else if (msg.StartsWith("SYSTEM:"))
                {
                    AppendChat(ClientLocalizer.Get("GAME_SYSTEM"), msg.Substring(7));
                }
                else
                {
                    AppendChat("?", msg);
                }
            }));
        }

        private void AppendChat(string sender, string text)
        {
            TxtChat.Text += $"[{sender}] {text}\n";
            ChatScroll.ScrollToEnd();
        }

        private void TxtChatInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) SendChatMessage();
        }

        private void BtnSendChat_Click(object sender, RoutedEventArgs e) => SendChatMessage();

        private void SendChatMessage()
        {
            string text = TxtChatInput.Text.Trim();
            if (string.IsNullOrEmpty(text)) return;

            _chatClient.SendMessage(SessionManager.UserId, SessionManager.FullName ?? ClientLocalizer.Get("PLAYER_FALLBACK"),
                _gameId, text);
            AppendChat("YO", text);
            TxtChatInput.Clear();
        }

        // ── Window chrome ────────────────────────────────────────────────────

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            if (!_gameEnded)
            {
                var r = MessageBox.Show(
                    ClientLocalizer.Get("GAME_WINDOW_CLOSE_CONFIRM"),
                    ClientLocalizer.Get("ATTENTION_TITLE"),
                    MessageBoxButton.YesNo);
                if (r != MessageBoxResult.Yes) return;
            }
            Close();
        }

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
            => WindowState = WindowState.Minimized;

        private void BtnSurrender_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                ClientLocalizer.Get("GAME_SURRENDER_CONFIRM"),
                ClientLocalizer.Get("GAME_SURRENDER_TITLE"),
                MessageBoxButton.YesNo);

            if (result != MessageBoxResult.Yes) return;

            try
            {
                if (_gameChannel != null)
                {
                    using (ServiceCallContext.CreateScope(_gameChannel))
                    {
                        _gameChannel.AbandonGame(_gameId, SessionManager.UserId);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[GameWindow] Abandon error: {ex.Message}");
            }

            _gameEnded = true;
            new GameListWindow().Show();
            Close();
        }
    }
}
