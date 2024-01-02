
using OIDCDemo.AuthorizationServer.Models;
using System.Collections.Concurrent;

namespace OIDCDemo.AuthorizationServer
{
    public class MemoryCodeStorage : ICodeStorage
    {
        private readonly IDictionary<string, DateTime> storage = new Dictionary<string, DateTime>();   
        public bool TryAddCode(string code, DateTime expiryTime)
        {
            lock (storage)
            {
                return storage.TryAdd(code, expiryTime);
            }
        }

        public bool TryGetToken(string code, out DateTime expiryTime)
        {
            lock (storage)
            {
                if (storage.TryGetValue(code, out expiryTime))
                {
                    if (expiryTime < DateTime.Now)
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
