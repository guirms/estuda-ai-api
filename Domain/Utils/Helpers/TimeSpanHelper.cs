namespace Domain.Utils.Helpers
{
    public static class TimeSpanHelper
    {
        public static string ToStringTime(this TimeSpan timeSpanRequest, string format = "hh\\:mm") => timeSpanRequest.ToString(format);
    }
}
