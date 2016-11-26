using System;

namespace CacheProvider
{
    public interface IConverter
    {
        string Serialize(object obj, Type type);
        dynamic Deserialize(string serializedObj, Type type);
    }
}
