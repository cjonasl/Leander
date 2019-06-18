USE [SAEDI_PRD]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[ShipmateConsignmentCreation](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[DateCreated] [datetime] NULL,
	[Success] [bit] NULL,
	[ErrorMessage] [varchar](500) NULL,
	[ReqServiceID] [int] NULL,
	[ReqRemittanceID] [int] NULL,
	[ReqConsignmentReference] [varchar](50) NULL,
    [ReqServiceKey] [varchar](25) NULL,
    [ReqName] [varchar](50) NULL,
	[ReqLine1] [varchar](250) NULL,
	[ReqCity] [varchar](40) NULL,
	[ReqPostcode] [varchar](10) NULL,
	[ReqCountry] [varchar](40) NULL,
	[ReqReference] [varchar](60) NULL,
	[ReqWeight] [int] NULL,
	[ReqWidth] [int] NULL,
	[ReqLength] [int] NULL,
	[ReqDepth] [int] NULL,
	[ResMessage] [varchar](500) NULL,
	[ResConsignmentReference] [varchar](100) NULL,
	[ResParcelReference] [varchar](100) NULL,
	[ResCarrier] [varchar](100) NULL,
	[ResServiceName] [varchar](100) NULL,
	[ResTrackingReference] [varchar](100) NULL,
	[ResCreatedBy] [varchar](100) NULL,
	[ResCreatedWith] [varchar](100) NULL,
	[ResCreatedAt] [datetime] NULL,
	[ResDeliveryName] [varchar](100) NULL,
	[ResLine1] [varchar](50) NULL,
	[ResLine2] [varchar](50) NULL,
	[ResLine3] [varchar](50) NULL,
	[ResCity] [varchar](40) NULL,
	[ResCounty] [varchar](40) NULL,
	[ResPostcode] [varchar](10) NULL,
	[ResCountry] [varchar](40) NULL,
	[ResMediaURL] [varchar](250) NULL,
	[ResMediaGUID] [varchar](50) NULL,
 CONSTRAINT [PK_ShipmateConsignmentCreation] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO