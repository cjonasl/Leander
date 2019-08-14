USE [ShopDirect]
GO

/****** Object:  StoredProcedure [dbo].[Retrieve_CustomerAccountDetailsByEmail]    Script Date: 13/08/2019 15:33:25 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



ALTER PROCEDURE [dbo].[Retrieve_CustomerAccountDetailsByEmail]

	@Email VARCHAR(MAX)
	

AS		

	select 

	CR.CUSTOMERID as CustomerID,

		CA.EMAIL as 'Email',

		CA.Password as 'Password',

			case

			when (LTRIM(RTRIM(isnull(Ca.Password,'')))) = ''   then 1

			else 0

		end as 'IsPasswordEmpty',	

		CAST(0 as bit)

		 as 'PasswordExpired',

		CAST(c.ClientPriorityBooking as BIT) as 'ClientPriorityBooking',

		c.ClientId as 'ClientId',

		cr.RetailClientID as 'RetailClientId',

		c.clientname as 'ClientName',

		1 as 'AccessLevel',1 as 'UserLevel',

		CA.LastLoginDate as LastLoginDate,

		CA.ClientCustRef as ClientCustRef,

		CR.CustomerGuId as CustomerGuId

	from Customer CR


	left join Client c on c.ClientId = CR.ClientID
	inner join CustomerAccount as CA 
	on CA.CustomerID = CR.CUSTOMERID

	where CA.EMAIL = @Email 



GO


