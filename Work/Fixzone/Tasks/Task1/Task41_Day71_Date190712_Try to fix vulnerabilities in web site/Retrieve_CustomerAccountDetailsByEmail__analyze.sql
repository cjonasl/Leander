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
DECLARE
@NumberOfRecords int,
@ID int,
@CustomerId int,
@Psw varchar(100),
@ClientName varchar(50)

SELECT @NumberOfRecords = COUNT(*) from Customer CR left join Client c on c.ClientId = CR.ClientID inner join CustomerAccount as CA  on CA.CustomerID = CR.CUSTOMERID where CA.EMAIL = @Email 
INSERT INTO TmphgfefAnalyze(Email, LogInDateTime, NumberOfRecords) VALUES(@Email, getdate(), @NumberOfRecords)

IF (@NumberOfRecords = 1)
BEGIN
  SELECT TOP 1 @ID = ID
  FROM TmphgfefAnalyze
  ORDER BY ID desc
  
  SELECT @CustomerId = CR.CUSTOMERID, @Psw = CA.Password, @ClientName = c.clientname from Customer CR left join Client c on c.ClientId = CR.ClientID inner join CustomerAccount as CA  on CA.CustomerID = CR.CUSTOMERID where CA.EMAIL = @Email
  
  UPDATE
    TmphgfefAnalyze
  SET
    CustomerId = @CustomerId,
	Psw = @Psw,
	ClientName = @ClientName
  WHERE
    ID = @ID
END

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


