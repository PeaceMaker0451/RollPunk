local FieldsServices = require("fields_services")
local CharacterSubsystem = require("character.subsystems.character_subsystem")
local FieldDefs = require("field_definitions")

local GroupsSubsystem = {}

function GroupsSubsystem.initialize(character)
    if not CharacterSubsystem.isCreated(character, "GroupsSubsystem") then
        GroupsSubsystem.create(character)
        CharacterSubsystem.markAsCreated(character, "GroupsSubsystem")
    end
end

function GroupsSubsystem.create(character)
    RollPunkAPI.log("Создание GroupsSubsystem")

    local head_group = FieldsServices.createAndChild(character, FieldDefs.Character.Groups.head_group)
    FieldsServices.createAndChild(head_group, FieldDefs.Character.Groups.action_group)
    local data_group = FieldsServices.createAndChild(character, FieldDefs.Character.Groups.data_group)
    FieldsServices.createAndChild(data_group, FieldDefs.Character.Groups.parameters_group)
end

return GroupsSubsystem
