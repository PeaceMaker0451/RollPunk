using Newtonsoft.Json.Linq;
using RollPunk.Entities;
using RollPunk.Modding.APIs;
using System;
using System.Collections.Generic;

namespace RollPunk.Players
{
    [EntityType("Player")]
    public class Player : Entity, IAPIHandler
    {
        private PlayerAPI _api;
        
        public bool IsAdmin { get; private set; }
        public Guid? TeamId { get; private set; }
        public Guid ClientId { get; private set; }

        public Player(string name, Guid clientId, bool isAdmin): base (name)
        {
            _api = new PlayerAPI(this);
            ClientId = clientId;
            IsAdmin = isAdmin;
        }

        public Player(EntityState objectData) : base (objectData) 
        {
            _api = new PlayerAPI(this);
        }

        protected override void ApplyPayload(Dictionary<string, JToken> payload)
        {
            IsAdmin = Get<bool>(payload, nameof(IsAdmin));
            
            // Безопасный парсинг PlayerID
            string playerIdStr = Get<string>(payload, nameof(ClientId));
            if (!Guid.TryParse(playerIdStr, out Guid parsedPlayerId))
                throw new InvalidOperationException($"Invalid PlayerID format: {playerIdStr}");
            ClientId = parsedPlayerId;
            
            // Безопасный парсинг TeamId
            string teamID = Get<string>(payload, nameof(TeamId));
            if (string.IsNullOrEmpty(teamID))
                TeamId = null;
            else if (!Guid.TryParse(teamID, out Guid parsedTeamId))
                throw new InvalidOperationException($"Invalid TeamId format: {teamID}");
            else
                TeamId = parsedTeamId;
        }

        protected override void WritePayload(Dictionary<string, JToken> payload)
        {
            payload[nameof(IsAdmin)] = IsAdmin;
            payload[nameof(ClientId)] = ClientId.ToString();
            payload[nameof(TeamId)] = TeamId == null? string.Empty : TeamId.ToString();
        }

        public API GetAPI()
        {
            return _api;
        }
    }
}
