using RollPunk.AccessPolicy;
using RollPunk.Fields;
using RollPunk.Modding;
using System;

namespace RollPunk.UIFields
{
    public class LineFieldAPI : ValueFieldAPI
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
            try
            {
                _lineFieldHandler.SetViewAccessLevel((PlayerRole)newAccessLevel);
            }
            catch(Exception ex)
            {
                LuaErrorsHandler.Handle(ex);
            }
        }

        public void setEditAccessLevel(int newAccessLevel) 
        { 
            try
            {
                _lineFieldHandler.SetEditAccessLevel((PlayerRole)newAccessLevel);
            }
            catch(Exception ex)
            {
                LuaErrorsHandler.Handle(ex);
            }
        }
    }
}
