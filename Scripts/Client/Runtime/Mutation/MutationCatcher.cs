using RollPunk.Debug;
using RollPunk.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RollPunk.Client.Runtime
{
    internal class MutationCatcher
    {
        FieldsRegistry _registry;

        HashSet<Guid> _pending = new();
        HashSet<Guid> _removed = new();
        bool _sendingBlocked = false;

        public MutationCatcher(FieldsRegistry registry)
        {
            _registry = registry;
            _registry.Changed += OnFieldChanged;
            _registry.FieldAdded += OnFieldChanged;
            _registry.FieldRemoved += OnFieldRemoved;
        }

        private void OnFieldRemoved(Field field)
        {
            void MarkDeleted(Field f)
            {
                _pending.Remove(f.ID);
                _removed.Add(f.ID);

                foreach (var innerField in field.Fields)
                    MarkDeleted(innerField);
            }

            MarkDeleted(field);

            if (_sendingBlocked == false)
                Flush();
        }

        private void OnFieldChanged(Field field)
        {
            _pending.Add(field.ID);

            if (_sendingBlocked == false)
                Flush();
        }

        public void BlockSending() => _sendingBlocked = true;
        public void UnblockSending() => _sendingBlocked = false;

        public void Flush()
        {
            if (_pending.Count == 0 && _removed.Count == 0) return;

            var changes = _pending.Select(id => _registry.FieldsDictionary[id].GetState()).ToList();
            var deletions = _removed.ToList();

            _pending.Clear();
            _removed.Clear();

            // лог
            StringBuilder sb = new StringBuilder();
            sb.Append("[color=deep_sky_blue]SessionDelta sended:");
            foreach (var change in changes)
                sb.Append($"\n{change.ToString()}");
            foreach (var del in deletions)
                sb.Append($"\nDeleted: {del}");
            sb.Append("[/color]");
            RPDebug.Log(sb.ToString());

            // отправка на сервер
            //_net.SendDelta(changes, deletions);
        }
    }
}
