using Application.Reports.ReportFiles.EggResultsReport;
using ClosedXML.Excel;
using Domain.Utils.Languages;
using iText.Kernel.Pdf;
using iText.Layout;
using Microsoft.AspNetCore.Http;

namespace Application.Reports.Utils.Generators
{
    public abstract class ReportFileConfiguration<T> where T : class
    {
        private readonly HttpContext? _context;

        protected ReportFileConfiguration(IHttpContextAccessor contextAccessor)
        {
            _context = contextAccessor.HttpContext;
        }

        public abstract MemoryStream GetPdfReport(T eggResultsReportFileRequest);
        public abstract MemoryStream GetExcelReport(T eggResultsReportFileRequest);

        protected static void CreatePdfInstance(out MemoryStream stream, out PdfDocument pdf)
        {
            TranslateLabels();

            stream = new MemoryStream();

            using var writer = new PdfWriter(stream);
            writer.SetCloseStream(false);

            pdf = new PdfDocument(writer);

            writer.Dispose();
        }

        protected static void CreateExcelInstance(out XLWorkbook workbook, out IXLWorksheet worksheet)
        {
            TranslateLabels();

            workbook = new XLWorkbook();

            worksheet = workbook.Worksheets.Add("ExcelReport")
                ?? throw new InvalidOperationException();
        }

        protected MemoryStream RenderPdf(MemoryStream stream, Document pdfReport)
        {
            pdfReport.Close();

            stream.Position = 0;

            if (_context != null)
                SetFileName($"{EggResultsReportFileLabels.Filename}.pdf");

            return stream;
        }

        protected MemoryStream RenderExcel(XLWorkbook workbook)
        {
            using var ms = new MemoryStream();

            workbook.SaveAs(ms);
            var content = ms.ToArray();

            if (_context != null)
                SetFileName($"{EggResultsReportFileLabels.Filename}.xlsx");

            return new MemoryStream(content);
        }

        private void SetFileName(string fileName)
        {
            if (_context != null)
            {
                _context.Response.Headers.Append("x-file-name", fileName);
                _context.Response.Headers.Append("Access-Control-Expose-Headers", "x-file-name");
                _context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
            }
        }

        private static string GenerateFileName() => $"EGGRESULTS_PF{DateTime.Now:yyyyMMddHHmmss}";

        private static void TranslateLabels()
        {
            EggResultsReportFileLabels.Filename = GenerateFileName();
            EggResultsReportFileLabels.Title = Translator.Translate("EggResultsReportTitle");
            EggResultsReportFileLabels.StartDate = Translator.Translate("StartDate");
            EggResultsReportFileLabels.TotalEggsDataTitle = Translator.Translate("TotalEggsDataTitle");
            EggResultsReportFileLabels.EggsProducedTitle = Translator.Translate("EggsProducedTitle");
            EggResultsReportFileLabels.VisionSystemTitle = Translator.Translate("VisionSystemTitle");
            EggResultsReportFileLabels.EndDate = Translator.Translate("EndDate");
            EggResultsReportFileLabels.TotalWeight = Translator.Translate("TotalWeight");
            EggResultsReportFileLabels.EggsQuantity = Translator.Translate("TotalEggsQuantity");
            EggResultsReportFileLabels.BoxesQuantity = Translator.Translate("TotalBoxesQuantity");
            EggResultsReportFileLabels.Category = Translator.Translate("Category");
            EggResultsReportFileLabels.Weight = Translator.Translate("Weight");
            EggResultsReportFileLabels.Quantity = Translator.Translate("Quantity");
            EggResultsReportFileLabels.Boxes = Translator.Translate("Boxes");
            EggResultsReportFileLabels.Value = Translator.Translate("Value");
            EggResultsReportFileLabels.DateFormat = Translator.Translate("DateFormat");
            EggResultsReportFileLabels.DateTimeFormat = $"{Translator.Translate("DateFormat")} HH:mm:ss";
        }


        public void Dispose(MemoryStream stream, Document pdfReport)
        {
            stream.Dispose();
            pdfReport.Close();
        }
    }
}
