using Application.ReportFiles.Utils.Constants;
using Application.Reports.ReportFiles.EggResultsReport;
using Application.Reports.Utils.Constants;
using Domain.Objects.Enums.Language;
using Domain.Utils.Languages;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace Application.Reports.Utils.Helpers
{
    internal class PdfReportFileHelper(Document pdfReport, PdfDocument pdf) : ReportFileConfig
    {
        internal void CreateHeader(string startDate, string endDate)
        {
            #region Plasson logo

            var headerTable = new Table(UnitValue.CreatePercentArray([70, 15, 15]));
            headerTable.SetWidth(UnitValue.CreatePercentValue(100));

            var image = GetImage(100, 20)
                .SetHorizontalAlignment(HorizontalAlignment.LEFT);
            var imageCell = new BaseCell(hideBorder: true, centerVert: true)
                .Add(image);

            #endregion

            #region Date and tittle

            var startDateParagraph = new BaseParagraph($"{EggResultsReportFileLabels.StartDate}:", horAlign: HorizontalAlignment.RIGHT, fontSize: 10)
                .Add(new Text("\n" + startDate));
            var startDateCell = new BaseCell(hideBorder: true)
                .Add(startDateParagraph);

            var endDateParagraph = new BaseParagraph($"{EggResultsReportFileLabels.EndDate}:", horAlign: HorizontalAlignment.RIGHT, fontSize: 10)
                .Add(new Text("\n" + endDate));
            var endDateCell = new BaseCell(hideBorder: true)
                .Add(endDateParagraph);

            headerTable.AddCell(imageCell);
            headerTable.AddCell(startDateCell);
            headerTable.AddCell(endDateCell);

            pdfReport.Add(headerTable);

            var headerTitleParagraph = new BaseParagraph(EggResultsReportFileLabels.Title, fontSize: 20, bold: true);

            pdfReport.Add(headerTitleParagraph);

            #endregion
        }

        internal void CreateTable(string[] headers, IList<IList<string>> rows, string? title = null, int? marginTop = null, int width = 100)
        {
            var columnsQty = GenerateArrayOfOnes(headers.Length);

            var table = new Table(UnitValue.CreatePercentArray(columnsQty))
                .SetWidth(UnitValue.CreatePercentValue(100));

            if (marginTop.HasValue)
                table.SetMarginTop(marginTop.Value);

            var showBold = Translator.CurrentLanguage != ELanguage.Chinese;

            if (title != null)
            {
                var titleCell = new BaseCell(hideBorder: true, rowspan: 1, colspan: headers.Length)
                    .Add(new TableLineParagraph(title, bold: showBold))
                    .SetPaddingBottom(5);

                table.AddHeaderCell(titleCell);
            }

            foreach (var header in headers)
            {
                var headerToAdd = new BaseCell()
                    .Add(new TableLineParagraph(header, bold: showBold, grayColor: true));

                table.AddCell(headerToAdd);
            }

            for (int i = 0; i < rows.Count; i++)
            {
                var row = rows[i];
                for (int j = 0; j < row.Count; j++)
                {
                    var rowToAdd = new BaseCell()
                        .Add(new TableLineParagraph(row[j], grayColor: i % 2 != 0));

                    table.AddCell(rowToAdd);
                }
            }

            table.SetWidth(UnitValue.CreatePercentValue(width));

            pdfReport.Add(table);
        }

        internal void CreateFooter()
        {
            var dateTimeFormat = EggResultsReportFileLabels.DateTimeFormat;

            var currentDateTime = DateTime.Now.ToString(dateTimeFormat);

            var dataHoraParagrafo = new Paragraph(currentDateTime)
                .SetFixedPosition(pdf.GetPageNumber(pdf.GetLastPage()), 468, 10, 100)
                .SetFontSize(10);

            pdfReport.Add(dataHoraParagrafo);
        }

        private static Image GetImage(int width, int height)
        {
            var decodedImageBytes = Convert.FromBase64String(ReportFileImage.PlassonLogo);
            var imageData = ImageDataFactory.Create(decodedImageBytes);

            return new Image(imageData)
                .SetWidth(width)
                .SetHeight(height);
        }

        private static float[] GenerateArrayOfOnes(int size) => Enumerable.Repeat(1.0f, size).ToArray();

        private class BaseCell : Cell
        {
            internal BaseCell(bool hideBorder = false, bool centerVert = false, int? rowspan = null, int? colspan = null) : base(rowspan ?? 1, colspan ?? 1)
            {
                SetPadding(0);
                SetMargin(0);

                if (hideBorder)
                    SetBorder(Border.NO_BORDER);

                if (centerVert)
                    SetVerticalAlignment(VerticalAlignment.MIDDLE);
            }
        }

        private class BaseParagraph : Paragraph
        {
            internal BaseParagraph(string content, HorizontalAlignment? horAlign = HorizontalAlignment.CENTER, bool bold = false, int? fontSize = null) : base(content)
            {
                if (bold)
                    SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD));

                if (horAlign.HasValue)
                {
                    SetHorizontalAlignment(horAlign.Value);
                    SetTextAlignment((TextAlignment)horAlign.Value);
                }

                if (fontSize.HasValue)
                    SetFontSize(fontSize.Value);
            }
        }

        private class TableLineParagraph : Paragraph
        {
            internal TableLineParagraph(string content, bool bold = false, bool grayColor = false, int? fontSize = null) : base(new Text(content))
            {
                if (bold)
                    SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD));

                if (grayColor)
                    SetBackgroundColor(PdfGrayColor);

                if (fontSize.HasValue)
                    SetFontSize(fontSize.Value);

                SetTextAlignment(TextAlignment.CENTER);
                SetHorizontalAlignment(HorizontalAlignment.CENTER);
                SetVerticalAlignment(VerticalAlignment.MIDDLE);
            }
        }
    }
}
