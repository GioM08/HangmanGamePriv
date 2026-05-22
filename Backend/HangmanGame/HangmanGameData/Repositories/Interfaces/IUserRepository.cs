using HangmanGameEntities.Dtos;

namespace HangmanGameData.Repositories
{
    public interface IUserRepository
    {
        bool EmailExists(string email);

        UserDto CreateUser(RegisterUserDto registerUserDto, string passwordHash);

        UserDto GetActiveUserByEmail(string email);

        string GetPasswordHashByEmail(string email);

        UserDto GetUserById(int userId);

        UserDto UpdateUserProfile(UpdateUserProfileDto updateUserProfileDto);
    }
}