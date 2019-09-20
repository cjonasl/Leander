USE [PAS]
GO

/****** Object:  StoredProcedure [dbo].[CLC_GetCustomerAppliance]    Script Date: 18/09/2019 11:41:55 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[CLC_GetCustomerAppliance]		
	@CustAPLID  Int
AS
	SET NOCOUNT ON
	
	SELECT	isnull(Man.ContactInfo,'Please contact the manufacture') as 'ManufactContactdetails',man.NAME as 'ManufactName' , sno as serialno,CUSTAPLID,CONTRACTSTART,CONTRACTEXPIRES, clientid,
	ca.PolicyNumber,m.DESCRIPTION ,m.MODEL as ItemCode,m.ALTCODE as modelnumber ,SNO as  SerialNumber, Modelid ,ca.SUPPLYDAT as DateOfPurchase,m.APPLIANCECD,
	case isnull(m.SKILLS,'') when ''	then isnull(apl.DEFAULTSKILLS,'')
			else m.SKILLS  end as SKILLS,SUPPLYDAT as DateofPurchase,CONVERT(VARCHAR(10), supplydat, 103)as DateofPurchaseAsString , c.CUSTOMERID,ca.APPLIANCEPRICE,  
			case when CONTRACTEXPIRES is not null then DATEADD(YY,-2,CONTRACTEXPIRES) else null end as MechanicalCoverStarts
	
	FROM custapl ca
	left join model m on ca.model=m.MODEL and m.APPLIANCECD=ca.APPLIANCECD and m.MFR=ca.MFR 
	left join Manufact man on man.MFR=m.MFR 
	left join POP_Apl apl  on apl.APPLIANCECD= m.APPLIANCECD
	join customer c on c.CUSTOMERID=ca.CUSTOMERID or c.CUSTOMERID=ca.OwnerCustomerID
	
	where ca.custaplid=@CustAPLID
GO
