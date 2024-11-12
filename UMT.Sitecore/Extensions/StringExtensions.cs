using System.Linq;

namespace UMT.Sitecore.Extensions
{
    public static class StringExtensions
    {
        private static readonly char[] AllowedClassNameCharacters = { '_' };
        private static readonly char[] AllowedFieldNameCharacters = { '_' };
        private static readonly char[] AllowedPathCharacters = { '_', '/' };

        public static string ToValidName(this string originalName, char[] allowedCharacters)
        {
            return new string(originalName.Where(c => char.IsLetterOrDigit(c) || allowedCharacters.Contains(c)).ToArray());
        }

        public static string ToValidClassName(this string originalName, string nameSpace)
        {
            var className = originalName.ToValidName(AllowedClassNameCharacters).EnsureDoesNotStartWithDigit();
            return !string.IsNullOrEmpty(nameSpace) ? $"{nameSpace}.{className}" : className;
        }

        public static string ToValidTableName(this string originalName, string nameSpace)
        {
            var tableName = originalName.ToValidName(AllowedClassNameCharacters).EnsureDoesNotStartWithDigit();
            return !string.IsNullOrEmpty(nameSpace) ? $"{nameSpace}_{tableName}" : tableName;
        }

        public static string ToValidItemName(this string originalName)
        {
            return originalName.ToValidName(AllowedClassNameCharacters);
        }

        public static string ToValidFieldName(this string originalName)
        {
            return originalName.ToValidName(AllowedFieldNameCharacters).EnsureDoesNotStartWithDigit();
        }
        
        public static string ToValidPath(this string originalPath)
        {
            return originalPath.Replace(' ', '_').ToValidName(AllowedPathCharacters);
        }

        public static string EnsureDoesNotStartWithDigit(this string originalValue)
        {
            if (originalValue?.Length > 0 && char.IsDigit(originalValue[0]))
            {
                return $"_{originalValue}";
            }

            return originalValue;
        }
    }
}