CREATE FUNCTION fn_Help2GetValue
(@Dt date)
returns varchar(10)
AS
BEGIN
DECLARE
@ReturnValue varchar(10)

IF (@Dt IS NULL)
  SET @ReturnValue = 'N/A'
ELSE
  SET @ReturnValue = CASE WHEN DAY(@Dt) < 10 THEN '0' ELSE '' END + CAST(DAY(@Dt) AS varchar(2)) + '/' + CASE WHEN MONTH(@Dt) < 10 THEN '0' ELSE '' END + CAST(MONTH(@Dt) AS varchar(2)) + '/' + CAST(YEAR(@Dt) AS varchar(4))

return @ReturnValue
END