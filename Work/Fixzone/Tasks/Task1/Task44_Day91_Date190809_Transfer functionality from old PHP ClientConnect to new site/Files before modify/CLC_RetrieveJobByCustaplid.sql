USE [PAS]
GO

/****** Object:  StoredProcedure [dbo].[CLC_RetrieveJobByCustaplid]    Script Date: 11/09/2019 13:16:03 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[CLC_RetrieveJobByCustaplid]

	@Custaplid INT

	

AS	

	-- retrieves job information

	SELECT

		/*customer info*/

		CA.CUSTOMERID AS 'CustomerId',	

		COALESCE(C.Firstname, '') + ' ' + COALESCE(C.surname, '') AS 'CustomerName',

		COALESCE(C.Firstname, '') AS 'Forename',

		COALESCE(C.surname, '') AS 'Surname',

		C.ADDR1		AS 'Addr1',

		C.ADDR2		AS 'Addr2',

		C.ADDR3		AS 'Addr3',

		C.POSTCODE	AS 'PostCode',

		C.TOWN		AS 'Town',

		C.COUNTY	AS 'County',

		C.COUNTRY	AS 'Country',

		C.TITLE	    AS 'Title',



		/*contact info*/

		C.EMAIL AS 'Email',

		C.TEL1 AS 'MobileNumber',

		C.TEL2 AS 'LandlineNumber',

		C.FAX AS 'FaxNumber',		

		C.SENDBY AS 'PreferredMethod',

		S.VISITCD,

		/*job status*/

		CA.TrackingID AS 'CurrentStatus',

		(CAST(ST.Status as varchar(max)) + '\' + CAST(SUBST.Status as VARCHAR(MAX))) AS 'ActualJobStatus',

	

		/* product info */

		CA.MODEL AS 'ItemNumber',

		

		CA.SNO AS 'SerialNumber',

		CA.UDF1 as 'RetailedInvoiceDate',

		CA.SUPPLYDAT AS 'DateOfPurchase',

	
		CA.CUSTAPLID as 'CustaplId',

		D.DiaryID as 'DiaryID',D.EventDate as VisitDate,

		CA.DOPTYPE as 'ProofOfPurchase',

		cast(CA.GUARANTEETYPE as INT) as 'GuaranteeType',

		CA.SUPPLIER as 'RetailerName',
		CA.SUPPLIERID as 'RetailerId',
		CA.SUPCONT as 'RetailerReference',

		S.CLIENTREF as 'ClientRef',

		convert(int,CA.UDF5) as 'CustomerType',

		S.POLICYNO	as PolicyNumber,

		



		/*repair info */

		CC.[DESC] AS 'RepairType',

		S.CALLTYPE AS 'FaultType',

		S.REPFAULT AS 'FaultDescription',

		CASE WHEN E.NAME is null or E.NAME = ''

			THEN 'Not Booked'

			else E.NAME

		END as 'RepairAgent',

		--E.NAME AS 'RepairAgent',

		D.LabourCost AS 'RepairCost',

		D.TagString2 AS 'RepairCostPaid',

		CC.VISITCD AS 'SelectedType',		

		E.ENGINEERID AS 'EngineerId',		

		E.DISPLAYNAME as 'EngineerDisplayname',

		E.ADDRESS as 'EngineerAddress',

		E.Postcode as 'EngineerPostcode',

		E.TELNO as 'EngineerTelNo1',

		E.TELNO2 as 'EngineerTelNo2',

		E.EMAIL as 'EngineerEmail',

		E.WEBUrl as 'EngineerWeb',

		E.Notes as 'EngineerOpenHours',

		E.EngMemo as 'EngineerNotes',

		CAST(CASE

			WHEN S.STATUSID < 5 THEN 1

			ELSE 0

		END AS BIT) AS 'IsEditEnabled',

		S.ClientId as 'StoreNumber',

		CL.ClientName as 'StoreName',

		CAST(Case when S.StatusId in ( 0,12) THEN 0

		ELSE 1

		END AS BIT) as 'IsAccepted',

		g.GuarLabour as 'WarrantyLabour',

		g.GuarParts as 'WarrantyParts',

		s.SERVICEID as 'ServiceId',

		C.SENDBY as 'ContactMethod',


		isnull(s.CALLSHEETNO,'') as AgentReferenceNo,c.MARKETFG as 'Customer_Survey', 
		c.RepairAddressFlag as 'IsRepairAddress',cast(s.statusid as int) as StatusId
		--,		Man.ContactInfo as 'ManufactContactdetails'
,st.StatusDesc as StatusText


	FROM 

	Customer C 

	INNER JOIN Custapl CA ON c.customerid = CA.customerid or  c.customerid = CA.OwnerCustomerID
LEFT  join service S on s.CUSTAPLID= ca.CUSTAPLID --and s.VISITCD  not in ('020','022','014','023','024')
LEFT join SpecJobMapping SM on sm.VisitType = s.VISITCD 
	--LEFT JOIN Model M ON CA.MODEL = M.MODEL AND CA.MFR = M.MFR and  CA.APPLIANCECD = M.APPLIANCECD
	----left join Manufact Man on man.MFR =m.MFR
	LEFT JOIN DiaryEnt D ON D.TagInteger1 = S.SERVICEID

	LEFT JOIN Enginrs E ON D.UserId = E.ENGINEERID	

	LEFT JOIN POP_CC CC	ON S.VISITCD =  CC.VISITCD

	LEFT JOIN [Status] ST ON ST.StatusID = s.STATUSID

	LEFT JOIN SubStatus SUBST ON SUBST.StatusID = ST.StatusID AND SUBST.SubStatusID = S.SUBSTATUS

	LEFT JOIN Client CL on CL.CLIENTID = S.CLIENTID

	LEFT JOIN [Guarantee] g on g.GuarID = CA.Guaranteetype



	WHERE ca.CUSTAPLID=@Custaplid

and sm.DummyJob<>1

	--select * from specjobmapping
GO


