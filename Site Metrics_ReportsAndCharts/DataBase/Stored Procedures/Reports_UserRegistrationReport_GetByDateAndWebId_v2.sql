USE [C28]
GO
/****** Object:  StoredProcedure [dbo].[Reports_UserRegistrationReport_GetByDateAndWebId_v2]    Script Date: 3/26/2017 8:46:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER proc [dbo].[Reports_UserRegistrationReport_GetByDateAndWebId_v2]
	
	    @QueryWebsiteId int = null
	   ,@QueryStartDate datetime2(7) = null
	   ,@QueryEndDate datetime2(7) = null
	   ,@QueryNewDate datetime2(7) = null


As

Begin

	SELECT [ur].[Date]
		  ,[ur].[WebsiteId]
		  ,[ur].[TotalRegistered]
		  ,[ur].[TotalReferrals]
		  ,[w].[Name]


		From dbo.[Report.UserRegistration] ur
		Left Join [dbo].[Website] w
		on w.[Id] = ur.[WebsiteId]
		WHERE (@QueryWebsiteId IS NULL OR WebsiteId = @QueryWebsiteId)
			AND (@QueryStartDate IS NULL OR [Date] >= @QueryStartDate)
			AND (@QueryNewDate IS NULL OR [Date] <= @QueryNewDate)

	ORDER BY [Date] ASC
	
	SELECT COUNT('WebsiteId')


		FROM [dbo].[Report.UserRegistration]
		WHERE (@QueryWebsiteId IS NULL OR WebsiteId = @QueryWebsiteId)
			AND (@QueryStartDate IS NULL OR [Date] >= @QueryStartDate)
			AND (@QueryNewDate IS NULL OR [Date] <= @QueryNewDate)


End

/*---------TEST CODE-------

Declare @QueryStartDate nvarchar(100) = '2017-02-13'
	   ,@QueryWebsiteId int = 55
   	   ,@QueryEndDate datetime2(7) = null

Exec dbo.Reports_UserRegistrationReport_GetByDateAndWebId_v2
@QueryWebsiteId
,@QueryStartDate
,@QueryEndDate


*/