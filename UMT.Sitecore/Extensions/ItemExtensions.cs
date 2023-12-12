using Sitecore;
using Sitecore.Data.Items;

namespace UMT.Sitecore.Extensions
{
    public static class ItemExtensions
    {
        public static bool HasPresentationDetails(this Item item)
        {
            return !string.IsNullOrEmpty(item?[FieldIDs.LayoutField]) || 
                   !string.IsNullOrEmpty(item?[FieldIDs.FinalLayoutField]);
        }
    }
}