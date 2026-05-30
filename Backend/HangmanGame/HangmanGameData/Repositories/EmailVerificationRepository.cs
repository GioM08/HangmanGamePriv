using System;
using System.Linq;
using HangmanGameData.Repositories.Interfaces;
using HangmanGameEntities.Dtos;

namespace HangmanGameData.Repositories
{
    public class EmailVerificationRepository : IEmailVerificationRepository
    {
        public UserDto GetActiveUserById(int userId)
        {
            using (var database = new HangmanGameDataContext())
            {
                Users user = database.Users
                    .FirstOrDefault(item => item.UserId == userId && item.IsActive == true);

                if (user == null)
                {
                    return null;
                }

                return MapToUserDto(user);
            }
        }

        public UserDto GetActiveUserByEmail(string email)
        {
            using (var database = new HangmanGameDataContext())
            {
                string normalizedEmail = email.Trim().ToLower();

                Users user = database.Users
                    .FirstOrDefault(item =>
                        item.Email.ToLower() == normalizedEmail &&
                        item.IsActive == true);

                if (user == null)
                {
                    return null;
                }

                return MapToUserDto(user);
            }
        }

        public bool IsEmailVerified(int userId)
        {
            using (var database = new HangmanGameDataContext())
            {
                Users user = database.Users
                    .FirstOrDefault(item => item.UserId == userId && item.IsActive == true);

                if (user == null)
                {
                    return false;
                }

                return user.IsEmailVerified;
            }
        }

        public void CreateVerificationCode(
            int userId,
            string email,
            string codeHash,
            string purpose,
            int expirationMinutes)
        {
            using (var database = new HangmanGameDataContext())
            {
                DateTime now = DateTime.Now;

                EmailVerificationCodes verificationCode = new EmailVerificationCodes();

                verificationCode.UserId = userId;
                verificationCode.Email = email;
                verificationCode.CodeHash = codeHash;
                verificationCode.Purpose = purpose;
                verificationCode.ExpiresAt = now.AddMinutes(expirationMinutes);
                verificationCode.CreatedAt = now;

                database.EmailVerificationCodes.InsertOnSubmit(verificationCode);
                database.SubmitChanges();
            }
        }

        public EmailVerificationCodes GetLatestValidCode(int userId, string email, string purpose)
        {
            using (var database = new HangmanGameDataContext())
            {
                string normalizedEmail = email.Trim().ToLower();

                return database.EmailVerificationCodes
                    .Where(code =>
                        code.UserId == userId &&
                        code.Email.ToLower() == normalizedEmail &&
                        code.Purpose == purpose &&
                        code.UsedAt == null &&
                        code.ExpiresAt >= DateTime.Now)
                    .OrderByDescending(code => code.CreatedAt)
                    .FirstOrDefault();
            }
        }

        public void MarkCodeAsUsed(int verificationCodeId)
        {
            using (var database = new HangmanGameDataContext())
            {
                EmailVerificationCodes code = database.EmailVerificationCodes
                    .FirstOrDefault(item => item.VerificationCodeId == verificationCodeId);

                if (code == null)
                {
                    return;
                }

                code.UsedAt = DateTime.Now;
                database.SubmitChanges();
            }
        }

        public void MarkEmailAsVerified(int userId)
        {
            using (var database = new HangmanGameDataContext())
            {
                Users user = database.Users
                    .FirstOrDefault(item => item.UserId == userId && item.IsActive == true);

                if (user == null)
                {
                    return;
                }

                user.IsEmailVerified = true;
                user.EmailVerifiedAt = DateTime.Now;
                user.UpdatedAt = DateTime.Now;

                database.SubmitChanges();
            }
        }

        public void UpdatePasswordHash(int userId, string newPasswordHash)
        {
            using (var database = new HangmanGameDataContext())
            {
                Users user = database.Users
                    .FirstOrDefault(item => item.UserId == userId && item.IsActive == true);

                if (user == null)
                {
                    return;
                }

                user.PasswordHash = newPasswordHash;
                user.UpdatedAt = DateTime.Now;

                database.SubmitChanges();
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
                CreatedAt = user.CreatedAt,
                IsEmailVerified = user.IsEmailVerified
            };
        }
    }
}
