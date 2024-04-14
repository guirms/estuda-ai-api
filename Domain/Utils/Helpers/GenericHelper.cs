using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;

namespace Domain.Utils.Helpers
{
    public static class GenericHelper
    {
        public static string? GetClaimValue(this JwtSecurityToken jwtSecurityTokenRequest, string key) =>
            jwtSecurityTokenRequest.Claims?.FirstOrDefault(c => c.Type == key)?.Value;

        public static T MapIgnoringNullProperties<T>(this object objectRequest, T dest)
        {
            foreach (var propertyName in objectRequest!.GetType().GetProperties().Where(p => !p.PropertyType.IsGenericType).Select(p => p.Name))
            {
                var value = objectRequest.GetType().GetProperty(propertyName)!.GetValue(objectRequest, null);

                if (value != null)
                    dest!.GetType().GetProperty(propertyName)!.SetValue(dest, value, null);
            }

            return dest;
        }

        public static string GetTag(this IConfiguration configurationRequest, string tag) => configurationRequest[tag]!;
    }
}
