ALTER PROCEDURE GetReportList
@ClientId int
AS
BEGIN

SELECT [_id], [ReportName], [Category], [File], [CategoryDisplayOrder], [ReportDisplayOrder], [PrintOptionPdf], [PrintOptionHtml], [ExportOptionExcel], [ExportOptionCsv], [ExportOptionPdf], [ExportOptionHtml], [ExportOptionImage]
FROM [dbo].[ReportSetting]
WHERE [AllClient] = 1
UNION
SELECT [_id], [ReportName], [Category], [File], [CategoryDisplayOrder], [ReportDisplayOrder], [PrintOptionPdf], [PrintOptionHtml], [ExportOptionExcel], [ExportOptionCsv], [ExportOptionPdf], [ExportOptionHtml], [ExportOptionImage]
FROM [dbo].[ReportSetting]
WHERE [AllClient] = @ClientId
END
GO
