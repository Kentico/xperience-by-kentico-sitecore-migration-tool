using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sitecore;
using Sitecore.Diagnostics;
using UMT.Sitecore.Diagnostics;
using UMT.Sitecore.Jobs;
using UMT.Sitecore.Models;
using File = System.IO.File;

namespace UMT.Sitecore.Pipelines.ExtractContent
{
    public class SerializeTemplates
    {
        public virtual void Process(ExtractContentArgs args)
        {
            Assert.ArgumentNotNull(args, nameof(args));
            UMTLog.Info($"{nameof(SerializeTemplates)} pipeline processor started");

            SaveSerializedTemplates(args.TargetTemplates, args.OutputFolderPath);

            UMTLog.Info($"{nameof(SerializeTemplates)} pipeline processor finished");
        }

        protected virtual void SaveSerializedTemplates(Dictionary<Guid, TargetContentType> templates, string folderPath)
        {
            foreach (var template in templates)
            {
                using (var file = File.CreateText(GenerateFileName(template.Value, folderPath)))
                {
                    var serializer = new JsonSerializer();
                    serializer.Serialize(file, template.Value.Elements);
                }
                UMTJob.IncreaseProcessedItems();
            }
        }

        protected virtual string GenerateFileName(TargetContentType template, string folderPath)
        {
            return MainUtil.MapPath($"{folderPath}/03.Templates.{template.Name}.{template.Id:D}.json");
        }
    }
}
