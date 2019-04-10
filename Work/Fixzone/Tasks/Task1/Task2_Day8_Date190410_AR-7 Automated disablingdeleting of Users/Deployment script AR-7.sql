/*
  Author: Jonas Leander

  Date: 5/4/2019

  Description: 
  Deployment script, Jira AR-7.
  1. Add a column "DisabledDate" in table dbo.UserWeb
  2. Add a table "DeletedUserWebRecordsSinceLastReport" for deleted users
*/

-- 1. Add a column "DisabledDate" in table UserWeb --
IF NOT EXISTS
(
  SELECT 1 
  FROM INFORMATION_SCHEMA.COLUMNS 
  WHERE TABLE_NAME = N'UserWeb' AND COLUMN_NAME = N'DisabledDate'
)
BEGIN
  ALTER TABLE UserWeb
  ADD DisabledDate date NULL;
END

-- 2. Add a table "DeletedUserWebRecordsSinceLastReport" for deleted users --
CREATE TABLE DeletedUserWebRecordsSinceLastReport
(
  [Userid] [varchar](25) NULL,
  [Fullname] [varchar](70) NULL,
  [Store] [varchar](50) NULL,
  [LastLoggedInDate] [date] NULL,
  [DisabledDate] [date] NULL,
  [DeletedDate] [date] NULL
)