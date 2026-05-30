using System;
using HangmanGameBusiness.Localization;
using HangmanGameBusiness.Security;
using HangmanGameData.Repositories;
using HangmanGameEntities.Dtos;

namespace HangmanGameBusiness.Users
{
    public class UserBusiness : IUserBusiness
    {
        private readonly IUserRepository userRepository;

        public UserBusiness()
        {
            this.userRepository = new UserRepository();
        }

        public UserBusiness(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public OperationResultDto RegisterUser(RegisterUserDto registerUserDto)
        {
            try
            {
                if (registerUserDto == null)
                {
                    return Fail(MessageKeys.UserDataRequired);
                }

                if (string.IsNullOrWhiteSpace(registerUserDto.FullName))
                {
                    return Fail(MessageKeys.FullNameRequired);
                }

                if (registerUserDto.BirthDate >= DateTime.Today)
                {
                    return Fail(MessageKeys.BirthDateInvalid);
                }

                if (string.IsNullOrWhiteSpace(registerUserDto.PhoneNumber))
                {
                    return Fail(MessageKeys.PhoneNumberRequired);
                }

                if (string.IsNullOrWhiteSpace(registerUserDto.Email))
                {
                    return Fail(MessageKeys.EmailRequired);
                }

                if (string.IsNullOrWhiteSpace(registerUserDto.Password))
                {
                    return Fail(MessageKeys.PasswordRequired);
                }

                if (userRepository.EmailExists(registerUserDto.Email))
                {
                    return Fail(MessageKeys.EmailAlreadyExists);
                }

                string passwordHash = PasswordHasher.HashPassword(registerUserDto.Password);
                UserDto createdUser = userRepository.CreateUser(registerUserDto, passwordHash);

                return Success(MessageKeys.UserRegisteredSuccessfully, createdUser);
            }
            catch (Exception)
            {
                return Fail(MessageKeys.UnexpectedError);
            }
        }

        public OperationResultDto Login(LoginDto loginDto)
        {
            try
            {
                if (loginDto == null)
                {
                    return Fail(MessageKeys.LoginDataRequired);
                }

                if (string.IsNullOrWhiteSpace(loginDto.Email))
                {
                    return Fail(MessageKeys.EmailRequired);
                }

                if (string.IsNullOrWhiteSpace(loginDto.Password))
                {
                    return Fail(MessageKeys.PasswordRequired);
                }

                UserDto user = userRepository.GetActiveUserByEmail(loginDto.Email);

                if (user == null)
                {
                    return Fail(MessageKeys.UserNotFound);
                }

                string storedPassword = userRepository.GetPasswordHashByEmail(loginDto.Email);
                bool passwordIsValid = PasswordHasher.VerifyPassword(loginDto.Password, storedPassword);

                if (!passwordIsValid)
                {
                    return Fail(MessageKeys.InvalidPassword);
                }

                return Success(MessageKeys.LoginSuccessful, user);
            }
            catch (Exception)
            {
                return Fail(MessageKeys.UnexpectedError);
            }
        }

        public OperationResultDto GetUserProfile(int userId)
        {
            try
            {
                if (userId <= 0)
                {
                    return Fail(MessageKeys.UserIdInvalid);
                }

                UserDto user = userRepository.GetUserById(userId);

                if (user == null)
                {
                    return Fail(MessageKeys.UserNotFound);
                }

                return Success(MessageKeys.UserProfileRetrievedSuccessfully, user);
            }
            catch (Exception)
            {
                return Fail(MessageKeys.UnexpectedError);
            }
        }

        public OperationResultDto UpdateUserProfile(UpdateUserProfileDto updateUserProfileDto)
        {
            try
            {
                if (updateUserProfileDto == null)
                {
                    return Fail(MessageKeys.UserProfileDataRequired);
                }

                if (updateUserProfileDto.UserId <= 0)
                {
                    return Fail(MessageKeys.UserIdInvalid);
                }

                if (string.IsNullOrWhiteSpace(updateUserProfileDto.FullName))
                {
                    return Fail(MessageKeys.FullNameRequired);
                }

                if (updateUserProfileDto.BirthDate >= DateTime.Today)
                {
                    return Fail(MessageKeys.BirthDateInvalid);
                }

                if (string.IsNullOrWhiteSpace(updateUserProfileDto.PhoneNumber))
                {
                    return Fail(MessageKeys.PhoneNumberRequired);
                }

                UserDto updatedUser = userRepository.UpdateUserProfile(updateUserProfileDto);

                if (updatedUser == null)
                {
                    return Fail(MessageKeys.UserNotFound);
                }

                return Success(MessageKeys.UserProfileUpdatedSuccessfully, updatedUser);
            }
            catch (Exception)
            {
                return Fail(MessageKeys.UnexpectedError);
            }
        }

        private OperationResultDto Success(string messageKey, UserDto user = null)
        {
            return new OperationResultDto
            {
                Success = true,
                MessageKey = messageKey,
                Message = MessageLocalizer.Get(messageKey),
                User = user
            };
        }

        private OperationResultDto Fail(string messageKey)
        {
            return new OperationResultDto
            {
                Success = false,
                MessageKey = messageKey,
                Message = MessageLocalizer.Get(messageKey)
            };
        }
    }
}
