using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Sitecore;
using Sitecore.Diagnostics;
using UMT.Sitecore.Configuration;
using UMT.Sitecore.Diagnostics;
using UMT.Sitecore.Models;
using File = System.IO.File;

namespace UMT.Sitecore.Pipelines.ExtractTemplates
{
    public class SerializeTemplates
    {
        public void Process(ExtractTemplatesArgs args)
        {
            Assert.ArgumentNotNull(args, nameof(args));
            UMTLog.Info($"{nameof(SerializeTemplates)} pipeline processor started");

            SaveSerializedTemplates(args.TargetTemplates, args.ExtractFolderName);

            UMTLog.Info($"{nameof(SerializeTemplates)} pipeline processor finished");
        }

        protected virtual void SaveSerializedTemplates(IList<DataClass> templates, string extractFolderName)
        {
            foreach (var template in templates)
            {
                using (StreamWriter file = File.CreateText(GenerateFileName(template, extractFolderName)))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, new[] { template });
                }
            }
        }

        protected virtual string GenerateFileName(DataClass template, string extractFolderName)
        {
            var folderPath = MainUtil.MapPath(UMTSettings.DataFolder + $"/{extractFolderName}");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            return MainUtil.MapPath(folderPath + $"/{template.ClassName}.{template.ClassGUID:D}.json");
        }
    }
}
