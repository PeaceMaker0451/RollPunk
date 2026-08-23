using Godot;
using RollPunk.AccessPolicy;
using RollPunk.Client.Game;
using RollPunk.ClientSide.Runtime.UI;
using RollPunk.HierarchyFields;
using RollPunk.UIFields;
using System;

namespace RollPunk.Client.Forms
{
    internal class SessionViewController : IFormPresenter<SessionView>
    {
        private readonly FieldControlsConstructor _constructor;
        private ClientSession _session;
        private SessionView _view;

        public SessionViewController(FieldControlsConstructor constructor)
        {
            _constructor = constructor;
        }

        public void Attach(SessionView form)
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

        public void Close()
        {
            _view?.Close();
        }

        private void BindSession()
        {
            GD.Print($"{_view} | {_session.Entities} | {_constructor} | {_session.Serializator}");

            _view.SetEntityViewVisibiblityRule((lineField) =>
            {
                var entity = lineField.GetEntityAncestor();
                if (entity == null)
                    throw new Exception("LineField don't have EntityField Ancestor");

                PlayerRole role = _session.OwnersRegistry.GetRelativePlayerRole(entity, _session.CurrentPlayer);
                return role >= lineField.ViewAccessLevel;
            });

            _view.SetEntityViewEditabilityRule((lineField) =>
            {
                var entity = lineField.GetEntityAncestor();
                if (entity == null)
                    throw new Exception("LineField don't have EntityField Ancestor");

                PlayerRole role = _session.OwnersRegistry.GetRelativePlayerRole(entity, _session.CurrentPlayer);
                return role >= lineField.EditAccessLevel;
            });

            _view.SetFieldListContainer(_session.Entities);
            _view.InitializeLogs(_session);
            _view.InitializePlayerList(_session);
            _view.InitializeEntityView(_constructor);

            _view.FieldListFieldSelected += (field) =>
            {
                if (field is EntityField entityField)
                    _view.ShowEntity(entityField);
            };
        }
    }
}
