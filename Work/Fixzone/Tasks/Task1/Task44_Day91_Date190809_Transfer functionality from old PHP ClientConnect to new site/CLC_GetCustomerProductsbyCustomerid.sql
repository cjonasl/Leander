USE [PAS]
GO

/****** Object:  StoredProcedure [dbo].[CLC_GetCustomerProductsbyCustomerid]    Script Date: 09/09/2019 11:58:21 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- Stored Procedure

--USE [ShopDirect]
--GO
--/****** Object:  StoredProcedure [dbo].[GetCustomerProducts]    Script Date: 17/03/2017 11:56:00 ******/
--SET ANSI_NULLS ON
--GO
--SET QUOTED_IDENTIFIER ON
--GO

ALTER PROCEDURE [dbo].[CLC_GetCustomerProductsbyCustomerid]

	@CustomerID INT = null

AS




	select  c.TITLE as 'Title', ca.custaplid,ca.CUSTOMERID,

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

			isnull(m.MODEL,'Model not found') as 'ItemCode', 
			c.clientid as ClientId,
			m.ModelId as ModelId,
			isnull(m.DESCRIPTION,'') as 'Description',isnull(m.PNC,'') as ModelPnc,
			isnull(su.supplier,'') as 'Brand', 
			isnull(m.ALTCODE,'') as 'ModelNumber',
			isnull(m.notes,'') as 'Notes',
			isnull(m.schemafile,'') as 'ImageFileName',
			isnull(m.MFR,'MFR not found')	as 'MFR',
			isnull(man.Name,'Manufacture not found') as Manufacturer,
			isnull(ca.sno,'') as SerialNumber
			,pa.APPLIANCECD,
			--ca.APPLIANCEPRICE as AppliancePrice,
			ca.supplydat as 'SupplyDat',ca.supplydat as 'DateOfPurchase'
			,			ca.CONTRACTSTART  as CONTRACTSTART,
			ca.CONTRACTEXPIRES,ca.PolicyNumber,isnull(ca.OwnerCustomerID ,0)  as OwnerCustomerId
		
			

 from 
	
	customer c 
	 join Custapl ca on ca.customerid= c.customerid
		
	left outer join model m on  CA.MODEL = M.MODEL AND CA.MFR = M.MFR and  CA.APPLIANCECD = M.APPLIANCECD

	left join Manufact Man on man.MFR =m.MFR
	left outer join supplier su on su.supplierid=m.supplierid
	left outer join pop_apl pa on ca.APPLIANCECD=pa.APPLIANCECD
	
	WHERE 
(ca.model<> 'DPAFAIL' and ca.APPLIANCECD<>'DPAF') and
c.CUSTOMERID=@CustomerID  or ca.OwnerCustomerID=@CustomerID
GO


