ALTER proCEDURE [dbo].[fz_GetSAEDIPartsByCall]

	@saediID varchar(20),
	@saediCallRef varchar(20)
AS



BEGIN


	SELECT DISTINCT

		P.[Id],	P.[OrderNumber],	P.[SAEDIFromID],P.[SAEDICallRef],	CASE WHEN (C.IsBulletin = 1) THEN ISNULL(C.INPUTsonyPartNumber, 'UNKNOWN CODE') 

	ELSE P.[CodeID] END AS CodeID,	P.[Stk],	CAST(P.[PartDescription] AS VARCHAR(MAX)) AS PartDescription,P.[Quantity],P.[UnitPrice],	P.[TransactionCode],	

	P.[OrderReference],	  	
	(case p.CodeID when '000000010' then SSRMA.RmaId else	ISNULL(P.[ReturnReference], '') end) as 	[ReturnReference],
	
	P.[ReturnRequired],	P.[ReturnDescription],	P.[DeliveryNumber],	P.[CourierReference], 	P.[StatusTitle],	P.[StatusID],

	P.[OrderDate],		P.[DispatchDate],	ISNULL(P.[Allocated],0) as Allocated,	ISNULL(P.[Estimated],0) as Estimated,	ISNULL(P.[Fitted],0) as Fitted,	ISNULL(P.[Primary],0) as [Primary],	

	ISNULL(P.[PartConsumption],0) as PartConsumption,

		-- JOINED FIELDS


			(case p.CodeID when '000000010' then SSRMA.RmaDocumentUrl else	ISNULL(R.RmaDocumentUrl, '') end) as RmaDocumentUrl, 

	    ISNULL(R.Success, '0') AS RmaDone,

		ISNULL(C.INPUTson, '') AS INPUTson,

		ISNULL(C.IsBulletin, '0') AS IsBulletin,

		ISNULL(C.INPUTisPrimary, '0') AS IsPrimary,

		ISNULL(C.INPUTwarrantyStatus, '0') AS WarrantyStatus,

		ISNULL(C.INPUTascMaterialId, '0') AS INPUTascMaterialId,

		ISNULL(SRD.shipmentStatus,'N/A') AS ShipmentStatus,

		ISNULL(SRD.validationStatus,'N/A') AS ValidationStatus	, 

		ISNULL(CourBS.RET_AddressLabelURL,'') as LabelUrl,	ISNULL(CourBS.RET_ConsignmentDocURL,'') as ConNoteUrl,
		ISNULL(CourBS.RetConsignmentNo,'') as ConsignmentNo,
		ISNULL(CourBS.RetBookingUniqueNumber,'') as BookingUniqueNumber,

		ISNULL(CourBS.INPUT_CourierID,'') as INPUT_CourierID,
 	(case p.CodeID when '000000010' then SSRMA.collectionaddedon else	R.collectionaddedon end) as collectionaddedon,
	(case p.CodeID when '000000010' then SSRMA.CollectionDate else R.CollectionDate end) as CollectionDate, 
	(case p.CodeID when '000000010' then SSRMA.Collectionref else	R.Collectionref end) as Collectionref,
R.ClientRef,
		p.returnCourierReference as RetCourierRef 


    FROM SAEDIParts P WITH (nolock)

    LEFT OUTER JOIN (SELECT DISTINCT SaediFromID, ClientRef, RmaId , INPUT_externalMatId, INPUT_partNumberReceived, RmaDocumentUrl, ErrorMessage , Success ,RMA.collectionaddedon,rma.CollectionDate,
     Collectionref

    FROM SONYRMA RMA WITH (nolock)

                     WHERE

						RMA.SaediFromID = @saediID  AND 

						RMA.ClientRef = @saediCallRef AND

						RMA.Success = '1' 


                     ) R 

    ON R.INPUT_partNumberReceived  = P.CodeID  AND

      (R.INPUT_externalMatId = CAST(P.SAEDICallRef AS VARCHAR(20)) + CAST(P.OrderNumber AS VARCHAR(20))OR 

       R.RmaId = P.ReturnReference)  

    LEFT OUTER JOIN SonySearchReturnData SRD ON 


        SRD.ascMaterialId = CAST(P.SAEDICallRef AS VARCHAR(20)) + CAST(P.OrderNumber AS VARCHAR(20)) AND 


        SRD.validationStatus != 'REJECTED'   
	LEFT OUTER JOIN (SELECT DISTINCT INPUTascMaterialId, INPUTpartReferenceNumber, SAEDIFromID, ClientRef, INPUTsonyPartNumber, successful, 

	                        INPUTson, INPUTremovePart, DateCreated, IsBulletin, INPUTisPrimary, INPUTwarrantyStatus 

                 FROM SONYUpdatePartConsumption UPC WITH (nolock)

	                 WHERE    UPC.SAEDIFromID = @saediID AND  UPC.ClientRef = @saediCallRef AND		                

	                UPC.successful = '1' AND    UPC.INPUTremovePart = '0' 		                 	                  

                    ) C  

	 ON ((C.INPUTsonyPartNumber = P.CodeID AND (C.INPUTpartReferenceNumber = P.OrderNumber
	
	 -- OR P.StatusID = 'V'
	 )) OR ( C.IsBulletin = '1'  and C.INPUTpartReferenceNumber <>'7998543') or ( C.IsBulletin = '1'  and C.INPUTpartReferenceNumber ='7998543' and P.CodeID='TECHNICALBULLETIN')  ) 
	 AND	 C.SAEDIFromID=P.SAEDIFromID and C.ClientRef = P.SAEDICallRef   and	                             	                    	    
		  C.DateCreated > -- Consumption must be AFTER last DELETED Consumption

		    ISNULL((SELECT TOP 1 DateCreated FROM SONYUpdatePartConsumption CDATE WITH (nolock)

			WHERE  CDATE.SAEDIFromID = P.SAEDIFromID AND 

				   CDATE.ClientRef = P.SAEDICallRef AND				  		       

				   CDATE.successful = '1' AND 

				   CDATE.INPUTremovePart = '1' AND

				 ((CDATE.INPUTsonyPartNumber = P.CodeID AND CDATE.INPUTpartReferenceNumber = P.OrderNumber-- OR P.StatusID = 'V'
				   ) OR   ( CDATE.IsBulletin = '1'  and CDATE.INPUTpartReferenceNumber ='7998543' and P.CodeID='TECHNICALBULLETIN') ) 

			ORDER BY CDATE.DateCreated DESC), DATEADD(year, -10, GETDate()))

    left outer join CourierBookingService CourBS on CourBS.ClientRef =p.SAEDICallRef and CourBS.PartReference=p.OrderNumber  and p.SAEDIFromID=CourBS.SaediId and CourBS.RET_Success=1
 left join Sonyswaprma SSRMA on SSRMA.ClientRef = p.SAEDICallRef and SSRMA.Success=1
    WHERE P.[SaediFromID] = @saediID AND 
    P.[SAEDICallRef] = @saediCallRef







END

GO


