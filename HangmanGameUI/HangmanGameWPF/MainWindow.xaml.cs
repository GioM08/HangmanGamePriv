using System.Windows;
using System.Windows.Input;
using HangmanGameWPF.Localization;

namespace HangmanGameWPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            string name = SessionManager.FullName?.ToUpper() ?? ClientLocalizer.Get("PLAYER_FALLBACK");

            TxtWelcome.Text = string.Format(ClientLocalizer.Get("MAIN_WELCOME"), name);
            TxtTitleBar.Text = string.Format(ClientLocalizer.Get("MAIN_TITLE_BAR"), name);
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            => DragMove();

        private void BtnClose_Click(object sender, RoutedEventArgs e)
            => Close();

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
            => WindowState = WindowState.Minimized;

        private void BtnGameList_Click(object sender, RoutedEventArgs e)
        {
            new GameListWindow().Show();
            Close();
        }

        private void BtnScore_Click(object sender, RoutedEventArgs e)
            => new ScoreWindow { Owner = this }.ShowDialog();

        private void BtnProfile_Click(object sender, RoutedEventArgs e)
            => new ProfileWindow { Owner = this }.ShowDialog();

        private void BtnFriends_Click(object sender, RoutedEventArgs e)
            => new FriendsWindow { Owner = this }.ShowDialog();

        private void BtnLeaderboard_Click(object sender, RoutedEventArgs e)
            => new LeaderboardWindow { Owner = this }.ShowDialog();

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            SessionManager.Logout();
            new LoginWindow().Show();
            Close();
        }
    }
}
