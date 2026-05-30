using HangmanGameEntities.Dtos;

namespace HangmanGameData.Repositories.Interfaces
{
    public interface IEmailVerificationRepository
    {
        UserDto GetActiveUserById(int userId);

        UserDto GetActiveUserByEmail(string email);

        bool IsEmailVerified(int userId);

        void CreateVerificationCode(int userId, string email, string codeHash, string purpose, int expirationMinutes);

        EmailVerificationCodes GetLatestValidCode(int userId, string email, string purpose);

        void MarkCodeAsUsed(int verificationCodeId);

        void MarkEmailAsVerified(int userId);

        void UpdatePasswordHash(int userId, string newPasswordHash);
    }
}
