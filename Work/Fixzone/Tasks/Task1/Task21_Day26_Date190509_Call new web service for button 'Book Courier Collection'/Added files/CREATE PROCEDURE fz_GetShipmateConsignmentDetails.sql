CREATE PROCEDURE fz_GetShipmateConsignmentDetails
@TrackingReference varchar(100)
AS
SELECT TOP 1
  ResConsignmentReference,
  ResParcelReference,
  ResCarrier,
  ResServiceName,
  ResTrackingReference,
  ResCreatedBy,
  ResCreatedWith,
  ResCreatedAt,
  ResDeliveryName,
  ResLine1,
  ResCity,
  ResPostcode,
  ResCountry
FROM
  ShipmateConsignmentCreation
WHERE
  ResTrackingReference = @TrackingReference