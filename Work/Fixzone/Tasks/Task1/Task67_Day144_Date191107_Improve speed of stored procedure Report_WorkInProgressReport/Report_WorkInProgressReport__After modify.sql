CREATE PROCEDURE [dbo].[Report_WorkInProgressReport]
@Clientid int
AS
BEGIN

CREATE TABLE #TmpTableService
(
  [SERVICEID] int NOT NULL,
  [CUSTOMERID] int NULL,
  [CUSTAPLID] int NULL,
  [CALLDATETIME] datetime NULL,
  [STATUSID] smallint NULL,
  [SUBSTATUS] int NULL,
  [Status] varchar(30) NULL,
  [TODAYDATETIME] datetime NULL,
  [CLIENTREF] varchar(20) NULL,
  [CLIENTID] int NULL,
  [JOBID] int NULL
)

INSERT INTO #TmpTableService
(
  [SERVICEID],
  [CUSTOMERID],
  [CUSTAPLID],
  [CALLDATETIME],
  [STATUSID],
  [SUBSTATUS],
  [Status],
  [TODAYDATETIME],
  [CLIENTREF],
  [CLIENTID],
  [JOBID]
)
SELECT
  s.[SERVICEID],
  s.[CUSTOMERID],
  s.[CUSTAPLID],
  s.[CALLDATETIME],
  s.[STATUSID],
  s.[SUBSTATUS],
  st.[Status],
  s.[TODAYDATETIME],
  s.[CLIENTREF],
  s.[CLIENTID],
  s.[JOBID]
FROM
  [service] s
  INNER JOIN [status] st ON s.[STATUSID] = st.[StatusID]
WHERE
  s.[CLIENTID] = @Clientid AND
  st.[StatusWIP] = 1


SELECT 
Customer.TITLE+'.'+Customer.FirstName+' '+Customer.SURNAME as Customer,
s.SERVICEID,
cast(s.SERVICEID as varchar(20)) as ServiceidText,
Custapl.MFR,
DiaryEnt.EventDate,
Enginrs.NAME,
s.STATUSID,
Custapl.CUSTOMERID,
Custapl.CUSTAPLID, 
Custapl.APPLIANCECD,
Custapl.MODEL,
cast(Custapl.SUPPLYDAT as date) SUPPLYDAT,
Customer.TEL1,
Customer.TEL2, 
Customer.EMAIL, 
s.Status,
SubStatus.Status substatus,
cast(s.TODAYDATETIME as date) TODAYDATETIME,
s.clientref,
CUSTOMER.X,
CUSTOMER.Y, 
s.clientid,
dateDiff(dd,s.CALLDATETIME,getdate()) as Job_Age,
case when dateDiff(dd,s.CALLDATETIME,getdate())<=0 then 1 end as Job_Age0_7,
case when dateDiff(dd,s.CALLDATETIME,getdate()) between 8 and 14 then 1 end as Job_Age8_14,
case when dateDiff(dd,s.CALLDATETIME,getdate()) between 15 and 21 then 1 end as Job_Age15_21,
case when dateDiff(dd,s.CALLDATETIME,getdate()) between 22 and 28 then 1 end as Job_Age22_28,
case when dateDiff(dd,s.CALLDATETIME,getdate()) > 28 then 1 end as Job_Age29,
client.clientname
FROM 
#TmpTableService s
INNER JOIN Customer ON (s.CUSTOMERID = Customer.CUSTOMERID)
INNER JOIN Custapl ON (s.CUSTAPLID = Custapl.CUSTAPLID)
LEFT OUTER JOIN DiaryEnt ON (s.SERVICEID = DiaryEnt.TagInteger1)
LEFT OUTER JOIN Enginrs ON (DiaryEnt.UserId = Enginrs.ENGINEERID)
LEFT OUTER JOIN SubStatus ON (s.STATUSID = SubStatus.StatusID)
AND (s.SUBSTATUS = SubStatus.SubStatusID)
left outer join jobstatistics on jobstatistics.statjobid=s.jobid 
left outer join client on (s.clientid = client.clientid)
ORDER BY
DiaryEnt.EventDate DESC, 
s.STATUSID
END
GO
