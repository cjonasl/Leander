SELECT COUNT(*)
FROM SAEDIParts
 
 SELECT COUNT(*)
   FROM
     SAEDIParts P WITH (nolock)
     LEFT OUTER JOIN
	 (
	   SELECT DISTINCT
	     ID,
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
		RMA.Success = '1'
	  ) R ON
	  P.SAEDIFromID = R.SaediFromID AND
	  P.SAEDICallRef = R.ClientRef AND
	  R.INPUT_partNumberReceived  = P.CodeID  AND
	  (R.INPUT_externalMatId = CAST(P.SAEDICallRef AS VARCHAR(20)) + CAST(P.OrderNumber AS VARCHAR(20)) OR R.RmaId = P.ReturnReference)

SELECT P.ID, COUNT(*)
   FROM
     SAEDIParts P WITH (nolock)
     LEFT OUTER JOIN
	 (
	   SELECT DISTINCT
	     ID,
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
		RMA.Success = '1'
	  ) R ON
	  P.SAEDIFromID = R.SaediFromID AND
	  P.SAEDICallRef = R.ClientRef AND
	  R.INPUT_partNumberReceived  = P.CodeID  AND
	  (R.INPUT_externalMatId = CAST(P.SAEDICallRef AS VARCHAR(20)) + CAST(P.OrderNumber AS VARCHAR(20)) OR R.RmaId = P.ReturnReference)
	  WHERE R.ID IS NOT NULL
	GROUP BY P.ID
	HAVING COUNT(*) > 1