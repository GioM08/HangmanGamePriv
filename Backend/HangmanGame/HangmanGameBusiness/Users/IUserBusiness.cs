using HangmanGameEntities.Dtos;

namespace HangmanGameBusiness.Users
{
    public interface IUserBusiness
    {
        OperationResultDto RegisterUser(RegisterUserDto registerUserDto);

        OperationResultDto Login(LoginDto loginDto);

        OperationResultDto GetUserProfile(int userId);

        OperationResultDto UpdateUserProfile(UpdateUserProfileDto updateUserProfileDto);
    }
}