USE [C28]
GO
/****** Object:  StoredProcedure [dbo].[Token_DeleteById]    Script Date: 3/18/2017 7:33:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER proc [dbo].[Token_DeleteById]
	@Id int
	
AS
/* ------------TEST CODE-------------

Declare @Id int = 200

		Select *
		From dbo.Token
		Where Id = @Id
		
		Execute dbo.Token_DeleteById
				@Id 
		
		Select *
		From dbo.Token
		Where Id = @Id

*/
BEGIN

	
		DELETE FROM [dbo].[Token]
		WHERE Id = @Id


END