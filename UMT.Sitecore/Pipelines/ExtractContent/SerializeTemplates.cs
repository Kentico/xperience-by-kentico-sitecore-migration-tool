﻿using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Sitecore;
using Sitecore.Diagnostics;
using UMT.Sitecore.Diagnostics;
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

        protected virtual void SaveSerializedTemplates(Dictionary<Guid, TargetContentType> templates, string outputFolderPath)
        {
            var folderPath = MainUtil.MapPath($"{outputFolderPath}/02.ContentTypes");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            
            foreach (var template in templates)
            {
                using (var file = File.CreateText(GenerateFileName(template.Value, folderPath)))
                {
                    var serializer = new JsonSerializer();
                    serializer.Serialize(file, new List<object>
                    {
                        template.Value.ContentType, 
                        template.Value.ContentTypeChannel
                    });
                }
            }
        }

        protected virtual string GenerateFileName(TargetContentType template, string folderPath)
        {
            return MainUtil.MapPath($"{folderPath}/{template.Name}.{template.Id:D}.json");
        }
    }
}