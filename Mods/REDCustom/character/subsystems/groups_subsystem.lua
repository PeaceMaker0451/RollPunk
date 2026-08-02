local FieldsServices = require("fields_services")
local CharacterSubsystem = require("character.subsystems.character_subsystem")
local FieldDefs = require("character.field_definitions")

local GroupsSubsystem = {}

function GroupsSubsystem.initialize(character)
    if not CharacterSubsystem.isCreated(character, "GroupsSubsystem") then
        GroupsSubsystem.create(character)
        CharacterSubsystem.markAsCreated(character, "GroupsSubsystem")
    end
end

function GroupsSubsystem.create(character)
    RollPunkAPI.log("Создание GroupsSubsystem")

    FieldsServices.createAndChild(character, FieldDefs.Groups.update_group)
    local head_group = FieldsServices.createAndChild(character, FieldDefs.Groups.head_group)
    FieldsServices.createAndChild(head_group, FieldDefs.Groups.action_group)
    local image_group = FieldsServices.createAndChild(head_group, FieldDefs.Groups.image_and_character_group)
    FieldsServices.createAndChild(image_group, FieldDefs.Groups.character_group)
    FieldsServices.createAndChild(head_group, FieldDefs.Groups.stats_group)
    local data_group = FieldsServices.createAndChild(character, FieldDefs.Groups.data_group)
    local skills_scroll_group = FieldsServices.createAndChild(data_group, FieldDefs.Groups.skills_scroll_group)
    FieldsServices.createAndChild(skills_scroll_group, FieldDefs.Groups.skills_group)
    FieldsServices.createAndChild(data_group, FieldDefs.Groups.parameters_group)
end

return GroupsSubsystem
