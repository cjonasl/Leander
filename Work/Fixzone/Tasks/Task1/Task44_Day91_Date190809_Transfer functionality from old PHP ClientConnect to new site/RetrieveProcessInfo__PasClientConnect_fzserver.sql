USE [PASClientconnect]
GO

/****** Object:  StoredProcedure [dbo].[RetrieveProcessInfo]    Script Date: 22/08/2019 14:41:41 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


ALTER PROCEDURE [dbo].[RetrieveProcessInfo]
@CustomerProcess bit= false	,
@ClientId int =0
AS	

if not exists (select processid from processdetail where ClientId=@ClientId and @ClientId<>0  and @CustomerProcess <>1)
begin
	SELECT D.ProcessID AS 'ProcessId',
		CAST(D.ProcessStep as INT) AS 'Step',
		H.ProcessAccessLevel AS 'AccessLevel',		
		CASE
			WHEN D.ExecuteLinkType = 0 THEN D.ExecuteLinkID
			ELSE NULL
		END AS 'ChildProcessId',d.CustomerProcess,
		P.PageURL AS 'PageURL',ClientId
	FROM ProcessDetail D
	INNER JOIN ProcessHeader H
	ON D.ProcessID = H.ProcessID and (( CustomerProcess=@CustomerProcess and @CustomerProcess=1) or @CustomerProcess=0)
	LEFT JOIN ProcessPage P
	ON D.ExecuteLinkID = P.PageID
	AND D.ExecuteLinkType = 1
	where ( clientid is null)
	

	 end

	 else
	 	SELECT D.ProcessID AS 'ProcessId',
		CAST(D.ProcessStep as INT) AS 'Step',
		H.ProcessAccessLevel AS 'AccessLevel',		
		CASE
			WHEN D.ExecuteLinkType = 0 THEN D.ExecuteLinkID
			ELSE NULL
		END AS 'ChildProcessId',d.CustomerProcess,
		P.PageURL AS 'PageURL',ClientId
	FROM ProcessDetail D
	INNER JOIN ProcessHeader H
	ON D.ProcessID = H.ProcessID and (( CustomerProcess=@CustomerProcess and @CustomerProcess=1) or @CustomerProcess=0)
	LEFT JOIN ProcessPage P
	ON D.ExecuteLinkID = P.PageID
	AND D.ExecuteLinkType = 1
	where (@ClientId=0  and clientid is null)
	 or (@ClientId>0  and clientid=@ClientId)




GO


