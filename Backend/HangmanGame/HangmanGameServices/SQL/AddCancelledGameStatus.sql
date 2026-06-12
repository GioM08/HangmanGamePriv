-- ============================================================
-- HangmanGame: Add Cancelled game status
-- Run this script on existing HangmanGameDB databases.
-- Status values:
--   0 = Waiting
--   1 = InProgress
--   2 = Finished
--   3 = Abandoned
--   4 = Cancelled
-- ============================================================
USE HangmanGameDB;
GO

IF OBJECT_ID('dbo.CK_Games_Status', 'C') IS NOT NULL
BEGIN
    ALTER TABLE Games DROP CONSTRAINT CK_Games_Status;
END;
GO

ALTER TABLE Games
ADD CONSTRAINT CK_Games_Status CHECK (Status IN (0, 1, 2, 3, 4));
GO

-- Optional cleanup for old waiting games that were left available by the bug.
-- Review before running in production, because this cancels every currently waiting game.
--
-- UPDATE Games
-- SET Status = 4,
--     FinishedAt = GETDATE(),
--     AbandonedByUserId = CreatorId
-- WHERE Status = 0
--   AND RetadorId IS NULL;
-- GO
