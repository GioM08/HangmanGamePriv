-- ============================================================
-- HangmanGame: Email verification and password recovery setup
-- ============================================================

USE HangmanGameDB;
GO

-- ============================================================
-- 1. Add email verification columns to Users
-- ============================================================

IF COL_LENGTH('dbo.Users', 'IsEmailVerified') IS NULL
BEGIN
    ALTER TABLE dbo.Users
    ADD IsEmailVerified BIT NOT NULL 
        CONSTRAINT DF_Users_IsEmailVerified DEFAULT 0;
END;
GO

IF COL_LENGTH('dbo.Users', 'EmailVerifiedAt') IS NULL
BEGIN
    ALTER TABLE dbo.Users
    ADD EmailVerifiedAt DATETIME NULL;
END;
GO

-- ============================================================
-- 2. Create EmailVerificationCodes table
-- This table is used for:
-- VERIFY_EMAIL
-- RESET_PASSWORD
-- ============================================================

IF NOT EXISTS (
    SELECT *
    FROM sys.tables
    WHERE name = 'EmailVerificationCodes'
      AND schema_id = SCHEMA_ID('dbo')
)
BEGIN
    CREATE TABLE dbo.EmailVerificationCodes (
        VerificationCodeId INT IDENTITY(1,1) PRIMARY KEY,

        UserId INT NOT NULL,
        Email NVARCHAR(100) NOT NULL,

        CodeHash NVARCHAR(255) NOT NULL,

        Purpose NVARCHAR(30) NOT NULL,
        -- VERIFY_EMAIL
        -- RESET_PASSWORD

        ExpiresAt DATETIME NOT NULL,
        UsedAt DATETIME NULL,
        CreatedAt DATETIME NOT NULL 
            CONSTRAINT DF_EmailVerificationCodes_CreatedAt DEFAULT GETDATE(),

        CONSTRAINT FK_EmailVerificationCodes_Users
            FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId),

        CONSTRAINT CK_EmailVerificationCodes_Purpose
            CHECK (Purpose IN ('VERIFY_EMAIL', 'RESET_PASSWORD'))
    );
END;
GO

-- ============================================================
-- 3. Helpful indexes
-- ============================================================

IF NOT EXISTS (
    SELECT *
    FROM sys.indexes
    WHERE name = 'IX_EmailVerificationCodes_UserId_Purpose'
)
BEGIN
    CREATE INDEX IX_EmailVerificationCodes_UserId_Purpose
    ON dbo.EmailVerificationCodes(UserId, Purpose);
END;
GO

IF NOT EXISTS (
    SELECT *
    FROM sys.indexes
    WHERE name = 'IX_EmailVerificationCodes_Email_Purpose'
)
BEGIN
    CREATE INDEX IX_EmailVerificationCodes_Email_Purpose
    ON dbo.EmailVerificationCodes(Email, Purpose);
END;
GO

IF NOT EXISTS (
    SELECT *
    FROM sys.indexes
    WHERE name = 'IX_EmailVerificationCodes_ExpiresAt'
)
BEGIN
    CREATE INDEX IX_EmailVerificationCodes_ExpiresAt
    ON dbo.EmailVerificationCodes(ExpiresAt);
END;
GO

-- ============================================================
-- 4. Optional check queries
-- ============================================================

SELECT 
    UserId,
    FullName,
    Email,
    IsEmailVerified,
    EmailVerifiedAt
FROM dbo.Users;
GO

SELECT *
FROM dbo.EmailVerificationCodes;
GO