namespace Microsoft.KeyVault.Core.Model
{
    public class Secret
    {
        public Secret() { }
        public Secret(string key, string value)
        {
            Key = key;
            Value = value;
        }
        public string Key { get; set; }
        public string Value { get; set; }
        public string Error { get; set; }
    }
}