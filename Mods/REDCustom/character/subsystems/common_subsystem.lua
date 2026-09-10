local FieldsServices = require("fields_services")
local CharacterSubsystem = require("character.subsystems.character_subsystem")
local FieldDefs = require("field_definitions")

local CommonSubsystem = {}

function CommonSubsystem.initialize(character)
    if not CharacterSubsystem.isCreated(character, "CommonSubsystem") then
        CommonSubsystem.create(character)
        CharacterSubsystem.markAsCreated(character, "CommonSubsystem")
    end
end

function CommonSubsystem.create(character)
    RollPunkAPI.log("Создание CommonSubsystem")
    local head_group = character.getField(FieldDefs.Character.Groups.head_group.name)
    
    local image_group = FieldsServices.createAndChild(head_group, FieldDefs.Character.Common.Groups.image_and_character_group)
    local character_group = FieldsServices.createAndChild(image_group, FieldDefs.Character.Common.Groups.character_group)
    local parameters_group = character.getField(FieldDefs.Character.Groups.parameters_group.name)
    
    FieldsServices.createAndChild(image_group, FieldDefs.Character.Common.character_image)
    FieldsServices.createAndChild(character_group, FieldDefs.Character.Common.name)
    FieldsServices.createAndChild(character_group, FieldDefs.Character.Common.nick_name)
    FieldsServices.createAndChild(character_group, FieldDefs.Character.Common.class)
    FieldsServices.createAndChild(character_group, FieldDefs.Character.Common.level)
    FieldsServices.createAndChild(parameters_group, FieldDefs.Character.Common.notes)
end

local function _onValidate(character, updated_field)
    
end

ModHookerAPI.addHook("Validate", _onValidate)

return CommonSubsystem
