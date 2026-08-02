local FieldsServices = require("fields_services")
local CharacterSubsystem = require("character.subsystems.character_subsystem")
local FieldDefs = require("character.field_definitions")

local CommonSubsystem = {}

function CommonSubsystem.initialize(character)
    if not CharacterSubsystem.isCreated(character, "CommonSubsystem") then
        CommonSubsystem.create(character)
        CharacterSubsystem.markAsCreated(character, "CommonSubsystem")
    end
end

function CommonSubsystem.create(character)
    RollPunkAPI.log("Создание CommonSubsystem")
    local character_group = character.getField(FieldDefs.Groups.character_group.name)
    local image_group = character.getField(FieldDefs.Groups.image_and_character_group.name)
    local parameters_group = character.getField(FieldDefs.Groups.parameters_group.name)
    
    FieldsServices.createAndChild(image_group, FieldDefs.Common.character_image)
    FieldsServices.createAndChild(character_group, FieldDefs.Common.name)
    FieldsServices.createAndChild(character_group, FieldDefs.Common.nick_name)
    FieldsServices.createAndChild(character_group, FieldDefs.Common.class)
    FieldsServices.createAndChild(character_group, FieldDefs.Common.level)
    
    FieldsServices.createAndChild(parameters_group, FieldDefs.Common.inventory)
    FieldsServices.createAndChild(parameters_group, FieldDefs.Common.implants)
    FieldsServices.createAndChild(parameters_group, FieldDefs.Common.notes)
end

function CommonSubsystem.validate(character, updated_field)
    if updated_field.name == FieldDefs.Common.name.name then
        character.setName(character.getField(FieldDefs.Common.name.name).getValue())
    end
end

local function _onValidate(character, updated_field)
    CommonSubsystem.validate(character, updated_field)
end

ModHookerAPI.addHook("Validate", _onValidate)

return CommonSubsystem
