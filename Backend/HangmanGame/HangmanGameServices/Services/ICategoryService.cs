using System.ServiceModel;
using HangmanGameEntities.Dtos;

namespace HangmanGameServices.Services
{
    [ServiceContract]
    public interface ICategoryService
    {
        [OperationContract]
        GameOperationResultDto GetAllCategories(string languageCode);

        [OperationContract]
        GameOperationResultDto GetWordsByCategory(int categoryId, string languageCode);
    }
}
