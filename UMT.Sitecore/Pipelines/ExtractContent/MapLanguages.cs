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
    public class MapLanguages
    {
        public virtual void Process(ExtractContentArgs args)
        {
            Assert.ArgumentNotNull(args, nameof(args));
            Assert.ArgumentNotNull(args.SourceLanguages, nameof(args.SourceLanguages));

            UMTLog.Info($"{nameof(MapLanguages)} pipeline processor started");

            args.TargetLanguages = GetTargetLanguages(args.SourceLanguages);
            UMTLog.Info($"{nameof(MapLanguages)}: " + args.TargetLanguages.Count + " languages have been mapped", true);

            UMTLog.Info($"{nameof(MapLanguages)} pipeline processor finished");
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
    }
}