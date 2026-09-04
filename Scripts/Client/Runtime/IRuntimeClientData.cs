using System;

namespace RollPunk.Client.Game
{
    public interface IRuntimeClientData
    {
        public Guid ClientID { get; }
        public string Name { get; }
    }
}
