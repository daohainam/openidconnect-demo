
using OIDCDemo.AuthorizationServer.Models;
using System.Collections.Concurrent;

namespace OIDCDemo.AuthorizationServer
{
    public class MemoryCodeStorage : ICodeStorage
    {
        private readonly IDictionary<string, CodeStorageValue> storage = new Dictionary<string, CodeStorageValue>();   
        public bool TryAddCode(string code, CodeStorageValue codeStorageValue)
        {
            lock (storage)
            {
                return storage.TryAdd(code, codeStorageValue);
            }
        }

        public bool TryGetToken(string code, out CodeStorageValue? codeStorageValue)
        {
            lock (storage)
            {
                if (storage.TryGetValue(code, out codeStorageValue))
                {
                    if (codeStorageValue.ExpiryTime < DateTime.Now)
                    {
                        storage.Remove(code);
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }

                return false;
            }
        }
    }
}
