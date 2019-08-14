USE [CAST_ClientDemo]
GO

/****** Object:  StoredProcedure [dbo].[RetrieveProduct]    Script Date: 14/08/2019 15:44:34 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[RetrieveProduct]    
  
 @ModelID  Int  
  
AS  
  
 -- variables   
  
 declare @Model varchar(max)  
  
 declare @Notes  varchar(max)  
  
   
  
 -- get model  
  
 select @Model = Model  
  
 from model  
  
 where MODELID = @ModelID  
  
   
  
 -- concatinations notes and select them  
  
 select @Notes =   
  
  stuff(  
  
  (select ('\n\r' + m2.notes + ' (' + m2.MFR + ')') AS [text()]   
  
  from model m2   
  
  where m1.MODEL = m2.model   
  
  for xml path('')),1,4,'')    
  
 from model m1  
  
 where m1.model = @Model  
  
 group by m1.model  
  
   
  
 -- replace symbols on \r\n  
  
 select @Notes = REPLACE(@Notes, '\n\r', char(13)+char(10))  
  
  
  
 SET NOCOUNT ON  
  
   
  
 --general info   
  
 SELECT model.MODEL as 'ItemCode',   
  
   isnull(model.description,'') as 'Description',   
  
   isnull(supplier.supplier,'') as 'Brand',   
   isnull(Brand,'') as 'ModelBrand',   
   isnull(model.ALTCODE,'') as 'ModelNum',  
  
   isnull(model.notes,'') as 'Notes',  
  
   isnull(model.schemafile,'') as 'ImageFileName',  
  
   isnull(model.MFR,'') as 'MFR',  
  
   isnull(rpr.SoftId,'') as 'SoftId',  
  
   isnull(model.ENTCATEGORY,'') AS 'Category',     
  
   isnull(model.ModelId,'') as 'ModelId',  
  
   CAST(rpr.ShowCallCentreRepair as BIT) as 'ShowCallCentreRepair',  
  
   isnull(model.PartsAvailable,'') as 'PartAvailable',  
  
   isnull(rpr.RepairFAQ,'')as 'RepairFaq',  
  
   isnull(rpr.CallCentreFAQ,'') as 'CCRepairFaq',  
  
   CASE WHEN       
  
     (SELECT COUNT(*)  
  
      FROM ModelXRef mxr   
  
      WHERE mxr.XMODModel = model.MODEL  
  
      ) > 0 THEN CAST(1 as BIT)  
  
     ELSE CAST(0 as BIT)  
  
    END as 'AlternativeFlag',  
  
  
  
   model.processid as 'BookRepairProcessId',  
   Case when (MANUALID is null) then '' else model.MODEL end as Model  
  
 FROM model  
  
 left outer join repairprofile rpr on rpr.repairid=model.repairid  
  
 left outer join supplier on supplier.supplierid=model.supplierid  
  
 where model.modelid = @ModelID  
  
   
  
 --Product Support  
  
 SELECT   
  
  CAST(isnull(rpr.RepairProductSupportEngineerID,0) as BIT) as 'ServiceFlag',  
  
  CASE  
  
   WHEN (RepairProductSupportEngineerID > 0) OR ((contact.TELNO > '')AND(contact.TELNO is not null)) THEN 'Call Helpline'  
  
   ELSE 'No service available, follow returns process'  
  
  END as 'ServiceText',  
  
  isnull(contact.DISPLAYNAME,'') as 'ServiceEngineerName',  
  
  isnull(contact.TELNO,'') as 'ServiceEngineerTelNo',  
  
  isnull(contact.NOTES,'') as 'ServiceEngineerNotes',  
  
  case when isnull(contact.WebURL,'') <> '' then  
  
    case when CHARINDEX('http',contact.WebURL) <= 0  
  
     then 'http://' + contact.WebURL  
  
     else contact.WebURL  
  
    end  
  
    else ''  
  
  end as 'ServiceUrlText',  
  
  isnull(contact.Email,'') as 'ServiceEngineerEmail',  
   Case when (MANUALID is null) then '' else model.MODEL end as Model  
  
 FROM model  
  
 left outer join repairprofile rpr on rpr.repairid=model.repairid  
  
 left join Enginrs contact on contact.ENGINEERID = rpr.RepairProductSupportEngineerID  
  
 where model.modelid = @ModelID  
  
   
  
 --CC repair  
  
 SELECT           
  
  CAST((CASE WHEN (isnull(rpr.RepairBookRepairEngineerID,0) <= 0 OR model.serviceexclude = 1) THEN 0  
  
     ELSE 1  
  
      END) as BIT) as 'ServiceFlag',  
  
  --rpr.RepairBookRepairDisableText as 'BookRepairText',  
  
  'Please read additional information below' as 'ServiceText',  
  
  isnull(rpcc.RepairProfileCCText,'') as 'ServiceCCText',  
  
  isnull(bkrep.DISPLAYNAME,'') as 'ServiceEngineerName',  
  
  isnull(bkrep.TELNO,'') as 'ServiceEngineerTelNo',  
  
  isnull(bkrep.NOTES,'') as 'ServiceEngineerNotes',  
  
  case when isnull(bkrep.WebURL,'') <> '' then  
  
    case when CHARINDEX('http',bkrep.WebURL) <= 0  
  
     then 'http://' + bkrep.WebURL  
  
     else bkrep.WebURL  
  
    end  
  
    else ''  
  
  end as 'ServiceUrlText',     
  
  isnull(bkrep.Email,'') as 'ServiceEngineerEmail'     
  
 FROM model  
  
 left outer join repairprofile rpr on rpr.repairid=model.repairid  
  
 left outer join supplier on supplier.supplierid=model.supplierid  
  
 left join Enginrs bkrep on bkrep.ENGINEERID = rpr.RepairBookRepairEngineerID  
  
 left join RepairProfileCC rpcc on rpcc.RepairProfileCCID = rpr.RepairProfileCCID  
  
 where model.modelid = @ModelID  
  
   
  
 --collect info  
  
 SELECT   
  
  CAST(isnull(rpr.CollectionAvailable,0) as BIT) as 'ServiceFlag',  
  
   isnull(bkrep.DISPLAYNAME,'') as 'ServiceEngineerName',  
  
   isnull(bkrep.TELNO,'') as 'ServiceEngineerTelNo',  
  
   isnull(bkrep.NOTES,'') as 'ServiceEngineerNotes',  
  
   case when isnull(bkrep.WebURL,'') <> '' then  
  
     case when CHARINDEX('http',bkrep.WebURL) <= 0  
  
      then 'http://' + bkrep.WebURL  
  
      else bkrep.WebURL  
  
     end  
  
     else ''  
  
   end as 'ServiceUrlText',     
  
   isnull(bkrep.Email,'') as 'ServiceEngineerEmail'  
  
 FROM model  
  
 left outer join repairprofile rpr on rpr.repairid=model.repairid  
  
 left outer join supplier on supplier.supplierid=model.supplierid  
  
 left join Enginrs bkrep on bkrep.ENGINEERID = rpr.RepairBookRepairEngineerID  
  
 where model.modelid = @ModelID  
  
   
  
 --Free spares  
  
 SELECT   
  
  CAST(isnull(rpr.RepairFreeSparesFlag,0) as BIT) as 'ServiceFlag',  
  
  'No service available' as 'ServiceText',  
  
  isnull(freesp.DISPLAYNAME,'') as 'ServiceEngineerName',  
  
  isnull(freesp.TELNO,'') as 'ServiceEngineerTelNo',  
  
  isnull(freesp.NOTES,'') as 'ServiceEngineerNotes',  
  
  case when isnull(freesp.WebURL,'') <> '' then  
  
    case when CHARINDEX('http',freesp.WebURL) <= 0  
  
     then 'http://' + freesp.WebURL  
  
     else freesp.WebURL  
  
    end  
  
    else ''  
  
  end as 'ServiceUrlText'     
  
 FROM model  
  
 left outer join repairprofile rpr on rpr.repairid=model.repairid  
  
 left outer join supplier on supplier.supplierid=model.supplierid  
  
 left join Enginrs freesp on freesp.ENGINEERID = rpr.RepairFreeSparesEngineerID  
  
 where model.modelid = @ModelID  
  
   
  
   
  
 --Charge spares  
  
 SELECT     
  
  CAST(isnull(rpr.RepairChargeableSparesFlag,0) as BIT) as 'ServiceFlag',  
  
  'No service available' as 'ServiceText',  
  
  isnull(charge.DISPLAYNAME,'') as 'ServiceEngineerName',  
  
  isnull(charge.TELNO,'') as 'ServiceEngineerTelNo',  
  
  isnull(charge.NOTES,'') as 'ServiceEngineerNotes',  
  
  case when isnull(charge.WebURL,'') <> '' then  
  
    case when CHARINDEX('http',charge.WebURL) <= 0  
  
     then 'http://' + charge.WebURL  
  
     else charge.WebURL  
  
    end  
  
    else ''  
  
  end as 'ServiceUrlText'        
  
 FROM model  
  
 left outer join repairprofile rpr on rpr.repairid=model.repairid  
  
 left outer join supplier on supplier.supplierid=model.supplierid  
  
 left join Enginrs charge on charge.ENGINEERID = rpr.RepairChargeableSparesEngineerID  
  
 where model.modelid = @ModelID  
  
   
  
  
GO


