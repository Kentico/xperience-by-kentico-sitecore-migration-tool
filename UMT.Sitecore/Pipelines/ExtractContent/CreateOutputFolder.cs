using System;
using System.IO;
using Sitecore;
using Sitecore.Diagnostics;
using UMT.Sitecore.Configuration;
using UMT.Sitecore.Diagnostics;

namespace UMT.Sitecore.Pipelines.ExtractContent
{
    public class CreateOutputFolder
    {
        public virtual void Process(ExtractContentArgs args)
        {
            Assert.ArgumentNotNull(args, nameof(args));
            UMTLog.Info($"{nameof(CreateOutputFolder)} pipeline processor started");

            try
            {
                var outputFolder = CreateExtractFolder(DateTime.Now);
                args.OutputFolderPath = outputFolder;
                
                UMTLog.Info($"Output folder created: {outputFolder}");
            }
            catch (Exception e)
            {
                UMTLog.Error($"Error creating output folder, please check UMT config settings", e);
                args.AbortPipeline();
            }
            
            UMTLog.Info($"{nameof(CreateOutputFolder)} pipeline processor finished");
        }

        protected virtual string CreateExtractFolder(DateTime extractDateTime)
        {
            var extractFolderName = extractDateTime.ToString(UMTSettings.DataFolderDateFormat);
            var folderPath = MainUtil.MapPath($"{UMTSettings.DataFolder}/{extractFolderName}");
            
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            return folderPath;
        }
    }
}