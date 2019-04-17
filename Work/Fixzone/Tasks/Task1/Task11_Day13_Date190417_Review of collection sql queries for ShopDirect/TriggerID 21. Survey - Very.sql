SELECT
  Customer.EMAIL AS MESSAGESRV_SURVEYV_HTML_EMAIL,
  [Service].ServiceID,
  DiaryEnt.DiaryID,
  CONVERT(char(10), DiaryEnt.EventDate, 103) AS EventDate, 
  dbo.fn_getCustomerName(Customer.TITLE, Customer.FIRSTNAME, Customer.SURNAME) AS [CustomerName], 
  COALESCE(Model.[DESCRIPTION], 'item') AS [DESC],
  '0800 092 9051' AS UKWPHONENUMBER,
  f.footer AS Footer,
  RC.RetailClientName AS Brand,
  RC.Domain + '/Survey' AS [Domain],
  RC.Domain + '/Content/img/ClientLogo.png' AS Logo,
  'We''d really like to know what you thought of us' AS VerySurvey,
  CASE
    WHEN (Custapl.POLICYNUMBER LIKE '%ESP') THEN 'Service Request'
	WHEN (Custapl.POLICYNUMBER LIKE '%MPI') THEN  'claim' ELSE  'Claim'
  END AS 'PolicyType'
FROM
  [DiaryEnt] --DiaryEnt columns: EventDate (date), DiaryID (integer)
  LEFT JOIN [service] ON [Service].ServiceId = DiaryEnt.TagInteger1 --service columns: ServiceID (integer)
  LEFT JOIN [Customer] ON Customer.CUSTOMERID = [service].CustomerID --Customer columns: EMAIL (text), CustomerName (TITLE, FIRSTNAME, SURNAME)
  LEFT JOIN [CustApl] ON Custapl.CUSTAPLID = [service].CustAplID --CustApl columns: PolicyType (Custapl.POLICYNUMBER)
  LEFT JOIN [RetailClient] RC ON RC.RetailCode = Customer.RetailClientID AND RC.RetailClientID = Customer.CLIENTID --RetailClient columns: Brand, Domain, Logo (RetailClientName, Domain)
  LEFT JOIN [Footer] F ON RIGHT(custapl.policynumber, 3) = F.[Type] --Footer columns: footer
  LEFT JOIN [Model] ON Model.APPLIANCECD = Custapl.APPLIANCECD AND Model.MFR = CustApl.MFR AND Model.Model = Custapl.Model --Model columns: DESCRIPTION
  LEFT JOIN SpecJobMapping ON SpecJobMapping.VisitType = [service].VISITCD --SpecJobMapping columns: No columns (used to filter)
WHERE
  DiaryEnt.EventDate BETWEEN '2017-12-01' AND GETDATE() AND
  Customer.RetailClientID = 2 AND
  Customer.EMAIL IS NOT NULL AND
  [service].STATUSID = 8  AND
  (SpecJobMapping.DummyJob <> 1 or SpecJobMapping.DummyJob is  null)