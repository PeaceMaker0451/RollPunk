using Newtonsoft.Json.Linq;
using RollPunk.AccessPolicy;
using RollPunk.Fields;
using RollPunk.Modding;
using System;

namespace RollPunk.UIFields
{
    public class LineFieldAPI : FieldAPI
    {
        readonly private LineField _lineFieldHandler;
        public int line_priority => _lineFieldHandler.LinePriority;
        public int view_access_level => (int)_lineFieldHandler.ViewAccessLevel;
        public int edit_access_level => (int)_lineFieldHandler.EditAccessLevel;

        public LineFieldAPI(LineField handler) : base(handler)
        {
            _lineFieldHandler = handler;
        }

        public void setLinePriority(int newPriority) { _lineFieldHandler.SetLinePriority(newPriority); }

        public void setViewAccessLevel(int newAccessLevel) 
        {
            if (!Enum.IsDefined(typeof(PlayerRole), newAccessLevel))
                throw new ArgumentException(
                    $"Invalid access level: {newAccessLevel}");

            _lineFieldHandler.SetViewAccessLevel((PlayerRole)newAccessLevel);
        }

        public void setEditAccessLevel(int newAccessLevel) 
        {
            if (!Enum.IsDefined(typeof(PlayerRole), newAccessLevel))
                throw new ArgumentException(
                    $"Invalid access level: {newAccessLevel}");

            _lineFieldHandler.SetEditAccessLevel((PlayerRole)newAccessLevel);
        }

        public void setVisibleName(string name)
        {
            _lineFieldHandler.SetVisibleName(name);
        }

        public object getValue() { return _lineFieldHandler.GetRawValue(); }
    }
}
