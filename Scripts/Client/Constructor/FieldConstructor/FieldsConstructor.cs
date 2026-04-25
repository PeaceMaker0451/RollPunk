using RollPunk.AccessPolicy;
using RollPunk.Debug;
using RollPunk.Fields;
using RollPunk.HierarchyFields;
using RollPunk.Modding.APIs;
using RollPunk.UIFields;
using System;
using System.Collections.Generic;

namespace RollPunk.Client.Fields
{
    internal class FieldsConstructor : IAPIHandler
    {
        private const string NameFieldName = "name";
        private const string VisibleName = "visible_name";
        private const string TypeFieldName = "type";
        private const string AdditionalDataFieldName = "additional_data";
        private const string LinePriorityDataFieldName = "line_priority";
        private const string EditableLevelDataFieldName = "edit_access_level";
        private const string VisibleLevelDataFieldName = "view_access_level";
        private const string ValueDataFieldName = "value";
        private const string MaxValueDataFieldName = "max_value";
        private const string MinValueDataFieldName = "min_value";
        private const string RuleName = "rule_name";

        private readonly Dictionary<string, Func<Dictionary<string, object>, Field>> _fieldCreators;

        private FieldsConstructorAPI _fieldConstructorAPI;

        public FieldsConstructor()
        {
            _fieldConstructorAPI = new FieldsConstructorAPI(this);

            _fieldCreators = new Dictionary<string, Func<Dictionary<string, object>, Field>>
        {
            { nameof(BoolField), CreateBoolField },
            { nameof(RuleField), CreateRuleField },
            { nameof(StringField), CreateStringField },
            { nameof(IntField), CreateIntField },
            { nameof(FieldsGroup), CreateFieldsGroup },
            { nameof(EntityField), CreateEntityField }
        };
        }

        public Field CreateField(Dictionary<string, object> fieldData)
        {
            string type = GetStringValue(fieldData, TypeFieldName);

            if (_fieldCreators.TryGetValue(type, out var createField))
            {
                //RPDebug.Log($"Создание поля типа {type}..");
                return createField(fieldData);
            }

            throw new ArgumentException($"Unknown field type: {type}");
        }

        public API GetAPI()
        {
            return _fieldConstructorAPI;
        }

        private Field CreateBoolField(Dictionary<string, object> fieldData)
        {
            string name = GetStringValue(fieldData, NameFieldName);
            string visibleName = GetStringValue(fieldData, VisibleName, name);
            bool defaultValue = GetBoolValue(fieldData, ValueDataFieldName);
            Dictionary<string, object> additionalData = GetDictionaryValue(fieldData, AdditionalDataFieldName);
            int linePriority = GetIntValue(fieldData, LinePriorityDataFieldName);
            int editLevel = GetIntValue(fieldData, EditableLevelDataFieldName, 0);
            int viewLevel = GetIntValue(fieldData, VisibleLevelDataFieldName, 0);

            return new BoolField(name, visibleName, (PlayerRole)viewLevel, (PlayerRole)editLevel, defaultValue, linePriority, additionalData);
        }

        private Field CreateRuleField(Dictionary<string, object> fieldData)
        {
            string name = GetStringValue(fieldData, NameFieldName);
            string visibleName = GetStringValue(fieldData, VisibleName, name);
            string ruleName = GetStringValue(fieldData, RuleName);
            Dictionary<string, object> additionalData = GetDictionaryValue(fieldData, AdditionalDataFieldName);
            int linePriority = GetIntValue(fieldData, LinePriorityDataFieldName);
            int editLevel = GetIntValue(fieldData, EditableLevelDataFieldName, 0);
            int viewLevel = GetIntValue(fieldData, VisibleLevelDataFieldName, 0);

            return new RuleField(name, visibleName, (PlayerRole)viewLevel, (PlayerRole)editLevel, ruleName, linePriority, additionalData);
        }

        private Field CreateStringField(Dictionary<string, object> fieldData)
        {
            string name = GetStringValue(fieldData, NameFieldName);
            string visibleName = GetStringValue(fieldData, VisibleName, name);
            string defaultValue = GetStringValue(fieldData, ValueDataFieldName, "");
            Dictionary<string, object> additionalData = GetDictionaryValue(fieldData, AdditionalDataFieldName);
            int linePriority = GetIntValue(fieldData, LinePriorityDataFieldName);
            int editLevel = GetIntValue(fieldData, EditableLevelDataFieldName, 0);
            int viewLevel = GetIntValue(fieldData, VisibleLevelDataFieldName, 0);

            return new StringField(name, visibleName, (PlayerRole)viewLevel, (PlayerRole)editLevel, defaultValue, linePriority, additionalData);
        }

        private Field CreateIntField(Dictionary<string, object> fieldData)
        {
            string name = GetStringValue(fieldData, NameFieldName);
            string visibleName = GetStringValue(fieldData, VisibleName, name);
            int defaultValue = GetIntValue(fieldData, ValueDataFieldName);
            int maxValue = GetIntValue(fieldData, MaxValueDataFieldName, 100);
            int minValue = GetIntValue(fieldData, MinValueDataFieldName, 0);
            Dictionary<string, object> additionalData = GetDictionaryValue(fieldData, AdditionalDataFieldName);
            int linePriority = GetIntValue(fieldData, LinePriorityDataFieldName);
            int editLevel = GetIntValue(fieldData, EditableLevelDataFieldName, 0);
            int viewLevel = GetIntValue(fieldData, VisibleLevelDataFieldName, 0);

            return new IntField(name, visibleName, (PlayerRole)viewLevel, (PlayerRole)editLevel, defaultValue, minValue, maxValue, linePriority, additionalData);
        }

        private Field CreateFieldsGroup(Dictionary<string, object> fieldData)
        {
            string name = GetStringValue(fieldData, NameFieldName);
            string visibleName = GetStringValue(fieldData, VisibleName, name);
            Dictionary<string, object> additionalData = GetDictionaryValue(fieldData, AdditionalDataFieldName);
            int linePriority = GetIntValue(fieldData, LinePriorityDataFieldName);
            int editLevel = GetIntValue(fieldData, EditableLevelDataFieldName, 0);
            int viewLevel = GetIntValue(fieldData, VisibleLevelDataFieldName, 0);

            return new FieldsGroup(name, visibleName, (PlayerRole)viewLevel, (PlayerRole)editLevel, linePriority, additionalData);
        }

        private Field CreateEntityField(Dictionary<string, object> fieldData)
        {
            string name = GetStringValue(fieldData, NameFieldName);
            Dictionary<string, object> additionalData = GetDictionaryValue(fieldData, AdditionalDataFieldName);

            return new EntityField(name, additionalData);
        }

        private string GetStringValue(Dictionary<string, object> fieldData, string key, string defaultValue = "")
        {
            return fieldData.TryGetValue(key, out object value) ? value as string ?? defaultValue : defaultValue;
        }

        private bool GetBoolValue(Dictionary<string, object> fieldData, string key, bool defaultValue = true)
        {
            return fieldData.TryGetValue(key, out object value) ? value is bool b ? b : defaultValue : defaultValue;
        }

        private int GetIntValue(Dictionary<string, object> fieldData, string key, int defaultValue = 0)
        {
            return fieldData.TryGetValue(key, out object value) ? value is double d ? Convert.ToInt32(d) : defaultValue : defaultValue;
        }

        private Dictionary<string, object> GetDictionaryValue(Dictionary<string, object> fieldData, string key)
        {
            return fieldData.TryGetValue(key, out object value) ? value as Dictionary<string, object> ?? new Dictionary<string, object>() : new Dictionary<string, object>();
        }
    }
}
