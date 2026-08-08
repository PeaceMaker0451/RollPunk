using NetcodeCommon;
using RollPunk.ClientNetcode;
using RollPunk.Debug;
using RollPunk.Fields;
using RollPunk.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RollPunk.Client.Game
{
    internal class MutationCatcher
    {
        private Session _session;
        private IDataBridge _dataBridge;
        private IReadOnlyFieldRegistry _registry;

        private HashSet<Guid> _fieldChanged = new();
        private HashSet<Guid> _fieldAdded = new();
        private HashSet<Guid> _fieldRemoved = new();
        private List<Event> _pendingLogs = new();
        private bool _isSendingBlocked = false;
        private bool _isChangesIgnoring = false;

        public MutationCatcher(Session session, IDataBridge dataBridge = null)
        {
            _session = session;
            _dataBridge = dataBridge;
            _registry = session.Registry;

            _registry.Changed += OnFieldChanged;
            _registry.FieldAdded += OnFieldAdded;
            _registry.FieldRemoved += OnFieldRemoved;
            _session.LogAdded += OnLogAdded;
        }

        public void BlockSending() => _isSendingBlocked = true;
        public void UnblockSending() => _isSendingBlocked = false;

        public void StartIgnore() => _isChangesIgnoring = true;
        public void StopIgnore() => _isChangesIgnoring = false;

        public void Flush()
        {
            if (_fieldChanged.Count == 0 && _fieldRemoved.Count == 0 && _fieldAdded.Count == 0) return;

            var changes = _fieldChanged.Select(id => _registry.FieldsDictionary[id]);
            var adds = _fieldAdded.Select(id => _registry.FieldsDictionary[id]);
            var deletions = _fieldRemoved.ToList();

            SessionPatch patch = new SessionPatch();

            foreach (var field in changes)
                patch.PendingFields.Add(FieldStateExtractor.ExctractExclusiveFieldState(field));

            foreach (var field in adds)
                patch.PendingFields.Add(FieldStateExtractor.ExtractFieldTreeState(field));

            foreach (var fieldId in deletions)
                patch.RemoveFields.Add(fieldId);

            foreach (var log in _pendingLogs)
                patch.PendingLogs.Add(log.GetState());

            // лог
            StringBuilder sb = new StringBuilder();
            sb.Append("[color=deep_sky_blue]SessionDelta sended:");
            foreach (var change in _fieldChanged)
                sb.Append($"\n{change.ToString()}");
            foreach (var del in _fieldRemoved)
                sb.Append($"\nDeleted: {del}");
            sb.Append("[/color]");
            RPDebug.Log(sb.ToString());

            // отправка на сервер
            if (_dataBridge != null)
                _dataBridge.SendSessionPatch(patch);

            _fieldChanged.Clear();
            _fieldRemoved.Clear();
            _fieldAdded.Clear();
        }

        private void OnFieldRemoved(Field field)
        {
            if (_isChangesIgnoring)
                return;

            void MarkDeleted(Field f)
            {
                _fieldChanged.Remove(f.ID);
                _fieldRemoved.Add(f.ID);

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

            _fieldChanged.Add(field.ID);
            RPDebug.Log($"[color=deep_sky_blue]Field changed catched {field.Name} ({field.ID})[/color]");

            if (_isSendingBlocked == false)
                Flush();
        }

        private void OnFieldAdded(Field field)
        {
            if (_isChangesIgnoring)
                return;
            
            _fieldAdded.Add(field.ID);
            RPDebug.Log($"[color=deep_sky_blue]Field added catched {field.Name} ({field.ID})[/color]");

            if (_isSendingBlocked == false)
                Flush();
        }

        private void OnLogAdded(Logs.Event log)
        {
            _pendingLogs.Add(log);
        }
    }
}
