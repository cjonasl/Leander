DECLARE
@saediID varchar(20),
@saediCallRef varchar(20)

SET @saediID = '3CCH23DE'
SET @saediCallRef = '1076'

SELECT
    ISNULL(P.[Allocated],0) as Allocated,
    P.[Id],
    P.[OrderNumber],
    P.[SAEDIFromID],
    P.[SAEDICallRef],
	P.[Stk],
	CAST(P.[PartDescription] AS VARCHAR(MAX)) AS PartDescription,
	P.[Quantity],
	P.[UnitPrice],
	P.[TransactionCode],	
	P.[OrderReference],	  	
	P.[ReturnRequired],
	P.[ReturnDescription],
	P.[DeliveryNumber],
	P.[CourierReference], 
	P.[StatusTitle],
	P.[StatusID],
	P.[OrderDate],
	P.[DispatchDate],
	ISNULL(P.[Allocated],0) as Allocated,
	ISNULL(P.[Estimated],0) as Estimated,
	ISNULL(P.[Fitted],0) as Fitted,
	ISNULL(P.[Primary],0) as [Primary],	
	ISNULL(P.[PartConsumption],0) as PartConsumption,
	 p.returnCourierReference as RetCourierRef 
FROM
  SAEDIParts P
WHERE
  P.[SaediFromID] = @saediID AND
  P.[SAEDICallRef] = @saediCallRef


UPDATE [dbo].[SAEDIParts]
SET [SAEDICallRef] = '1076', --1077
ReturnReference = 'DL56TI'
WHERE Id = 1350667

SELECT * FROM [dbo].[SAEDIParts]
WHERE Id = 1350639




INPUT_partNumberReceived  = P.CodeID 



	   SELECT DISTINCT
	     INPUT_partNumberReceived,
		 RmaId,
	     SaediFromID,
		 ClientRef,		 
		 INPUT_externalMatId,		 
		 RmaDocumentUrl,
		 ErrorMessage,
		 Success,
		 RMA.collectionaddedon,
		 rma.CollectionDate,
         Collectionref
      FROM
	    SONYRMA RMA
      WHERE
		RMA.SaediFromID = '3CCH23DE' AND
		RMA.RmaId = 'DL56TI'

		SELECT * FROM SONYRMA 
		WHERE RmaId = 'DL56TI'

		UPDATE SONYRMA
		SET INPUT_partNumberReceived = '180277811'
		WHERE
		SaediFromID = '3CCH23DE' AND
		RmaId = 'DL56TI'
