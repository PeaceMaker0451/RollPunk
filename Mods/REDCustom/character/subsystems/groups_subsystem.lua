local FieldsServices = require("fields_services")
local CharacterSubsystem = require("character.subsystems.character_subsystem")

---@class GroupsSubsystem : CharacterSubsystem
local GroupsSubsystem = setmetatable({}, CharacterSubsystem)
GroupsSubsystem.__index = GroupsSubsystem

---@type FieldsGroupData
local _head_group =
{
    name = "HeadGroup",
    type = "FieldsGroup",
    additional_data = { container_type = "VBox", label_visible = false }
}

---@type FieldsGroupData
local _update_group =
{
    name = "UpdateGroup",
    type = "FieldsGroup",
    additional_data = { container_type = "HFlow", label_visible = false }
}

---@type FieldsGroupData
local _action_group =
{
    name = "ActionGroup",
    visible_name = "Действия",
    type = "FieldsGroup",
    additional_data = { container_type = "HFlow", label_visible = true }
}

---@type FieldsGroupData
local _character_group =
{
    name = "CharacterGroup",
    visible_name = "Персонаж",
    type = "FieldsGroup",
    additional_data = { container_type = "HFlow", label_visible = true }
}

---@type FieldsGroupData
local _stats_group =
{
    name = "StatsGroup",
    visible_name = "Статы",
    type = "FieldsGroup",
    additional_data = { container_type = "HFlow", label_visible = true }
}

---@type FieldsGroupData
local _data_group =
{
    name = "DataGroup",
    type = "FieldsGroup",
    additional_data = { container_type = "HBox", label_visible = false }
}

---@type FieldsGroupData
local _skills_scroll_group =
{
    name = "SkillsScrollGroup",
    visible_name = "Навыки",
    type = "FieldsGroup",
    additional_data = {
        container_type = "Scroll",
        vertical_scroll = true,
        horizontal_scroll = false,
        label_visible = true
    }
}

---@type FieldsGroupData
local _skills_group =
{
    name = "SkillsGroup",
    visible_name = "",
    type = "FieldsGroup",
    additional_data = { container_type = "VBox", label_visible = false }
}

---@type FieldsGroupData
local _parameters_group =
{
    name = "ParametersGroup",
    visible_name = "Параметры",
    type = "FieldsGroup",
    additional_data = { container_type = "VBox", label_visible = true, stretch_ratio = 3.0 }
}

GroupsSubsystem.head_group_name = _head_group.name
GroupsSubsystem.update_group_name = _update_group.name
GroupsSubsystem.action_group_name = _action_group.name
GroupsSubsystem.character_group_name = _character_group.name
GroupsSubsystem.stats_group_name = _stats_group.name
GroupsSubsystem.data_group_name = _data_group.name
GroupsSubsystem.skills_scroll_group_name = _skills_scroll_group.name
GroupsSubsystem.skills_group_name = _skills_group.name
GroupsSubsystem.parameters_group_name = _parameters_group.name


function GroupsSubsystem.initialize()
    GroupsSubsystem.name = "GroupsSubsystem"
end

function GroupsSubsystem.create(character)
    RollPunkAPI.log("Создание GroupsSubsystem")

    FieldsServices.createAndChild(character, _update_group)
    local head_group = FieldsServices.createAndChild(character, _head_group)
    FieldsServices.createAndChild(head_group, _action_group)
    FieldsServices.createAndChild(head_group, _character_group)
    FieldsServices.createAndChild(head_group, _stats_group)
    local data_group = FieldsServices.createAndChild(character, _data_group)
    local skills_scroll_group = FieldsServices.createAndChild(data_group, _skills_scroll_group)
    FieldsServices.createAndChild(skills_scroll_group, _skills_group)
    FieldsServices.createAndChild(data_group, _parameters_group)
end

function GroupsSubsystem.connect(character)
end

return GroupsSubsystem
