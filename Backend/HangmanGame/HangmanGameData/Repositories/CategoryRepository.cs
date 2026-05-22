using System.Collections.Generic;
using System.Linq;
using HangmanGameEntities.Dtos;

namespace HangmanGameData.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        public List<CategoryDto> GetAllCategories(string languageCode)
        {
            using (var db = new HangmanGameDataContext())
            {
                return db.Categories
                    .Where(c => c.LanguageCode == languageCode)
                    .Select(c => new CategoryDto
                    {
                        CategoryId = c.CategoryId,
                        Name = c.Name,
                        LanguageCode = c.LanguageCode
                    })
                    .ToList();
            }
        }

        public List<WordDto> GetWordsByCategory(int categoryId, string languageCode)
        {
            using (var db = new HangmanGameDataContext())
            {
                var category = db.Categories.FirstOrDefault(c => c.CategoryId == categoryId);
                string categoryName = category?.Name ?? string.Empty;

                return db.Words
                    .Where(w => w.CategoryId == categoryId && w.LanguageCode == languageCode)
                    .Select(w => new WordDto
                    {
                        WordId = w.WordId,
                        Text = w.Text,
                        Hint = w.Hint,
                        CategoryId = w.CategoryId,
                        CategoryName = categoryName,
                        Difficulty = w.Difficulty
                    })
                    .ToList();
            }
        }
    }
}
