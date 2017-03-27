USE [C28]
GO
/****** Object:  StoredProcedure [dbo].[Reports_UserRegistrationReport_GetAll]    Script Date: 3/26/2017 8:45:55 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER proc [dbo].[Reports_UserRegistrationReport_GetAll]

AS

BEGIN

SELECT 
	 [Date]
	,[WebsiteId]
	,[TotalRegistered]
	,[TotalReferrals]
FROM 
	[C28].[dbo].[Report.UserRegistration]

WHERE [Date] > DATEADD(day, -3, CONVERT (date, SYSDATETIME()))

ORDER BY [WebsiteId], [Date] desc

END