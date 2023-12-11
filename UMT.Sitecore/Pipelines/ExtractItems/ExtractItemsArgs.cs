using System.Collections.Generic;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using Sitecore.Pipelines;
using UMT.Sitecore.Configuration;
using UMT.Sitecore.Models;

namespace UMT.Sitecore.Pipelines.ExtractItems
{
    public class ExtractItemsArgs : PipelineArgs
    {
        public List<string> ContentPaths { get; set; }
        
        public ChannelMap SourceChannel { get; set; }
        
        public TargetChannel TargetChannel { get; set; }
        
        public List<Language> SourceLanguages { get; set; }
        
        public List<ContentLanguage> TargetLanguages { get; set; }
        
        public List<Item> SourceItems { get; set; }
        
        public List<TargetItem> TargetItems { get; set; }
    }
}