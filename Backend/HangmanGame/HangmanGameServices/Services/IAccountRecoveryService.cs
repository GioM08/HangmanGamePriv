using System.ServiceModel;
using HangmanGameEntities.Dtos;

namespace HangmanGameServices.Services
{
    [ServiceContract]
    public interface IAccountRecoveryService
    {
        [OperationContract]
        EmailOperationResultDto SendEmailVerificationCode(int userId);

        [OperationContract]
        EmailOperationResultDto VerifyEmailCode(VerifyEmailDto verifyEmailDto);

        [OperationContract]
        EmailOperationResultDto RequestPasswordReset(ForgotPasswordDto forgotPasswordDto);

        [OperationContract]
        EmailOperationResultDto ResetPassword(ResetPasswordDto resetPasswordDto);
    }
}
