SELECT ctm.RetailClientID, ctm.CLIENTID, RIGHT(cap.policynumber, 3) AS 'Type'
FROM Custapl cap
INNER JOIN Customer ctm ON ISNULL(cap.OwnerCustomerID, cap.CUSTOMERID) = ctm.CUSTOMERID
LEFT JOIN RetailClient rcl ON ctm.RetailClientID = rcl.RetailCode AND ctm.CLIENTID = rcl.RetailClientID
LEFT JOIN Footer ftr ON RIGHT(cap.policynumber, 3) = ftr.[Type]
WHERE
dbo.fnFilter_CustomerUserID(ctm.UserID, 'SDPOLICY') = 1
AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, '2018-01-29', getdate()) = 1
AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
AND (rcl.RetailID IS NULL OR ftr.ID IS NULL)