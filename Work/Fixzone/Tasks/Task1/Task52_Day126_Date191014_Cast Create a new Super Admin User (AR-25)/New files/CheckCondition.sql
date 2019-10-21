CREATE FUNCTION dbo.CheckCondition
(
  @IsAnd bit,
  @Value1 varchar(100),
  @Value2 varchar(100)
) returns bit
AS
BEGIN
DECLARE
@ReturnValue bit

IF (@IsAnd = 1)
BEGIN
  IF (@Value1 = '' OR (@Value2 IS NOT NULL AND @Value2 LIKE @Value1))
    SET @ReturnValue = 1
  ELSE
    SET @ReturnValue = 0
END
ELSE
BEGIN
  IF (@Value1 = '' OR @Value2 IS NULL OR (@Value2 IS NOT NULL AND @Value2 NOT LIKE @Value1))
    SET @ReturnValue = 0
  ELSE
    SET @ReturnValue = 1
END

return @ReturnValue
END