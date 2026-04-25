using Godot;
using RollPunk.Fields;
using System;
using System.Collections.Generic;

namespace RollPunk.UIFields
{
    public class FieldControlsConstructor
    {
        private Dictionary<Type, Func<Field, FieldControl>> _fieldsControls;
        
        private const string s_stringFieldControlScene = "res://Scenes/ControlNodes/LineFields/StringFieldControl.tscn";
        private const string s_multilineStringFieldControlScene = "res://Scenes/ControlNodes/LineFields/MultilineStringFieldControl.tscn";
        private const string s_intFieldControlScene = "res://Scenes/ControlNodes/LineFields/IntFieldControl.tscn";
        private const string s_statFieldControlScene = "res://Scenes/ControlNodes/LineFields/StatFieldControl.tscn";
        private const string s_boolFieldControlScene = "res://Scenes/ControlNodes/LineFields/BoolFieldControl.tscn";
        private const string s_ruleFieldControlScene = "res://Scenes/ControlNodes/LineFields/RuleFieldControl.tscn";
        private const string s_fieldsTableControlScene = "res://Scenes/ControlNodes/FieldsTable.tscn";

        public FieldControlsConstructor()
        {
            _fieldsControls = new Dictionary<Type, Func<Field, FieldControl>>()
            {
                {typeof(StringField), CreateStringFieldControl},
                {typeof(IntField), CreateIntFieldControl},
                {typeof(BoolField), CreateBoolFieldControl},
                {typeof(RuleField), CreateRuleFieldControl},
                {typeof(FieldsGroup), CreateFieldsGroupFieldControl}
            };
        }

        public void AddNewFieldControlConstructor(Type fieldType, Func<Field, FieldControl> fieldControlConstructor)
        {
            _fieldsControls.Add(fieldType, fieldControlConstructor);
        }

        public FieldControl CreateFieldControl(Field field)
        {
            if (_fieldsControls.TryGetValue(field.GetType(), out var constructor) == false)
                throw new Exception($"ControlsConstructor don't know type \'{field.GetType()}\'");

            return constructor.Invoke(field);
        }

        private FieldControl CreateStringFieldControl(Field field)
        {
            if (field is not StringField)
                throw new InvalidOperationException("Wrong field type");
            
            StringFieldControl fieldControl;

            if (field.AdditionalData.TryGetValue("is_multiline", out object isMultilineObject) && isMultilineObject is bool isMultiline && isMultiline)
                fieldControl = (StringFieldControl)CreateFieldControlInstance(s_multilineStringFieldControlScene, field);
            else
                fieldControl = (StringFieldControl)CreateFieldControlInstance(s_stringFieldControlScene, field);

            fieldControl.Initialize((StringField)field);
            
            return fieldControl;
        }

        private FieldControl CreateIntFieldControl(Field field)
        {
            if (field is not IntField)
                throw new InvalidOperationException("Wrong field type");

            IntFieldControl fieldControl;
            
            if (field.AdditionalData.TryGetValue("is_stat", out object isStatObject) && isStatObject is bool isStat && isStat)
                fieldControl = (IntFieldControl)CreateFieldControlInstance(s_statFieldControlScene, field);
            else
                fieldControl = (IntFieldControl)CreateFieldControlInstance(s_intFieldControlScene, field);

            fieldControl.Initialize((IntField)field);
            
            return fieldControl;
        }

        private FieldControl CreateBoolFieldControl(Field field)
        {
            if (field is not BoolField)
                throw new InvalidOperationException("Wrong field type");

            var fieldControl = (BoolFieldControl)CreateFieldControlInstance(s_boolFieldControlScene, field);
            fieldControl.Initialize((BoolField)field);

            return fieldControl;
        }

        private FieldControl CreateRuleFieldControl(Field field)
        {
            if (field is not RuleField)
                throw new InvalidOperationException("Wrong field type");

            var fieldControl = (RuleFieldControl)CreateFieldControlInstance(s_ruleFieldControlScene, field);
            fieldControl.Initialize((RuleField)field);

            return fieldControl;
        }

        private FieldControl CreateFieldsGroupFieldControl(Field field)
        {
            if (field is not FieldsGroup)
                throw new InvalidOperationException("Wrong field type");

            FieldsTableControl fieldControl = (FieldsTableControl)CreateFieldControlInstance(s_fieldsTableControlScene, field);
            fieldControl.Initialize((FieldsGroup)field, this);

            return fieldControl;
        }

        private FieldControl CreateFieldControlInstance(string scenePath, Field field)
        {
            FieldControl fieldControl = null;
            fieldControl = GD.Load<PackedScene>(scenePath).Instantiate() as FieldControl;

            return fieldControl;
        }
    }
}