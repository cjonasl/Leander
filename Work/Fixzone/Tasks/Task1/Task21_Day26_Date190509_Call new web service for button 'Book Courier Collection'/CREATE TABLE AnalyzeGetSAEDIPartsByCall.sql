USE [SAEDI_PRD]
GO

/****** Object:  Table [dbo].[AnalyzeGetSAEDIPartsByCall]    Script Date: 13/05/2019 14:11:09 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE AnalyzeGetSAEDIPartsByCall
(
    [JonasID] [int] NOT NULL,
	[TheDate] [datetime] NULL,
	[SaediID] [varchar](20) NULL,
	[SaediCallRef] [varchar](20) NULL,
	[Id] [int] NULL,
	[INPUT_partNumberReceived] [varchar](20) NULL,
	[ascMaterialId] [varchar](20) NULL,
	[INPUTsonyPartNumber] [varchar](20) NULL,
	[ClientRef1] [varchar](10) NULL,
	[ClientRef2] [varchar](20) NULL
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO