CREATE PROCEDURE [dbo].[CLC_GetCustomerProductsbyCustomerIdAndClientId]
@CustomerId int,
@ClientId int
AS

CREATE TABLE #TmpTableCustomer
(
  [CUSTOMERID] int NOT NULL,
  [TITLE] varchar(5) NULL,
  [FIRSTNAME] varchar(20) NULL,
  [SURNAME] varchar(50) NULL,
  [ADDR1] varchar(60) NULL,
  [ADDR2] varchar(60) NULL,
  [ADDR3] varchar(60) NULL,
  [TOWN] varchar(60) NULL,
  [COUNTY] varchar(40) NULL,
  [POSTCODE] varchar(50) NULL,
  [TEL1] varchar(20) NULL,
  [TEL2] varchar(20) NULL,
  [TEL3] varchar(20) NULL,
  [NOTES] varchar(max) NULL,
  [EMAIL] varchar(128) NULL,
  [CLIENTID] int NULL,
  [SENDBY] smallint NULL,
  [COUNTRY] varchar(50) NULL
)

CREATE TABLE #TmpCustApl
(
  [CUSTAPLID] int NOT NULL,
  [CUSTOMERID] int NULL,
  [APPLIANCECD] varchar(5) NULL,
  [MODEL] varchar(25) NULL,
  [SNO] varchar(30) NULL,
  [SUPPLYDAT] date NULL,
  [CONTRACTSTART] date NULL,
  [CONTRACTEXPIRES] date NULL,
  [MFR] varchar(3) NULL,
  [POLICYNUMBER] varchar(30) NULL,
  [OwnerCustomerID] int NULL
)

INSERT INTO #TmpTableCustomer
(
  [CUSTOMERID],
  [TITLE],
  [FIRSTNAME],
  [SURNAME],
  [ADDR1],
  [ADDR2],
  [ADDR3],
  [TOWN],
  [COUNTY],
  [POSTCODE],
  [TEL1],
  [TEL2],
  [TEL3],
  [NOTES],
  [EMAIL],
  [CLIENTID],
  [SENDBY],
  [COUNTRY]
)
SELECT
  [CUSTOMERID],
  [TITLE],
  [FIRSTNAME],
  [SURNAME],
  [ADDR1],
  [ADDR2],
  [ADDR3],
  [TOWN],
  [COUNTY],
  [POSTCODE],
  [TEL1],
  [TEL2],
  [TEL3],
  [NOTES],
  [EMAIL],
  [CLIENTID],
  [SENDBY],
  [COUNTRY]
FROM	
  Customer
WHERE
  [CUSTOMERID] IN
  (
   SELECT DISTINCT
     [CUSTOMERID]
   FROM
     [dbo].[Custapl]
   WHERE
     (CUSTOMERID = @CustomerId OR OwnerCustomerID = @CustomerId) AND
	 ISNULL(model, '') <> 'DPAFAIL' AND
	 ISNULL(APPLIANCECD, '') <> 'DPAF'
  )
  
INSERT INTO #TmpCustApl
(
  [CUSTAPLID],
  [CUSTOMERID],
  [APPLIANCECD],
  [MODEL],
  [SNO],
  [SUPPLYDAT],
  [CONTRACTSTART],
  [CONTRACTEXPIRES],
  [MFR],
  [POLICYNUMBER],
  [OwnerCustomerID]
)
SELECT
  [CUSTAPLID],
  [CUSTOMERID],
  [APPLIANCECD],
  [MODEL],
  [SNO],
  [SUPPLYDAT],
  [CONTRACTSTART],
  [CONTRACTEXPIRES],
  [MFR],
  [POLICYNUMBER],
  [OwnerCustomerID]
FROM
  [dbo].[Custapl]
WHERE
   (CUSTOMERID = @CustomerId OR OwnerCustomerID = @CustomerId) AND
   ISNULL(model, '') <> 'DPAFAIL' AND
   ISNULL(APPLIANCECD, '') <> 'DPAF'

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
	#TmpTableCustomer c 
	join #TmpCustApl ca on ca.customerid= c.customerid	
	left outer join model m on  CA.MODEL = M.MODEL AND CA.MFR = M.MFR and  CA.APPLIANCECD = M.APPLIANCECD
	left join Manufact Man on man.MFR =m.MFR
	left outer join supplier su on su.supplierid=m.supplierid
	left outer join pop_apl pa on ca.APPLIANCECD=pa.APPLIANCECD
	WHERE 
    ((0 = (SELECT COUNT(*) FROM [service] s1 WHERE s1.CUSTAPLID = ca.CUSTAPLID)) 
    or (@ClientId IN(SELECT DISTINCT ISNULL(s2.CLIENTID, 0) FROM [service] s2 WHERE s2.CUSTAPLID = ca.CUSTAPLID)))
GO
