ALTER PROCEDURE [dbo].[RetrieveStatusSummary]
	@StoreId int = null
AS
begin

	SET NOCOUNT ON;
DECLARE @UnReadClientNotes INT =0
DECLARE @WAQueryRaised INT =0
DECLARE @WAQueryAnswered INT =0
DECLARE @WA INT =0
 
--if(@StoreId<>673)
-- begin


select 
@UnReadClientNotes =COUNT( distinct s.serviceid)
from Service S -- WITH (NOLOCK)
join ServiceNotes notes -- WITH (NOLOCK)
 on notes.serviceId = S.SERVICEID
Where S.ClientID=@StoreId     and   (notes.Communication='T' and visibility='C')and 
            ( notes.[Read] is null)	

select @WAQueryRaised = count(s.serviceid) 
from Service S  --WITH (NOLOCK)

Where S.ClientID=@StoreId 	 and 
s.STATUSID=12 and s.SUBSTATUS=3

select @WAQueryAnswered = count(s.SERVICEID) 
from Service S  --WITH (NOLOCK)
Where  S.ClientID=@StoreId	 and 
s.STATUSID=12 and s.SUBSTATUS=4

select @WA = count(s.serviceid) 
from Service S -- WITH (NOLOCK)
Where S.ClientID=@StoreId 	 and
 s.STATUSID=12  and s.SUBSTATUS not in (5,6,3,4) 


		
		--select SUBSTATUS,* from service where STATUSID=12 and CLIENTID=673
--end

select @UnReadClientNotes as 'UnReadClientNotes',
		@WAQueryRaised as 'WAQueryRaised',
		@WAQueryAnswered as 'WAQueryAnswered',
		@WA as 'WaitingforApproval'
		
  OPTION  (RECOMPILE) ; 
end
GO