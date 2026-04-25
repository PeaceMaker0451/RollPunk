local FieldsServices = require("fields_services")
local CharacterSubsystem = require("character.subsystems.character_subsystem")

---@class HealthSubsystem : CharacterSubsystem
---@field character_group FieldGroupAPI
---@field action_group FieldGroupAPI
---@field parameters_group FieldGroupAPI
---@field will_field LineFieldAPI
---@field body_field LineFieldAPI
---@
---@field hp_field LineFieldAPI
---@field armor_field LineFieldAPI
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

local function addRuleHooks(self)
    ModHookerAPI.addHook(_damage_rule_data.hook,function (character)
        if self == nil or character ~= self.character then
            return
        end

        UIAPI.openIntDialogue("Введите число урона:", function(result)
            HealthSubsystem.damage(self,  result)
        end)
    end)

    ModHookerAPI.addHook(_heal_rule_data.hook,function (character)
        if self == nil or character ~= self.character then
            return
        end

        UIAPI.openIntDialogue("Введите число здоровья:", function(result)
            HealthSubsystem.heal(self,  result)
        end)
    end)
end

local function _updateMaxHealth(self)
    local bodyValue = self.body_field.getValue()
    local willValue = self.will_field.getValue()
    local newMaxHP = (((bodyValue + willValue) / 4) * 5) + 10

    self.hp_field.setMaxValue(newMaxHP)
end

function HealthSubsystem:_create()
    RollPunkAPI.log("Создание HealthSubsystem")
    
    self.character.addRule(ConstructorAPI.createRule(_heal_rule_data))
    self.character.addRule(ConstructorAPI.createRule(_damage_rule_data))
    
    self.hp_field = FieldsServices.createAndChild(self.character_group, _hp_field_data)
    self.armor_field = FieldsServices.createAndChild(self.parameters_group, _armor_field_data)
    FieldsServices.createAndChild(self.action_group, _heal_rule_field_data)
    FieldsServices.createAndChild(self.action_group, _damage_rule_field_data)

    addRuleHooks(self)
end

function HealthSubsystem:_connect()
    RollPunkAPI.log("Присоединение HealthSubsystem")

    self.hp_field = self.character.getField(_hp_field_data.name)
    
    self.armor_field = self.character.getField( _armor_field_data.name)
    
    addRuleHooks(self)
end

function HealthSubsystem:new(character, action_group, character_group, parameters_group, body_stat_field, will_stat_field)
    ---@type HealthSubsystem
    local instance = CharacterSubsystem.new(self, "HealthSubsystem", character)    
    
    instance.character_group = character_group
    instance.action_group = action_group
    instance.parameters_group = parameters_group
    instance.body_field = body_stat_field
    instance.will_field = will_stat_field
    
    if instance:isCreated() == false then
        instance:_create()
        instance:markAsCreated()
    else
        instance:_connect()
    end

    _updateMaxHealth(instance)
    return instance    
end

function HealthSubsystem:heal(value)
    self.hp_field.setValue(self.hp_field.getValue() + value)
end

function HealthSubsystem:damage(value)
    local armor_value = self.armor_field.getValue()

    if armor_value > value/2 then
        armor_value = value/2
    end

    local damage = value - armor_value

    self.hp_field.setValue(self.hp_field.getValue() - damage)
end

function HealthSubsystem:validate(updatedField)
    RollPunkAPI.log(self.body_field.name .. " " .. self.body_field.id)
    RollPunkAPI.log(updatedField == self.body_field or updatedField == self.will_field)

    if updatedField == self.body_field or updatedField == self.will_field then
        _updateMaxHealth(self)
    end
end

return HealthSubsystem