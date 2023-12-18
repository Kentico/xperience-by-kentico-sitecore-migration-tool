using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Sitecore;
using Sitecore.Diagnostics;
using UMT.Sitecore.Diagnostics;
using UMT.Sitecore.Models;

namespace UMT.Sitecore.Pipelines.ExtractContent
{
    public class SerializeItems
    {
        public virtual void Process(ExtractContentArgs args)
        {
            Assert.ArgumentNotNull(args, nameof(args));
            UMTLog.Info($"{nameof(SerializeItems)} pipeline processor started");

            SaveSerializedLanguages(args.TargetLanguages, args.OutputFolderPath);
            SaveSerializedChannel(args.TargetChannel, args.OutputFolderPath);
            SaveSerializedItems(args.TargetItems, args.OutputFolderPath);

            UMTLog.Info($"{nameof(SerializeItems)} pipeline processor finished");
        }

        protected virtual void SaveSerializedLanguages(List<ContentLanguage> languages, string folderPath)
        {
            var fileName = MainUtil.MapPath($"{folderPath}/01.Languages.json");
            using (var file = File.CreateText(fileName))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(file, languages);
            }
        }

        protected virtual void SaveSerializedChannel(TargetChannel channel, string folderPath)
        {
            var fileName = MainUtil.MapPath($"{folderPath}/02.Channel.{channel.Name}.{channel.Id:D}.json");
            using (var file = File.CreateText(fileName))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(file, channel.Elements);
            }
        }

        protected virtual void SaveSerializedItems(IList<TargetItem> items, string folderPath)
        {
            foreach (var item in items)
            {
                using (var file = File.CreateText(GenerateFileName(item, folderPath)))
                {
                    var serializer = new JsonSerializer();
                    serializer.Serialize(file, item.Elements);
                }
            }
        }

        protected virtual string GenerateFileName(TargetItem item, string folderPath)
        {
            var prefix = item.IsWebPage ? "06.WebPage" : "06.Content";
            return MainUtil.MapPath($"{folderPath}/{prefix}.{item.Name}.{item.Id:D}.json");
        }
    }
}