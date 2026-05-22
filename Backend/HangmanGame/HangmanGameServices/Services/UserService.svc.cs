using HangmanGameBusiness.Users;
using HangmanGameEntities.Dtos;

namespace HangmanGameServices.Services
{
    public class UserService : IUserService
    {
        private readonly IUserBusiness userBusiness;

        public UserService()
        {
            this.userBusiness = new UserBusiness();
        }

        public OperationResultDto RegisterUser(RegisterUserDto registerUserDto)
        {
            return userBusiness.RegisterUser(registerUserDto);
        }

        public OperationResultDto Login(LoginDto loginDto)
        {
            return userBusiness.Login(loginDto);
        }

        public OperationResultDto GetUserProfile(int userId)
        {
            return userBusiness.GetUserProfile(userId);
        }

        public OperationResultDto UpdateUserProfile(UpdateUserProfileDto updateUserProfileDto)
        {
            return userBusiness.UpdateUserProfile(updateUserProfileDto);
        }
    }
}