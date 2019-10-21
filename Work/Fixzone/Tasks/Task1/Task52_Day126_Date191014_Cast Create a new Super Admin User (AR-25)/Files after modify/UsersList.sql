ALTER PROCEDURE [dbo].[UsersList]	
	@StoreId INT,
	@ReturnLines Int,
	@PageNumber  Int,
	@startRowNum Int OUTPUT,
	@endRowNum Int OUTPUT,
	@countItems Int OUTPUT
AS
	SET NOCOUNT ON
	
	set @startRowNum = (@PageNumber - 1) * @ReturnLines + 1

	--users list
	SELECT * 
	FROM (
		select ROW_NUMBER()OVER(ORDER BY usr.Level, usr.UserId) as 'RowNum',
		usr.Userid as 'EmployeeID',
		usr.Fullname as 'Name',
		usr.Level as 'Level',
		usr.Lastacdt as 'LastLogin', 
		usr.Created as 'UserCreated',
		CAST(usr.Enabled as BIT) as 'Enabled'
		from [userWeb] usr			
		WHERE usr.ClientID = @StoreId
	) AS UserList
	WHERE UserList.RowNum BETWEEN @startRowNum AND (@startRowNum + @ReturnLines - 1)
	order by UserList.Level, UserList.EmployeeID

	--list information
	select @countItems = isnull(count(*),0), 
		   @startRowNum = isnull(@startRowNum,0),
		   @endRowNum = 
				case when (@startRowNum + @ReturnLines - 1) > count(*) then count(*)
					else (@startRowNum + @ReturnLines - 1)
				end 
	from [userWeb]
	WHERE ClientID = @StoreId
GO


