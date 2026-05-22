using System;
using System.Linq;
using HangmanGameEntities.Dtos;

namespace HangmanGameData.Repositories
{
    public class UserRepository : IUserRepository
    {
        public bool EmailExists(string email)
        {
            using (var database = new HangmanGameDataContext())
            {
                return database.Users.Any(user => user.Email == email);
            }
        }

        public UserDto CreateUser(RegisterUserDto registerUserDto, string passwordHash)
        {
            using (var database = new HangmanGameDataContext())
            {
                Users newUser = new Users();

                newUser.FullName = registerUserDto.FullName;
                newUser.BirthDate = registerUserDto.BirthDate;
                newUser.PhoneNumber = registerUserDto.PhoneNumber;
                newUser.Email = registerUserDto.Email;
                newUser.PasswordHash = passwordHash;
                newUser.GlobalScore = 0;
                newUser.CreatedAt = DateTime.Now;
                newUser.IsActive = true;

                database.Users.InsertOnSubmit(newUser);
                database.SubmitChanges();

                return MapToUserDto(newUser);
            }
        }

        public UserDto GetActiveUserByEmail(string email)
        {
            using (var database = new HangmanGameDataContext())
            {
                Users userFound = database.Users
                    .FirstOrDefault(user => user.Email == email && user.IsActive == true);

                if (userFound == null)
                {
                    return null;
                }

                return MapToUserDto(userFound);
            }
        }

        public string GetPasswordHashByEmail(string email)
        {
            using (var database = new HangmanGameDataContext())
            {
                Users userFound = database.Users
                    .FirstOrDefault(user => user.Email == email && user.IsActive == true);

                if (userFound == null)
                {
                    return null;
                }

                return userFound.PasswordHash;
            }
        }

        private UserDto MapToUserDto(Users user)
        {
            return new UserDto
            {
                UserId = user.UserId,
                FullName = user.FullName,
                BirthDate = user.BirthDate,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                GlobalScore = user.GlobalScore,
                CreatedAt = user.CreatedAt
            };
        }

        public UserDto GetUserById(int userId)
        {
            using (var database = new HangmanGameDataContext())
            {
                Users userFound = database.Users
                    .FirstOrDefault(user => user.UserId == userId && user.IsActive == true);

                if (userFound == null)
                {
                    return null;
                }

                return MapToUserDto(userFound);
            }
        }

        public UserDto UpdateUserProfile(UpdateUserProfileDto updateUserProfileDto)
        {
            using (var database = new HangmanGameDataContext())
            {
                Users userFound = database.Users
                    .FirstOrDefault(user => user.UserId == updateUserProfileDto.UserId && user.IsActive == true);

                if (userFound == null)
                {
                    return null;
                }

                userFound.FullName = updateUserProfileDto.FullName;
                userFound.BirthDate = updateUserProfileDto.BirthDate;
                userFound.PhoneNumber = updateUserProfileDto.PhoneNumber;
                userFound.UpdatedAt = DateTime.Now;

                database.SubmitChanges();

                return MapToUserDto(userFound);
            }
        }


    }
}