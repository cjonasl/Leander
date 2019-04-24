USE [ShopDirect]
GO

/****** Object:  UserDefinedFunction [dbo].[fn_getCustomerName]    Script Date: 24/04/2019 13:44:43 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE FUNCTION [dbo].[fn_getCustomerName] (

     @Title varchar(5)=NULL,
	 @firstname varchar(20)=null,
	 @Surname varchar(50)=null

) returns varchar(80) as

 

begin

 declare @name nvarchar(80)=''
select @name=COALESCE(@Title, '')+' '+COALESCE(@firstname, '')+' '+COALESCE(@Surname, '')  ;

 


      return Upper(@Name)

end

GO


