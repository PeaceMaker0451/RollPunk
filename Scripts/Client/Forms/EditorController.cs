using RollPunk.AccessPolicy;
using RollPunk.Client.Game;
using RollPunk.HierarchyFields;
using RollPunk.MembersExposing;
using System;

namespace RollPunk.Client.Forms
{
    public class EditorController : IFormPresenter<Editor>
    {
        private ClientSession _session;
        private Editor _view;

        public void Attach(Editor form)
        {
            _view = form;
            _view.InitializeEntityView(new());

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

            _view.SetFieldsContainer(_session.Entities);

            _view.FieldsTreeFieldSelected += (field) =>
            {
                if (field is EntityField entity)
                    _view.ShowEntity(entity);
                else
                    _view.ShowRawData(ExposedObjectBuilder.Build(field));
            };
        }
    }
}
