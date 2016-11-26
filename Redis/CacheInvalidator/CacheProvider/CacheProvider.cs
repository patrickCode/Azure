using StackExchange.Redis;

namespace CacheProvider
{
    public class CacheProvider
    {
        private readonly string _connectionString;
        private IConnectionMultiplexer _connection;
        private IDatabase _cacheDb;
        public CacheProvider(string connectionString)
        {
            _connectionString = connectionString;
            Connect();
        }
        public CacheProvider(IConnectionMultiplexer connectionMux, IDatabase cacheDatabase)
        {
            _connection = connectionMux;
            _cacheDb = cacheDatabase;
        }
        private void Connect()
        {   
            _connection = ConnectionMultiplexer.Connect(_connectionString);
            _cacheDb = _connection.GetDatabase();
        }

        private void CheckConnection()
        {
            if (_connection != null && _cacheDb != null)
            {
                if (!_connection.IsConnected)
                    Connect();
            }
            else
            {
                Connect();
            }
        }

        public bool Exists(string key)
        {
            CheckConnection();
            return _cacheDb.KeyExists(key);
        }

        public dynamic Get(string key, ICachePolicy cachePolicy)
        {
            CheckConnection();
            if (!Exists(key))
                return null;
            var serializedCachedValue = _cacheDb.StringGet(key);
            return cachePolicy.Converter.Deserialize(serializedCachedValue, cachePolicy.ContentType);
            
        }

        public void Remove(string key)
        {
            CheckConnection();
            if (Exists(key))
                _cacheDb.KeyDelete(key);
        }

        public void Set(string key, dynamic value, ICachePolicy cachePolicy)
        {
            CheckConnection();
            var cachedValue = cachePolicy.Converter.Serialize(value, cachePolicy.ContentType);
            if (cachePolicy.LifeTimeCaching)
                _cacheDb.StringSet(key, cachedValue);
            else
                _cacheDb.StringSet(key, cachedValue, cachePolicy.TimeToLive);
        }
    }
}