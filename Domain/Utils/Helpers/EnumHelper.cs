using System.ComponentModel;

namespace Domain.Utils.Helpers
{
    public static class EnumHelper
    {
        public static string GetDescription(this Enum enumRequest)
        {
            var fieldInfo = enumRequest.GetType().GetField(enumRequest.ToString());
            var attribute = fieldInfo?.GetCustomAttributes(typeof(DescriptionAttribute), false)
                .FirstOrDefault() as DescriptionAttribute;

            return attribute?.Description ?? enumRequest.ToString();
        }

        public static string GetName(this Enum enumRequest)
        {
            var enumName = Enum.GetName(enumRequest.GetType(), enumRequest);

            return enumName.ToSafeValue();
        }

        public static T? GetEnumValueByDescription<T>(string descrIption) where T : Enum
        {
            foreach (var field in typeof(T).GetFields())
            {
                if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute && attribute.Description == descrIption)
                    return (T?)field.GetValue(null);
            }

            throw new InvalidOperationException("FileDescriptionError");
        }

        public static IEnumerable<T> GetEnumProperties<T>() where T : Enum => Enum.GetValues(typeof(T)).Cast<T>();

        public static bool IsValidEnumValue(this Enum enumRequest) => Enum.IsDefined(enumRequest.GetType(), enumRequest);
    }
}
