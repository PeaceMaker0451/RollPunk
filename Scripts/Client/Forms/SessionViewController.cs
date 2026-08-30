using Godot;
using RollPunk.AccessPolicy;
using RollPunk.Client.Game;
using RollPunk.ClientSide.Runtime.UI;
using RollPunk.Fields;
using RollPunk.HierarchyFields;
using RollPunk.MembersExposing;
using RollPunk.UI;
using RollPunk.UIFields;
using System;
using System.Reflection;

namespace RollPunk.Client.Forms
{
    internal class SessionViewController : IFormPresenter<SessionView>
    {
        private readonly FieldControlsConstructor _constructor;
        private ClientSession _session;
        private SessionView _view;

        public SessionViewController()
        {
            _constructor = new();
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
            GD.Print($"{_view}");
            GD.Print($"{_session.Entities}");
            GD.Print($"{_constructor}");
            GD.Print($"{_session.Serializator}");

            _view.SetEntityViewVisibiblityRule((lineField) =>
            {
                var entity = lineField.GetEntityAncestor();
                if (entity == null)
                    throw new Exception("LineField don't have EntityField Ancestor");

                PlayerRole role = _session.OwnersRegistry.GetRelativePlayerRole(entity, _session.CurrentPlayer, true);
                return role >= lineField.ViewAccessLevel;
            });

            _view.SetEntityViewEditabilityRule((lineField) =>
            {
                var entity = lineField.GetEntityAncestor();
                if (entity == null)
                    throw new Exception("LineField don't have EntityField Ancestor");

                PlayerRole role = _session.OwnersRegistry.GetRelativePlayerRole(entity, _session.CurrentPlayer, true);
                return role >= lineField.EditAccessLevel;
            });

            _view.SetActionLabelText(_session.PlayerSpace.ActionsTabName);
            _view.RenderActions(_session.PlayerSpace.Actions);
            _view.InitializeLogs(_session);
            _view.InitializePlayerList(_session);
            _view.InitializeEntityView(_constructor);

            var renderer = _view.FindChild("ExposedRenderer") as ExposedObjectRenderer;

            _session.PlayerSpace.Actions.Changed += () => _view.RenderActions(_session.PlayerSpace.Actions);
            _session.PlayerSpace.DisplayedEntityChanged += () => _view.ShowEntity(_session.PlayerSpace.DisplayedEntity);

            _view.FieldListFieldSelected += (field) =>
            {
                if (field is EntityField entityField)
                    _view.ShowEntity(entityField);
            };
        }
    }
}
