CREATE PROCEDURE GetReportList
@ClientId int
AS
BEGIN

SELECT [ReportName], [PrintOptionPdf], [PrintOptionHtml], [ExportOptionExcel], [ExportOptionCsv], [ExportOptionPdf], [ExportOptionHtml], [ExportOptionImage]
FROM [dbo].[ReportSetting]
WHERE [AllClient] = 1
UNION
SELECT [ReportName], [PrintOptionPdf], [PrintOptionHtml], [ExportOptionExcel], [ExportOptionCsv], [ExportOptionPdf], [ExportOptionHtml], [ExportOptionImage]
FROM [dbo].[ReportSetting]
WHERE [AllClient] = @ClientId
END