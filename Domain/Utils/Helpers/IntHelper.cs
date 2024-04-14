namespace Domain.Utils.Helpers
{
    public static class IntHelper
    {
        public static string ToFormatedInt(this int intRequest, string format = "00.00") => intRequest.ToString(format).Replace(',', '.');
    }
}
