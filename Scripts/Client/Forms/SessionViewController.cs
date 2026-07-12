using RollPunk.AccessPolicy;
using RollPunk.Client.Runtime;
using RollPunk.ClientSide.Runtime.UI;
using RollPunk.HierarchyFields;
using RollPunk.UIFields;
using System;

namespace RollPunk.Client.Forms
{
    internal class SessionViewController : IFormController<GameView>
    {
        public GameView View { get; set; }
        public string FormPath => "res://Scenes/FormsScenes/GameView.tscn";
        
        private readonly FieldControlsConstructor _constructor;
        private ClientSession _session;

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
            
            View.Initialize(_session.Entities, _constructor, session.Serializator);

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
    }
}
