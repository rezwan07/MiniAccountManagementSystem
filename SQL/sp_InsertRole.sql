USE [MiniAccountDB]
GO
/****** Object:  StoredProcedure [dbo].[sp_InsertRole]    Script Date: 6/23/2025 3:29:55 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[sp_InsertRole]
    @RoleName NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE Name = @RoleName)
    BEGIN
        INSERT INTO AspNetRoles (Id, Name)
        VALUES (NEWID(), @RoleName)
    END
END