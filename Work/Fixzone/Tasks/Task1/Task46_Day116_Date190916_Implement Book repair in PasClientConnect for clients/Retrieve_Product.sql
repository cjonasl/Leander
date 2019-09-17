USE [PAS]
GO

/****** Object:  StoredProcedure [dbo].[Retrieve_Product]    Script Date: 16/09/2019 10:25:48 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[Retrieve_Product]		
	@ModelID  Int
AS
	SET NOCOUNT ON
	
	SELECT	model.MODEL as 'ItemCode', 
			isnull(model.description,'') as 'Description', 
			isnull(supplier.supplier,'') as 'Brand', 
			isnull(model.ALTCODE,'') as 'ModelNumber',
			isnull(model.notes,'') as 'Notes',
			isnull(model.schemafile,'') as 'ImageFileName',
			isnull(model.MFR,'')	as 'MFR',

			CAST(isnull(rpr.RepairProductSupportEngineerID,0) as BIT) as 'ProductSupportFlag',
			CASE
				WHEN (RepairProductSupportEngineerID > 0) OR ((contact.TELNO > '')AND(contact.TELNO is not null)) THEN 'Call Helpline'
				ELSE 'No service available, follow returns process'
			END as 'ProductSupportText',
			isnull(contact.DISPLAYNAME,'') as 'ProductSupportName',
			isnull(contact.TELNO,'') as 'ProductSupportTelNo',
			isnull(contact.NOTES,'') as 'ProductSupportNotes',
			isnull(contact.WebURL,'') as 'ProductSupportUrlText',
			isnull(contact.Email,'') as 'ProductSupportEmail',

			CAST(isnull(rpr.RepairFreeSparesFlag,0) as BIT) as 'FreeSparesFlag',
			'No service available' as 'FreeSparesText',
			isnull(freesp.DISPLAYNAME,'') as 'FreeSparesName',
			isnull(freesp.TELNO,'') as 'FreeSparesTelNo',
			isnull(freesp.NOTES,'') as 'FreeSparesNotes',
			isnull(freesp.WebURL,'') as 'FreeSparesUrlText',
			
			CAST(isnull(rpr.RepairChargeableSparesFlag,0) as BIT) as 'ChargeableFlag',
			'No service available' as 'ChargeableText',
			isnull(charge.DISPLAYNAME,'') as 'ChargeableName',
			isnull(charge.TELNO,'') as 'ChargeableTelNo',
			isnull(charge.NOTES,'') as 'ChargeableNotes',
			isnull(charge.WebURL,'') as 'ChargeableUrlText',
			apl.monitorfg as MonitorFlag,
						
			CAST((CASE WHEN (--			isnull(model.REPAIRID,0) <= 0 OR 
			model.serviceexclude = 1) THEN 0
				  ELSE 1
		     END) as BIT) as 'BookRepairFlag',

			'No service available, follow returns process' as 'BookRepairText',
			isnull(rpcc.RepairProfileCCText,'') as 'BookRepairCCText',
			'' as 'BookRepairName',
			'' as 'BookRepairTelNo',
			'' as 'BookRepairNotes',
			'' as 'BookRepairUrlText',			
			'' as 'BookRepairEmail',			
			
			CAST(isnull(rpr.CollectionAvailable,0) as BIT) as 'CollectRepairFlag',isnull(Devicetype,0) as Devicetype,
			isnull(bkrep.DISPLAYNAME,'') as 'CollectRepairName',
			isnull(bkrep.TELNO,'') as 'CollectRepairTelNo',
			isnull(bkrep.NOTES,'') as 'CollectRepairNotes',
			isnull(bkrep.WebURL,'') as 'CollectRepairUrlText',			
			isnull(bkrep.Email,'') as 'CollectRepairEmail',

			isnull(rpr.SoftId,'') as 'SoftId',
			isnull(model.ENTCATEGORY,'') AS 'Category',			
			isnull(model.ModelId,'') as 'ModelId',
			CAST(rpr.ShowCallCentreRepair as BIT) as 'ShowCallCentreRepair',
			isnull(model.PartsAvailable,'') as 'PartAvailable',
			
			case 
				when rpr.CollectionAvailable = 1 THEN isnull(rpr.CallCentreFAQ,'')
				ELSE isnull(rpr.RepairFAQ,'')
			END as 'RepairFaq',

			CASE WHEN 			 
					(SELECT COUNT(*)
					 FROM ModelXRef mxr 
					 WHERE mxr.XMODModel = model.MODEL
						) > 0 THEN CAST(1 as BIT)
					ELSE CAST(0 as BIT)
			 END as 'AlternativeFlag',

			model.processid as 'BookRepairProcessId',

			CASE				
				WHEN model.PNC = 'Y' then 'AEP'
				else ''
			end as 'ModelPnc',isnull(Man.ContactInfo,'Please contact the manufacture') as 'ManufactContactdetails',
			man.NAME as 'ManufactName',TroubleshootID, apl.appliancecd as ApplianceCD, 
			apl.[desc]  as ApplianceDesc , 
			case isnull(model.SKILLS,'') when ''	then isnull(apl.DEFAULTSKILLS,'')
			else model.SKILLS  end as SKILLS
			
			--isnull(apl.DEFAULTSKILLS,'')+isnull(model.SKILLS,'') as skills
	FROM model 
	 join Manufact man on man.MFR=Model.MFR
			 join pop_apl apl on apl.APPLIANCECD=model.APPLIANCECD
	left outer join repairprofile rpr on rpr.repairid=model.repairid
	left outer join supplier on supplier.supplierid=model.supplierid
	left join Enginrs bkrep on bkrep.ENGINEERID = rpr.RepairBookRepairEngineerID
	left join Enginrs charge on charge.ENGINEERID = rpr.RepairChargeableSparesEngineerID
	left join Enginrs freesp on freesp.ENGINEERID = rpr.RepairFreeSparesEngineerID
	left join Enginrs contact on contact.ENGINEERID = rpr.RepairProductSupportEngineerID
	left join RepairProfileCC rpcc on rpcc.RepairProfileCCID = rpr.RepairProfileCCID
	left join ProcessKey procKey on procKey.Model = model.MODEL
			AND procKey.MFR = model.MFR
			AND procKey.ApplianceCD = model.APPLIANCECD
			
	where model.modelid = @modelid
GO


