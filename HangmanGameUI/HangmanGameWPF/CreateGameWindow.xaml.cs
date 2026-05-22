using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HangmanGameWPF.Services;

namespace HangmanGameWPF
{
    public partial class CreateGameWindow : Window
    {
        private List<CategoryDto> _categories = new List<CategoryDto>();
        private List<WordDto> _words = new List<WordDto>();

        public CreateGameWindow()
        {
            InitializeComponent();
            LoadCategories();
        }

        private void LoadCategories()
        {
            try
            {
                var client = ServiceClientFactory.CreateCategoryClient();
                var result = client.GetAllCategories("ES");
                ServiceClientFactory.CloseChannel(client);

                if (result.Success && result.Categories != null)
                {
                    _categories = result.Categories;
                    CbCategory.ItemsSource = _categories;
                    CbCategory.DisplayMemberPath = "Name";
                    if (_categories.Count > 0)
                        CbCategory.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[CreateGame] Load categories error: {ex.Message}");
                TxtError.Text = "! Error al cargar categorias del servidor.";
            }
        }

        private void CbCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CbCategory.SelectedItem is CategoryDto selected)
                LoadWords(selected.CategoryId);
        }

        private void LoadWords(int categoryId)
        {
            try
            {
                var client = ServiceClientFactory.CreateCategoryClient();
                var result = client.GetWordsByCategory(categoryId, "ES");
                ServiceClientFactory.CloseChannel(client);

                if (result.Success && result.Words != null)
                {
                    _words = result.Words;
                    CbWord.ItemsSource = _words;
                    CbWord.DisplayMemberPath = "Text";
                    CbWord.SelectedIndex = _words.Count > 0 ? 0 : -1;
                    UpdateHint();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[CreateGame] Load words error: {ex.Message}");
            }
        }

        private void UpdateHint()
        {
            if (CbWord.SelectedItem is WordDto word && !string.IsNullOrEmpty(word.Hint))
                TxtHint.Text = $"> Pista: {word.Hint}";
            else
                TxtHint.Text = string.Empty;
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();
        private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();
        private void BtnCancel_Click(object sender, RoutedEventArgs e) => Close();

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            TxtError.Text = "";

            if (CbWord.SelectedItem == null)
            {
                TxtError.Text = "! Seleccione una categoria y una palabra.";
                return;
            }

            var selectedWord = (WordDto)CbWord.SelectedItem;

            try
            {
                var dummy = new DummyGameCallback();
                var ch = ServiceClientFactory.CreateGameClient(dummy);
                var result = ch.CreateGame(new CreateGameDto
                {
                    CreatorId = SessionManager.UserId,
                    WordId = selectedWord.WordId,
                    Description = TxtDescription.Text.Trim()
                });
                ServiceClientFactory.CloseChannel(ch);

                if (result.Success && result.Game != null)
                {
                    Close();
                    new GameWindow(result.Game, true).Show();
                    // Close the GameListWindow owner
                    (Owner as GameListWindow)?.Close();
                }
                else
                {
                    TxtError.Text = $"! {result.Message}";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[CreateGame] Create error: {ex.Message}");
                TxtError.Text = "! Error al crear la partida.";
            }
        }
    }
}
