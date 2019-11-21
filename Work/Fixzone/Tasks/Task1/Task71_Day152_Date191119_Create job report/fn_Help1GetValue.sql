CREATE FUNCTION fn_Help1GetValue
(@Str varchar(max))
returns varchar(max)
AS
BEGIN
DECLARE
@ReturnValue varchar(max)

IF ISNULL(LTRIM(RTRIM(@Str)), '') = ''
  SET @ReturnValue = 'N/A'
ELSE
  SET @ReturnValue = LTRIM(RTRIM(@Str))

return @ReturnValue
END