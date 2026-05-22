using System.Windows;
using System.Windows.Media;

namespace HangmanGameWPF
{
    public partial class GameResultWindow : Window
    {
        public GameResultWindow(string title, string message, int points)
        {
            InitializeComponent();
            TxtTitle.Text = title;
            TxtResultMsg.Text = message;
            TxtPoints.Text = points.ToString();

            if (title.Contains("GANASTE"))
                TxtTitle.Foreground = new SolidColorBrush(Color.FromRgb(0x00, 0xFF, 0x41));
            else
                TxtTitle.Foreground = new SolidColorBrush(Color.FromRgb(0xFF, 0x44, 0x44));
        }

        private void BtnContinue_Click(object sender, RoutedEventArgs e) => Close();
    }
}
