local FieldsServices = require("fields_services")
local CharacterSubsystem = require("character.subsystems.character_subsystem")
local FieldDefs = require("field_definitions")

local HealthSubsystem = {}

local function _updateMaxHealth(character)
    local bodyValue = character.getField(FieldDefs.Character.Stats.body.name).getValue()
    local willValue = character.getField(FieldDefs.Character.Stats.will.name).getValue()
    local newMaxHP = (((bodyValue + willValue) / 4) * 5) + 10

    local hp = character.getField(FieldDefs.Character.Health.hp.name)
    hp.setMaxValue(newMaxHP)
    hp.setValue(newMaxHP)
end

function HealthSubsystem.initialize(character)
    if not CharacterSubsystem.isCreated(character, "HealthSubsystem") then
        HealthSubsystem.create(character)
        CharacterSubsystem.markAsCreated(character, "HealthSubsystem")
    else
        HealthSubsystem.connect(character)
    end
    
    _updateMaxHealth(character)
end

function HealthSubsystem.create(character)
    RollPunkAPI.log("Создание HealthSubsystem")
    
    character.addRule(ConstructorAPI.createRule(FieldDefs.Character.Health.Rules.heal))
    character.addRule(ConstructorAPI.createRule(FieldDefs.Character.Health.Rules.damage))
    character.addRule(ConstructorAPI.createRule(FieldDefs.Character.Health.Rules.heal_ui))
    character.addRule(ConstructorAPI.createRule(FieldDefs.Character.Health.Rules.damage_ui))

    local character_group = character.getField(FieldDefs.Character.Common.Groups.character_group.name)
    local parameters_group = character.getField(FieldDefs.Character.Groups.parameters_group.name)
    local action_group = character.getField(FieldDefs.Character.Groups.action_group.name)
    
    FieldsServices.createAndChild(character_group, FieldDefs.Character.Health.hp)
    FieldsServices.createAndChild(character_group, FieldDefs.Character.Health.armor)
    FieldsServices.createAndChild(action_group, FieldDefs.Character.Health.RuleFields.heal_ui)
    FieldsServices.createAndChild(action_group, FieldDefs.Character.Health.RuleFields.damage_ui)
end

function HealthSubsystem.connect(character)
    RollPunkAPI.log("Присоединение HealthSubsystem")
    local version = character.getAdditionalDataField("version")
end

function HealthSubsystem.heal(character, value)
    local hp_field = character.getField(FieldDefs.Character.Health.hp.name)
    hp_field.setValue(hp_field.getValue() + value)
    SessionAPI.addUserLog(character.getField(FieldDefs.Character.Common.nick_name.name).getValue(), "восстанавливает здоровье:" .. value)    
end

function HealthSubsystem.damage(character, value)
    local hp_field = character.getField(FieldDefs.Character.Health.hp.name)
    local armor_value = character.getField(FieldDefs.Character.Health.armor.name).getValue()

    if armor_value > value/2 then
        armor_value = value/2
    end

    local damage = value - armor_value

    hp_field.setValue(hp_field.getValue() - damage)

    SessionAPI.addUserLog(character.getField(FieldDefs.Character.Common.nick_name.name).getValue(), "получает урон: " .. damage)
end

local function _onValidate(character, updatedField)
    if updatedField.name == FieldDefs.Character.Stats.body.name or updatedField.name == FieldDefs.Character.Stats.will.name then
        _updateMaxHealth(character)
    end
end

local function _onDamage(character, value)
    HealthSubsystem.damage(character, value)
end

local function _onHeal(character, value)
    HealthSubsystem.heal(character, value)
end

local function _onDamageUI(character)
    FormsAPI.Dialogs.showIntDialog("Введите число урона:", nil, function(confirmed, result)
            if confirmed == false or result == 0 then
                return
            end
        
            character.getRule(FieldDefs.Character.Health.Rules.damage.name).execute({result})
        end)
end

local function _onHealUI(character)
    FormsAPI.Dialogs.showIntDialog("Введите число здоровья:", nil, function(confirmed, result)
        if confirmed == false or result == 0 then
                return
            end
        
        character.getRule(FieldDefs.Character.Health.Rules.heal.name).execute({result})
        end)
end

ModHookerAPI.addHook("Validate", _onValidate)
ModHookerAPI.addHook(FieldDefs.Character.Health.Rules.damage.hook, _onDamage)
ModHookerAPI.addHook(FieldDefs.Character.Health.Rules.heal.hook, _onHeal)
ModHookerAPI.addHook(FieldDefs.Character.Health.Rules.damage_ui.hook, _onDamageUI)
ModHookerAPI.addHook(FieldDefs.Character.Health.Rules.heal_ui.hook, _onHealUI)

return HealthSubsystem
