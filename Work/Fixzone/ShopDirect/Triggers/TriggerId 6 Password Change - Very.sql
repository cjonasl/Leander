select 
  hist.Userid as MESSAGESRV_EmlCustVPC_HTML_EMAIL, 
  hist._id ,
  hist.Created,
  cs.RetailClientID, cs.CUSTOMERID,
  LTRIM(REPLACE(Ltrim(COALESCE(CS.TITLE,'')) + ' ' + LTRIM(COALESCE(CS.FIRSTNAME,'')) + ' ' + LTRIM(COALESCE(CS.SURNAME,'')), '  ', ' ')) AS CustomerName,  
 f.footer as Footer,
   RC.RetailClientName as Brand,
  RC.Domain AS [Domain],
  RC.Domain+'/Content/img/ClientLogo.png' as Logo,
  'Your password has been changed' AS VeryPasswordChanged
from  userpasswordchangehistory hist
LEFT JOIN TriggerRes ON TRIGGERID=6 AND TRIGGERFIELDLAST='_id' AND TriggerRes.TriggerValue=hist._id
inner join Customer cs on cs.CLIENTCUSTREF=hist.customerclientref
left join Custapl ca on cs.CUSTOMERID=ca.CUSTOMERID 
  LEFT JOIN [RetailClient] as RC on RC.RetailCode=cs.RetailClientID and RC.RetailClientID=cs.CLIENTID
  left join Footer as F on right(ca.policynumber,3) =f.Type
WHERE Cs.CUSTOMERID IS NOT NULL
AND TriggerRes.id IS NULL
and ca.POLICYNUMBER like '%ESP' 
AND Cs.EMAIL=hist.Userid AND Cs.RetailClientID=2
group by 
hist.Userid ,
  hist._id ,
  hist.Created,
  cs.RetailClientID, cs.CUSTOMERID,
   cs.TITLE,
 cs.FIRSTNAME,
cs.SURNAME, f.footer,
   RC.RetailClientName ,
  RC.Domain