using Newtonsoft.Json.Linq;
using RollPunk.Entities;
using RollPunk.Modding.APIs;

namespace RollPunk.Players
{
    [EntityType("Player")]
    public class Player : Entity, IAPIHandler
    {
        private PlayerAPI _api;
        
        public bool IsAdmin { get; private set; }
        public Guid? TeamId { get; private set; }
        public Guid PlayerID { get; private set; }

        public Player(string name, Guid playerID, bool isAdmin): base (name)
        {
            _api = new PlayerAPI(this);
            PlayerID = playerID;
            IsAdmin = isAdmin;
        }

        public Player(EntityState objectData) : base (objectData) 
        {
            _api = new PlayerAPI(this);
        }

        protected override void ApplyPayload(Dictionary<string, JToken> payload)
        {
            IsAdmin = Get<bool>(payload, nameof(IsAdmin));
            PlayerID = Guid.Parse(Get<string>(payload, nameof(PlayerID)));
            
            string teamID = Get<string>(payload, nameof(TeamId));
            
            if(teamID == string.Empty)
                TeamId = null;
            else
                TeamId = Guid.Parse(teamID);
        }

        protected override void WritePayload(Dictionary<string, JToken> payload)
        {
            payload[nameof(IsAdmin)] = IsAdmin;
            payload[nameof(PlayerID)] = PlayerID.ToString();
            payload[nameof(TeamId)] = TeamId == null? string.Empty : TeamId.ToString();
        }

        public API GetAPI()
        {
            return _api;
        }
    }
}
