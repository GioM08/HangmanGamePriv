using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HangmanGameWPF.Localization;

namespace HangmanGameWPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            SelectCurrentLanguage();
            ClientLanguageContext.LanguageChanged += ClientLanguageContext_LanguageChanged;

            UpdateLocalizedHeader();
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
        }

        private void ClientLanguageContext_LanguageChanged(object sender, System.EventArgs e)
        {
            SelectCurrentLanguage();
            UpdateLocalizedHeader();
        }

        private void UpdateLocalizedHeader()
        {
            string name = SessionManager.FullName?.ToUpper() ?? ClientLocalizer.Get("PLAYER_FALLBACK");

            TxtWelcome.Text = string.Format(ClientLocalizer.Get("MAIN_WELCOME"), name);
            TxtTitleBar.Text = string.Format(ClientLocalizer.Get("MAIN_TITLE_BAR"), name);
        }

        protected override void OnClosed(System.EventArgs e)
        {
            ClientLanguageContext.LanguageChanged -= ClientLanguageContext_LanguageChanged;
            base.OnClosed(e);
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
