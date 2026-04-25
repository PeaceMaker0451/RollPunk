using RollPunk.Debug;
using RollPunk.Fields;
using RollPunk.HierarchyFields;
using RollPunk.Modding;

namespace RollPunk.Client.Runtime
{
    internal class EntityInitializer
    {
        private const string InitializeHookName = "EntityInitialized";

        private MutationCatcher _catcher;
        private FieldsRegistry _fieldsRegistry;
        private ModHooker _modHooker;

        public EntityInitializer(FieldsRegistry registry, ModHooker modHooker, MutationCatcher catcher = null)
        {
            _catcher = catcher;
            _fieldsRegistry = registry;
            _modHooker = modHooker;

            _fieldsRegistry.FieldAdded += OnFieldAdded;
        }

        private void OnFieldAdded(Field field)
        {
            if (field is EntityField entityField)
            {
                RPDebug.Log($"[color=teal]>>EntityField added catched - Initializing - {field.Name}[/color]");

                if (_catcher != null)
                    _modHooker.BatchHook(_catcher, InitializeHookName, entityField.GetAPI());
                else
                    _modHooker.CallHook(InitializeHookName, entityField.GetAPI());
            }
        }
    }
}
