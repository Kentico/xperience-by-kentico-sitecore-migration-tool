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

            SaveSerializedTemplates(args.TargetItems, DateTime.Now.ToString(UMTSettings.DataFolderDateFormat));

            UMTLog.Info($"{nameof(SerializeItems)} pipeline processor finished");
        }

        protected virtual void SaveSerializedTemplates(IList<TargetItem> items, string extractFolderName)
        {
            foreach (var item in items)
            {
                using (StreamWriter file = File.CreateText(GenerateFileName(item, extractFolderName)))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, item.Elements);
                }
            }
        }

        protected virtual string GenerateFileName(TargetItem item, string extractFolderName)
        {
            var folderPath = MainUtil.MapPath(UMTSettings.DataFolder + $"/{extractFolderName}");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            return MainUtil.MapPath(folderPath + $"/{item.Name}.{item.Id:D}.json");
        }
    }
}