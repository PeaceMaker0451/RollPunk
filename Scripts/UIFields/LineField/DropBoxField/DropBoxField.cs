using Newtonsoft.Json.Linq;
using RollPunk.AccessPolicy;
using RollPunk.Entities;
using System.Collections.Generic;

namespace RollPunk.UIFields
{
    [EntityType("DropBox")]
    public class DropBoxField : LineField
    {
        private List<string> _lines;
        private int _selectedLine;

        public IReadOnlyList<string> Lines => _lines;
        public int SelectedLine => _selectedLine;
        
        public DropBoxField(string name, string visibleName, PlayerRole viewAccessLevel, PlayerRole editAcessLevel, List<string> lines, int selected, int linePriority, Dictionary<string, object> additionalData = null) 
            : base(name, visibleName, viewAccessLevel, editAcessLevel, typeof(DropBoxFieldAPI), linePriority, additionalData)
        {
            _lines = lines;
            _selectedLine = selected;
        }

        public void SetValue(int line)
        {
            _selectedLine = line;
            RaiseChanged();
        }

        public int GetSelectedValue()
        {
            return _selectedLine;
        }

        public string GetSelectedString()
        {
            return _lines[_selectedLine]; 
        }

        public override object GetRawValue()
        {
            return _selectedLine;
        }

        protected override void WritePayload(Dictionary<string, JToken> payload)
        {
            base.WritePayload(payload);

            payload[nameof(SelectedLine)] = _selectedLine;
            Set(payload, nameof(Lines), _lines);
        }

        protected override void ApplyPayload(Dictionary<string, JToken> payload)
        {
            base.ApplyPayload(payload);

            _lines = Get<List<string>>(payload, nameof(Lines));
            _selectedLine = Get<int>(payload, nameof(SelectedLine));  
        }
    }
}
