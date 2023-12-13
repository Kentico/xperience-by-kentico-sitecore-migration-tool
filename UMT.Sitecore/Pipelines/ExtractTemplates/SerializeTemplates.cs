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
        public virtual void Process(ExtractTemplatesArgs args)
        {
            Assert.ArgumentNotNull(args, nameof(args));
            UMTLog.Info($"{nameof(SerializeTemplates)} pipeline processor started");

            SaveSerializedTemplates(args.TargetTemplates, args.ExtractFolderName);

            UMTLog.Info($"{nameof(SerializeTemplates)} pipeline processor finished");
        }

        protected virtual void SaveSerializedTemplates(IList<TargetContentType> templates, string extractFolderName)
        {
            foreach (var template in templates)
            {
                using (var file = File.CreateText(GenerateFileName(template, extractFolderName)))
                {
                    var serializer = new JsonSerializer();
                    serializer.Serialize(file, template.Elements);
                }
            }
        }

        protected virtual string GenerateFileName(TargetContentType template, string extractFolderName)
        {
            var folderPath = MainUtil.MapPath(UMTSettings.DataFolder + $"/{extractFolderName}");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            return MainUtil.MapPath(folderPath + $"/{template.Name}.{template.Id:D}.json");
        }
    }
}
