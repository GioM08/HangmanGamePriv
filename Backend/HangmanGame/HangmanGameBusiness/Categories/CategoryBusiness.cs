using System;
using HangmanGameBusiness.Localization;
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
                languageCode = NormalizeLanguageCode(languageCode);

                var categories = _categoryRepository.GetAllCategories(languageCode);
                return new GameOperationResultDto
                {
                    Success = true,
                    MessageKey = MessageKeys.CategoriesRetrievedSuccessfully,
                    Message = MessageLocalizer.Get(MessageKeys.CategoriesRetrievedSuccessfully),
                    Categories = categories
                };
            }
            catch (Exception)
            {
                return Fail(MessageKeys.UnexpectedError);
            }
        }

        public GameOperationResultDto GetWordsByCategory(int categoryId, string languageCode)
        {
            try
            {
                if (categoryId <= 0)
                    return Fail(MessageKeys.CategoryInvalid);

                languageCode = NormalizeLanguageCode(languageCode);

                var words = _categoryRepository.GetWordsByCategory(categoryId, languageCode);
                return new GameOperationResultDto
                {
                    Success = true,
                    MessageKey = MessageKeys.WordsRetrievedSuccessfully,
                    Message = MessageLocalizer.Get(MessageKeys.WordsRetrievedSuccessfully),
                    Words = words
                };
            }
            catch (Exception)
            {
                return Fail(MessageKeys.UnexpectedError);
            }
        }

        private static string NormalizeLanguageCode(string languageCode)
        {
            if (string.IsNullOrWhiteSpace(languageCode))
                return "ES";

            string code = languageCode.Trim();
            int dashIndex = code.IndexOf('-');
            if (dashIndex > 0)
                code = code.Substring(0, dashIndex);

            return code.ToUpperInvariant();
        }

        private static GameOperationResultDto Fail(string messageKey)
        {
            return new GameOperationResultDto
            {
                Success = false,
                MessageKey = messageKey,
                Message = MessageLocalizer.Get(messageKey)
            };
        }
    }
}
