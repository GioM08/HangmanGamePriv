using System;
using HangmanGameData.Repositories;
using HangmanGameEntities.Dtos;
using HangmanGameBusiness.Security;

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
            if (registerUserDto == null)
            {
                return new OperationResultDto
                {
                    Success = false,
                    Message = "User data is required."
                };
            }

            if (string.IsNullOrWhiteSpace(registerUserDto.FullName))
            {
                return new OperationResultDto
                {
                    Success
                    
                    
                    = false,
                    Message = "Full name is required."
                };
            }

            if (registerUserDto.BirthDate >= DateTime.Today)
            {
                return new OperationResultDto
                {
                    Success = false,
                    Message = "Birth date is not valid."
                };
            }

            if (string.IsNullOrWhiteSpace(registerUserDto.PhoneNumber))
            {
                return new OperationResultDto
                {
                    Success = false,
                    Message = "Phone number is required."
                };
            }

            if (string.IsNullOrWhiteSpace(registerUserDto.Email))
            {
                return new OperationResultDto
                {
                    Success = false,
                    Message = "Email is required."
                };
            }

            if (string.IsNullOrWhiteSpace(registerUserDto.Password))
            {
                return new OperationResultDto
                {
                    Success = false,
                    Message = "Password is required."
                };
            }

            if (userRepository.EmailExists(registerUserDto.Email))
            {
                return new OperationResultDto
                {
                    Success = false,
                    Message = "Email already exists."
                };
            }

            string passwordHash = PasswordHasher.HashPassword(registerUserDto.Password);

            UserDto createdUser = userRepository.CreateUser(registerUserDto, passwordHash);

            return new OperationResultDto
            {
                Success = true,
                Message = "User registered successfully.",
                User = createdUser
            };
        }

        public OperationResultDto Login(LoginDto loginDto)
        {
            if (loginDto == null)
            {
                return new OperationResultDto
                {
                    Success = false,
                    Message = "Login data is required."
                };
            }

            if (string.IsNullOrWhiteSpace(loginDto.Email))
            {
                return new OperationResultDto
                {
                    Success = false,
                    Message = "Email is required."
                };
            }

            if (string.IsNullOrWhiteSpace(loginDto.Password))
            {
                return new OperationResultDto
                {
                    Success = false,
                    Message = "Password is required."
                };
            }

            UserDto user = userRepository.GetActiveUserByEmail(loginDto.Email);

            if (user == null)
            {
                return new OperationResultDto
                {
                    Success = false,
                    Message = "User not found."
                };
            }

            string storedPassword = userRepository.GetPasswordHashByEmail(loginDto.Email);

            bool passwordIsValid = PasswordHasher.VerifyPassword(loginDto.Password, storedPassword);

            if (!passwordIsValid)
            {
                return new OperationResultDto
                {
                    Success = false,
                    Message = "Invalid password."
                };
            }

            return new OperationResultDto
            {
                Success = true,
                Message = "Login successful.",
                User = user
            };
        }

        public OperationResultDto GetUserProfile(int userId)
        {
            try
            {
                if (userId <= 0)
                {
                    return new OperationResultDto
                    {
                        Success = false,
                        Message = "User id is not valid."
                    };
                }

                UserDto user = userRepository.GetUserById(userId);

                if (user == null)
                {
                    return new OperationResultDto
                    {
                        Success = false,
                        Message = "User not found."
                    };
                }

                return new OperationResultDto
                {
                    Success = true,
                    Message = "User profile retrieved successfully.",
                    User = user
                };
            }
            catch (Exception)
            {
                return new OperationResultDto
                {
                    Success = false,
                    Message = "An unexpected error occurred while getting the user profile."
                };
            }
        }

        public OperationResultDto UpdateUserProfile(UpdateUserProfileDto updateUserProfileDto)
        {
            try
            {
                if (updateUserProfileDto == null)
                {
                    return new OperationResultDto
                    {
                        Success = false,
                        Message = "User profile data is required."
                    };
                }

                if (updateUserProfileDto.UserId <= 0)
                {
                    return new OperationResultDto
                    {
                        Success = false,
                        Message = "User id is not valid."
                    };
                }

                if (string.IsNullOrWhiteSpace(updateUserProfileDto.FullName))
                {
                    return new OperationResultDto
                    {
                        Success = false,
                        Message = "Full name is required."
                    };
                }

                if (updateUserProfileDto.BirthDate >= DateTime.Today)
                {
                    return new OperationResultDto
                    {
                        Success = false,
                        Message = "Birth date is not valid."
                    };
                }

                if (string.IsNullOrWhiteSpace(updateUserProfileDto.PhoneNumber))
                {
                    return new OperationResultDto
                    {
                        Success = false,
                        Message = "Phone number is required."
                    };
                }

                UserDto updatedUser = userRepository.UpdateUserProfile(updateUserProfileDto);

                if (updatedUser == null)
                {
                    return new OperationResultDto
                    {
                        Success = false,
                        Message = "User not found."
                    };
                }

                return new OperationResultDto
                {
                    Success = true,
                    Message = "User profile updated successfully.",
                    User = updatedUser
                };
            }
            catch (Exception)
            {
                return new OperationResultDto
                {
                    Success = false,
                    Message = "An unexpected error occurred while updating the user profile."
                };
            }
        }
    }
}