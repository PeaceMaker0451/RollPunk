using MoonSharp.Interpreter;
using RollPunk.Modding.APIs;
using static System.Net.WebRequestMethods;

namespace RollPunk.Players
{
    public class PlayerAPI : HeldAPI
    {
        private readonly Player _player;

        public string name => _player.Name;
        public string player_id => _player.PlayerID.ToString();
        public string team_id => _player.TeamId == null ? null: _player.TeamId.ToString();
        public bool is_admin => _player.IsAdmin;

        public PlayerAPI(Player handler) : base(handler)
        {
            _player = handler;
        }

        [MoonSharpHidden]
        public Player GetPlayer()
        {
            return _player;
        }
    }
}
