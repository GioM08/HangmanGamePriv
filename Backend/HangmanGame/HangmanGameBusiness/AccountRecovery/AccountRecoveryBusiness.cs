using System;
using HangmanGameBusiness.Email;
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
                    return Fail("User id is not valid.");
                }

                UserDto user = emailVerificationRepository.GetActiveUserById(userId);

                if (user == null)
                {
                    return Fail("User was not found.");
                }

                if (emailVerificationRepository.IsEmailVerified(userId))
                {
                    return Fail("Email is already verified.");
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

                return Success("Verification code sent successfully.");
            }
            catch (Exception)
            {
                return Fail("An unexpected error occurred while sending the verification code.");
            }
        }

        public EmailOperationResultDto VerifyEmailCode(VerifyEmailDto verifyEmailDto)
        {
            try
            {
                if (verifyEmailDto == null)
                {
                    return Fail("Verification data is required.");
                }

                if (verifyEmailDto.UserId <= 0)
                {
                    return Fail("User id is not valid.");
                }

                if (string.IsNullOrWhiteSpace(verifyEmailDto.Code))
                {
                    return Fail("Verification code is required.");
                }

                UserDto user = emailVerificationRepository.GetActiveUserById(verifyEmailDto.UserId);

                if (user == null)
                {
                    return Fail("User was not found.");
                }

                EmailVerificationCodes latestCode =
                    emailVerificationRepository.GetLatestValidCode(
                        user.UserId,
                        user.Email,
                        VerifyEmailPurpose
                    );

                if (latestCode == null)
                {
                    return Fail("Verification code is invalid or expired.");
                }

                bool codeIsValid = VerificationCodeHasher.VerifyCode(
                    verifyEmailDto.Code.Trim(),
                    latestCode.CodeHash
                );

                if (!codeIsValid)
                {
                    return Fail("Verification code is invalid or expired.");
                }

                emailVerificationRepository.MarkCodeAsUsed(latestCode.VerificationCodeId);
                emailVerificationRepository.MarkEmailAsVerified(user.UserId);

                return Success("Email verified successfully.");
            }
            catch (Exception)
            {
                return Fail("An unexpected error occurred while verifying the email.");
            }
        }

        public EmailOperationResultDto RequestPasswordReset(ForgotPasswordDto forgotPasswordDto)
        {
            try
            {
                if (forgotPasswordDto == null)
                {
                    return Fail("Email is required.");
                }

                if (string.IsNullOrWhiteSpace(forgotPasswordDto.Email))
                {
                    return Fail("Email is required.");
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

                return Success("If the email exists and is verified, a password reset code has been sent.");
            }
            catch (Exception)
            {
                return Fail("An unexpected error occurred while requesting the password reset.");
            }
        }

        public EmailOperationResultDto ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            try
            {
                if (resetPasswordDto == null)
                {
                    return Fail("Password reset data is required.");
                }

                if (string.IsNullOrWhiteSpace(resetPasswordDto.Email))
                {
                    return Fail("Email is required.");
                }

                if (string.IsNullOrWhiteSpace(resetPasswordDto.Code))
                {
                    return Fail("Verification code is required.");
                }

                if (string.IsNullOrWhiteSpace(resetPasswordDto.NewPassword))
                {
                    return Fail("New password is required.");
                }

                if (resetPasswordDto.NewPassword.Length < 6)
                {
                    return Fail("New password must contain at least 6 characters.");
                }

                UserDto user = emailVerificationRepository.GetActiveUserByEmail(
                    resetPasswordDto.Email
                );

                if (user == null)
                {
                    return Fail("Verification code is invalid or expired.");
                }

                EmailVerificationCodes latestCode =
                    emailVerificationRepository.GetLatestValidCode(
                        user.UserId,
                        user.Email,
                        ResetPasswordPurpose
                    );

                if (latestCode == null)
                {
                    return Fail("Verification code is invalid or expired.");
                }

                bool codeIsValid = VerificationCodeHasher.VerifyCode(
                    resetPasswordDto.Code.Trim(),
                    latestCode.CodeHash
                );

                if (!codeIsValid)
                {
                    return Fail("Verification code is invalid or expired.");
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

                return Success("Password reset successfully.");
            }
            catch (Exception)
            {
                return Fail("An unexpected error occurred while resetting the password.");
            }
        }

        private EmailOperationResultDto Success(string message)
        {
            return new EmailOperationResultDto
            {
                Success = true,
                Message = message
            };
        }

        private EmailOperationResultDto Fail(string message)
        {
            return new EmailOperationResultDto
            {
                Success = false,
                Message = message
            };
        }
    }
}
