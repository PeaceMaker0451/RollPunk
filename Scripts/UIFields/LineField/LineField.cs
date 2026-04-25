using Newtonsoft.Json.Linq;
using RollPunk.AccessPolicy;
using RollPunk.Entities;
using RollPunk.Fields;
using System;
using System.Collections.Generic;

namespace RollPunk.UIFields
{
    public abstract partial class LineField : ValueField
    {
        public event Action LinePriorityChanged;
        public event Action VisibleNameChanged;
        public event Action ViewAccessLevelChanged;
        public event Action EditAccessLevelChanged;

        public string VisibleName { get; private set;  }
        public int LinePriority { get; private set; }
        public PlayerRole ViewAccessLevel { get; private set; }
        public PlayerRole EditAccessLevel { get; private set; }

        public LineField(string name, string visibleName, PlayerRole viewAccessLevel, PlayerRole editAcessLevel, Type apiType, int linePriority = 0, Dictionary<string, object> additionalData = null)
            : base(name, apiType, additionalData)
        {
            VisibleName = visibleName;
            LinePriority = linePriority;
            ViewAccessLevel = viewAccessLevel;
            EditAccessLevel = editAcessLevel;
        }

        public LineField(EntityState data, Type apiType) : base(data, apiType) { }

        public void SetVisibleName(string name)
        {
            VisibleName = name;
            VisibleNameChanged?.Invoke();
        }
        
        public void SetLinePriority(int _newPriority)
        {
            LinePriority = _newPriority;
            LinePriorityChanged?.Invoke();
            RaiseChanged();
        }

        public void SetViewAccessLevel(PlayerRole minLevel)
        {
            ViewAccessLevel = minLevel;
            ViewAccessLevelChanged?.Invoke();
        }

        public void SetEditAccessLevel(PlayerRole minLevel)
        {
            EditAccessLevel = minLevel;
            EditAccessLevelChanged?.Invoke();
        }

        protected override void ApplyPayload(Dictionary<string, JToken> payload)
        {
            base.ApplyPayload(payload);

            VisibleName = Get<string>(payload, nameof(VisibleName));
            LinePriority = Get<int>(payload, nameof(LinePriority));

            ViewAccessLevel = (PlayerRole)Get<int>(payload, nameof(ViewAccessLevel)); 
            EditAccessLevel = (PlayerRole)Get<int>(payload, nameof(EditAccessLevel)); 
        }

        protected override void WritePayload(Dictionary<string, JToken> payload)
        {
            base.WritePayload(payload);

            payload.Add(nameof(VisibleName), VisibleName);
            payload.Add(nameof(LinePriority), LinePriority);

            payload.Add(nameof(ViewAccessLevel), (int)ViewAccessLevel);
            payload.Add(nameof(EditAccessLevel), (int)EditAccessLevel);
        }
    }
}