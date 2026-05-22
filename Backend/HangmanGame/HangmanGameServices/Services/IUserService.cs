using System.ServiceModel;
using HangmanGameEntities.Dtos;

namespace HangmanGameServices.Services
{
    [ServiceContract]
    public interface IUserService
    {
        [OperationContract]
        OperationResultDto RegisterUser(RegisterUserDto registerUserDto);

        [OperationContract]
        OperationResultDto Login(LoginDto loginDto);

        [OperationContract]
        OperationResultDto GetUserProfile(int userId);

        [OperationContract]
        OperationResultDto UpdateUserProfile(UpdateUserProfileDto updateUserProfileDto);
    }
}