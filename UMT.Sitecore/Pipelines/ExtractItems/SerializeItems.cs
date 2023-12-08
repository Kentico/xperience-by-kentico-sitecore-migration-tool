using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Sitecore;
using Sitecore.Diagnostics;
using UMT.Sitecore.Abstractions;
using UMT.Sitecore.Configuration;
using UMT.Sitecore.Diagnostics;
using UMT.Sitecore.Models;

namespace UMT.Sitecore.Pipelines.ExtractItems
{
    public class SerializeItems
    {
        public void Process(ExtractItemsArgs args)
        {
            Assert.ArgumentNotNull(args, nameof(args));
            UMTLog.Info($"{nameof(SerializeItems)} pipeline processor started");

            SaveSerializedTemplates(args.TargetItems, DateTime.Now.ToString(UMTSettings.DataFolderDateFormat));

            UMTLog.Info($"{nameof(SerializeItems)} pipeline processor finished");
        }

        protected virtual void SaveSerializedTemplates(IList<ITargetItem> items, string extractFolderName)
        {
            foreach (var item in items)
            {
                using (StreamWriter file = File.CreateText(GenerateFileName(item, extractFolderName)))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, new[] { item });
                }
            }
        }

        protected virtual string GenerateFileName(ITargetItem item, string extractFolderName)
        {
            var folderPath = MainUtil.MapPath(UMTSettings.DataFolder + $"/{extractFolderName}");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            return MainUtil.MapPath(folderPath + $"/{item.GetName()}.{item.GetId():D}.json");
        }
    }
}