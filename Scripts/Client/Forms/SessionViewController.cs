using Godot;
using RollPunk.AccessPolicy;
using RollPunk.Client.Game;
using RollPunk.ClientSide.Runtime.UI;
using RollPunk.HierarchyFields;
using RollPunk.UIFields;
using System;

namespace RollPunk.Client.Forms
{
    internal class SessionViewController : IFormController<GameView>
    {   
        private readonly FieldControlsConstructor _constructor;
        private ClientSession _session;

        public GameView View { get; private set; }
        public string FormPath => "res://Scenes/FormsScenes/GameView.tscn";

        public IFormHandle FormHandle { get; private set; }

        public SessionViewController(FieldControlsConstructor constructor)
        {
            _constructor = constructor;
        }

        public void Initialize()
        {
            // Инициализация будет вызвана при установке сессии
        }

        public void SetSession(ClientSession session)
        {
            _session = session;

            GD.Print($"{View} | {_session.Entities} | {_constructor} | {session.Serializator}");
            
            View.Initialize(_session, _constructor, session.Serializator);

            View.EntityView.SetViewRule((lineField) =>
            {
                var entity = lineField.GetEntityAncestor();
                if (entity == null)
                    throw new Exception("LineField don't have EntityField Ancestor");
                
                PlayerRole role = _session.OwnersRegistry.GetRelativePlayerRole(entity, _session.CurrentPlayer);
                return role >= lineField.ViewAccessLevel;
            });

            View.EntityView.SetEditRule((lineField) =>
            {
                var entity = lineField.GetEntityAncestor();
                if (entity == null)
                    throw new Exception("LineField don't have EntityField Ancestor");

                PlayerRole role = _session.OwnersRegistry.GetRelativePlayerRole(entity, _session.CurrentPlayer);
                return role >= lineField.EditAccessLevel;
            });
        }

        public void SetView(GameView view)
        {
            View = view;
        }

        public void SetFormHandle(IFormHandle handle)
        {
            FormHandle = handle;
        }
    }
}
