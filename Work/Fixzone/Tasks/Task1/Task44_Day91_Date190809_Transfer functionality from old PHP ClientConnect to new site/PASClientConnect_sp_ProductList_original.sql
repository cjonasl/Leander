USE [PAS]
GO

/****** Object:  StoredProcedure [dbo].[ProductList]    Script Date: 19/08/2019 15:58:07 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

--updates for stored procedures
ALTER PROCEDURE [dbo].[ProductList]		
	@Criteria varchar(MAX), 
	@ReturnLines Int,
	@PageNumber  Int,
	@Clientid int=0
AS
	SET NOCOUNT ON

	DECLARE @RecordCount INT

	Declare @SearchWords nvarchar(100) --parameter for full text search
	Declare @CriteriaAltcode nvarchar(100) --parameter for altcode
		
	--Remove spaces for search by altcode
	SET @CriteriaAltcode = REPLACE(@Criteria, ' ', '')

	--Update criteria for full text search
	--Remove comma
	SET @SearchWords = REPLACE(@Criteria, ',', ' ')
	SET @SearchWords = LTRIM(RTRIM(@SearchWords))	
	SET @SearchWords = REPLACE(@SearchWords,char(34), char(39)+char(39))
	

	--Remove all spacec, excepr one where they exist
	While CharIndex('  ', @SearchWords) > 0
		Set @SearchWords = Replace(@SearchWords, '  ', ' ')
	
	--Replace all spaces (after remove we have only one space between words)
	SET @SearchWords = REPLACE(@SearchWords,' ', ' AND ')
	
	--Remove all symbols from criteria	
	SET @Criteria =REPLACE(REPLACE(REPLACE(@Criteria,'\',''),'/',''),'-','')
	SET @Criteria = LTRIM(RTRIM(@Criteria))
	
	DECLARE @startRowNum integer
	set @startRowNum = (@PageNumber - 1) * @ReturnLines + 1

	--templorary table
	DECLARE @Results TABLE(
		RowNumber INT, 
		Id INT,
		MODEL VARCHAR(25),
		description VARCHAR(40),
		supplier VARCHAR(50),
		ALTCODE VARCHAR(25),
		AlternativeFlag	BIT
	)

	declare @ClientModelRestriction bit
	select @ClientModelRestriction = ClientModelRestriction   from client where clientid=@Clientid
if(@ClientModelRestriction=1)
	 --products list
begin
	
 INSERT INTO @Results
	 SELECT ROW_NUMBER() OVER (ORDER BY m1.MODEL) AS RowNumber,
			 m1.MODELID,
			 m1.MODEL, m1.description,
			 supplier.supplier,
			 m1.ALTCODE,
			 CASE WHEN 			 
					(SELECT COUNT(*)
					 FROM ModelXRef mxr 
					 WHERE mxr.XMODModel = t1.MODEL
						) > 0 THEN 1
					ELSE 0
			 END
	  from model m1
	  left outer join supplier on supplier.supplierid=m1.supplierid
	  join (
			 select model as 'model', m2.mfr as 'mfr'
			 from model m2  join Manufact man on man.MFR=m2.MFR
			 join pop_apl apl on apl.APPLIANCECD=m2.APPLIANCECD
			 left outer join supplier s2 on s2.supplierid = m2.supplierid
			 where
			 m2.model like @Criteria + '%'
			 or m2.model like @CriteriaAltcode + '%'
			 or m2.altcode like @Criteria + '%'
			 or m2.altcode like @CriteriaAltcode + '%'
			 or s2.Supplier like @Criteria + '%'
			 group by model,m2.mfr
			 union
			 select model as 'model', MAX(mfr) as 'mfr'
			 from model m2             
			 where
			 CONTAINS(m2.description,@SearchWords)             
			 group by model
			 
	  ) t1 on t1.MODEL = m1.MODEL
			 and t1.mfr = m1.MFR 
		
		
	

	end	
	else
	
	begin
	 INSERT INTO @Results
	 SELECT ROW_NUMBER() OVER (ORDER BY m1.MODEL) AS RowNumber,
			 m1.MODELID,
			 m1.MODEL, m1.description,
			 supplier.supplier,
			 m1.ALTCODE,
			 CASE WHEN 			 
					(SELECT COUNT(*)
					 FROM ModelXRef mxr 
					 WHERE mxr.XMODModel = t1.MODEL
						) > 0 THEN 1
					ELSE 0
			 END
	  from model m1
	  left outer join supplier on supplier.supplierid=m1.supplierid
	  join (
			 select model as 'model', m2.mfr as 'mfr'
			 from model m2  join Manufact man on man.MFR=m2.MFR
			 join pop_apl apl on apl.APPLIANCECD=m2.APPLIANCECD
			 left outer join supplier s2 on s2.supplierid = m2.supplierid
			 where
			 m2.model like @Criteria + '%'
			 or m2.model like @CriteriaAltcode + '%'
			 or m2.altcode like @Criteria + '%'
			 or m2.altcode like @CriteriaAltcode + '%'
			 or s2.Supplier like @Criteria + '%'
			 group by model,m2.mfr
			 union
			 select model as 'model', MAX(mfr) as 'mfr'
			 from model m2             
			 where
			 CONTAINS(m2.description,@SearchWords)             
			 group by model
			 
	  ) t1 on t1.MODEL = m1.MODEL
			 and t1.mfr = m1.MFR 
		
		join clientmodelrestriction CM on 	cm.model=m1.MODEL AND cm.ApplianceCD=m1.APPLIANCECD
		AND cm.MFR =m1.mfr AND cm.ClientID=@Clientid
	
	
	end
	--list information
	SELECT @RecordCount = COUNT(*)
	FROM @Results

	SELECT CAST(R.Id as varchar(max)) as 'ModelID', R.MODEL as 'ItemCode', R.description as 'Descr', R.supplier as 'Brand', R.ALTCODE as 'Model',
			isnull(@RecordCount,0) as 'ElemCount', isnull(@startRowNum,0) as 'StartElem', 
			case when (@startRowNum + @ReturnLines - 1) > @RecordCount then @RecordCount
				else (@startRowNum + @ReturnLines - 1)
			end as 'LastElem', AlternativeFlag as 'AlternativeFlag'
	FROM @Results R
	WHERE R.RowNumber BETWEEN @startRowNum AND (@startRowNum + @ReturnLines - 1)

GO


