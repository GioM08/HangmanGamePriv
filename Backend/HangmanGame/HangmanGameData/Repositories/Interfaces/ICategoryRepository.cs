using System.Collections.Generic;
using HangmanGameEntities.Dtos;

namespace HangmanGameData.Repositories
{
    public interface ICategoryRepository
    {
        List<CategoryDto> GetAllCategories(string languageCode);
        List<WordDto> GetWordsByCategory(int categoryId, string languageCode);
    }
}
