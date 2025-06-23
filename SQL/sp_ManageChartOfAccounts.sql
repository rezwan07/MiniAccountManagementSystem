USE [MiniAccountDB]
GO
/****** Object:  StoredProcedure [dbo].[sp_ManageChartOfAccounts]    Script Date: 6/23/2025 3:30:59 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[sp_ManageChartOfAccounts]
    @Action NVARCHAR(10),           
    @Id INT = NULL,                 
    @Name NVARCHAR(100) = NULL,
    @ParentId INT = NULL,
    @AccountType NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @Action = 'Insert'
    BEGIN
        INSERT INTO ChartOfAccounts (Name, ParentId, AccountType)
        VALUES (@Name, @ParentId, @AccountType)
    END

    ELSE IF @Action = 'Update'
    BEGIN
        UPDATE ChartOfAccounts
        SET Name = @Name,
            ParentId = @ParentId,
            AccountType = @AccountType
        WHERE Id = @Id
    END

    ELSE IF @Action = 'Delete'
    BEGIN
        DELETE FROM ChartOfAccounts WHERE Id = @Id
    END
END