/*
  Portfolio database: handy T-SQL to run against Azure SQL (VS Code -> SQL Server/mssql extension).
  The TABLES are created by EF Core Code-First when the app first starts.
  Run this file AFTER the site has started once, connected to the PortfolioDb database
  (not master — Azure SQL doesn't support USE, so open the query tab on PortfolioDb).
*/

-- 1. Who has messaged me? (newest first)
SELECT TOP (50) Id, Name, Email, Message, CreatedAtUtc
FROM ContactMessages
ORDER BY CreatedAtUtc DESC;
GO

-- 2. JOIN: every project with its company
SELECT e.Company, e.Role, e.Type, p.Title, p.StartDate, p.EndDate
FROM Experiences AS e
INNER JOIN Projects AS p ON p.ExperienceId = e.Id
ORDER BY p.StartDate DESC;
GO

-- 3. Technologies are stored as a JSON array; OPENJSON turns them into rows.
--    GROUP BY shows which tools I've used most.
SELECT t.[value] AS Technology, COUNT(*) AS ProjectsUsingIt
FROM Projects AS p
CROSS APPLY OPENJSON(p.Technologies) AS t
GROUP BY t.[value]
ORDER BY ProjectsUsingIt DESC, Technology;
GO

-- 4. Stored procedure: recent messages for the last N days
CREATE OR ALTER PROCEDURE dbo.usp_GetRecentContactMessages
    @Days INT = 7
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, Email, Message, CreatedAtUtc
    FROM ContactMessages
    WHERE CreatedAtUtc >= DATEADD(DAY, -@Days, SYSUTCDATETIME())
    ORDER BY CreatedAtUtc DESC;
END
GO

EXEC dbo.usp_GetRecentContactMessages @Days = 30;
GO

-- 5. Transaction: delete test messages safely (check the count, then COMMIT or ROLLBACK)
BEGIN TRANSACTION;
    DELETE FROM ContactMessages WHERE Email LIKE '%@example.com';
    SELECT @@ROWCOUNT AS RowsDeleted;
-- COMMIT;     -- run this if the count looks right
-- ROLLBACK;   -- or this to undo
