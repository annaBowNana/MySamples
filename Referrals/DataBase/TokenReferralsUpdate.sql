USE [C28]
GO
/****** Object:  StoredProcedure [dbo].[Token_Referrals_Update]    Script Date: 3/18/2017 7:34:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER proc [dbo].[Token_Referrals_Update]
					@TokenHash nvarchar(128)
AS

BEGIN

UPDATE [dbo].[Token]
   SET [Used] = GETUTCDATE()
 WHERE @TokenHash = [TokenHash]

END

/*----TEST CODE ------
Select * from [dbo].[Token]
Declare @TokenHash nvarchar(128) = 'BBD1F03B-96B8-46CB-9AAC-96E95D5AE4D7'

Execute [dbo].[Token_Referrals_Update] @TokenHash

Select * from [dbo].[Token]
Where @TokenHash = [TokenHash]

---- END TEST CODE ----*/
