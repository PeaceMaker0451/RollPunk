using RollPunk.ClientSide.Runtime;
using RollPunk.Modding.APIs;
using System.Collections.Generic;

namespace RollPunk.Client.Game
{
    internal class RollpunkRootApis
    {
        private List<API> _apis;
        
        public IReadOnlyList<API> Apis => _apis;

        public RollpunkRootApis()
        {
            _apis = new List<API>()
            {
                Root.Forms.GetAPI(),
                new RollPunkAPI(),

            };
        }
    }
}
