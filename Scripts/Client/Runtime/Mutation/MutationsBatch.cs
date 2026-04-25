using System;

namespace RollPunk.Client.Runtime
{
    internal struct MutationsBatch : IDisposable
    {
        MutationCatcher _catcher;
        public MutationsBatch(MutationCatcher catcher)
        {
            _catcher = catcher;
            _catcher.BlockSending();
        }

        public void Dispose()
        {
            _catcher.UnblockSending();
            _catcher.Flush();
        }
    }
}
