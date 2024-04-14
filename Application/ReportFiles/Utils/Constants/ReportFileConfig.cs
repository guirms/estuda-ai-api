using ClosedXML.Excel;
using iText.Kernel.Colors;

namespace Application.ReportFiles.Utils.Constants
{
    internal class ReportFileConfig
    {
        private static readonly int GrayTableLineRed = 239;
        private static readonly int GrayTableLineGreen = 239;
        private static readonly int GrayTableLineBlue = 239;

        internal static readonly string ChineseLangFilePath = $@"{Environment.CurrentDirectory}/Utils/Fonts/NotoSansSC-Regular.ttf";

        internal const string PdfExtension = "application/pdf";
        internal const string ExcelExtension = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        protected static readonly XLColor ExcelGrayColor =
            XLColor.FromColor(System.Drawing.Color.FromArgb
                (GrayTableLineRed, GrayTableLineGreen, GrayTableLineBlue));

        protected static readonly DeviceRgb PdfGrayColor =
            new(GrayTableLineRed, GrayTableLineGreen, GrayTableLineBlue);
    }
}
