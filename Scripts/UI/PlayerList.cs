using Godot;
using NetcodeCommon;
using RollPunk.Players;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RollPunk.Scripts.UI
{
    public partial class PlayerList: ItemList
    {
        private Session _session;
        private Dictionary<int, Guid> _itemToPlayer;

        public void Initialize(Session session)
        {
            if(session == null)
                throw new NullReferenceException(nameof(session));
            
            _session = session;
            Clear();

            _itemToPlayer = new Dictionary<int, Guid>();
            
            foreach (var player in session.Players)
                AddPlayer(player.Value);


            session.PlayerAdded += (guid) =>
            {
                foreach (var player in session.Players)
                    AddPlayer(player.Value);
            };//OnPlayerAdded;

            session.PlayerRemoved += (guid) =>
            {
                foreach (var player in session.Players)
                    AddPlayer(player.Value);
            };//OnPlayerRemoved;
        }

        private void OnPlayerRemoved(Guid guid)
        {
            RemoveItem(GetPlayerItemId(guid));
            _itemToPlayer.Remove(GetPlayerItemId(guid));
        }

        private void OnPlayerAdded(Guid guid)
        {
            var player = _session.Players[guid];
            AddPlayer(player);
        }

        private int GetPlayerItemId(Guid id)
        {
            return _itemToPlayer.Where((kvp) => kvp.Value == id).FirstOrDefault().Key;
        }

        private void AddPlayer(Player player)
        {
            if (player.IsOnline == false)
                return;
            
            int id = AddItem($"{player.Name} ({player.IsAdmin})", selectable: false);
            _itemToPlayer.Add(id, player.ClientId);
        }
    }
}
