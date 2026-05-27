-- ============================================================
-- HangmanGame: Create friends-related tables
-- Run this script on HangmanGameDB after Users table exists
-- ============================================================

USE HangmanGameDB;
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'FriendRequests')
BEGIN
    CREATE TABLE dbo.FriendRequests (
        FriendRequestId INT IDENTITY(1,1) PRIMARY KEY,

        SenderUserId INT NOT NULL,
        ReceiverUserId INT NOT NULL,

        Status INT NOT NULL DEFAULT 0,
        -- 0 = Pending
        -- 1 = Accepted
        -- 2 = Rejected
        -- 3 = Cancelled

        CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
        UpdatedAt DATETIME NULL,

        CONSTRAINT FK_FriendRequests_Sender
            FOREIGN KEY (SenderUserId) REFERENCES dbo.Users(UserId),

        CONSTRAINT FK_FriendRequests_Receiver
            FOREIGN KEY (ReceiverUserId) REFERENCES dbo.Users(UserId),

        CONSTRAINT CK_FriendRequests_NotSameUser
            CHECK (SenderUserId <> ReceiverUserId)
    );
END;
GO

-- ============================================================
-- Computed columns to normalize the friendship pair
-- Example: 1-2 and 2-1 become the same pair: UserAId=1, UserBId=2
-- ============================================================

IF COL_LENGTH('dbo.FriendRequests', 'UserAId') IS NULL
BEGIN
    ALTER TABLE dbo.FriendRequests
    ADD UserAId AS CASE
        WHEN SenderUserId < ReceiverUserId THEN SenderUserId
        ELSE ReceiverUserId
    END PERSISTED;
END;
GO

IF COL_LENGTH('dbo.FriendRequests', 'UserBId') IS NULL
BEGIN
    ALTER TABLE dbo.FriendRequests
    ADD UserBId AS CASE
        WHEN SenderUserId > ReceiverUserId THEN SenderUserId
        ELSE ReceiverUserId
    END PERSISTED;
END;
GO

-- ============================================================
-- Avoid duplicate active relationships
-- Prevents:
-- User 1 sends to User 2 while another pending/accepted relation exists
-- User 2 sends to User 1 while another pending/accepted relation exists
-- ============================================================

IF NOT EXISTS (
    SELECT *
    FROM sys.indexes
    WHERE name = 'UX_FriendRequests_UserPair_Active'
)
BEGIN
    CREATE UNIQUE INDEX UX_FriendRequests_UserPair_Active
    ON dbo.FriendRequests(UserAId, UserBId)
    WHERE Status IN (0, 1);
END;
GO

-- ============================================================
-- Test query
-- ============================================================

SELECT *
FROM dbo.FriendRequests;
GO