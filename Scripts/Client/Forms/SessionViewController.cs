using RollPunk.AccessPolicy;
using RollPunk.Client.Runtime;
using RollPunk.ClientSide.Runtime.UI;
using RollPunk.Debug;
using RollPunk.HierarchyFields;
using RollPunk.UI.Forms;
using RollPunk.UIFields;
using System;

namespace RollPunk.Client.Forms
{
    internal class SessionViewController
    {
        private const string GameViewScenePath = "res://Scenes/FormsScenes/GameView.tscn";
        
        private UIController _ui;
        private FieldControlsConstructor _constructor;
        private Session _session;

        public GameView GameView { get; private set; }
        
        public SessionViewController(UIController uIController, FieldControlsConstructor fieldControlsConstructor)
        {
            _ui = uIController;
            _constructor = fieldControlsConstructor;
        }

        public void OpenSessionView(Session session)
        {
            _session = session;
            
            if (_ui.LoadFormAsMainFrameTab(GameViewScenePath, 1, out Form form) == false || form is not GameView gameView)
                throw new InvalidOperationException("Unnable to load the GameView form");

            GameView = gameView;
            GameView.Initialize(_session.Entities, _constructor, session.Serializator);

            GameView.EntityView.SetViewRule((lineField) =>
            {
                var entity = lineField.GetEntityAncestor();
                if (entity == null)
                    throw new Exception("LineField don't have EntityField Ancestor");
                
                PlayerRole role = _session.OwnersRegistry.GetRelativePlayerRole(entity, _session.CurrentPlayer);

                //RPDebug.DebugLog($"[color=dim_gray][SessionView] Linefield View Check - {lineField.Name} ViewLevel: [color=crimson]{lineField.ViewAccessLevel}[/color]" +
                //    $"\nEntity relative role: [color=crimson]{role}[/color] - Should be viewable [b][color=teal]{role >= lineField.ViewAccessLevel}[/color][/b][/color]");

                return role >= lineField.ViewAccessLevel;
            });

            GameView.EntityView.SetEditRule((lineField) =>
            {
                var entity = lineField.GetEntityAncestor();
                if (entity == null)
                    throw new Exception("LineField don't have EntityField Ancestor");

                PlayerRole role = _session.OwnersRegistry.GetRelativePlayerRole(entity, _session.CurrentPlayer);

                //RPDebug.DebugLog($"[color=gray][SessionView] Linefield Edit Check - {lineField.Name} EditLevel: [color=crimson]{lineField.EditAccessLevel}[/color]" +
                //$"\nEntity relative role: [color=crimson]{role}[/color] - Should be editable [b][color=teal]{role >= lineField.EditAccessLevel}[/color][/b][/color]");

                return role >= lineField.EditAccessLevel;
            });
        }

        public void CloseSessionView()
        {
            _ui.CloseForm(GameView);
        }
    }
}
