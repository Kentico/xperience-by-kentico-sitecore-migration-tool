using System;
using System.Collections.Generic;
using System.Linq;

namespace UMT.Sitecore.Extensions
{
    public static class StringExtensions
    {
        private static readonly char[] AllowedClassNameCharacters = { '_' };
        private static readonly char[] AllowedFieldNameCharacters = { '_' };
        private static readonly char[] AllowedPathCharacters = { '_', '/' };
        private const int CodeNameMaxLength = 67; // CodeName should be 100 characters or less (100 - Guid length - 1) 
        private const int FolderNameMaxLength = 17; // FolderName should be 50 characters or less (50 - Guid length - 1) 

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

        public static string ToValidCodename(this string originalName, Guid id)
        {
            var shortItemName = originalName;
            if (shortItemName.Length > CodeNameMaxLength)
            {
                shortItemName = shortItemName.Substring(0, CodeNameMaxLength);
            }
            return $"{shortItemName}-{id:N}";
        }
        
        public static string ToValidFolderName(this string originalName, Guid id)
        {
            var shortItemName = originalName;
            if (shortItemName.Length > FolderNameMaxLength)
            {
                shortItemName = shortItemName.Substring(0, FolderNameMaxLength);
            }
            return $"{shortItemName}-{id:N}";
        }

        public static int GetTreeDepthLevel(this string contentPath)
        {
            return contentPath?.Trim('/').Count(x => x == '/') ?? 0;
        }

        public static string GetItemPath(this string fullPath, IList<string> contentRoots)
        {
            foreach (var contentRoot in contentRoots)
            {
                fullPath = fullPath.Replace(contentRoot, String.Empty);
            }

            return fullPath;
        }

        public static List<string> GetPathsToRemove(this IList<string> rootPaths)
        {
            if (rootPaths != null && rootPaths.Count > 0)
            {
                return rootPaths.Select(rootPath =>
                {
                    var segments = rootPath.Split(new [] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                    segments = segments.Take(segments.Length - 1).ToArray();
                    return "/" + string.Join("/", segments);
                }).OrderByDescending(x => x.Length).ToList();
            }

            return new List<string>();
        }
    }
}