using System;
using HangmanGameData.Repositories;
using HangmanGameEntities.Dtos;

namespace HangmanGameBusiness.Categories
{
    public class CategoryBusiness : ICategoryBusiness
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryBusiness()
        {
            _categoryRepository = new CategoryRepository();
        }

        public CategoryBusiness(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public GameOperationResultDto GetAllCategories(string languageCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(languageCode))
                    languageCode = "ES";

                var categories = _categoryRepository.GetAllCategories(languageCode);
                return new GameOperationResultDto { Success = true, Categories = categories };
            }
            catch (Exception)
            {
                return new GameOperationResultDto { Success = false, Message = "Error al obtener categorias." };
            }
        }

        public GameOperationResultDto GetWordsByCategory(int categoryId, string languageCode)
        {
            try
            {
                if (categoryId <= 0)
                    return new GameOperationResultDto { Success = false, Message = "Categoria invalida." };

                if (string.IsNullOrWhiteSpace(languageCode))
                    languageCode = "ES";

                var words = _categoryRepository.GetWordsByCategory(categoryId, languageCode);
                return new GameOperationResultDto { Success = true, Words = words };
            }
            catch (Exception)
            {
                return new GameOperationResultDto { Success = false, Message = "Error al obtener palabras." };
            }
        }
    }
}
