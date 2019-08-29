ALTER PROCEDURE [dbo].[GetJobsListByKey]
	@StoreId int = null,
	@StatusKey varchar(100),
	@ReturnLines INT=10,
	@PageNumber INT=1
	
AS
		DECLARE @Results TABLE(
		Id INT,
		RepairNo VARCHAR(20),
		rowNumber int,
		CustomerName VARCHAR(MAX),
		PostCode VARCHAR(8),
			Address varchar(500),
		Description VARCHAR(400),
	
	
		
	
		StoreId INT,
		Logged DATETIME	,RetailClientId int
		)
DECLARE @startRowNum integer
	set @startRowNum = (@PageNumber - 1) * @ReturnLines + 1


if(@StatusKey='UnReadClientNotes')
begin

insert into @Results
	SELECT 			
	distinct	service.serviceid as 'Id',
		'FZ'+CAST(service.serviceid as varchar(10)) AS 'RepairNo',
		ROW_NUMBER() OVER (ORDER BY min(service.serviceid) DESC) AS RowNumber,
		COALESCE(customer.Firstname, '') + ' ' + COALESCE(customer.surname, '') AS 'CustomerName',
		customer.postcode AS 'Postcode',customer.ADDR1 as Address,
		model.description AS 'Description',	
		service.ClientId,
		service.TODAYDATETIME as 'logged',customer.RetailClientID 
	FROM service
	INNER JOIN customer on service.customerid=customer.customerid
	INNER JOIN custapl on service.custaplid=custapl.custaplid
	LEFT JOIN model on custapl.model=model.model AND custapl.MFR = model.MFR  and model.APPLIANCECD= custapl.APPLIANCECD
	LEFT JOIN DiaryEnt on DiaryEnt.TagInteger1 = service.SERVICEID
	LEFT JOIN Enginrs on Enginrs.ENGINEERID = DiaryEnt.UserId
	join ServiceNotes notes on notes.serviceId = service.SERVICEID
	
Where Service.ClientID=@StoreId 	 and (notes.Communication='T' and visibility='C') and 
	( notes.[Read] is null )--and notes.userId not in(select userid from userweb)
	
--order by service.CALLDATETIME
group by service.serviceid,customer.Firstname,customer.surname,customer.postcode,customer.ADDR1 ,
		model.description,	
		service.ClientId,
		service.TODAYDATETIME,customer.RetailClientID 

end
else if(@StatusKey='WAQueryRaised')
begin 
insert into @Results
	SELECT 		distinct		
		s.serviceid as 'Id',
		'FZ'+CAST(s.serviceid as varchar(10)) AS 'RepairNo',	ROW_NUMBER() OVER (ORDER BY calldatetime DESC) AS RowNumber,
		COALESCE(customer.Firstname, '') + ' ' + COALESCE(customer.surname, '') AS 'CustomerName',
		customer.postcode AS 'Postcode',customer.ADDR1 as Address,
		model.description AS 'Description',		
		s.ClientId,
		s.TODAYDATETIME as 'logged',customer.RetailClientID 
	FROM service s
	INNER JOIN customer on s.customerid=customer.customerid
	INNER JOIN custapl on s.custaplid=custapl.custaplid
	LEFT JOIN model on custapl.model=model.model AND custapl.MFR = model.MFR  and model.APPLIANCECD= custapl.APPLIANCECD
	LEFT JOIN DiaryEnt on DiaryEnt.TagInteger1 = s.SERVICEID
	LEFT JOIN Enginrs on Enginrs.ENGINEERID = DiaryEnt.UserId
	
Where --S.ClientID=@StoreId 	 and 
	s.STATUSID=12 and s.SUBSTATUS=3 and
	  s.ClientID = @StoreId 			 
			 -- and  service.CALLDATETIME>DATEADD(MONTH,-@months,getdate())

end
else if(@StatusKey='WAQueryAnswered')
begin
insert into @Results
	SELECT 		distinct		
		s.serviceid as 'Id',
		'FZ'+CAST(s.serviceid as varchar(10)) AS 'RepairNo',ROW_NUMBER() OVER (ORDER BY calldatetime DESC) AS RowNumber,
		COALESCE(customer.Firstname, '') + ' ' + COALESCE(customer.surname, '') AS 'CustomerName',
		customer.postcode AS 'Postcode',customer.ADDR1 as Address,
		model.description AS 'Description',		
		s.ClientId,
		s.TODAYDATETIME as 'logged',customer.RetailClientID 
	FROM service s
	INNER JOIN customer on s.customerid=customer.customerid
	INNER JOIN custapl on s.custaplid=custapl.custaplid
	LEFT JOIN model on custapl.model=model.model AND custapl.MFR = model.MFR  and model.APPLIANCECD= custapl.APPLIANCECD
	LEFT JOIN DiaryEnt on DiaryEnt.TagInteger1 = s.SERVICEID
	LEFT JOIN Enginrs on Enginrs.ENGINEERID = DiaryEnt.UserId

Where --S.ClientID=@StoreId 	 and 
	s.STATUSID=12 and s.SUBSTATUS=4
	and
	  s.ClientID = @StoreId 			 
			 -- and  service.CALLDATETIME>DATEADD(MONTH,-@months,getdate())

end
else if(@StatusKey='WaitingforApproval')
begin
insert into @Results
	SELECT 			distinct	
		s.serviceid as 'Id',
		'FZ'+CAST(s.serviceid as varchar(10)) AS 'RepairNo',ROW_NUMBER() OVER (ORDER BY
		(SELECT max(HistoryDateTime)
	FROM StatHist SH
        WHERE SH.HistoryRefID = s.SERVICEID  and sh.HistoryStatusID=12  group by HistoryRefID,HistoryStatusID )
		  
		) AS RowNumber,
		COALESCE(customer.Firstname, '') + ' ' + COALESCE(customer.surname, '') AS 'CustomerName',
		customer.postcode AS 'Postcode',customer.ADDR1 as Address,
		model.description AS 'Description',		
		s.ClientId,
		s.TODAYDATETIME as 'logged',customer.RetailClientID 
	FROM service s
	INNER JOIN customer on s.customerid=customer.customerid
	INNER JOIN custapl on s.custaplid=custapl.custaplid
	LEFT JOIN model on custapl.model=model.model AND custapl.MFR = model.MFR  and model.APPLIANCECD= custapl.APPLIANCECD
	LEFT JOIN DiaryEnt on DiaryEnt.TagInteger1 = s.SERVICEID
	LEFT JOIN Enginrs on Enginrs.ENGINEERID = DiaryEnt.UserId

Where S.ClientID=@StoreId 	 and 
	 s.STATUSID=12 and s.SUBSTATUS not in (5,6,3,4)
	 -- service.ClientID = @StoreId 			and 
			 -- and  service.CALLDATETIME>DATEADD(MONTH,-@months,getdate())		 		 
end

declare  @RecordCount int

select @RecordCount = count(id) from @Results

select *,isnull(@RecordCount,0) as 'ElemCount', isnull(@startRowNum,0) as 'StartElem', 
			case when (@startRowNum + @ReturnLines - 1) > @RecordCount then @RecordCount
				else (@startRowNum + @ReturnLines - 1)
			end as 'LastElem' from @Results    WHERE RowNumber BETWEEN @startRowNum AND @startRowNum + @ReturnLines - 1

			order by RowNumber

GO


