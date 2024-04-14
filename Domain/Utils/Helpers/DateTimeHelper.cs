namespace Domain.Utils.Helpers
{
    public static class DateTimeHelper
    {
        public static DateTime ToMinDateTime(this DateTime dateTimeRequest) => dateTimeRequest.Date;
        public static DateTime ToMaxDateTime(this DateTime dateTimeRequest) => dateTimeRequest.AddDays(1).AddSeconds(-1);
        public static string ToStringTime(this DateTime dateTimeRequest, string format = "hh:mm") => dateTimeRequest.ToString(format);
        public static string ToStringBrFormat(this DateTime dateTimeRequest, string format = "yyyy-MM-dd HH:mm:ss") => dateTimeRequest.ToString(format);
    }
}
