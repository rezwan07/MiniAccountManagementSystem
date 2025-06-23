USE [MiniAccountDB]
GO
/****** Object:  StoredProcedure [dbo].[sp_AssignUserRole]    Script Date: 6/23/2025 3:24:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[sp_AssignUserRole]
    @UserEmail NVARCHAR(256),
    @RoleName NVARCHAR(256)
AS
BEGIN
    DECLARE @UserId NVARCHAR(450)
    DECLARE @RoleId NVARCHAR(450)

    SELECT @UserId = Id FROM AspNetUsers WHERE Email = @UserEmail
    SELECT @RoleId = Id FROM AspNetRoles WHERE Name = @RoleName

    IF NOT EXISTS (
        SELECT 1 FROM AspNetUserRoles WHERE UserId = @UserId AND RoleId = @RoleId
    )
    BEGIN
        INSERT INTO AspNetUserRoles (UserId, RoleId)
        VALUES (@UserId, @RoleId)
    END
END