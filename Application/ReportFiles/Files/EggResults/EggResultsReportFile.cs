using Application.ReportFiles.Utils.Constants;
using Application.Reports.Utils.Generators;
using Application.Reports.Utils.Helpers;
using ClosedXML.Excel;
using Domain.Objects.Requests.Report;
using Domain.Objects.Responses.Machine;
using Domain.Utils.Helpers;
using Domain.Utils.Languages;
using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using Microsoft.AspNetCore.Http;

namespace Application.Reports.ReportFiles.EggResultsReport
{
    public class EggResultsReportFile(IHttpContextAccessor context) : ReportFileConfiguration<EggResultsReportFileRequest>(context)
    {
        public override MemoryStream GetPdfReport(EggResultsReportFileRequest eggResultsReportFileRequest)
        {
            CreatePdfInstance(out MemoryStream stream, out PdfDocument pdf);
            var pdfReport = new Document(pdf);

            var pdfReportHelper = new PdfReportFileHelper(pdfReport, pdf)
                ?? throw new InvalidOperationException();

            if (Translator.CurrentLanguage == Domain.Objects.Enums.Language.ELanguage.Chinese)
                SetLang(ref pdfReport);

            CreatePdfReport(eggResultsReportFileRequest, pdfReportHelper);

            return RenderPdf(stream, pdfReport);
        }

        public override MemoryStream GetExcelReport(EggResultsReportFileRequest eggResultsReportFileRequest)
        {
            CreateExcelInstance(out XLWorkbook workbook, out IXLWorksheet worksheet);

            var excelReportHelper = new ExcelReportFileHelper(worksheet
                ?? throw new InvalidOperationException());

            CreateExcelReport(eggResultsReportFileRequest, excelReportHelper);

            return RenderExcel(workbook);
        }

        private static void SetLang(ref Document pdfReport)
        {
            var fontChinese = PdfFontFactory.CreateFont(ReportFileConfig.ChineseLangFilePath, PdfEncodings.IDENTITY_H);
            pdfReport.SetFont(fontChinese);
        }

        private static void CreatePdfReport(EggResultsReportFileRequest eggResultsReportFileRequest, PdfReportFileHelper pdfReportHelper)
        {
            #region Header

            var formattedDates = FormatDates(eggResultsReportFileRequest.StartDate, eggResultsReportFileRequest.EndDate);

            pdfReportHelper.CreateHeader(formattedDates[0], formattedDates[1]);

            #endregion

            #region Body

            #region Total eggs data

            var totalEggsDataRow = new List<IList<string>>
            {
                    new List<string>
                    {
                        eggResultsReportFileRequest.TotalEggsData.EggsWeight,
                        eggResultsReportFileRequest.TotalEggsData.EggsQuantity,
                        eggResultsReportFileRequest.TotalEggsData.BoxesQuantity
                    }
            };

            pdfReportHelper.CreateTable(headers: GetColumnNames(ETableType.TotalEggsData), rows: totalEggsDataRow, title: EggResultsReportFileLabels.TotalEggsDataTitle, marginTop: 10);

            #endregion

            #region General production data

            List<IList<string>> generalProductionDataRows = [];

            foreach (var prodData in eggResultsReportFileRequest.GeneralProductionData)
            {
                generalProductionDataRows.Add(new List<string>
                {
                    prodData.EggName,
                    prodData.EggWeight,
                    prodData.EggQuantity,
                    prodData.BoxQuantity
                });
            }

            pdfReportHelper.CreateTable(headers: GetColumnNames(ETableType.EggsProduced), rows: generalProductionDataRows, title: EggResultsReportFileLabels.EggsProducedTitle, marginTop: 10);

            #endregion

            #region Vision system statistics

            List<IList<string>> visionSystemSatisticsRows = [];

            foreach (var prodData in eggResultsReportFileRequest.VisionSystemStatistics)
            {
                visionSystemSatisticsRows.Add(new List<string>
                {
                    prodData.EggName,
                    $"{prodData.EggPercentage.ToFormatedDecimal()}%"
                });
            }

            pdfReportHelper.CreateTable(headers: GetColumnNames(ETableType.VisionSystem), rows: visionSystemSatisticsRows, title: EggResultsReportFileLabels.VisionSystemTitle, marginTop: 10);

            #endregion

            #endregion

            #region Footer

            pdfReportHelper.CreateFooter();

            #endregion
        }

        private static void CreateExcelReport(EggResultsReportFileRequest eggResultsReportFileRequest, ExcelReportFileHelper excelReportHelper)
        {
            #region Necessary properties

            MapPropertiesToExcelData(out IList<ExcelData> excelDataList, eggResultsReportFileRequest.TotalEggsData, eggResultsReportFileRequest.GeneralProductionData, eggResultsReportFileRequest.VisionSystemStatistics);

            #endregion

            #region Report header

            var formattedDates = FormatDates(eggResultsReportFileRequest.StartDate, eggResultsReportFileRequest.EndDate);

            excelReportHelper.CreateReportHeader(formattedDates[0], formattedDates[1]);

            #endregion

            #region Table header and body

            var startColumn = 1;
            for (var i = 0; i < excelDataList.Count; i++)
            {
                #region Table header

                var columnNames = excelDataList[i].ColumNames;

                excelReportHelper.CreateTableHeader(excelDataList[i].Title, columnNames, i == excelDataList.Count - 1);

                #endregion

                #region Body

                if (i > 0)
                    startColumn += excelDataList[i - 1].ColumNames.Length + 1;

                excelReportHelper.CreateBody(excelDataList[i].Rows, columnNames.Length, startColumn, excelDataList[i].TableType);

                #endregion
            }

            #endregion

            #region Footer

            excelReportHelper.CreateFooter();

            #endregion
        }

        private static void MapPropertiesToExcelData(out IList<ExcelData> excelDataList, TotalEggsData totalEggsData, IEnumerable<GeneralProductionData> generalProductionData, IEnumerable<VisionSystemStatistics> visionSystemStatistics)
        {
            string[] generalProductionDataRows = generalProductionData
                .SelectMany(g => new string[] { g.EggName, g.EggWeight, g.EggQuantity, g.BoxQuantity }).ToArray();

            string[] visionSystemStatisticsRows = visionSystemStatistics
                .SelectMany(g => new string[] { g.EggName.Replace(":", string.Empty), $"{g.EggPercentage.ToFormatedDecimal()}%" }).ToArray();

            string[] totalEggsDataRows = { totalEggsData.EggsWeight, totalEggsData.EggsQuantity, totalEggsData.BoxesQuantity };


            excelDataList = new List<ExcelData>
            {
                new ExcelData
                {
                    Title = EggResultsReportFileLabels.EggsProducedTitle,
                    ColumNames = GetColumnNames(ETableType.EggsProduced),
                    Rows = generalProductionDataRows,
                    TableType = ETableType.EggsProduced
                },
                new ExcelData
                {
                    Title = EggResultsReportFileLabels.VisionSystemTitle,
                    ColumNames = GetColumnNames(ETableType.VisionSystem),
                    Rows = visionSystemStatisticsRows,
                    TableType = ETableType.VisionSystem
                },
                new ExcelData
                {
                    Title = EggResultsReportFileLabels.TotalEggsDataTitle,
                    ColumNames = GetColumnNames(ETableType.TotalEggsData),
                    Rows = totalEggsDataRows,
                    TableType = ETableType.TotalEggsData
                }
            };
        }

        private static string[] GetColumnNames(ETableType columnType)
        {
            return columnType switch
            {
                ETableType.TotalEggsData => new string[] { EggResultsReportFileLabels.TotalWeight, EggResultsReportFileLabels.EggsQuantity, EggResultsReportFileLabels.BoxesQuantity },
                ETableType.EggsProduced => new string[] { EggResultsReportFileLabels.Category, EggResultsReportFileLabels.Weight, EggResultsReportFileLabels.Quantity, EggResultsReportFileLabels.Boxes },
                ETableType.VisionSystem => new string[] { EggResultsReportFileLabels.Category, EggResultsReportFileLabels.Value },
                _ => throw new InvalidOperationException()
            };
        }

        private static string[] FormatDates(string startDate, string endDate)
        {
            var dateFormat = EggResultsReportFileLabels.DateFormat;

            var formattedStartDate = DateTime.Parse(startDate).ToString(dateFormat).ToSafeValue();
            var formattedEndDate = DateTime.Parse(endDate).ToString(dateFormat).ToSafeValue();

            string[] datesResponse = { formattedStartDate, formattedEndDate };
            return datesResponse;
        }
    }
}
