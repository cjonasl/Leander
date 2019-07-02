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
	[DateTimeRowInserted] [datetime] NULL,
	[SendRequestSuccess] [bit] NULL,
	[SendRequestErrorMessage] [varchar](500) NULL,
	[ResTrackingReference] [varchar](100) NULL,
    [LABEL_CREATED] [datetime] NULL,
    [MANIFESTED] [datetime] NULL,
    [COLLECTED] [datetime] NULL,
    [IN_TRANSIT] [datetime] NULL,
    [DELIVERED] [datetime] NULL,
    [DELIVERY_FAILED] [datetime] NULL,
	[ReqServiceID] [int] NULL,
	[ReqRemittanceID] [int] NULL,
	[ReqConsignmentReference] [varchar](50) NULL,
    [ReqServiceKey] [varchar](25) NULL,
    [ReqCollectionFromName] [varchar](50) NULL,
	[ReqCollectionFromLine1] [varchar](50) NULL,
	[ReqCollectionFromLine2] [varchar](50) NULL,
	[ReqCollectionFromLine3] [varchar](50) NULL,
	[ReqCollectionFromCompanyName] [varchar](50) NULL,
	[ReqCollectionFromTelephone] [varchar](25) NULL,
	[ReqCollectionFromEmailAddress] [varchar](50) NULL,
	[ReqCollectionFromCity] [varchar](50) NULL,
	[ReqCollectionFromPostcode] [varchar](10) NULL,
	[ReqCollectionFromCountry] [varchar](50) NULL,
	[ReqDeliveryToName] [varchar](50) NULL,
	[ReqDeliveryToLine1] [varchar](50) NULL,
	[ReqDeliveryToLine2] [varchar](50) NULL,
	[ReqDeliveryToLine3] [varchar](50) NULL,
	[ReqDeliveryToCompanyName] [varchar](50) NULL,
	[ReqDeliveryToTelephone] [varchar](25) NULL,
	[ReqDeliveryToEmailAddress] [varchar](50) NULL,
	[ReqDeliveryToCity] [varchar](50) NULL,
	[ReqDeliveryToPostcode] [varchar](10) NULL,
	[ReqDeliveryToCountry] [varchar](50) NULL,
	[ReqParcelReference] [varchar](50) NULL,
	[ReqParcelWeight] [int] NULL,
	[ReqParcelWidth] [int] NULL,
	[ReqParcelLength] [int] NULL,
	[ReqParcelDepth] [int] NULL,
	[ResMessage] [varchar](500) NULL,
	[ResConsignmentReference] [varchar](100) NULL,
	[ResParcelReference] [varchar](100) NULL,
	[ResCarrier] [varchar](100) NULL,
	[ResServiceName] [varchar](100) NULL,
	[ResCreatedBy] [varchar](100) NULL,
	[ResCreatedWith] [varchar](100) NULL,
	[ResCreatedAt] [datetime] NULL,
	[ResMediaURL] [varchar](250) NULL,
	[ResMediaGUID] [varchar](50) NULL,
 CONSTRAINT [PK_ShipmateConsignmentCreation] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO