using RollPunk.AccessPolicy;
using RollPunk.Client.Runtime;
using RollPunk.ClientSide.Runtime.UI;
using RollPunk.HierarchyFields;
using RollPunk.UIFields;
using System;

namespace RollPunk.Client.Forms
{
    internal class SessionViewController : IFormController
    {
        private readonly GameView _view;
        private readonly FieldControlsConstructor _constructor;
        private ClientSession _session;

        public SessionViewController(GameView view, FieldControlsConstructor constructor)
        {
            _view = view;
            _constructor = constructor;
        }

        public void Initialize()
        {
            // Инициализация будет вызвана при установке сессии
        }

        public void SetSession(ClientSession session)
        {
            _session = session;
            
            _view.Initialize(_session.Entities, _constructor, session.Serializator);

            _view.EntityView.SetViewRule((lineField) =>
            {
                var entity = lineField.GetEntityAncestor();
                if (entity == null)
                    throw new Exception("LineField don't have EntityField Ancestor");
                
                PlayerRole role = _session.OwnersRegistry.GetRelativePlayerRole(entity, _session.CurrentPlayer);
                return role >= lineField.ViewAccessLevel;
            });

            _view.EntityView.SetEditRule((lineField) =>
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
