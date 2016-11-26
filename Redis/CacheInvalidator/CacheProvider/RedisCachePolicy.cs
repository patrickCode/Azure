using System;
using System.Collections.Generic;

namespace CacheProvider
{
    public class RedisCachePolicy : ICachePolicy
    {
        private Type _contentType;
        private IConverter _converter;

        private Dictionary<string, object> _policies { get; set; }

        public TimeSpan TimeToLive { get; set; }

        public Type ContentType
        {
            get
            {
                return _contentType ?? typeof(string);
            }

            set
            {
                if (value == null)
                    throw new Exception("Value cannot be null");
                _contentType = value;
            }
        }

        public IConverter Converter
        {
            get
            {
                return _converter ?? new JsonConverter();
            }

            set
            {
                if (value == null)
                    throw new Exception("Converter cannot be null");
                _converter = value;
            }
        }

        public bool LifeTimeCaching { get; set; }

        public RedisCachePolicy()
        {
            _policies = new Dictionary<string, object>();
            TimeToLive = TimeSpan.FromHours(12);
            LifeTimeCaching = true;
        }

        public Dictionary<string, object> Resolve()
        {
            CreatePolicyDict();
            return _policies;
        }

        private void CreatePolicyDict()
        {
            if (_policies.ContainsKey("TimeToLive"))
                _policies.Remove("TimeToLive");
            _policies.Add("TimeToLive", TimeToLive);
            if (_policies.ContainsKey("ContentType"))
                _policies.Remove("ContentType");
            _policies.Add("ContentType", ContentType);
            if (_policies.ContainsKey("Converter"))
                _policies.Remove("Converter");
            _policies.Add("Converter", Converter);
            if (_policies.ContainsKey("LifeTimeCaching"))
                _policies.Remove("LifeTimeCaching");
            _policies.Add("LifeTimeCaching", LifeTimeCaching);
        }
    }
}