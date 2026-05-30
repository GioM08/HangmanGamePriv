using HangmanGameBusiness.Categories;
using HangmanGameBusiness.Localization;
using HangmanGameEntities.Dtos;
using HangmanGameServices.Localization;

namespace HangmanGameServices.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryBusiness _categoryBusiness;

        public CategoryService()
        {
            _categoryBusiness = new CategoryBusiness();
        }

        public GameOperationResultDto GetAllCategories(string languageCode)
        {
            SetLanguage();
            return _categoryBusiness.GetAllCategories(languageCode);
        }

        public GameOperationResultDto GetWordsByCategory(int categoryId, string languageCode)
        {
            SetLanguage();
            return _categoryBusiness.GetWordsByCategory(categoryId, languageCode);
        }

        private static void SetLanguage()
        {
            LanguageContext.SetLanguage(RequestLanguageReader.GetLanguageCode());
        }
    }
}
