local FieldsServices = require("fields_services")
local CharacterSubsystem = require("character.subsystems.character_subsystem")

---@class HealthSubsystem : CharacterSubsystem
local HealthSubsystem = setmetatable({}, CharacterSubsystem)
HealthSubsystem.__index = HealthSubsystem

---@type IntFieldData
local _hp_field_data =
{
    name = "HP",
    visible_name = "Очки здоровья",
    type = "IntField",
    value = 200,
    max_value = 200,
    min_value = 0,
    view_access_level = 0,
    edit_access_level = 2,
    line_priority = 70,
    additional_data = { show_max = true, vertical = true }
}


---@type IntFieldData
local _armor_field_data =
{
    name = "Armor",
    type = "IntField",
    visible_name = "Броня",
    value = 10,
    max_value = 1000,
    min_value = 0,
    view_access_level = 0,
    edit_access_level = 2,
    line_priority = 51,
}

---@type RuleData
local _damage_rule_data =
{
    name = "Damage",
    type = "Rule",
    hook = "Damage"
}

---@type RuleFieldData
local _damage_rule_field_data =
{
    name = "DamageRuleField",
    rule_name = _damage_rule_data.name,
    visible_name = "Нанести урон",
    type = "RuleField",
    line_priority = 69,
}

---@type RuleData
local _heal_rule_data =
{
    name = "Heal",
    type = "Rule",
    hook = "Heal"
}

---@type RuleFieldData
local _heal_rule_field_data =
{
    name = "HealRuleField",
    rule_name = _heal_rule_data.name,
    visible_name = "Лечить",
    type = "RuleField",
    line_priority = 68,
}

local _character_group_name 
local _parameters_group_name
local _action_group_name
local _body_stat_name
local _will_stat_name

HealthSubsystem.heal_hook_name = _heal_rule_data.hook
HealthSubsystem.damage_hook_name = _damage_rule_data.hook

local function _updateMaxHealth(character)
    local bodyValue = character.getField(_body_stat_name).getValue()
    local willValue = character.getField(_will_stat_name).getValue()
    local newMaxHP = (((bodyValue + willValue) / 4) * 5) + 10

    character.getField(_hp_field_data.name).setMaxValue(newMaxHP)
end

local function _upgradeTo051(character)
    RollPunkAPI.log("Обновление подсистемы здоровья до версии 0.5.1...")
    
    local healButton = character.getField(_heal_rule_field_data.name)
    local damageButton = character.getField(_damage_rule_field_data.name)

    healButton.setRuleName(_heal_rule_data.name)
    damageButton.setRuleName(_damage_rule_data.name)

    local healRule = character.getRule(_heal_rule_data.name)
    local damageRule = character.getRule(_damage_rule_data.name)

    healRule.setHook(_heal_rule_data.hook)
    damageRule.setHook(_damage_rule_data.hook)
end

function HealthSubsystem.initialize(character_group_name, parameters_group_name, action_group_name, body_stat_name, will_stat_name)
    HealthSubsystem.name = "HealthSubsystem"
    
    _character_group_name = character_group_name
    _parameters_group_name = parameters_group_name
    _action_group_name = action_group_name
    _body_stat_name = body_stat_name
    _will_stat_name = will_stat_name
end

function HealthSubsystem.create(character)
    RollPunkAPI.log("Создание HealthSubsystem")
    
    character.addRule(ConstructorAPI.createRule(_heal_rule_data))
    character.addRule(ConstructorAPI.createRule(_damage_rule_data))

    local character_group = character.getField(_character_group_name)
    local parameters_group = character.getField(_parameters_group_name)
    local action_group = character.getField(_action_group_name)
    
    FieldsServices.createAndChild(character_group, _hp_field_data)
    FieldsServices.createAndChild(parameters_group, _armor_field_data)
    FieldsServices.createAndChild(action_group, _heal_rule_field_data)
    FieldsServices.createAndChild(action_group, _damage_rule_field_data)
end

function HealthSubsystem.connect(character)
    RollPunkAPI.log("Присоединение HealthSubsystem")

    local version = character.getAdditionalDataField("version")

    if version == nil then
        _upgradeTo051(character)
    end
end

function HealthSubsystem.heal(character, value)
    local hp_field = character.getField(_hp_field_data.name)
    hp_field.setValue(hp_field.getValue() + value)
end

function HealthSubsystem.damage(character, value)
    local hp_field = character.getField(_hp_field_data.name)
    local armor_value = character.getField(_armor_field_data.name).getValue()

    if armor_value > value/2 then
        armor_value = value/2
    end

    local damage = value - armor_value

    hp_field.setValue(hp_field.getValue() - damage)
end

local function _onValidate(character, updatedField)
    if updatedField.name == _body_stat_name or updatedField.name == _will_stat_name then
        _updateMaxHealth(character)
    end
end

local function _onDamage(character)
    UIAPI.openIntDialogue("Введите число урона:", function(result)
            HealthSubsystem.damage(character,  result)
        end)
end

local function _onHeal(character)
    UIAPI.openIntDialogue("Введите число здоровья:", function(result)
            HealthSubsystem.heal(character,  result)
        end)
end

ModHookerAPI.addHook("Validate", _onValidate)
ModHookerAPI.addHook(_damage_rule_data.hook, _onDamage)
ModHookerAPI.addHook(_heal_rule_data.hook, _onHeal)

return HealthSubsystem