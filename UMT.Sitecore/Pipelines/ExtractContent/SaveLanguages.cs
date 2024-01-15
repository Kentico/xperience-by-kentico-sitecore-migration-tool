using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data.Managers;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using UMT.Sitecore.Configuration;
using UMT.Sitecore.Diagnostics;
using UMT.Sitecore.Models;

namespace UMT.Sitecore.Pipelines.ExtractContent
{
    public class SaveLanguages : BaseSaveProcessor
    {
        public virtual void Process(ExtractContentArgs args)
        {
            Assert.ArgumentNotNull(args, nameof(args));
            Assert.ArgumentNotNull(args.SourceLanguages, nameof(args.SourceLanguages));

            UMTLog.Info($"{nameof(SaveLanguages)} pipeline processor started");
            UMTLog.Info($"Saving languages JSON file...", true);

            try
            {
                var targetLanguages = GetTargetLanguages(args.SourceLanguages);
                SaveSerializedLanguages(targetLanguages, args.OutputFolderPath);
                UMTLog.Info($"{targetLanguages.Count} languages mapped and saved", true);
            }
            catch (Exception e)
            {
                UMTLog.Error($"Error saving languages, please check logs for more details", true, e);
                args.AbortPipeline();
            }

            UMTLog.Info($"{nameof(SaveLanguages)} pipeline processor finished");
        }

        protected virtual List<ContentLanguage> GetTargetLanguages(IList<Language> languages)
        {
            var mappedLanguages = new List<ContentLanguage>();

            foreach (var language in languages)
            {
                var languageItem = LanguageManager.GetLanguageItem(language, Factory.GetDatabase(UMTSettings.Database));
                var languageFallbackId = languages.FirstOrDefault(x => string.Equals(x.Name, languageItem[FieldIDs.FallbackLanguage]))?.Origin?.ItemId?.Guid;
                var mappedLanguage = new ContentLanguage
                {
                    ContentLanguageName = language.Name,
                    ContentLanguageCultureFormat = language.CultureInfo.Name,
                    ContentLanguageDisplayName = language.CultureInfo.DisplayName,
                    ContentLanguageIsDefault = string.Equals(language.Name, Settings.DefaultLanguage,
                        StringComparison.OrdinalIgnoreCase),
                    ContentLanguageGUID = UMTConfiguration.LanguageMapping.GetTargetLanguageId(language.Origin.ItemId.Guid),
                    ContentLanguageFallbackContentLanguageGuid = languageFallbackId.HasValue ? 
                        UMTConfiguration.LanguageMapping.GetTargetLanguageId(languageFallbackId.Value) : (Guid?)null
                };
                mappedLanguages.Add(mappedLanguage);
            }

            return mappedLanguages;
        }
        
        
        protected virtual void SaveSerializedLanguages(List<ContentLanguage> languages, string outputFolderPath)
        {
            var folderPath = CreateFileExtractFolder($"{outputFolderPath}/01.Configuration");
            var fileName = $"{folderPath}/01.Languages.json";
            SerializeToFile(languages, fileName);
        }
    }
}