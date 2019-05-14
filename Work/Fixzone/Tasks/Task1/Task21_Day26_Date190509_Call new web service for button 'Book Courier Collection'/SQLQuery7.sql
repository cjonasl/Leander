DECLARE
@saediID varchar(20),
@saediCallRef varchar(20)

SET @saediID = '3CCH23DE'
SET @saediCallRef = '1076'

SELECT * FROM SAEDIParts P
WHERE P.[SaediFromID] = @saediID --AND P.[SAEDICallRef] = @saediCallRef
ORDER BY P.Id

--1350667

UPDATE
  SAEDIParts
SET
 [SAEDICallRef] = '1076',
 [OrderDate] = '2019-04-15',
 [DispatchDate] = NULL
WHERE
  Id = 1350667
