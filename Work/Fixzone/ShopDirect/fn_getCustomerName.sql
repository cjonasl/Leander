USE [ShopDirect_test]
GO

CREATE FUNCTION [dbo].[fn_getCustomerName]
(
  @Title varchar(5),
  @FirstName varchar(20),
  @SurName varchar(50)
)
RETURNS varchar(80)
AS
BEGIN
 return LTRIM(REPLACE(LTRIM(COALESCE(UPPER(LEFT(@Title, 1)) + LOWER(RIGHT(@Title, LEN(@Title) - 1)), '')) + ' ' + LTRIM(COALESCE(UPPER(LEFT(@FirstName, 1)) + LOWER(RIGHT(@FirstName, LEN(@FirstName) - 1)),'')) + ' '   + LTRIM(COALESCE(UPPER(LEFT(@SurName, 1)) +  LOWER(RIGHT(@SurName, LEN(@SurName) - 1)),'')), '  ', ' '))
END