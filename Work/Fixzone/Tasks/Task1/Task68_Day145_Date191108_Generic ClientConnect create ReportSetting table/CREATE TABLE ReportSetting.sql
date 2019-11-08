DROP TABLE ReportSetting
GO

CREATE TABLE ReportSetting
(
  _id int identity(1,1) NOT NULL primary key,
  ReportName varchar(100) NOT NULL,
  AllClient bit NULL,
  ClientId int NULL,
  PrintOptionPdf bit NULL,
  PrintOptionHtml bit NULL,
  ExportOptionExcel bit NULL,
  ExportOptionCsv bit NULL,
  ExportOptionPdf bit NULL,
  ExportOptionHtml bit NULL,
  ExportOptionImage varchar(10) NULL
)
GO

INSERT INTO ReportSetting(ReportName, AllClient, ClientId, PrintOptionPdf, PrintOptionHtml, ExportOptionExcel, ExportOptionCsv, ExportOptionPdf, ExportOptionHtml, ExportOptionImage)
VALUES('All open jobs', 1, NULL, 1, 1, 1, 1, 1, 1, 'png')

INSERT INTO ReportSetting(ReportName, AllClient, ClientId, PrintOptionPdf, PrintOptionHtml, ExportOptionExcel, ExportOptionCsv, ExportOptionPdf, ExportOptionHtml, ExportOptionImage)
VALUES('WIP', 1, NULL, 1, 1, 1, 1, 1, 1, 'png')