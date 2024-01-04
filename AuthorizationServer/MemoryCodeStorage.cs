
using OIDCDemo.AuthorizationServer.Models;
using System.Collections.Concurrent;

namespace OIDCDemo.AuthorizationServer
{
    public class MemoryCodeStorage : ICodeStorage
    {
        private readonly IDictionary<string, CodeStorageValue> storage = new Dictionary<string, CodeStorageValue>();
        private static SpinLock spinLock = new SpinLock();
        public bool TryAddCode(string code, CodeStorageValue codeStorageValue)
        {
            bool lockTaken = false;

            spinLock.Enter(ref lockTaken);
            if (lockTaken)
            {
                var b = storage.TryAdd(code, codeStorageValue);
                spinLock.Exit();

                return b;
            }

            return false;
        }

        public bool TryGetToken(string code, out CodeStorageValue? codeStorageValue)
        {
            bool lockTaken = false;

            spinLock.Enter(ref lockTaken);
            if (lockTaken)
            {
                if (storage.TryGetValue(code, out codeStorageValue))
                {
                    if (codeStorageValue.ExpiryTime < DateTime.Now)
                    {
                        storage.Remove(code);

                        spinLock.Exit();
                        return false;
                    }
                    else
                    {
                        spinLock.Exit();
                        return true;
                    }
                }

                spinLock.Exit();
            }

            codeStorageValue = null;
            return false;
        }

        public bool TryRemove(string code)
        {
            bool lockTaken = false;

            spinLock.Enter(ref lockTaken);
            if (lockTaken)
            {
                spinLock.Exit();
                return storage.Remove(code);
            }

            return false;
        }
    }
}
