USE [ShopDirect_test]
GO

IF EXISTS(SELECT 1 FROM sys.objects WHERE name = 'fnFilter_WithinDateRange')
DROP FUNCTION fnFilter_WithinDateRange

IF EXISTS(SELECT 1 FROM sys.objects WHERE name = 'fnFilter_RetailClient')
DROP FUNCTION fnFilter_RetailClient

IF EXISTS(SELECT 1 FROM sys.objects WHERE name = 'fnFilter_CustomerUserID')
DROP FUNCTION fnFilter_CustomerUserID

IF EXISTS(SELECT 1 FROM sys.objects WHERE name = 'fnFilter_ValueExists')
DROP FUNCTION fnFilter_ValueExists

IF EXISTS(SELECT 1 FROM sys.objects WHERE name = 'fnFilter_ServiceStatus')
DROP FUNCTION fnFilter_ServiceStatus

IF EXISTS(SELECT 1 FROM sys.objects WHERE name = 'fnFilter_ServiceSubStatus')
DROP FUNCTION fnFilter_ServiceSubStatus

IF EXISTS(SELECT 1 FROM sys.objects WHERE name = 'fnFilter_EntitledServiceType')
DROP FUNCTION fnFilter_EntitledServiceType

IF EXISTS(SELECT 1 FROM sys.objects WHERE name = 'fnFilter_EligibleForCourierCollection')
DROP FUNCTION fnFilter_EligibleForCourierCollection

IF EXISTS(SELECT 1 FROM sys.objects WHERE name = 'fnFilter_EntitledEngineer')
DROP FUNCTION fnFilter_EntitledEngineer

IF EXISTS(SELECT 1 FROM sys.objects WHERE name = 'fnFilter_NotContractStatus')
DROP FUNCTION fnFilter_NotContractStatus

IF EXISTS(SELECT 1 FROM sys.objects WHERE name = 'fnFilter_DiaryEntDateIsToday')
DROP FUNCTION fnFilter_DiaryEntDateIsToday

IF EXISTS(SELECT 1 FROM sys.objects WHERE name = 'fnFilter_PolicyType')
DROP FUNCTION fnFilter_PolicyType
GO

CREATE FUNCTION [dbo].[fnFilter_WithinDateRange]
(
  @TargetDate date,
  @DateFrom date,
  @DateTo date
)
RETURNS bit
AS
BEGIN
DECLARE
@Result bit

IF (@TargetDate IS NOT NULL AND @TargetDate BETWEEN @DateFrom AND @DateTo)
  SET @Result = 1
ELSE
  SET @Result = 0

return @Result
END
GO

CREATE FUNCTION [dbo].[fnFilter_RetailClient]
(
  @RetailCode int,
  @RetailClientName varchar(50)
)
RETURNS bit
AS
BEGIN
DECLARE
@Result bit

IF ((@RetailCode = 1 AND @RetailClientName = 'Littlewoods') OR (@RetailCode = 2 AND @RetailClientName = 'Very'))
  SET @Result = 1
ELSE
  SET @Result = 0

return @Result
END
GO

CREATE FUNCTION [dbo].[fnFilter_CustomerUserID]
(
  @CustomerUserID varchar(11),
  @TargetCustomerUserID varchar(11)
)
RETURNS bit
AS
BEGIN
DECLARE
@Result bit

IF (@CustomerUserID = @TargetCustomerUserID)
  SET @Result = 1
ELSE
  SET @Result = 0

return @Result
END
GO

CREATE FUNCTION [dbo].[fnFilter_ValueExists]
(
  @Val varchar(max)
)
RETURNS bit
AS
BEGIN
DECLARE
@Result bit

IF (@Val IS NOT NULL AND @Val <> '')
  SET @Result = 1
ELSE
  SET @Result = 0

return @Result
END
GO

CREATE FUNCTION [dbo].[fnFilter_ServiceStatus]
(
  @StatusID int,
  @StatusName varchar(30)
)
RETURNS bit
AS
BEGIN
DECLARE
@Result bit

IF EXISTS
(
  SELECT 1 FROM [dbo].[status]
  WHERE StatusID = @StatusID AND [Status] = @StatusName
)
  SET @Result = 1
ELSE
  SET @Result = 0

return @Result
END
GO

CREATE FUNCTION [dbo].[fnFilter_ServiceSubStatus]
(
  @SubStatus int,
  @TargetSubStatus int
)
RETURNS bit
AS
BEGIN
DECLARE
@Result bit

IF (@SubStatus = @TargetSubStatus)
  SET @Result = 1
ELSE
  SET @Result = 0

return @Result
END
GO

CREATE FUNCTION [dbo].[fnFilter_EntitledServiceType]
(
  @DummyJob bit
)
RETURNS bit
AS
BEGIN
DECLARE
@Result bit

IF (@DummyJob iS NULL OR @DummyJob <> 1)
  SET @Result = 1
ELSE
  SET @Result = 0

return @Result
END
GO

CREATE FUNCTION [dbo].[fnFilter_EligibleForCourierCollection]
(
  @Flag varchar(1)
)
RETURNS bit
AS
BEGIN
DECLARE
@Result bit

IF (@Flag = 'T')
  SET @Result = 1
ELSE
  SET @Result = 0

return @Result
END
GO

CREATE FUNCTION [dbo].[fnFilter_EntitledEngineer]
(
  @Flag bit
)
RETURNS bit
AS
BEGIN
DECLARE
@Result bit

IF (@Flag = 0)
  SET @Result = 1
ELSE
  SET @Result = 0

return @Result
END
GO

CREATE FUNCTION [dbo].[fnFilter_NotContractStatus]
(
  @ContractStatus smallint,
  @NotContractStatus smallint
)
RETURNS bit
AS
BEGIN
DECLARE
@Result bit

IF (@ContractStatus IS NULL OR @ContractStatus <> @NotContractStatus)
  SET @Result = 1
ELSE
  SET @Result = 0

return @Result
END
GO

CREATE FUNCTION [dbo].[fnFilter_DiaryEntDateIsToday]
(
  @EventDate date
)
RETURNS bit
AS
BEGIN
DECLARE
@Result bit

IF (@EventDate = CONVERT(DATE, GETDATE()))
  SET @Result = 1
ELSE
  SET @Result = 0

return @Result
END
GO

CREATE FUNCTION [dbo].[fnFilter_PolicyType]
(
  @PolicyNumber varchar(25),
  @TargetPolicyType varchar(25)
)
RETURNS bit
AS
BEGIN
DECLARE
@Result bit

IF
(
  (@PolicyNumber LIKE '%ESP' AND @TargetPolicyType = 'Service Guarantee') OR
  (@PolicyNumber LIKE '%RPG' AND @TargetPolicyType = 'Replacement Guarantee') OR
  (@PolicyNumber LIKE '%MPI' AND @TargetPolicyType = 'Mobile')
)
  SET @Result = 1
ELSE
  SET @Result = 0

return @Result
END
GO