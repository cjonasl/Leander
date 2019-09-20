SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ProductListInfo]
	@Clientid int
AS
	declare @ClientModelRestriction bit
	select @ClientModelRestriction = ClientModelRestriction   from client where clientid=@Clientid
if(@ClientModelRestriction=1)
	 --products list
begin
	 SELECT 
			 m1.MODELID,
			 m1.MODEL,
			 m1.description,
			 supplier.supplier,
			 m1.ALTCODE,
			 t1.model AS 'SearchCriteria1',
			 t1.altcode AS 'SearchCriteria1',
			 t1.Supplier AS 'SearchCriteria1'
	  from model m1
	  left outer join supplier on supplier.supplierid=m1.supplierid
	  join (
			 select model as 'model', m2.altcode, s2.Supplier, m2.mfr as 'mfr'
			 from model m2  join Manufact man on man.MFR=m2.MFR
			 join pop_apl apl on apl.APPLIANCECD=m2.APPLIANCECD
			 left outer join supplier s2 on s2.supplierid = m2.supplierid		 
	  ) t1 on t1.MODEL = m1.MODEL
			 and t1.mfr = m1.MFR 
	end	
	else
	begin
	 SELECT
			 m1.MODELID,
			 m1.MODEL, m1.description,
			 supplier.supplier,
			 m1.ALTCODE,
			 CASE WHEN 			 
					(SELECT COUNT(*)
					 FROM ModelXRef mxr 
					 WHERE mxr.XMODModel = t1.MODEL
						) > 0 THEN 1
					ELSE 0
			 END,
             t1.model AS 'SearchCriteria1', t1.altcode AS 'SearchCriteria1', t1.Supplier AS 'SearchCriteria1'
	  from model m1
	  left outer join supplier on supplier.supplierid=m1.supplierid
	  join (
			 select model as 'model', m2.altcode, s2.Supplier, m2.mfr as 'mfr'
			 from model m2  join Manufact man on man.MFR=m2.MFR
			 join pop_apl apl on apl.APPLIANCECD=m2.APPLIANCECD
			 left outer join supplier s2 on s2.supplierid = m2.supplierid
			 
	  ) t1 on t1.MODEL = m1.MODEL
			 and t1.mfr = m1.MFR 	
		join clientmodelrestriction CM on 	cm.model=m1.MODEL AND cm.ApplianceCD=m1.APPLIANCECD
		AND cm.MFR =m1.mfr AND cm.ClientID=@Clientid
	end
GO


