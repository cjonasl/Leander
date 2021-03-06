ALTER proc [dbo].[Report_WorkInProgressReport]
@Clientid int
as
begin
SELECT 
Customer.TITLE+'.'+Customer.FirstName+' '+Customer.SURNAME as Customer,
service.SERVICEID,
cast(service.SERVICEID as varchar(20)) as ServiceidText,
Custapl.MFR,
DiaryEnt.EventDate,
Enginrs.NAME,
service.STATUSID,
Custapl.CUSTOMERID,
Custapl.CUSTAPLID, 
Custapl.APPLIANCECD,
Custapl.MODEL,
cast(Custapl.SUPPLYDAT as date) SUPPLYDAT,
Customer.TEL1,
Customer.TEL2, 
Customer.EMAIL, 
status.Status,
SubStatus.Status substatus,
cast(service.TODAYDATETIME as date) TODAYDATETIME,
service.clientref,
CUSTOMER.X,
CUSTOMER.Y, 
service.clientid,
dateDiff(dd,service.CALLDATETIME,getdate()) as Job_Age,
case when dateDiff(dd,service.CALLDATETIME,getdate())<=0 then 1 end as Job_Age0_7,
case when dateDiff(dd,service.CALLDATETIME,getdate()) between 8 and 14 then 1 end as Job_Age8_14,
case when dateDiff(dd,service.CALLDATETIME,getdate()) between 15 and 21 then 1 end as Job_Age15_21,
case when dateDiff(dd,service.CALLDATETIME,getdate()) between 22 and 28 then 1 end as Job_Age22_28,
case when dateDiff(dd,service.CALLDATETIME,getdate()) > 28 then 1 end as Job_Age29,
client.clientname
FROM 
service
INNER JOIN Customer ON (service.CUSTOMERID = Customer.CUSTOMERID)
INNER JOIN Custapl ON (service.CUSTAPLID = Custapl.CUSTAPLID)
LEFT OUTER JOIN DiaryEnt ON (service.SERVICEID = DiaryEnt.TagInteger1)
LEFT OUTER JOIN Enginrs ON (DiaryEnt.UserId = Enginrs.ENGINEERID)
INNER JOIN status ON (service.STATUSID = status.StatusID)
LEFT OUTER JOIN SubStatus ON (service.STATUSID = SubStatus.StatusID)
AND (service.SUBSTATUS = SubStatus.SubStatusID)
left outer join jobstatistics on jobstatistics.statjobid=service.jobid 
left outer join client on (service.clientid = client.clientid)
WHERE 
 service.clientid=@Clientid and
status.StatusWIP = 1 
ORDER BY
DiaryEnt.EventDate DESC, 
service.STATUSID

end