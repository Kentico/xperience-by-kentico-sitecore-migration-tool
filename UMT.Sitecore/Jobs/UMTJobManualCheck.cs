using System;

namespace UMT.Sitecore.Jobs
{
    public class UMTJobManualCheck
    {
        public EntityType EntityType { get; set; }
        public string EntityName { get; set; }
        public Guid EntityId { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            return $"{EntityType} \"{EntityName}\" ({EntityId}): {Message}";
        }
    }

    public enum EntityType
    {
        Template,
        Field,
        Item
    }
}