SELECT    TOP 25  Customer.EMAIL AS MESSAGESRV_EMLBESV_HTML_EMAIL,      CustomerEnrolment.EnroleID,     CAST(CustomerEnrolment.EnroleCode AS CHAR(36)) AS EnroleCode,        CustomerEnrolment.CustomerID,     f.footer as Footer,
RC.RetailClientName as Brand,     case when CustomerAccount.ID is null then     RC.Domain+'/account/Enrolment?EnrolmentCode='+cast(enrolecode as varchar(200) )     else       RC.Domain+'/account/SignIn'     end    AS [Domain],
RC.Domain+'/Content/img/ClientLogo.png' as Logo,     LTRIM(REPLACE(Ltrim(COALESCE(Customer.TITLE,'')) + ' ' + LTRIM(COALESCE(Customer.FIRSTNAME,'')) + ' '   + LTRIM(COALESCE(Customer.SURNAME,'')), '  ', ' ')) AS CustomerName,     COUNT(CustApl.APPLIANCECD) as 'AppCount',     case when CustApl.contractDt <'2018-12-01' then      'Don&#39;t forget you have access to our Product Support within the Online Service Centre where you will find set up support, hints and tips and annual health checks or if you experience any faults with your electrical item in the future you can raise a Service Request.'     else     'You now have access to our Product Support within the Online Service Centre where you will find set up support, hints and tips and annual health checks or if you experience any faults with your electrical item in the future you can raise a Service Request.'     end     as Content    
INTO #EnroleEmail_VeryTemp    FROM CustomerEnrolment  
LEFT JOIN TriggerRes ON (TRIGGERID=71 or triggerid= 1 or triggerid=69)  AND TRIGGERFIELDLAST='EnroleID' AND TriggerRes.TriggerValue=CustomerEnrolment.EnroleID  
LEFT JOIN Customer ON Customer.CUSTOMERID=CustomerEnrolment.CustomerID   
LEFT JOIN Custapl ON  isnull(CustApl.OwnerCustomerID,CustApl.CUSTOMERID)=Customer.CUSTOMERID 
left join CustomerAccount on CustomerEnrolment.CustomerID = CustomerAccount.CustomerID  
LEFT JOIN POP_Apl ON POP_Apl.APPLIANCECD=CUSTAPL.APPLIANCECD    
LEFT JOIN [RetailClient] as RC on RC.RetailCode=Customer.RetailClientID and RC.RetailClientID=Customer.CLIENTID    
left join Footer as F on right(custapl.policynumber,3) =f.Type  
WHERE (ValidFlag=1 AND Customer.CUSTOMERID IS NOT NULL   AND COALESCE(Customer.EMAIL,'')<>'' AND Customer.RetailClientID=2   AND Customer.UserID='SDPOLICY'   and CustApl.contractstatus <>60    ANd Custapl.POLICYNUMBER like '%ESP'  AND CustomerEnrolment.LinkType=0 AND TriggerRes.id IS NULL   AND CustApl.CUSTOMERID IS NOT NULL   AND Custapl.CREATEDDATETIME >= '2019-01-01'  and (Custapl.ContractDt >'2018-01-28' and Custapl.ContractDt <'2018-12-01')    AND (custapl.CONTRACTCANCELDATE IS NULL or custapl.CONTRACTCANCELDATE='1900-01-01')       )  
 group by enroleid, Customer.email, enrolecode, CustomerEnrolment.CustomerID, Customer.TITLE, Customer.FIRSTNAME, Customer.SURNAME,f.Footer,rc.RetailClientName,rc.Domain,CustApl.contractDt,CustomerAccount.ID
 
 order by  min(Custapl.ContractDt );    

SELECT top 25    EEV.MESSAGESRV_EMLBESV_HTML_EMAIL,    EEV.EnroleID, EEV.EnroleCode, EEV.CustomerID, EEV.CustomerName,    COALESCE(model.[DESCription], 'Electrical Items') AS ElectricalItem,        EEV.footer as Footer,     
EEV.Brand as Brand,    EEV.Domain AS [Domain],    EEV.Logo as Logo,     'Welcome to the Service Guarantee Online Service Centre' AS VeryEnrollment,     EEV.Content  
FROM #EnroleEmail_VeryTemp  EEV  
LEFT JOIN Customer ON Customer.CUSTOMERID=EEV.CustomerID   LEFT JOIN Custapl ON isnull(CustApl.OwnerCustomerID,CustApl.CUSTOMERID)=Customer.CUSTOMERID    
LEFT JOIN model ON model.APPLIANCECD=CUSTAPL.APPLIANCECD and model.MODEL = custapl.MODEL and model.MFR=custapl.MFR  
where Customer.UserID='SDPOLICY'  and Custapl.CONTRACTSTATUS <> 60     ANd Custapl.POLICYNUMBER like '%ESP'  AND  Custapl.CREATEDDATETIME >= '2019-01-01' and (Custapl.ContractDt >'2018-01-28' and Custapl.ContractDt <'2018-12-01') AND (custapl.CONTRACTCANCELDATE IS NULL or custapl.CONTRACTCANCELDATE='1900-01-01')
group by  EEV.MESSAGESRV_EMLBesV_HTML_EMAIL,    EEV.EnroleID, EEV.EnroleCode, EEV.CustomerID, EEV.CustomerName,   model.[DESCription],      EEV.footer ,     EEV.Brand ,    EEV.Domain ,    EEV.Logo ,    EEV.Content  