USE [PASClientconnect]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[SearchCustomers]
	@Surname VARCHAR(100) = '',
	@Postcode VARCHAR(100) = '',
	@TelNo VARCHAR(100) = '',
	@PolicyNumber VARCHAR(100) = '',
	@ClientCustRef VARCHAR(100) = '',
	@Address VARCHAR(100) = '',
	@UseAndInWhereCondition bit,
	@ReturnLines INT,
	@PageNumber INT,
	@StoreId int
AS
	-- returns Customers list according to search criteria	
	
	SET @Surname = '%' + LTRIM(RTRIM(ISNULL(@Surname, ''))) + '%'
	SET @Postcode = '%' + LTRIM(RTRIM(ISNULL(@Postcode, ''))) + '%'
	SET @TelNo = '%' + LTRIM(RTRIM(ISNULL(@TelNo, ''))) + '%'
	SET @PolicyNumber = '%' + LTRIM(RTRIM(ISNULL(@PolicyNumber, ''))) + '%'
	SET @ClientCustRef = '%' + LTRIM(RTRIM(ISNULL(@ClientCustRef, ''))) + '%'
	SET @Address = '%' + LTRIM(RTRIM(ISNULL(@Address, ''))) + '%'
	DECLARE @postcodeWithoutSpaces VARCHAR(10)
	SET @postcodeWithoutSpaces = REPLACE(LTRIM(RTRIM(@Postcode)),' ','')

	DECLARE @startRowNum integer
	set @startRowNum = (@PageNumber - 1) * @ReturnLines + 1

	DECLARE @Results TABLE(
		CustomerId INT,
		--RepairNo VARCHAR(20),
		Logged DATETIME,
		CustomerName VARCHAR(MAX),
		PostCode VARCHAR(8),
			Address varchar(50),
		--Description VARCHAR(40),
		--Status VARCHAR(30),
		RecordCount INT,
		RetailClientid varchar(20),
		--LeadTime DATETIME,
		StoreId INT,
		StoreName VARCHAR(50)
		)

if (@UseAndInWhereCondition = 1)
BEGIN
	WITH CustomersFiltered AS
	(SELECT ROW_NUMBER() OVER (ORDER BY CreatedDateTime DESC) AS RowNumber,
			ROW_NUMBER() OVER (ORDER BY CreatedDateTime ) AS InverseRowNumber,
			CustomerId,	CreatedDateTime AS Logged, CustomerName,
			Address, Postcode, RetailClientName, StoreId, StoreName		 
		FROM   (select distinct customer.CreatedDateTime ,customer.CustomerId,	COALESCE(customer.title, '') +' '+		COALESCE(customer.Firstname, '') + ' ' + COALESCE(customer.surname, '') AS CustomerName,
		customer.ADDR1+' ,'+		COALESCE(customer.addr2, '') + ' ' as Address,Client.ClientName,customer.postcode,r.RetailClientName,customer.CLIENTID  as StoreId,
		Client.ClientName as StoreName
		from  customer  					
		JOIN Client on customer.CLIENTID = Client.ClientID	
		left join retailclient r on r.RetailID = customer.RetailClientID
		left join Custapl CA on ( isnull(ca.ownercustomerid,CA.CUSTOMERID)=customer.customerid  )	
		where
		  (@Surname ='%%' or isnull(customer.SURNAME, '') like @Surname) and
          (@Postcode ='%%' or isnull(customer.POSTCODE, '') like @Postcode or isnull(customer.POSTCODE,'') like @postcodeWithoutSpaces or replace(isnull(customer.POSTCODE, ''), ' ', '') like @postcodeWithoutSpaces) and
		  (@TelNo ='%%' or isnull(customer.TEL1, '') like @TelNo or isnull(customer.TEL2, '') like @TelNo) and
		  (@PolicyNumber ='%%' or isnull(CA.POLICYNUMBER, '') like @PolicyNumber) and
		  (@ClientCustRef ='%%' or isnull(customer.CLIENTCUSTREF, '') like @ClientCustRef) and
		  (@Address ='%%' or isnull(customer.ADDR1, '') like @Address)
		)	as R
     )
	INSERT INTO @Results
	SELECT J.CUSTOMERID,J.Logged, J.CustomerName, J.Postcode,J.Address, RowNumber+InverseRowNumber-1 AS RecordCount, J.RetailClientName, J.StoreId, J.StoreName
	FROM CustomersFiltered J
	WHERE J.RowNumber BETWEEN @startRowNum AND @startRowNum + @ReturnLines - 1
END
ELSE
BEGIN
	WITH CustomersFiltered AS
	(SELECT ROW_NUMBER() OVER (ORDER BY CreatedDateTime DESC) AS RowNumber,
			ROW_NUMBER() OVER (ORDER BY CreatedDateTime ) AS InverseRowNumber,
			CustomerId,	CreatedDateTime AS Logged, CustomerName,
			Address, Postcode, RetailClientName, StoreId, StoreName		 
		FROM   (select distinct customer.CreatedDateTime ,customer.CustomerId,	COALESCE(customer.title, '') +' '+		COALESCE(customer.Firstname, '') + ' ' + COALESCE(customer.surname, '') AS CustomerName,
		customer.ADDR1+' ,'+		COALESCE(customer.addr2, '') + ' ' as Address,Client.ClientName,customer.postcode,r.RetailClientName,customer.CLIENTID  as StoreId,
		Client.ClientName as StoreName
		from  customer  					
		JOIN Client on customer.CLIENTID = Client.ClientID	
		left join retailclient r on r.RetailID = customer.RetailClientID
		left join Custapl CA on ( isnull(ca.ownercustomerid,CA.CUSTOMERID)=customer.customerid  )	
		where
		  (@Surname ='%%' or isnull(customer.SURNAME, '') like @Surname) or
          (@Postcode ='%%' or isnull(customer.POSTCODE, '') like @Postcode or isnull(customer.POSTCODE,'') like @postcodeWithoutSpaces or replace(isnull(customer.POSTCODE, ''), ' ', '') like @postcodeWithoutSpaces) or
		  (@TelNo ='%%' or isnull(customer.TEL1, '') like @TelNo or isnull(customer.TEL2, '') like @TelNo) or
		  (@PolicyNumber ='%%' or isnull(CA.POLICYNUMBER, '') like @PolicyNumber) or
		  (@ClientCustRef ='%%' or isnull(customer.CLIENTCUSTREF, '') like @ClientCustRef) or
		  (@Address ='%%' or isnull(customer.ADDR1, '') like @Address)
		)	as R
     )
	INSERT INTO @Results
	SELECT J.CUSTOMERID,J.Logged, J.CustomerName, J.Postcode,J.Address, RowNumber+InverseRowNumber-1 AS RecordCount, J.RetailClientName, J.StoreId, J.StoreName
	FROM CustomersFiltered J
	WHERE J.RowNumber BETWEEN @startRowNum AND @startRowNum + @ReturnLines - 1
END	 
	DECLARE @RecordCount integer
	SELECT TOP 1 @RecordCount = RecordCount
	FROM @Results

	SELECT R.CUSTOMERID as 'CUSTOMERID',  R.Logged as 'Logged', R.CustomerName as 'CustomerName', 
		R.PostCode as 'Postcode',  
		R.Address as Address,
		case when R.StoreId is not null then R.StoreId
			else 0		
		end as 'StoreId',
		case when R.StoreName is not null then R.StoreName
			else ''
		end as 'StoreName',RetailClientid,
		isnull(@RecordCount,0) as 'ElemCount', isnull(@startRowNum,0) as 'StartElem', 
			case when (@startRowNum + @ReturnLines - 1) > @RecordCount then @RecordCount
				else (@startRowNum + @ReturnLines - 1)
			end as 'LastElem'
	FROM @Results R
GO


