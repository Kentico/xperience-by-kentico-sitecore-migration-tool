using System.Linq;

namespace UMT.Sitecore.Extensions
{
    public static class StringExtensions
    {
        private static readonly char[] AllowedNameCharacters = new[] { '_', '-', '.' };

        public static string ToValidName(this string originalName)
        {
            return new string(originalName.Where(c => char.IsLetterOrDigit(c) || AllowedNameCharacters.Contains(c)).ToArray());
        }

        public static string ToValidClassName(this string originalName, string nameSpace)
        {
            return $"{nameSpace}.{originalName.ToValidName()}";
        }

        public static string ToValidTableName(this string originalName, string nameSpace)
        {
            return $"{nameSpace}_{originalName.ToValidName()}";
        }
    }
}