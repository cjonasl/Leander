USE [PAS]
GO

/****** Object:  StoredProcedure [dbo].[SearchJobs]    Script Date: 19/08/2019 16:06:50 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[SearchJobs]
@Jobid varchar(20)='',
	@Surname VARCHAR(50)='',
	@Postcode VARCHAR(10)='',
	@TelNo VARCHAR(20)='',
	@PolicyNumber VARCHAR(20)='',
	@ClientCustRef VARCHAR(25)='',
	@Address VARCHAR(20)='',
	@ReturnLines INT,
	@PageNumber INT,
	@StoreId int
AS
	
	-- returns jobs list according to search criteria	
	SET @Jobid = '%' + LTRIM(RTRIM(@Jobid)) + '%'
	SET @Surname = '%' + LTRIM(RTRIM(@Surname)) + '%'
	SET @Postcode = '%' + LTRIM(RTRIM(@Postcode)) + '%'
	SET @TelNo = '%' + LTRIM(RTRIM(@TelNo)) + '%'
	SET @PolicyNumber = '%' + LTRIM(RTRIM(@PolicyNumber)) + '%'
	SET @ClientCustRef = '%' + LTRIM(RTRIM(@ClientCustRef)) + '%'
	SET @Address = '%' + LTRIM(RTRIM(@Address)) + '%'
	DECLARE @postcodeWithoutSpaces VARCHAR(10)
	SET @postcodeWithoutSpaces = REPLACE(LTRIM(RTRIM(@Postcode)),' ','')

	DECLARE @startRowNum integer
	set @startRowNum = (@PageNumber - 1) * @ReturnLines + 1

	DECLARE @Results TABLE(
		Id INT,
		RepairNo VARCHAR(20),
		Logged DATETIME,
		CustomerName VARCHAR(MAX),
		PostCode VARCHAR(8),
			Address varchar(50),
		Description VARCHAR(40),
		Status VARCHAR(30),
		RecordCount INT,
		LeadTime DATETIME,
		StoreId INT,
		StoreName VARCHAR(50)
		)

		 
	;WITH JobsFiltered AS
	(SELECT ROW_NUMBER() OVER (ORDER BY calldatetime DESC) AS RowNumber,
			ROW_NUMBER() OVER (ORDER BY calldatetime ) AS InverseRowNumber,
			serviceid AS Id,
			'FZ'+CAST(serviceid as char(10)) AS RepairNo,
			calldatetime AS Logged,
			COALESCE(Firstname, '') + ' ' + COALESCE(surname, '') AS CustomerName,
			 Postcode, Address,
			Description,
			status AS Status,
			ExpectedTurnaroundDays AS LeadTime,
			 StoreId,
			ClientName as StoreName
		FROM   (
		
		select distinct serviceid ,calldatetime,Client.ClientName,service.CLIENTID as StoreId,Enginrs.ExpectedTurnaroundDays,status.status,	customer.Firstname,customer.surname,
		customer.postcode,customer.ADDR1 as Address,model.description AS Description
		
		 from service
		INNER JOIN customer on service.customerid=customer.customerid
		INNER JOIN custapl on service.custaplid=custapl.custaplid
		LEFT JOIN model on custapl.model=model.model and custapl.MFR = model.MFR  and custapl.APPLIANCECD=model.APPLIANCECD
		left JOIN status on service.statusid=status.statusid
		LEFT JOIN RepairProfile on service.RepairId = RepairProfile.RepairID
		LEFT JOIN Enginrs on RepairProfile.RepairBookRepairEngineerID = Enginrs.ENGINEERID 					
		JOIN Client on service.CLIENTID = Client.ClientID
		LEFT join DiaryEnt de on de.TagInteger1 = Service.SERVICEID
		where (isnull(customer.POSTCODE,'') like @Postcode or isnull(customer.POSTCODE,'') like @postcodeWithoutSpaces) 
		and  isnull(customer.ADDR1,'') like @Address 
		and isnull(customer.CLIENTCUSTREF,'') like @ClientCustRef  
		 and (isnull(customer.TEL1,'') like @TelNo or isnull(customer.TEL2,'') like @TelNo)
		 and isnull(custapl.POLICYNUMBER,'') like @PolicyNumber and  isnull(customer.SURNAME,'') like @Surname	and isnull(service.serviceid,'') like @Jobid
		)	as R
		)	
			 
 --select * from JobsFiltered
	INSERT INTO @Results
	SELECT J.Id, J.RepairNo, J.Logged, J.CustomerName, J.Postcode,J.Address, J.Description, J.Status, RowNumber+InverseRowNumber-1 AS RecordCount, J.LeadTime, J.StoreId, J.StoreName
	FROM JobsFiltered J
	WHERE J.RowNumber BETWEEN @startRowNum AND @startRowNum + @ReturnLines - 1
	
	DECLARE @RecordCount integer
	SELECT TOP 1 @RecordCount = RecordCount
	FROM @Results

	

	SELECT R.Id as 'Id', R.RepairNo as 'RepairNo', R.Logged as 'Logged', R.CustomerName as 'CustomerName', 
		R.PostCode as 'Postcode', R.Description as 'Description', R.Status as 'Status', 
		case when R.LeadTime is not null then CONVERT(varchar(12), R.LeadTime, 103)
			else ''
		end as 'LeadTime',R.Address as Address,
		case when R.StoreId is not null then R.StoreId
			else 0		
		end as 'StoreId',
		case when R.StoreName is not null then R.StoreName
			else ''
		end as 'StoreName',
		isnull(@RecordCount,0) as 'ElemCount', isnull(@startRowNum,0) as 'StartElem', 
			case when (@startRowNum + @ReturnLines - 1) > @RecordCount then @RecordCount
				else (@startRowNum + @ReturnLines - 1)
			end as 'LastElem'
	FROM @Results R

GO


