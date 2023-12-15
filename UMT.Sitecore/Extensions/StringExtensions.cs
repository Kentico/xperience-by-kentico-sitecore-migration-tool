using System.Linq;

namespace UMT.Sitecore.Extensions
{
    public static class StringExtensions
    {
        private static readonly char[] AllowedClassNameCharacters = { '_', '-', '.' };
        private static readonly char[] AllowedFieldNameCharacters = { '_' };

        public static string ToValidName(this string originalName, char[] allowedCharacters)
        {
            return new string(originalName.Where(c => char.IsLetterOrDigit(c) || allowedCharacters.Contains(c)).ToArray());
        }

        public static string ToValidClassName(this string originalName, string nameSpace)
        {
            return $"{nameSpace}.{originalName.ToValidName(AllowedClassNameCharacters)}";
        }

        public static string ToValidTableName(this string originalName, string nameSpace)
        {
            return $"{nameSpace}_{originalName.ToValidName(AllowedClassNameCharacters)}";
        }

        public static string ToValidItemName(this string originalName)
        {
            return originalName.ToValidName(AllowedClassNameCharacters);
        }

        public static string ToValidFieldName(this string originalName)
        {
            return originalName.ToValidName(AllowedFieldNameCharacters);
        }
    }
}