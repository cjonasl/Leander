  SELECT
     RmaId
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
		RMA.SaediFromID = '3CCH23DE'  AND 
		RMA.ClientRef = '1076' AND
		RMA.Success = '1'
	  ) R ON R.INPUT_partNumberReceived  = P.CodeID  AND (R.INPUT_externalMatId = CAST(P.SAEDICallRef AS VARCHAR(20)) + CAST(P.OrderNumber AS VARCHAR(20)) OR  R.RmaId = P.ReturnReference)


SELECT *
FROM SAEDIParts
WHERE Id = 1350667

SELECT INPUT_partNumberReceived, SaediFromID, ClientRef, Success FROM SONYRMA
WHERE RmaId = 'DL56TI'


UPDATE SONYRMA
SET ClientRef = '1076'
WHERE RmaId = 'DL56TI'