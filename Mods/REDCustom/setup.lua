local BaseActions = {}

---@type EntityFieldData
local _actions_entity_data = 
{
    type = "EntityField",
    name = "Меню",
}

---@type RuleData
local _create_character_rule_data = 
{
    type = "Rule",
    name = "CreateCharacter",
    visible_name = "Создать персонажа",
    hook = "CreateCharacter"
}

---@type RuleData
local _load_character_rule_data = 
{
    type = "Rule",
    name = "LoadCharacter",
    visible_name = "Загрузить персонажа",
    hook = "LoadCharacter"
}

local _create_character_function
local _load_character_function

---@param rule_data RuleData
---@return RuleFieldAPI
local function _createRuleField(rule_data)
    ---@type RuleFieldData
    local rule_field_data = 
    {
        type = "RuleField",
        name = rule_data.name,
        visible_name = rule_data.visible_name,
        view_access_level = 0,
        edit_access_level = 0,
        rule_name = rule_data.hook
    }
    
    ---@type RuleFieldAPI
    local rule_field = ConstructorAPI.createField(rule_field_data)
    return rule_field
end

function BaseActions.setup(create_character_function, load_character_function)
    _create_character_function = create_character_function
    _load_character_function = load_character_function    

---@type EntityFieldAPI    
    local entity = ConstructorAPI.createField(_actions_entity_data)

    local create_character_rule = ConstructorAPI.createRule(_create_character_rule_data)
    entity.addRule(create_character_rule)
    local create_character_rule_field = _createRuleField(_create_character_rule_data)
    entity.addField(create_character_rule_field)

    local load_character_rule = ConstructorAPI.createRule(_load_character_rule_data)
    entity.addRule(load_character_rule)
    local load_character_rule_field = _createRuleField(_load_character_rule_data)
    entity.addField(load_character_rule_field)

    SessionAPI.addEntityField(entity)
end

local function _create_character(actions_field)
    if _create_character_function ~= nil then
        _create_character_function()
    else
        RollPunkAPI.log("Функция создания не установлена!")
    end
end

local function _load_character(ActionsField)
    if _load_character_function ~= nil then
        _load_character_function()
    else
        RollPunkAPI.log("Функция загрузки не установлена!")
    end
end


ModHookerAPI.addHook(_create_character_rule_data.hook, _create_character)
ModHookerAPI.addHook(_load_character_rule_data.hook, _load_character)

return BaseActions