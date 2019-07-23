--USE [ShopDirect]
--GO

/****** Object:  StoredProcedure [dbo].[GetCustomerProductsByGuidAllClients]    Script Date: 22/07/2019 12:21:06 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

--exec dbo.GetCustomerProductsByGuidAllClients '8563B2E8-639C-4080-A044-AB210C7CB49D'
--exec dbo.GetCustomerProductsByGuidAllClients__jonas2 '8563B2E8-639C-4080-A044-AB210C7CB49D'

--exec dbo.GetCustomerProductsByGuidAllClients_JonasTest '8563B2E8-639C-4080-A044-AB210C7CB49D'

CREATE PROCEDURE [dbo].[GetCustomerProductsByGuidAllClients_JonasTest]
@CustomerGuid nvarchar(max)
AS
CREATE TABLE #TmpTable121212
(
  [SERVICEID] [int] NULL,
  [STATUSID] [smallint] NULL,
  [VISITCD] [varchar](3) NULL,
  [CUSTAPLID] [int] NOT NULL,
  [MODEL] [varchar](25) NULL,
  [SNO] [varchar](30) NULL,
  [CONTRACTCANCELDATE] [date] NULL,
  [APPLIANCEPRICE] [float] NULL,
  [SUPPLYDAT] [date] NULL,
  [ServiceStart] [date] NULL,
  [ServiceExpiry] [date] NULL,
  [APPLIANCECD] [varchar](5) NULL,
  [OnlineBookingExcluded] [bit] NULL,
  [CONTRACTSTART] [date] NULL,
  [CONTRACTEXPIRES] [date] NULL,
  [MFR] [varchar](3) NULL,
  [CUSTOMERID] [int] NOT NULL,
  [TITLE] [varchar](5) NULL,
  [FIRSTNAME] [varchar](20) NULL,
  [SURNAME] [varchar](50) NULL,
  [ADDR1] [varchar](60) NULL,
  [ADDR2] [varchar](60) NULL,
  [ADDR3] [varchar](60) NULL,
  [TOWN] [varchar](60) NULL,
  [COUNTY] [varchar](40) NULL,
  [POSTCODE] [varchar](8) NULL,
  [TEL1] [varchar](20) NULL,
  [TEL2] [varchar](20) NULL,
  [TEL3] [varchar](20) NULL,
  [NOTES] [varchar](max) NULL,
  [EMAIL] [varchar](128) NULL,
  [SENDBY] [smallint] NULL,
  [CLIENTID] [int] NULL,
  [COUNTRY] [varchar](50) NULL,
  [CustomerGuid] [uniqueidentifier] NULL
)

INSERT INTO #TmpTable121212
(
  [SERVICEID],
  [STATUSID],
  [VISITCD],
  [CUSTAPLID],
  [MODEL],
  [SNO],
  [CONTRACTCANCELDATE],
  [APPLIANCEPRICE],
  [SUPPLYDAT],
  [ServiceStart],
  [ServiceExpiry],
  [APPLIANCECD],
  [OnlineBookingExcluded],
  [CONTRACTSTART],
  [CONTRACTEXPIRES],
  [MFR],
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
  [SENDBY],
  [CLIENTID],
  [COUNTRY],
  [CustomerGuid]
)
SELECT
  s.[SERVICEID],
  s.[STATUSID],
  s.[VISITCD],
  ca.[CUSTAPLID],
  ca.[MODEL],
  ca.[SNO],
  ca.[CONTRACTCANCELDATE],
  ca.[APPLIANCEPRICE],
  ca.[SUPPLYDAT],
  ca.[ServiceStart],
  ca.[ServiceExpiry],
  ca.[APPLIANCECD],
  ca.[OnlineBookingExcluded],
  ca.[CONTRACTSTART],
  ca.[CONTRACTEXPIRES],
  ca.[MFR],
  c.[CUSTOMERID],
  c.[TITLE],
  c.[FIRSTNAME],
  c.[SURNAME],
  c.[ADDR1],
  c.[ADDR2],
  c.[ADDR3],
  c.[TOWN],
  c.[COUNTY],
  c.[POSTCODE],
  c.[TEL1],
  c.[TEL2],
  c.[TEL3],
  c.[NOTES],
  c.[EMAIL],
  c.[SENDBY],
  c.[CLIENTID],
  c.[COUNTRY],
  c.[CustomerGuid]
 FROM
   custapl ca
   INNER JOIN customer c on ca.CUSTOMERID = c.CUSTOMERID OR ca.OwnerCustomerID = c.CUSTOMERID
   LEFT OUTER JOIN [service] s ON ca.CUSTAPLID = s.CUSTAPLID
 WHERE
   c.CustomerGuid = cast(@CustomerGuid as uniqueidentifier)

select 
  tmp.serviceid,
  tmp.TITLE as 'Title',
  st.statusid,
  tmp.custaplid,
  tmp.FIRSTNAME as 'Forename',
  tmp.SURNAME as 'Surname',
  tmp.POSTCODE as 'Postcode',
  (tmp.ADDR1 +  isnull(',' + tmp.NOTES,'')) as 'Addr1',
  tmp.ADDR2 as 'Addr2',
  tmp.ADDR3 as 'Addr3',
  tmp.TEL1 as 'MobileTel',
  tmp.TEL2 as 'LandlineTel',
  tmp.TEL3 as 'TEL3',
  tmp.EMAIL as 'Email',
  tmp.TOWN as 'Town',
  tmp.COUNTY as 'County',
  tmp.COUNTRY as 'Country',
  D.EventDate  as 'VisitDate',
  tmp.CustomerId as 'CustomerId',
  tmp.customerGuid as 'CustomerGuid',
  CAST(tmp.SENDBY as int) as 'ContactMethod',
  m.MODEL as 'ItemCode', 
  tmp.clientid as ClientId,
  m.ModelId as ModelId,
  isnull(pa.[desc],'') as 'Description', 
  isnull(su.supplier,'') as 'Brand', 
  isnull(m.MODEL,'') as 'ModelNumber',
  isnull(m.notes,'') as 'Notes',
  isnull(m.schemafile,'') as 'ImageFileName',
  isnull(m.MFR,'')	as 'MFR',
  isnull(man.Name,'') as Manufacturer,
  tmp.sno as SerialNumber,
  tmp.APPLIANCEPRICE as AppliancePrice,
  tmp.supplydat as 'SupplyDat',
  tmp.ServiceStart as ServiceStartDate,
  tmp.ServiceExpiry as ServiceExpiryDate,	
  st.Status as [status],
  st.statusCustomerDescription,
  tmp.appliancecd as appliancecd,
  tmp.OnlineBookingExcluded as OnlineBookingExcluded,
  m.processId as processId,
  tmp.CONTRACTSTART,
  tmp.CONTRACTEXPIRES,
  pa.MonitorFg as MonitorFlag,
  ft.FraudResult as FraudResult
FROM
(
  select max(serviceid) latestServiceiD, custaplid
  from #TmpTable121212
  where SERVICEID IS NOT NULL
  group by CUSTAPLID
)
as latestService
INNER JOIN #TmpTable121212 tmp on tmp.SERVICEID = latestService.latestServiceiD and tmp.CUSTAPLID = latestService.CUSTAPLID AND tmp.SERVICEID IS NOT NULL
LEFT JOIN DiaryEnt D ON D.TagInteger1 = tmp.SERVICEID
left outer join model m on  tmp.MODEL = M.MODEL AND tmp.MFR = M.MFR and  tmp.APPLIANCECD = M.APPLIANCECD
left join Manufact Man on man.MFR = m.MFR
left outer join supplier su on su.supplierid = m.supplierid
left outer join pop_apl pa on tmp.APPLIANCECD = pa.APPLIANCECD
left join FraudTests ft on tmp.SERVICEID = ft.SERVICEID
left join pop_cc pc on pc.visitcd = tmp.visitcd
join [status] as st on st.statusid = tmp.statusid
UNION 
SELECT
  0 as serviceid, 
  tmp2.TITLE as 'Title',
  0 statusid,
  tmp2.custaplid,
  tmp2.FIRSTNAME as 'Forename',
  tmp2.SURNAME as 'Surname',
  tmp2.POSTCODE as 'Postcode',
  (tmp2.ADDR1 +  isnull(',' + tmp2.NOTES,'')) as 'Addr1',
  tmp2.ADDR2 as 'Addr2',
  tmp2.ADDR3 as 'Addr3',
  tmp2.TEL1 as 'MobileTel',
  tmp2.TEL2 as 'LandlineTel',
  tmp2.TEL3 as 'TEL3',
  tmp2.EMAIL as 'Email',
  tmp2.TOWN as 'Town',
  tmp2.COUNTY as 'County',
  tmp2.COUNTRY as 'Country',
  D.EventDate  as 'VisitDate',
  tmp2.CustomerId as 'CustomerId',
  tmp2.CustomerGuid as 'CustomerGuid',
  CAST(tmp2.SENDBY as int) as 'ContactMethod',
  m.MODEL as 'ItemCode', 
  tmp2.clientid as ClientId,
  m.ModelId as ModelId,
  isnull(pa.[desc],'') as 'Description', 
  isnull(s.supplier,'') as 'Brand', 
  isnull(m.MODEL,'') as 'ModelNumber',
  isnull(m.notes,'') as 'Notes',
  isnull(m.schemafile,'') as 'ImageFileName',
  isnull(m.MFR,'')	as 'MFR',
  isnull(man.Name,'') as Manufacturer,
  tmp2.sno as SerialNumber,
  tmp2.APPLIANCEPRICE as AppliancePrice,
  tmp2.supplydat as 'SupplyDat',
  tmp2.ServiceStart as ServiceStartDate,
  tmp2.ServiceExpiry as ServiceExpiryDate,
  '' as [status],
  '' statusCustomerDescription,
  tmp2.appliancecd as appliancecd,
  tmp2.OnlineBookingExcluded as OnlineBookingExcluded,
  m.processId as processId,
  tmp2.CONTRACTSTART,
  tmp2.CONTRACTEXPIRES,
  pa.MonitorFg as MonitorFlag,
  ft.FraudResult as FraudResult
FROM
	#TmpTable121212 tmp2
	left outer join model m on  tmp2.MODEL = M.MODEL AND tmp2.MFR = M.MFR and tmp2.APPLIANCECD = M.APPLIANCECD AND tmp2.SERVICEID IS NULL
	left join Manufact Man on man.MFR = m.MFR
	left outer join supplier s on s.supplierid = m.supplierid
	left outer join pop_apl pa on tmp2.APPLIANCECD = pa.APPLIANCECD
	left join service sr on sr.custaplid = tmp2.custaplid
	LEFT JOIN DiaryEnt D ON D.TagInteger1 = Sr.SERVICEID
	left join FraudTests as ft on ft.Serviceid=sr.SERVICEID
WHERE 
	 sr.serviceid is null
	 and ((DATEADD(year,3, tmp2.ServiceExpiry)>getdate() or tmp2.ServiceExpiry is null) or (DATEADD(year,3,tmp2.CONTRACTEXPIRES)>getdate()) or tmp2.CONTRACTEXPIRES is null) and tmp2.CONTRACTCANCELDATE is null 