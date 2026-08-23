using Godot;
using NetcodeCommon;
using RollPunk.Client.Game;
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
            
            foreach (var player in session.Players)
                AddItem(player.Value.Name, selectable: false);

            _itemToPlayer = new Dictionary<int, Guid>();

            session.PlayerAdded += OnPlayerAdded;
            session.PlayerRemoved += OnPlayerRemoved;
        }

        private void OnPlayerRemoved(Guid guid)
        {
            RemoveItem(GetPlayerItemId(guid));
            _itemToPlayer.Remove(GetPlayerItemId(guid));
        }

        private void OnPlayerAdded(Guid guid)
        {
           int id = AddItem(_session.Players[guid].Name, selectable: false);
            _itemToPlayer.Add(id, guid);
        }

        private int GetPlayerItemId(Guid id)
        {
            return _itemToPlayer.Where((kvp) => kvp.Value == id).FirstOrDefault().Key;
        }
    }
}
