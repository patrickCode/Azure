using System;
using Newtonsoft.Json;

namespace CacheProvider
{
    public class JsonConverter : IConverter
    {
        public dynamic Deserialize(string serializedObj, Type type)
        {
            if (type == typeof(string))
                return serializedObj;
            var currentObject = JsonConvert.DeserializeObject(serializedObj, type);
            if (currentObject != null)
                return currentObject;
            throw new Exception("Unable to deserialize");
        }

        public string Serialize(object obj, Type type)
        {
            if (obj.GetType() == typeof(string))
                return obj as string;
            if (obj.GetType() == type)
                return JsonConvert.SerializeObject(obj);
            throw new Exception("Unable to serialize");
        }
    }
}
