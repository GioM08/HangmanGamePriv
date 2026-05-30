using System;
using HangmanGameBusiness.Email;
using HangmanGameBusiness.Localization;
using HangmanGameBusiness.Security;
using HangmanGameData;
using HangmanGameData.Repositories;
using HangmanGameData.Repositories.Interfaces;
using HangmanGameEntities.Dtos;

namespace HangmanGameBusiness.AccountRecovery
{
    public class AccountRecoveryBusiness : IAccountRecoveryBusiness
    {
        private const string VerifyEmailPurpose = "VERIFY_EMAIL";
        private const string ResetPasswordPurpose = "RESET_PASSWORD";
        private const int CodeExpirationMinutes = 10;

        private readonly IEmailVerificationRepository emailVerificationRepository;
        private readonly IEmailSender emailSender;

        public AccountRecoveryBusiness()
        {
            this.emailVerificationRepository = new EmailVerificationRepository();
            this.emailSender = new SmtpEmailSender();
        }

        public AccountRecoveryBusiness(
            IEmailVerificationRepository emailVerificationRepository,
            IEmailSender emailSender)
        {
            if (emailVerificationRepository == null)
            {
                throw new ArgumentNullException("emailVerificationRepository");
            }

            if (emailSender == null)
            {
                throw new ArgumentNullException("emailSender");
            }

            this.emailVerificationRepository = emailVerificationRepository;
            this.emailSender = emailSender;
        }

        public EmailOperationResultDto SendEmailVerificationCode(int userId)
        {
            try
            {
                if (userId <= 0)
                {
                    return Fail(MessageKeys.UserIdInvalid);
                }

                UserDto user = emailVerificationRepository.GetActiveUserById(userId);

                if (user == null)
                {
                    return Fail(MessageKeys.UserNotFound);
                }

                if (emailVerificationRepository.IsEmailVerified(userId))
                {
                    return Fail(MessageKeys.EmailAlreadyVerified);
                }

                string code = VerificationCodeGenerator.GenerateSixDigitCode();
                string codeHash = VerificationCodeHasher.HashCode(code);

                emailVerificationRepository.CreateVerificationCode(
                    user.UserId,
                    user.Email,
                    codeHash,
                    VerifyEmailPurpose,
                    CodeExpirationMinutes
                );

                emailSender.SendEmail(
                    user.Email,
                    "Hangman Game - Email verification code",
                    string.Format(
                        "Your Hangman Game email verification code is: {0}\n\nThis code expires in {1} minutes.",
                        code,
                        CodeExpirationMinutes
                    )
                );

                return Success(MessageKeys.VerificationCodeSentSuccessfully);
            }
            catch (Exception)
            {
                return Fail(MessageKeys.UnexpectedError);
            }
        }

        public EmailOperationResultDto VerifyEmailCode(VerifyEmailDto verifyEmailDto)
        {
            try
            {
                if (verifyEmailDto == null)
                {
                    return Fail(MessageKeys.VerificationDataRequired);
                }

                if (verifyEmailDto.UserId <= 0)
                {
                    return Fail(MessageKeys.UserIdInvalid);
                }

                if (string.IsNullOrWhiteSpace(verifyEmailDto.Code))
                {
                    return Fail(MessageKeys.VerificationCodeRequired);
                }

                UserDto user = emailVerificationRepository.GetActiveUserById(verifyEmailDto.UserId);

                if (user == null)
                {
                    return Fail(MessageKeys.UserNotFound);
                }

                EmailVerificationCodes latestCode =
                    emailVerificationRepository.GetLatestValidCode(
                        user.UserId,
                        user.Email,
                        VerifyEmailPurpose
                    );

                if (latestCode == null)
                {
                    return Fail(MessageKeys.VerificationCodeInvalidOrExpired);
                }

                bool codeIsValid = VerificationCodeHasher.VerifyCode(
                    verifyEmailDto.Code.Trim(),
                    latestCode.CodeHash
                );

                if (!codeIsValid)
                {
                    return Fail(MessageKeys.VerificationCodeInvalidOrExpired);
                }

                emailVerificationRepository.MarkCodeAsUsed(latestCode.VerificationCodeId);
                emailVerificationRepository.MarkEmailAsVerified(user.UserId);

                return Success(MessageKeys.EmailVerifiedSuccessfully);
            }
            catch (Exception)
            {
                return Fail(MessageKeys.UnexpectedError);
            }
        }

        public EmailOperationResultDto RequestPasswordReset(ForgotPasswordDto forgotPasswordDto)
        {
            try
            {
                if (forgotPasswordDto == null)
                {
                    return Fail(MessageKeys.EmailRequired);
                }

                if (string.IsNullOrWhiteSpace(forgotPasswordDto.Email))
                {
                    return Fail(MessageKeys.EmailRequired);
                }

                UserDto user = emailVerificationRepository.GetActiveUserByEmail(
                    forgotPasswordDto.Email
                );

                if (user != null && user.IsEmailVerified)
                {
                    string code = VerificationCodeGenerator.GenerateSixDigitCode();
                    string codeHash = VerificationCodeHasher.HashCode(code);

                    emailVerificationRepository.CreateVerificationCode(
                        user.UserId,
                        user.Email,
                        codeHash,
                        ResetPasswordPurpose,
                        CodeExpirationMinutes
                    );

                    emailSender.SendEmail(
                        user.Email,
                        "Hangman Game - Password reset code",
                        string.Format(
                            "Your Hangman Game password reset code is: {0}\n\nThis code expires in {1} minutes.",
                            code,
                            CodeExpirationMinutes
                        )
                    );
                }

                return Success(MessageKeys.PasswordResetCodeSent);
            }
            catch (Exception)
            {
                return Fail(MessageKeys.UnexpectedError);
            }
        }

        public EmailOperationResultDto ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            try
            {
                if (resetPasswordDto == null)
                {
                    return Fail(MessageKeys.PasswordResetDataRequired);
                }

                if (string.IsNullOrWhiteSpace(resetPasswordDto.Email))
                {
                    return Fail(MessageKeys.EmailRequired);
                }

                if (string.IsNullOrWhiteSpace(resetPasswordDto.Code))
                {
                    return Fail(MessageKeys.VerificationCodeRequired);
                }

                if (string.IsNullOrWhiteSpace(resetPasswordDto.NewPassword))
                {
                    return Fail(MessageKeys.NewPasswordRequired);
                }

                if (resetPasswordDto.NewPassword.Length < 6)
                {
                    return Fail(MessageKeys.NewPasswordTooShort);
                }

                UserDto user = emailVerificationRepository.GetActiveUserByEmail(
                    resetPasswordDto.Email
                );

                if (user == null)
                {
                    return Fail(MessageKeys.VerificationCodeInvalidOrExpired);
                }

                EmailVerificationCodes latestCode =
                    emailVerificationRepository.GetLatestValidCode(
                        user.UserId,
                        user.Email,
                        ResetPasswordPurpose
                    );

                if (latestCode == null)
                {
                    return Fail(MessageKeys.VerificationCodeInvalidOrExpired);
                }

                bool codeIsValid = VerificationCodeHasher.VerifyCode(
                    resetPasswordDto.Code.Trim(),
                    latestCode.CodeHash
                );

                if (!codeIsValid)
                {
                    return Fail(MessageKeys.VerificationCodeInvalidOrExpired);
                }

                string newPasswordHash = PasswordHasher.HashPassword(
                    resetPasswordDto.NewPassword
                );

                emailVerificationRepository.UpdatePasswordHash(
                    user.UserId,
                    newPasswordHash
                );

                emailVerificationRepository.MarkCodeAsUsed(
                    latestCode.VerificationCodeId
                );

                return Success(MessageKeys.PasswordResetSuccessfully);
            }
            catch (Exception)
            {
                return Fail(MessageKeys.UnexpectedError);
            }
        }

        private EmailOperationResultDto Success(string messageKey)
        {
            return new EmailOperationResultDto
            {
                Success = true,
                MessageKey = messageKey,
                Message = MessageLocalizer.Get(messageKey)
            };
        }

        private EmailOperationResultDto Fail(string messageKey)
        {
            return new EmailOperationResultDto
            {
                Success = false,
                MessageKey = messageKey,
                Message = MessageLocalizer.Get(messageKey)
            };
        }
    }
}
