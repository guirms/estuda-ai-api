using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text.RegularExpressions;

namespace Domain.Utils.Helpers
{
    public static partial class StringHelper
    {
        [GeneratedRegex(@"[a-zA-Z]")]
        private static partial Regex cpfPattern();
        [GeneratedRegex(@"[a-zA-Z]")]
        private static partial Regex cnpjPattern();
        [GeneratedRegex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")]
        private static partial Regex emailPattern();

        public static string ToSafeValue(this string? stringRequest) => stringRequest ?? string.Empty;

        public static int? ToIntSafe(this string? stringRequest)
        {
            try
            {
                if (stringRequest == null)
                    return null;

                return int.Parse(stringRequest);
            }
            catch
            {
                return null;
            }
        }

        public static int ToInt(this string stringRequest, string exceptionMessage)
        {
            try
            {
                return int.Parse(stringRequest);
            }
            catch
            {
                throw new FormatException(exceptionMessage);
            }
        }

        public static string ToDocument(this string stringRequest)
        {
            if (stringRequest.Length == 11)
                return stringRequest.ToCpf();
            else if (stringRequest.Length == 14)
                return stringRequest.ToCnpj();

            throw new ArgumentException("InvalidDocument");
        }

        public static string ToCpf(this string stringRequest)
        {
            if (stringRequest.Length != 11)
                throw new ArgumentException("Invalid CPF");

            var cpfPattern = @"(\d{3})(\d{3})(\d{3})(\d{2})";
            var replacement = "$1.$2.$3-$4";

            return Regex.Replace(stringRequest, cpfPattern, replacement);
        }

        public static string ToCnpj(this string stringRequest)
        {
            if (stringRequest.Length != 14)
                throw new ArgumentException("InvalidCnpj");

            var cnpjPattern = @"(\d{2})(\d{3})(\d{3})(\d{4})(\d{2})";
            var replacement = "$1.$2.$3/$4-$5";

            return Regex.Replace(stringRequest, cnpjPattern, replacement);
        }

        public static string ToZipCode(this string stringRequest)
        {
            if (stringRequest.Length == 8)
                return stringRequest.Insert(5, "-");

            return stringRequest;
        }

        public static DateTime? ToDateTimeSafe(this string? stringRequest)
        {
            if (DateTime.TryParse(stringRequest, out var dateTimeParsed))
                return dateTimeParsed;

            return null;
        }

        public static string ToValidIp(this string stringRequest)
        {
            if (stringRequest.Contains("::ffff:"))
                stringRequest = stringRequest.Replace("::ffff:", string.Empty);

            if (stringRequest == "::1")
                stringRequest = "localhost";

            return stringRequest;
        }

        public static bool IsValidEmail(this string stringRequest) => !stringRequest.IsNullOrEmpty() && emailPattern().IsMatch(stringRequest) && stringRequest.Length <= 89;

        public static bool IsValidDocument(this string stringRequest)
        {
            if (stringRequest.Length == 11)
                return IsValidCpf(stringRequest);
            else if (stringRequest.Length == 14)
                return IsValidCnpj(stringRequest);

            return false;
        }

        public static string? GetClaimValue(this string stringRequest, string key)
        {
            try
            {
                return new JwtSecurityTokenHandler().ReadJwtToken(stringRequest.ToCleanJwtToken())?
                    .Claims?.FirstOrDefault(c => c.Type == key)?.Value;
            }
            catch
            {
                return null;
            }
        }

        public static string ToCleanJwtToken(this string stringRequest) => stringRequest.Replace("Bearer ", string.Empty);

        public static bool IsValidLength(this string stringRequest, int length) => !stringRequest.IsNullOrEmpty() && stringRequest.Length <= length;

        public static bool IsValidCpf(this string stringRequest)
        {
            if (stringRequest.Length != 11 || cpfPattern().Matches(stringRequest).Count > 0 || stringRequest.Distinct().Count() == 1)
                return false;

            stringRequest = stringRequest.Trim();

            var tempCpf = stringRequest[..9];

            var sum = 0;
            for (var i = 0; i < 9; i++)
                sum += int.Parse(tempCpf[i].ToString()) * (10 - i);

            var remainder = sum % 11;
            var digit1 = remainder < 2 ? 0 : 11 - remainder;

            tempCpf += digit1;

            sum = 0;
            for (int i = 0; i < 10; i++)
                sum += int.Parse(tempCpf[i].ToString()) * (11 - i);

            remainder = sum % 11;
            var digit2 = remainder < 2 ? 0 : 11 - remainder;

            tempCpf += digit2;

            return stringRequest.EndsWith(tempCpf);
        }

        public static bool IsValidCnpj(this string stringRequest)
        {
            if (cnpjPattern().Matches(stringRequest).Count > 0)
                return false;

            var mult1 = new int[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            var mult2 = new int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

            stringRequest = stringRequest.Trim();

            if (stringRequest.Length != 14)
                return false;

            string? tempCnpj = stringRequest[..12];
            var sum = 0;
            for (var i = 0; i < 12; i++)
                sum += int.Parse(tempCnpj[i].ToString()) * mult1[i];

            var rest = sum % 11;
            if (rest < 2)
                rest = 0;
            else
                rest = 11 - rest;

            var digit = rest.ToString();
            tempCnpj += digit;

            sum = 0;
            for (var i = 0; i < 13; i++)
                sum += int.Parse(tempCnpj[i].ToString()) * mult2[i];

            rest = sum % 11;
            if (rest < 2)
                rest = 0;
            else
                rest = 11 - rest;

            digit += rest.ToString();

            return stringRequest.EndsWith(digit);
        }

        public static bool IsValidLatitude(this string stringRequest)
        {
            stringRequest = stringRequest.Replace(".", ",");

            if (stringRequest.Length > 10)
                return false;

            if (double.TryParse(stringRequest, out double latitudeValue))
                return latitudeValue >= -90 && latitudeValue <= 90;

            return false;
        }

        public static bool IsValidLongitude(this string stringRequest)
        {
            stringRequest = stringRequest.Replace(".", ",");

            if (stringRequest.Length > 10)
                return false;

            if (double.TryParse(stringRequest, out double longitudeValue))
                return longitudeValue >= -180 && longitudeValue <= 180;

            return false;
        }
    }
}
