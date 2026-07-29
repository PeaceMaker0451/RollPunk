using NetcodeCommon;
using RollPunk.ClientNetcode;
using RollPunk.Debug;
using RollPunk.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RollPunk.Client.Game
{
    internal class MutationCatcher
    {
        private IDataBridge _dataBridge;
        private FieldsRegistry _registry;

        private HashSet<Guid> _pending = new();
        private HashSet<Guid> _added = new();
        private HashSet<Guid> _removed = new();
        private bool _isSendingBlocked = false;
        private bool _isChangesIgnoring = false;

        public MutationCatcher(FieldsRegistry registry, IDataBridge dataBridge = null)
        {
            _dataBridge = dataBridge;
            _registry = registry;

            _registry.Changed += OnFieldChanged;
            _registry.FieldAdded += OnFieldAdded;
            _registry.FieldRemoved += OnFieldRemoved;
        }

        public void BlockSending() => _isSendingBlocked = true;
        public void UnblockSending() => _isSendingBlocked = false;

        public void StartIgnore() => _isChangesIgnoring = true;
        public void StopIgnore() => _isChangesIgnoring = false;

        public void Flush()
        {
            if (_pending.Count == 0 && _removed.Count == 0 && _added.Count == 0) return;

            var changes = _pending.Select(id => _registry.FieldsDictionary[id]);
            var adds = _added.Select(id => _registry.FieldsDictionary[id]);
            var deletions = _removed.ToList();

            SessionPatch patch = new SessionPatch();

            foreach (var field in changes)
                patch.PendingFields.Add(FieldStateExtractor.ExctractExclusiveFieldState(field));

            foreach (var field in adds)
                patch.PendingFields.Add(FieldStateExtractor.ExtractFieldTreeState(field));

            foreach (var fieldId in deletions)
                patch.RemoveFields.Add(fieldId);

            // лог
            StringBuilder sb = new StringBuilder();
            sb.Append("[color=deep_sky_blue]SessionDelta sended:");
            foreach (var change in _pending)
                sb.Append($"\n{change.ToString()}");
            foreach (var del in _removed)
                sb.Append($"\nDeleted: {del}");
            sb.Append("[/color]");
            RPDebug.Log(sb.ToString());

            // отправка на сервер
            if (_dataBridge != null)
                _dataBridge.SendSessionPatch(patch);

            _pending.Clear();
            _removed.Clear();
            _added.Clear();
        }

        private void OnFieldRemoved(Field field)
        {
            if (_isChangesIgnoring)
                return;

            void MarkDeleted(Field f)
            {
                _pending.Remove(f.ID);
                _removed.Add(f.ID);

                foreach (var innerField in field.Fields)
                    MarkDeleted(innerField);
            }

            MarkDeleted(field);
            RPDebug.Log($"[color=deep_sky_blue]Field removed catched {field.Name} ({field.ID})[/color]");

            if (_isSendingBlocked == false)
                Flush();
        }

        private void OnFieldChanged(Field field)
        {
            if (_isChangesIgnoring)
                return;

            _pending.Add(field.ID);
            RPDebug.Log($"[color=deep_sky_blue]Field changed catched {field.Name} ({field.ID})[/color]");

            if (_isSendingBlocked == false)
                Flush();
        }

        private void OnFieldAdded(Field field)
        {
            if (_isChangesIgnoring)
                return;
            
            _added.Add(field.ID);
            RPDebug.Log($"[color=deep_sky_blue]Field added catched {field.Name} ({field.ID})[/color]");

            if (_isSendingBlocked == false)
                Flush();
        }
    }
}
