CREATE FUNCTION [dbo].[fn_getCustomerTel]
(
  @Tel1 varchar(20),
  @Tel2 varchar(20),
  @Tel3 varchar(20)
)
RETURNS varchar(20)
AS
BEGIN
DECLARE
@Tel varchar(20)

SET @Tel =
  CASE LEFT(@Tel1, 2) 
    WHEN '07' THEN @Tel1
    ELSE  
    CASE LEFT(@Tel2, 2)
      WHEN '07' THEN @Tel2
      ELSE @Tel3
    END 
  END

return @Tel
END