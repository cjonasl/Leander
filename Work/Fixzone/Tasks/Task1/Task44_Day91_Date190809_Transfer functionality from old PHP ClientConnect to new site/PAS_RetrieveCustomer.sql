USE [PAS]
GO

/****** Object:  StoredProcedure [dbo].[RetrieveCustomer]    Script Date: 09/09/2019 11:38:19 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


ALTER PROCEDURE [dbo].[RetrieveCustomer]

	@CustomerID INT = null

AS

	-- select customer info	

	SELECT  c.TITLE as 'Title', 

			c.FIRSTNAME as 'Forename',

			c.SURNAME as 'Surname',

			c.POSTCODE as 'Postcode',

			(c.ADDR1 +  isnull(',' +c.NOTES,'')) as 'Addr1',

			c.ADDR2 as 'Addr2',

			c.ADDR3 as 'Addr3',

			c.TEL1 as 'MobileTel',

			c.TEL2 as 'LandlineTel',

			c.TEL3 as 'TEL3',

			c.EMAIL as 'Email',

			c.TOWN as 'Town',

			c.COUNTY as 'County',

			c.COUNTRY as 'Country',

			c.CustomerId as 'CustomerId',

			CAST(c.SENDBY as int) as 'ContactMethod',

				c.CLIENTCUSTREF as 'CLIENTCUSTREF',

			cc.Addr1 as 'CollectAddr1',

			cc.Addr2 as 'CollectAddr2',

			cc.Addr3 as 'CollectAddr3',

			cc.County as 'CollectCounty',

			cc.Town as 'CollectTown',

			cc.Postcode as 'CollectPostcode',
			 c.marketfg as 'Customer_Survey',
			 c.RepairAddressFlag as 'IsRepairAddress',
			 CAST(c.CLIENTID AS varchar(15)) as 'CLIENTID',
			 c.CLIENTCUSTREF,isnull(R.RetailClientName,'') as RetailClientName,
			 c.RetailClientID as RetailClient
			 

			

	FROM Customer c

	left join CustomerContacts cc on cc.CustomerId = c.CUSTOMERID
	left join RetailClient R on R.RetailCode=c.RetailClientID

	WHERE c.CustomerID = @CustomerID
GO


