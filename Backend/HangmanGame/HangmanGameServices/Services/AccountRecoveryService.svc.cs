using HangmanGameBusiness.AccountRecovery;
using HangmanGameEntities.Dtos;

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
            return accountRecoveryBusiness.SendEmailVerificationCode(userId);
        }

        public EmailOperationResultDto VerifyEmailCode(VerifyEmailDto verifyEmailDto)
        {
            return accountRecoveryBusiness.VerifyEmailCode(verifyEmailDto);
        }

        public EmailOperationResultDto RequestPasswordReset(ForgotPasswordDto forgotPasswordDto)
        {
            return accountRecoveryBusiness.RequestPasswordReset(forgotPasswordDto);
        }

        public EmailOperationResultDto ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            return accountRecoveryBusiness.ResetPassword(resetPasswordDto);
        }
    }
}
