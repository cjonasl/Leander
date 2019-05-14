 DECLARE
@saediID varchar(20),
@saediCallRef varchar(20)

SET @saediID = '3CCH23DE'
SET @saediCallRef = '1076'   
	
	SELECT DISTINCT
	getdate(),
	@saediID,
	@saediCallRef,
    P.[Id],
    R.INPUT_partNumberReceived,
	SRD.ascMaterialId,
	C.INPUTsonyPartNumber,
	CourBS.ClientRef,
	SSRMA.ClientRef
   FROM
     SAEDIParts P WITH (nolock)
     LEFT OUTER JOIN
	 (
	   SELECT DISTINCT
	     SaediFromID,
		 ClientRef,
		 RmaId,
		 INPUT_externalMatId,
		 INPUT_partNumberReceived,
		 RmaDocumentUrl,
		 ErrorMessage,
		 Success,
		 RMA.collectionaddedon,
		 rma.CollectionDate,
         Collectionref
      FROM
	    SONYRMA RMA WITH (nolock)
      WHERE
		RMA.SaediFromID = @saediID  AND 
		RMA.ClientRef = @saediCallRef AND
		RMA.Success = '1'
	  ) R ON R.INPUT_partNumberReceived  = P.CodeID  AND (R.INPUT_externalMatId = CAST(P.SAEDICallRef AS VARCHAR(20)) + CAST(P.OrderNumber AS VARCHAR(20)) OR  R.RmaId = P.ReturnReference)

     LEFT OUTER JOIN SonySearchReturnData SRD ON SRD.ascMaterialId = CAST(P.SAEDICallRef AS VARCHAR(20)) + CAST(P.OrderNumber AS VARCHAR(20)) AND  SRD.validationStatus != 'REJECTED'   
	 LEFT OUTER JOIN
		 (
	        SELECT DISTINCT INPUTascMaterialId, INPUTpartReferenceNumber, SAEDIFromID, ClientRef, INPUTsonyPartNumber, successful, INPUTson, INPUTremovePart, DateCreated, IsBulletin, INPUTisPrimary, INPUTwarrantyStatus 
            FROM SONYUpdatePartConsumption UPC WITH (nolock)
	        WHERE UPC.SAEDIFromID = @saediID AND  UPC.ClientRef = @saediCallRef AND UPC.successful = '1' AND    UPC.INPUTremovePart = '0'
         ) C  ON ((C.INPUTsonyPartNumber = P.CodeID AND (C.INPUTpartReferenceNumber = P.OrderNumber)) OR ( C.IsBulletin = '1'  and C.INPUTpartReferenceNumber <>'7998543') or ( C.IsBulletin = '1' and C.INPUTpartReferenceNumber ='7998543' and P.CodeID='TECHNICALBULLETIN'))
	    AND	 C.SAEDIFromID=P.SAEDIFromID and C.ClientRef = P.SAEDICallRef   and	                             	                    	    
    C.DateCreated >
	ISNULL((SELECT TOP 1 DateCreated FROM SONYUpdatePartConsumption CDATE WITH (nolock)
	WHERE  CDATE.SAEDIFromID = P.SAEDIFromID AND 
	CDATE.ClientRef = P.SAEDICallRef AND				  		       
	CDATE.successful = '1' AND 
	CDATE.INPUTremovePart = '1' AND
	((CDATE.INPUTsonyPartNumber = P.CodeID AND CDATE.INPUTpartReferenceNumber = P.OrderNumber) OR (CDATE.IsBulletin = '1' and CDATE.INPUTpartReferenceNumber ='7998543' and P.CodeID='TECHNICALBULLETIN'))
	ORDER BY CDATE.DateCreated DESC), DATEADD(year, -10, GETDate()))

    LEFT OUTER JOIN CourierBookingService CourBS on CourBS.ClientRef =p.SAEDICallRef and CourBS.PartReference=p.OrderNumber  and p.SAEDIFromID=CourBS.SaediId and CourBS.RET_Success=1
    LEFT OUTER JOIN Sonyswaprma SSRMA on SSRMA.ClientRef = p.SAEDICallRef and SSRMA.Success=1
    WHERE
	  P.[SaediFromID] = @saediID AND
	  P.[SAEDICallRef] = @saediCallRef