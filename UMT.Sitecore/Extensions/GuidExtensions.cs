using System;
using System.Security.Cryptography;
using System.Text;

namespace UMT.Sitecore.Extensions
{
    public static class GuidExtensions
    {
        public static Guid GenerateDerivedGuid(this Guid originalGuid, params string[] salt)
        {
            var input = originalGuid + string.Concat(salt);

            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));

                return new Guid(hash);
            }
        }
    }
}