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
	[RepMessage] [varchar](500) NULL,
	[RepConsignmentReference] [varchar](100) NULL,
	[RepParcelReference] [varchar](100) NULL,
	[RepCarrier] [varchar](100) NULL,
	[RepServiceName] [varchar](100) NULL,
	[RepTrackingReference] [varchar](100) NULL,
	[RepCreatedBy] [varchar](100) NULL,
	[RepCreatedWith] [varchar](100) NULL,
	[RepCreatedAt] [datetime] NULL,
	[RepDeliveryName] [varchar](100) NULL,
	[RepLine1] [varchar](50) NULL,
	[RepLine2] [varchar](50) NULL,
	[RepLine3] [varchar](50) NULL,
	[RepCity] [varchar](40) NULL,
	[RepCounty] [varchar](40) NULL,
	[RepPostcode] [varchar](10) NULL,
	[RepCountry] [varchar](40) NULL,
	[RepPdf] [varchar](max) NULL,
	[RepZpl] [varchar](max) NULL,
	[RepPng] [varchar](max) NULL,
 CONSTRAINT [PK_Shipmate] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO