SELECT COUNT(*)
FROM Custapl cap
INNER JOIN Customer ctm ON ISNULL(cap.OwnerCustomerID, cap.CUSTOMERID) = ctm.CUSTOMERID
WHERE
dbo.fnFilter_CustomerUserID(ctm.UserID, 'SDPOLICY') = 1
AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, '2018-01-29', getdate()) = 1
AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  
SELECT COUNT(*)
FROM Custapl cap
INNER JOIN Customer ctm ON ISNULL(cap.OwnerCustomerID, cap.CUSTOMERID) = ctm.CUSTOMERID
INNER JOIN RetailClient rcl ON ctm.RetailClientID = rcl.RetailCode AND ctm.CLIENTID = rcl.RetailClientID
INNER JOIN Footer ftr ON RIGHT(cap.policynumber, 3) = ftr.[Type]
WHERE
dbo.fnFilter_CustomerUserID(ctm.UserID, 'SDPOLICY') = 1
AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, '2018-01-29', getdate()) = 1
AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1