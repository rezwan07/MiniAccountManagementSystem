USE [MiniAccountDB]
GO
/****** Object:  StoredProcedure [dbo].[sp_SaveVoucher]    Script Date: 6/23/2025 3:31:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[sp_SaveVoucher]
    @VoucherType NVARCHAR(50),
    @VoucherDate DATE,
    @ReferenceNo NVARCHAR(100),
    @Details VoucherDetailType READONLY
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Vouchers (VoucherType, VoucherDate, ReferenceNo)
    VALUES (@VoucherType, @VoucherDate, @ReferenceNo);

    DECLARE @NewVoucherId INT = SCOPE_IDENTITY();

    INSERT INTO VoucherDetails (VoucherId, AccountId, DebitAmount, CreditAmount, Remarks)
    SELECT @NewVoucherId, AccountId, DebitAmount, CreditAmount, Remarks
    FROM @Details;
END