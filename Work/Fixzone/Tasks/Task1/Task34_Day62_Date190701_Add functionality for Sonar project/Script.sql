--Ran ok on live db 2019-07-01 15:37
CREATE TABLE SonySonarEngineer
(
	[_id] [int] IDENTITY(1,1) NOT NULL primary key,
	[SAEDIID] [varchar](11) NOT NULL,
	[Active] [bit] NOT NULL,
CONSTRAINT UC_SonySonarEngineer_SAEDIID UNIQUE (SAEDIID)
)
GO

CREATE PROCEDURE fz_SonySonarEngineerIsActive
@SAEDIID varchar(11)
AS
IF EXISTS
(
  SELECT 1
  FROM SonySonarEngineer
  WHERE SAEDIID = @SAEDIID AND Active = 1
)
  return 1
ELSE
  return 0
GO
