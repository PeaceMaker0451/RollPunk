using Godot;
using RollPunk.AccessPolicy;
using RollPunk.Client.Game;
using RollPunk.ClientSide.Runtime.UI;
using RollPunk.HierarchyFields;
using RollPunk.UIFields;
using RollPunk.UI.Forms;
using System;

namespace RollPunk.Client.Forms
{
    internal class SessionViewController : IFormPresenter<GameView>
    {
        private readonly FieldControlsConstructor _constructor;
        private ClientSession _session;
        private GameView _view;

        public SessionViewController(FieldControlsConstructor constructor)
        {
            _constructor = constructor;
        }

        public void Attach(GameView form)
        {
            _view = form;

            if (_session != null)
                BindSession();
        }

        public void SetSession(ClientSession session)
        {
            _session = session;

            if (_view != null)
                BindSession();
        }

        private void BindSession()
        {
            GD.Print($"{_view} | {_session.Entities} | {_constructor} | {_session.Serializator}");

            _view.Initialize(_session, _constructor);

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
