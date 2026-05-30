using HangmanGameEntities.Dtos;

namespace HangmanGameBusiness.AccountRecovery
{
    public interface IAccountRecoveryBusiness
    {
        EmailOperationResultDto SendEmailVerificationCode(int userId);

        EmailOperationResultDto VerifyEmailCode(VerifyEmailDto verifyEmailDto);

        EmailOperationResultDto RequestPasswordReset(ForgotPasswordDto forgotPasswordDto);

        EmailOperationResultDto ResetPassword(ResetPasswordDto resetPasswordDto);
    }
}
