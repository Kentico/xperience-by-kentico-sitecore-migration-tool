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

        public static Guid ToContentItemDataGuid(this Guid originalGuid, Guid languageId)
        {
            return originalGuid.GenerateDerivedGuid("ContentItemData", languageId.ToString());
        }

        public static Guid ToContentItemCommonDataGuid(this Guid originalGuid, string languageName)
        {
            return originalGuid.GenerateDerivedGuid("ContentItemCommonData", languageName);
        }

        public static Guid ToContentItemLanguageMetadataGuid(this Guid originalGuid, string languageName)
        {
            return originalGuid.GenerateDerivedGuid("ContentItemLanguageMetadata", languageName);
        }

        public static Guid ToWebPageItemGuid(this Guid originalGuid)
        {
            return originalGuid.GenerateDerivedGuid("WebPageItem");
        }

        public static Guid ToWebPageUrlPathGuid(this Guid originalGuid, Guid languageId)
        {
            return originalGuid.GenerateDerivedGuid("WebPageUrlPath", languageId.ToString());
        }
    }
}