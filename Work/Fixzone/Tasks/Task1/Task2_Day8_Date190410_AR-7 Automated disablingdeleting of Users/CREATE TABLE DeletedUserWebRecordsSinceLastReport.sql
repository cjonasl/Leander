IF NOT EXISTS
(
  SELECT 1
  FROM INFORMATION_SCHEMA.TABLES
  WHERE TABLE_NAME = N'DeletedUserWebRecordsSinceLastReport'
)
BEGIN
  CREATE TABLE DeletedUserWebRecordsSinceLastReport
  (
    [Userid] [varchar](25) NULL,
    [Fullname] [varchar](70) NULL,
    [Store] [varchar](50) NULL,
    [LastLoggedInDate] [date] NULL,
    [DisabledDate] [date] NULL,
    [DeletedDate] [date] NULL
  )
END