using HangmanGameBusiness.Categories;
using HangmanGameEntities.Dtos;

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
            return _categoryBusiness.GetAllCategories(languageCode);
        }

        public GameOperationResultDto GetWordsByCategory(int categoryId, string languageCode)
        {
            return _categoryBusiness.GetWordsByCategory(categoryId, languageCode);
        }
    }
}
