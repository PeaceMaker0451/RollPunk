using MoonSharp.Interpreter;
using RollPunk.Debug;
using RollPunk.Fields;
using RollPunk.HierarchyFields;
using RollPunk.Modding;
using System.Collections.Generic;

namespace RollPunk.Client.Runtime
{
    internal class EntityValidator
    {
        private const string ValidateHookName = "Validate";

        private MutationCatcher _catcher;
        private FieldsRegistry _fieldsRegistry;
        private ModHooker _modHooker;
        private readonly HashSet<EntityField> _validating = new();

        private int _validatingEmbeding = 0;
        private int _validatingEmbedingLimit = 2;

        public EntityValidator(FieldsRegistry registry, ModHooker modHooker, MutationCatcher catcher = null)
        {
            _catcher = catcher;
            _fieldsRegistry = registry;
            _modHooker = modHooker;

            _fieldsRegistry.Changed += OnChanged;
            _fieldsRegistry.FieldAdded += OnFieldAdded;
        }

        private void OnFieldAdded(Field field)
        {
            RPDebug.Log($"[color=forest_green]>>Field added catched - {field.Name} ({field.ID})[/color]");
            if (field is EntityField)
            {
                RPDebug.Log($"[color=forest_green]>>Added field is EntityField - SKIPPING[/color]");
                return;
            } 

            Validate(field);
        }

        private void OnChanged(Field field)
        {
            RPDebug.Log($"[color=forest_green]>>Field changed catched - {field.Name} ({field.ID})[/color]");
            Validate(field);
        }

        private void Validate(Field field)
        {
            EntityField entity = field.GetEntityAncestor();

            if (_validating.Contains(entity))
            {
                RPDebug.Log($"[color=forest_green]>< >< Entity {entity.Name} already validating - SKIPPING[/color]");
                return;
            }
                

            if (_validatingEmbeding >= _validatingEmbedingLimit)
                throw new ScriptRuntimeException("Превышен лимит вложенности валидации");

            _validating.Add(entity);
            RPDebug.Log($"[color=forest_green]Validating {entity.Name} because of {field.Name}[/color]");
            _validatingEmbeding++;
            CallValidationHook(entity, field);
            RPDebug.Log($"[color=forest_green]Validation of {entity.Name} ended[/color]");
            _validating.Remove(entity);
            _validatingEmbeding--;
        }

        private void CallValidationHook(EntityField entity, Field changedField)
        {
            if (_catcher != null)
                _modHooker.BatchHook(_catcher, ValidateHookName, entity.GetAPI(), changedField.GetAPI());
            else
                _modHooker.CallHook(ValidateHookName, entity.GetAPI(), changedField.GetAPI());
        }
    }
}
