using System;
using System.Collections.Generic;

namespace CacheProvider
{
    public interface ICachePolicy
    {   
        TimeSpan TimeToLive { get; set; }
        bool LifeTimeCaching { get; set; }
        Type ContentType { get; set; }
        IConverter Converter { get; set; }
        Dictionary<string, object> Resolve();     
    }
}