CREATE FUNCTION fn_GetMostRecentDate
(
  @Date1 date,
  @Date2 date,
  @Date3 date
)
returns date
AS
BEGIN
DECLARE @MostRecentDate date

SET @MostRecentDate =
    CASE
        WHEN @Date1 >= @Date2 AND @Date1 >= @Date3 THEN @Date1
        WHEN @Date2 >= @Date1 AND @Date2 >= @Date3 THEN @Date2
        WHEN @Date3 >= @Date1 AND @Date3 >= @Date2 THEN @Date3
        ELSE @Date1
    END

return @MostRecentDate
END