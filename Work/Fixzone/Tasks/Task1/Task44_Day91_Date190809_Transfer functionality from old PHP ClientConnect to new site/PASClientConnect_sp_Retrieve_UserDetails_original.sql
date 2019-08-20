USE [PAS]
GO

/****** Object:  StoredProcedure [dbo].[Retrieve_UserDetails]    Script Date: 19/08/2019 15:39:04 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[Retrieve_UserDetails]

	@UserId VARCHAR(MAX)

AS		

	select 

		u.Userid as 'UserId',

		u.Fullname as 'UserName',

		u.Password as 'UserPassword',

		case

			when (LTRIM(RTRIM(isnull(u.Password,'')))) = ''   then 1

			else 0

		end as 'IsPasswordEmpty',

		CAST(u.Enabled as int) as 'Enabled',

		case 

			when u.PasswordValidUntilDate > GETDATE()

				then CAST(0 as bit)

			else CAST(1 as bit)

		end as 'PasswordExpired',

		CAST(c.ClientPriorityBooking as BIT) as 'ClientPriorityBooking',

		c.ClientId as 'UserStoreId',

		c.clientname as 'UserStoreName',

		u.Level as 'AccessLevel',u.level as 'UserLevel',
		--,
		u.ReminderQuestion as 'ReminderQuestion',

		u.ReminderAnswer as 'ReminderAnswer',

		u.DateOfBirth as 'DateOfBirth',

		c.[ClientCountry ] as 'UserStoreCountry',CAST(c.ClientDisabled as BIT) as 'ClientDisabled', CAST(c.ClientOnStopFg as BIT) as 'ClientOnStopFg',
		c.ClientOnStopNotes  as 'ClientOnStopNotes' ,Cast(c.ClientBookingType as int) ClientBookingType,IsCallCenterUser

	from UserWeb u

	left join Client c on c.ClientId = u.ClientID

	where UserId = @UserId

GO


