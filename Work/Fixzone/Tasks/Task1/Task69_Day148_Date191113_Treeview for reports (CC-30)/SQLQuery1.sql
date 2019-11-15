CREATE TABLE [dbo].[ReportSetting](
	[_id] [int] IDENTITY(1,1) NOT NULL,
	[ReportName] [varchar](100) NOT NULL,
	[Category] [varchar](100) NOT NULL,
	[File] [varchar](200) NOT NULL,
	[AllClient] [bit] NULL,
	[ClientId] [int] NULL,
	[PrintOptionPdf] [bit] NULL,
	[PrintOptionHtml] [bit] NULL,
	[ExportOptionExcel] [bit] NULL,
	[ExportOptionCsv] [bit] NULL,
	[ExportOptionPdf] [bit] NULL,
	[ExportOptionHtml] [bit] NULL,
	[ExportOptionImage] [varchar](10) NULL,
PRIMARY KEY CLUSTERED 
(
	[_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

INSERT INTO ReportSetting
(
	[ReportName],
	[Category],
	[File],
	[AllClient],
	[ClientId],
	[PrintOptionPdf],
	[PrintOptionHtml],
	[ExportOptionExcel],
	[ExportOptionCsv],
	[ExportOptionPdf],
	[ExportOptionHtml],
	[ExportOptionImage]
)
VALUES
(
	'All open jobs',
	'Job',
	'Content\DashBoardReports\Job\All open jobs.mrt',
	1,
	NULL,
	1,
	1,
	1,
	1,
	1,
	1,
	'png'
)
GO

INSERT INTO ReportSetting
(
	[ReportName],
	[Category],
	[File],
	[AllClient],
	[ClientId],
	[PrintOptionPdf],
	[PrintOptionHtml],
	[ExportOptionExcel],
	[ExportOptionCsv],
	[ExportOptionPdf],
	[ExportOptionHtml],
	[ExportOptionImage]
)
VALUES
(
	'WIP',
	'Job',
	'Content\DashBoardReports\Job\WIP.mrt',
	1,
	NULL,
	1,
	1,
	1,
	1,
	1,
	1,
	'png'
)