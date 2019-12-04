CREATE PROCEDURE CustomerViewJob
/*
0: serviceId exists and match given postCode
1: Error, serviceId does not exist
2: Error, postCode does not match post code for serviceId
*/
@ServiceId int,
@PostCode varchar(25)
AS
BEGIN
DECLARE
@CustomerId int,
@PCode varchar(8),
@RetValue int

SET @RetValue = 0 --Default

SELECT @CustomerId = CUSTOMERID
FROM [service]
WHERE SERVICEID = @ServiceId

IF (@CustomerId IS NULL)
  SET @RetValue = 1
ELSE
BEGIN
  SELECT @PCode = REPLACE(LTRIM(RTRIM(POSTCODE)), ' ', '') 
  FROM Customer
  WHERE CUSTOMERID = @CustomerId

  IF (@PCode IS NULL OR @PCode <> @PostCode)
    SET @RetValue = 2
END

SELECT @RetValue
END
