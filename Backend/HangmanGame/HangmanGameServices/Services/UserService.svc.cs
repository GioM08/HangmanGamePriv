using HangmanGameBusiness.Localization;
using HangmanGameBusiness.Users;
using HangmanGameEntities.Dtos;
using HangmanGameServices.Localization;

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
            SetLanguage();
            return userBusiness.RegisterUser(registerUserDto);
        }

        public OperationResultDto Login(LoginDto loginDto)
        {
            SetLanguage();
            return userBusiness.Login(loginDto);
        }

        public OperationResultDto GetUserProfile(int userId)
        {
            SetLanguage();
            return userBusiness.GetUserProfile(userId);
        }

        public OperationResultDto UpdateUserProfile(UpdateUserProfileDto updateUserProfileDto)
        {
            SetLanguage();
            return userBusiness.UpdateUserProfile(updateUserProfileDto);
        }

        private static void SetLanguage()
        {
            LanguageContext.SetLanguage(RequestLanguageReader.GetLanguageCode());
        }
    }
}
