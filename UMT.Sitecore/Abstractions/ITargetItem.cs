using System;

namespace UMT.Sitecore.Abstractions
{
    public interface ITargetItem
    {
        string GetName();
        Guid GetId();
    }
}