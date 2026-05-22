using HangmanGameEntities.Dtos;

namespace HangmanGameBusiness.Categories
{
    public interface ICategoryBusiness
    {
        GameOperationResultDto GetAllCategories(string languageCode);
        GameOperationResultDto GetWordsByCategory(int categoryId, string languageCode);
    }
}
