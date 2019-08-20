USE [ShopDirect]
GO

/****** Object:  StoredProcedure [dbo].[JobList]    Script Date: 19/08/2019 14:25:05 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


ALTER PROCEDURE [dbo].[JobList]
	@Criteria VARCHAR(MAX),
	@ReturnLines INT,
	@PageNumber INT,
	@StoreId int
AS
	declare @ClientAllEngineers int
	declare @ClientEngineers varchar(max)
	--templorary table
	DECLARE @EngIds TABLE(				
		Id INT
	)
	
	-- get clientallengineers
	--select @ClientAllEngineers = clientallengineers, @ClientEngineers = ClientEngineers
	--from client 
	--where clientId = @StoreId

	---- get engineer list
	--IF (@ClientAllEngineers = 1)
	--BEGIN
	--	Insert into @EngIds(Id)
	--	select ENGINEERID
	--	from Enginrs
	--END 
	--ELSE 
	--BEGIN
	--	insert into @EngIds(Id)
	--	select Value from UTILfn_Split(@ClientEngineers,'~')
	--END
	
	-- returns jobs list according to search criteria	
	SET @Criteria = '%' + LTRIM(RTRIM(@Criteria)) + '%'
	
	DECLARE @postcodeWithoutSpaces VARCHAR(MAX)
	SET @postcodeWithoutSpaces = REPLACE(LTRIM(RTRIM(@Criteria)),' ','')

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
			COALESCE(customer.Firstname, '') + ' ' + COALESCE(customer.surname, '') AS CustomerName,
			customer.postcode AS Postcode,customer.ADDR1 as Address,
			model.description AS Description,
			status.status AS Status,
			Enginrs.ExpectedTurnaroundDays AS LeadTime,
			service.CLIENTID  as StoreId,
			Client.ClientName as StoreName
		FROM service
		INNER JOIN customer on service.customerid=customer.customerid
		INNER JOIN custapl on service.custaplid=custapl.custaplid
		LEFT JOIN model on custapl.model=model.model and custapl.MFR = model.MFR  and custapl.APPLIANCECD=model.APPLIANCECD
		INNER JOIN status on service.statusid=status.statusid
		LEFT JOIN RepairProfile on service.RepairId = RepairProfile.RepairID
		LEFT JOIN Enginrs on RepairProfile.RepairBookRepairEngineerID = Enginrs.ENGINEERID 					
		JOIN Client on service.CLIENTID = Client.ClientID
		LEFT join DiaryEnt de on de.TagInteger1 = Service.SERVICEID
		--	AND de.UserId in (select Id from @EngIds)
		LEFT join SpecJobMapping SM on sm.VisitType = service.VISITCD and sm.Clientid=@storeid
		
		WHERE service.CLIENTID=@StoreId and (isnull(sm.DummyJob,0)<>1  or sm._id is null)--and service.VISITCD not in ('020','022','014','023') 
		and
		 ( custapl.model LIKE @Criteria
			OR upper(customer.surname) LIKE @Criteria
			OR upper(customer.FIRSTNAME) LIKE @Criteria
			OR upper(customer.postcode) LIKE @Criteria
			OR upper(customer.postcode) LIKE @postcodeWithoutSpaces
			OR upper(customer.POSTCODESEARCH) LIKE @postcodeWithoutSpaces
			OR upper(customer.addr1) LIKE @Criteria
			OR upper(customer.addr2) LIKE @Criteria
			OR upper(customer.addr3) LIKE @Criteria
			OR upper(customer.county) LIKE @Criteria
			OR upper(customer.town) LIKE @Criteria
			OR upper(customer.tel1) LIKE @Criteria
			OR upper(customer.tel2) LIKE @Criteria
			or UPPer(customer.CLIENTCUSTREF) like @Criteria
			OR upper(customer.FIRSTNAME + ' ' + customer.SURNAME) LIKE @Criteria
			OR 'FZ'+CAST(serviceid as char(10)) LIKE @Criteria)) 

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


