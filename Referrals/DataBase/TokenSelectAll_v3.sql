USE [C28]
GO
/****** Object:  StoredProcedure [dbo].[Token_SelectAll_v3]    Script Date: 3/18/2017 7:30:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER proc [dbo].[Token_SelectAll_v3]
	
	    @UserId nvarchar(128)
	   ,@TokenType int
	   ,@CurrentPage int = 1
	   ,@ItemsPerPage int = 10


As

Begin

	SELECT [Id]
      ,[DateCreated]
      ,[Used]
      ,[UserId]
      ,[TokenType]
      ,[CouponId]
      ,[Email]

	FROM [dbo].[Token]

		Where UserId = @UserId
	AND TokenType = @TokenType

	 ORDER BY [DateCreated] DESC

	OFFSET ((@CurrentPage - 1) * @ItemsPerPage) ROWS
             FETCH NEXT  @ItemsPerPage ROWS ONLY 
			 
/* different query for counting the number of items*/
			  
	SELECT count(Id)
	FROM [dbo].[Token]
		Where UserId = @UserId
	AND TokenType = @TokenType

End

/*------TEST CODE------

Declare 
	   @UserId nvarchar(128) = 'TestGuyUSERID' 	
	   ,@TokenType int = 3
	   ,@CurrentPage int = 1
	  
Exec dbo.Token_SelectAll_v3
		@UserId
		,@TokenType

*/