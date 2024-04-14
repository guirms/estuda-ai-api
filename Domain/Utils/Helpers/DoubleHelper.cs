namespace Domain.Utils.Helpers
{
    public static class DoubleHelper
    {
        public static string ToFormatedDecimal(this double doubleRequest, string format = "00.00") => doubleRequest.ToString(format).Replace(',', '.');
    }
}
