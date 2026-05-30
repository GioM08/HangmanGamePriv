using HangmanGameBusiness.AccountRecovery;
using HangmanGameBusiness.Localization;
using HangmanGameEntities.Dtos;
using HangmanGameServices.Localization;

namespace HangmanGameServices.Services
{
    public class AccountRecoveryService : IAccountRecoveryService
    {
        private readonly IAccountRecoveryBusiness accountRecoveryBusiness;

        public AccountRecoveryService()
        {
            this.accountRecoveryBusiness = new AccountRecoveryBusiness();
        }

        public EmailOperationResultDto SendEmailVerificationCode(int userId)
        {
            SetLanguage();
            return accountRecoveryBusiness.SendEmailVerificationCode(userId);
        }

        public EmailOperationResultDto VerifyEmailCode(VerifyEmailDto verifyEmailDto)
        {
            SetLanguage();
            return accountRecoveryBusiness.VerifyEmailCode(verifyEmailDto);
        }

        public EmailOperationResultDto RequestPasswordReset(ForgotPasswordDto forgotPasswordDto)
        {
            SetLanguage();
            return accountRecoveryBusiness.RequestPasswordReset(forgotPasswordDto);
        }

        public EmailOperationResultDto ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            SetLanguage();
            return accountRecoveryBusiness.ResetPassword(resetPasswordDto);
        }

        private static void SetLanguage()
        {
            LanguageContext.SetLanguage(RequestLanguageReader.GetLanguageCode());
        }
    }
}
