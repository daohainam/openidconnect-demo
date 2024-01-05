namespace OIDCDemo.AuthorizationServer
{
    public class MemoryRefreshTokenStorage : IRefreshTokenStorage
    {
        private readonly int MaxSize;
        private readonly List<string> tokens = []; // we can use a better implementation here
        private SpinLock spinLock = new();

        public MemoryRefreshTokenStorage() { 
            MaxSize = int.MaxValue;
        }

        public MemoryRefreshTokenStorage(int maxSize)
        {
            MaxSize = maxSize;
        }

        public bool TryAddToken(string token)
        {
            var lockTaken = false;
            spinLock.TryEnter(ref lockTaken);
            if (lockTaken)
            {
                if (!tokens.Contains(token))
                {
                    while (tokens.Count >= MaxSize)
                    {
                        tokens.RemoveAt(0);
                    }
                    tokens.Add(token);

                    spinLock.Exit();
                    return true;
                }
                else
                {
                    spinLock.Exit();
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public bool Contains(string token)
        {
            var lockTaken = false;
            spinLock.TryEnter(ref lockTaken);
            if (lockTaken)
            {
                bool b = tokens.Contains(token);
                spinLock.Exit();

                return b;
            }
            else
            {
                return false;
            }
        }
    }
}
