using System;
using System.Collections.Generic;
using Sitecore.Data.Templates;
using Sitecore.Pipelines;
using UMT.Sitecore.Configuration;
using UMT.Sitecore.Models;

namespace UMT.Sitecore.Pipelines.ExtractTemplates
{
    public class ExtractTemplatesArgs : PipelineArgs
    {
        public string ExtractFolderName { get; }
        
        public ChannelMap SourceChannel { get; set; }

        public IList<Template> SourceTemplates { get; set; }

        public IList<TargetContentType> TargetTemplates { get; set; }

        public ExtractTemplatesArgs()
        {
            ExtractFolderName = DateTime.Now.ToString(UMTSettings.DataFolderDateFormat);
        }
        public ExtractTemplatesArgs(DateTime triggeredAt)
        {
            ExtractFolderName = triggeredAt.ToString(UMTSettings.DataFolderDateFormat);
        }
    }
}
