using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Sitecore;
using Sitecore.Diagnostics;
using UMT.Sitecore.Configuration;
using UMT.Sitecore.Diagnostics;
using UMT.Sitecore.Models;

namespace UMT.Sitecore.Pipelines.ExtractItems
{
    public class SerializeItems
    {
        public virtual void Process(ExtractItemsArgs args)
        {
            Assert.ArgumentNotNull(args, nameof(args));
            UMTLog.Info($"{nameof(SerializeItems)} pipeline processor started");

            var extractFolderName = CreateExtractFolder(DateTime.Now);
            SaveSerializedLanguages(args.TargetLanguages, extractFolderName);
            SaveSerializedChannel(args.TargetChannel, extractFolderName);
            SaveSerializedItems(args.TargetItems, extractFolderName);

            UMTLog.Info($"{nameof(SerializeItems)} pipeline processor finished");
        }

        protected virtual string CreateExtractFolder(DateTime extractDateTime)
        {
            var extractFolderName = DateTime.Now.ToString(UMTSettings.DataFolderDateFormat);
            var folderPath = MainUtil.MapPath(UMTSettings.DataFolder + $"/{extractFolderName}");
            
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            return folderPath;
        }

        protected virtual void SaveSerializedLanguages(List<ContentLanguage> languages, string extractFolderName)
        {
            var fileName = MainUtil.MapPath(extractFolderName + $"/01.Languages.json");
            using (var file = File.CreateText(fileName))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(file, languages);
            }
        }

        protected virtual void SaveSerializedChannel(TargetChannel channel, string extractFolderName)
        {
            var fileName = MainUtil.MapPath(extractFolderName + $"/02.Channel.{channel.Name}.{channel.Id:D}.json");
            using (var file = File.CreateText(fileName))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(file, channel.Elements);
            }
        }

        protected virtual void SaveSerializedItems(IList<TargetItem> items, string extractFolderName)
        {
            foreach (var item in items)
            {
                using (var file = File.CreateText(GenerateFileName(item, extractFolderName)))
                {
                    var serializer = new JsonSerializer();
                    serializer.Serialize(file, item.Elements);
                }
            }
        }

        protected virtual string GenerateFileName(TargetItem item, string extractFolderName)
        {
            var prefix = item.IsWebPage ? "03.WebPage" : "03.Content";
            return MainUtil.MapPath(extractFolderName + $"/{prefix}.{item.Name}.{item.Id:D}.json");
        }
    }
}