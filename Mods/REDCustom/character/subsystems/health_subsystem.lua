local FieldsServices = require("fields_services")
local CharacterSubsystem = require("character.subsystems.character_subsystem")
local FieldDefs = require("character.field_definitions")

local HealthSubsystem = {}

local function _updateMaxHealth(character)
    local bodyValue = character.getField(FieldDefs.Stats.body.name).getValue()
    local willValue = character.getField(FieldDefs.Stats.will.name).getValue()
    local newMaxHP = (((bodyValue + willValue) / 4) * 5) + 10

    character.getField(FieldDefs.Health.hp.name).setMaxValue(newMaxHP)
end

local function _upgradeTo051(character)
    RollPunkAPI.log("Обновление подсистемы здоровья до версии 0.5.1...")
    
    local healButton = character.getField(FieldDefs.RuleFields.heal.name)
    local damageButton = character.getField(FieldDefs.RuleFields.damage.name)

    healButton.setRuleName(FieldDefs.Rules.heal.name)
    damageButton.setRuleName(FieldDefs.Rules.damage.name)

    local healRule = character.getRule(FieldDefs.Rules.heal.name)
    local damageRule = character.getRule(FieldDefs.Rules.damage.name)

    healRule.setHook(FieldDefs.Rules.heal.hook)
    damageRule.setHook(FieldDefs.Rules.damage.hook)
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
    
    character.addRule(ConstructorAPI.createRule(FieldDefs.Rules.heal))
    character.addRule(ConstructorAPI.createRule(FieldDefs.Rules.damage))

    local character_group = character.getField(FieldDefs.Groups.character_group.name)
    local parameters_group = character.getField(FieldDefs.Groups.parameters_group.name)
    local action_group = character.getField(FieldDefs.Groups.action_group.name)
    
    FieldsServices.createAndChild(character_group, FieldDefs.Health.hp)
    FieldsServices.createAndChild(character_group, FieldDefs.Health.armor)
    FieldsServices.createAndChild(action_group, FieldDefs.RuleFields.heal)
    FieldsServices.createAndChild(action_group, FieldDefs.RuleFields.damage)
end

function HealthSubsystem.connect(character)
    RollPunkAPI.log("Присоединение HealthSubsystem")

    local version = character.getAdditionalDataField("version")

    if version == nil then
        _upgradeTo051(character)
    end
end

function HealthSubsystem.heal(character, value)
    local hp_field = character.getField(FieldDefs.Health.hp.name)
    hp_field.setValue(hp_field.getValue() + value)
end

function HealthSubsystem.damage(character, value)
    local hp_field = character.getField(FieldDefs.Health.hp.name)
    local armor_value = character.getField(FieldDefs.Health.armor.name).getValue()

    if armor_value > value/2 then
        armor_value = value/2
    end

    local damage = value - armor_value

    hp_field.setValue(hp_field.getValue() - damage)
end

function HealthSubsystem.validate(character, updatedField)
    if updatedField.name == FieldDefs.Stats.body.name or updatedField.name == FieldDefs.Stats.will.name then
        _updateMaxHealth(character)
    end
end

local function _onValidate(character, updatedField)
    HealthSubsystem.validate(character, updatedField)
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
ModHookerAPI.addHook(FieldDefs.Rules.damage.hook, _onDamage)
ModHookerAPI.addHook(FieldDefs.Rules.heal.hook, _onHeal)

return HealthSubsystem
