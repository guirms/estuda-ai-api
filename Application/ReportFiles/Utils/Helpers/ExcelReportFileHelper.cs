using Application.ReportFiles.Utils.Constants;
using Application.Reports.ReportFiles.EggResultsReport;
using Application.Reports.Utils.Constants;
using ClosedXML.Excel;

namespace Application.Reports.Utils.Helpers
{
    internal class ExcelReportFileHelper(IXLWorksheet ws) : ReportFileConfig
    {
        private int _currentRow = 3;
        private int _currentColumn = 1;


        internal void CreateReportHeader(string startDate, string endDate)
        {
            MergeCells("A1:K2", border: false, heigth: 20);

            using var imageStream = new MemoryStream(Convert.FromBase64String(ReportFileImage.PlassonLogo));

            var plassonLogo = ws.AddPicture(imageStream)
                .MoveTo(ws.Cell("A1"))
                .ScaleHeight(.19)
                .ScaleWidth(.25);

            plassonLogo.Placement = ClosedXML.Excel.Drawings.XLPicturePlacement.FreeFloating;
            plassonLogo.Top = 6;
            plassonLogo.Left = 5;

            var startDateField = $"{EggResultsReportFileLabels.StartDate}: {startDate}";
            var endDateField = $"{EggResultsReportFileLabels.EndDate}: {endDate}";

            AddColumn($"{startDateField}      {endDateField}", row: 1, column: 1, border: false, bold: true, horAlign: XLAlignmentHorizontalValues.Right, vertAlign: XLAlignmentVerticalValues.Center);
        }

        internal void CreateTableHeader(string title, string[] columNames, bool isLastColumn)
        {
            MergeCells("A3:D3");
            MergeCells("F3:G3");
            MergeCells("I3:K3");

            AddColumn(title, addGrayColor: true, bold: true, fontSize: 14);
            _currentRow++;

            var columnNamesLength = columNames.Length;
            for (var i = 0; i < columnNamesLength; i++)
            {
                AddColumn(columNames[i], addGrayColor: true, skIpColumn: true, bold: true);

                if (i == columnNamesLength - 1 && !isLastColumn)
                    AddEmptyColumn();
            }

            _currentRow--;
        }

        internal void CreateBody(string[] bodyContents, int tableLength, int startColumn, ETableType tableType)
        {
            var oldCurrentColumn = _currentColumn;
            var oldCurrentRow = _currentRow;
            _currentRow += 2;
            _currentColumn = startColumn;

            var bodyContentsLength = bodyContents.Length;

            for (var i = 0; i < bodyContentsLength; i++)
            {
                var horAlign = XLAlignmentHorizontalValues.Center;
                int? width = null;

                if (tableType != ETableType.TotalEggsData && i % tableLength == 0)
                    horAlign = XLAlignmentHorizontalValues.Left;

                if (tableType == ETableType.VisionSystem)
                    width = 12;

                AddColumn(bodyContents[i], skIpColumn: true, horAlign: horAlign, width: width);

                if (i != 0 && (i + 1) % tableLength == 0)
                {
                    _currentColumn = startColumn;
                    _currentRow++;
                }
            }

            _currentColumn = oldCurrentColumn;
            _currentRow = oldCurrentRow;
        }

        internal void CreateFooter()
        {
            var newRowToMerge = ws.LastRowUsed().RowNumber() + 1;
            var lastColumnUsed = ws.LastColumnUsed().ColumnNumber();


            var dateTimeFormat = EggResultsReportFileLabels.DateTimeFormat;
            var currentDateTime = DateTime.Now.ToString(dateTimeFormat);

            int[] cellsAddress = [newRowToMerge, 1, newRowToMerge + 1, lastColumnUsed];
            MergeCells(cellsAddress, border: false);
            AddColumn(currentDateTime, row: newRowToMerge, column: 1, border: false, horAlign: XLAlignmentHorizontalValues.Right, vertAlign: XLAlignmentVerticalValues.Bottom);
        }

        private void MergeCells(int[] cellsAddress, bool border = true, double? heigth = null)
        {
            var mergedCells = ws.Range(cellsAddress[0], cellsAddress[1], cellsAddress[2], cellsAddress[3]).Merge();

            ApplyMergedCellOptions(mergedCells, border, heigth);
        }

        private void MergeCells(string rangeAddress, bool border = true, double? heigth = null)
        {
            var mergedCells = ws.Range(rangeAddress).Merge();

            ApplyMergedCellOptions(mergedCells, border, heigth);
        }

        private static void ApplyMergedCellOptions(IXLRange mergedCells, bool border, double? heigth)
        {
            if (heigth.HasValue)
                mergedCells.Worksheet.RowHeight = heigth.Value;

            if (border)
            {
                mergedCells.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                mergedCells.Style.Border.OutsideBorderColor = XLColor.Black;
            }
        }

        private void AddColumn(string columnContent, int? row = null, int? column = null, bool addGrayColor = false, bool border = true, bool skIpColumn = false, bool bold = false, int? fontSize = null, int? width = null, XLAlignmentHorizontalValues horAlign = XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues vertAlign = XLAlignmentVerticalValues.Center)
        {
            if (!row.HasValue)
                row = _currentRow;

            if (!column.HasValue)
                column = _currentColumn;

            var newCell = ws.Cell(row.Value, column.Value);
            newCell.Value = columnContent;
            newCell.Style.Alignment.SetHorizontal(horAlign);
            newCell.Style.Alignment.SetVertical(vertAlign);

            if (addGrayColor)
                newCell.Style.Fill.BackgroundColor = ExcelGrayColor;

            if (border)
            {
                newCell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                newCell.Style.Border.OutsideBorderColor = XLColor.Black;
            }

            if (bold)
                newCell.Style.Font.SetBold();

            if (fontSize.HasValue)
                newCell.Style.Font.FontSize = fontSize.Value;

            if (width.HasValue)
                newCell.WorksheetColumn().Width = width.Value;
            else
                newCell.WorksheetColumn().AdjustToContents();

            if (skIpColumn)
                _currentColumn++;
        }

        private void AddEmptyColumn()
        {
            var emptyColumn = ws.Cell(_currentRow, _currentColumn);
            emptyColumn.WorksheetColumn().Width = 2;
            _currentColumn++;
        }
    }

    internal class ExcelData
    {
        internal required string Title { get; set; }
        internal required string[] ColumNames { get; set; }
        internal required string[] Rows { get; set; }
        internal required ETableType TableType { get; set; }
    }

    internal enum ETableType
    {
        TotalEggsData,
        EggsProduced,
        VisionSystem
    }
}
