using System.Collections.Generic;
using Sitecore.Data.Items;
using Sitecore.Pipelines;
using UMT.Sitecore.Models;

namespace UMT.Sitecore.Pipelines.ExtractItems
{
    public class ExtractItemsArgs : PipelineArgs
    {
        public List<string> ContentPaths { get; set; }
        
        public Channel Channel { get; set; }
        
        public List<Item> SourceItems { get; set; }
        
        public List<TargetItem> TargetItems { get; set; }
    }
}