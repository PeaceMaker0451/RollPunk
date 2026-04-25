local FieldsServices = require("fields_services")
local CharacterSubsystem = require("character.subsystems.character_subsystem")

---@class GroupsSubsystem : CharacterSubsystem
---@
---@field head_group FieldGroupAPI
---@field action_group FieldGroupAPI
---@field character_group FieldGroupAPI
---@field update_group FieldGroupAPI
---@field data_group FieldGroupAPI
---@field stats_group FieldGroupAPI
---@field skills_scroll_group FieldGroupAPI
---@field skills_group FieldGroupAPI
---@field parameters_group FieldGroupAPI
---@
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


function GroupsSubsystem:_create()
    RollPunkAPI.log("Создание GroupsSubsystem")
    self.update_group = FieldsServices.createAndChild(self.character, _update_group)
    self.head_group = FieldsServices.createAndChild(self.character, _head_group)
    self.action_group = FieldsServices.createAndChild(self.head_group, _action_group)
    self.character_group = FieldsServices.createAndChild(self.head_group, _character_group)
    self.stats_group = FieldsServices.createAndChild(self.head_group, _stats_group)
    self.data_group = FieldsServices.createAndChild(self.character, _data_group)
    self.skills_scroll_group = FieldsServices.createAndChild(self.data_group, _skills_scroll_group)
    self.skills_group = FieldsServices.createAndChild(self.skills_scroll_group, _skills_group)
    self.parameters_group = FieldsServices.createAndChild(self.data_group, _parameters_group)
end

function GroupsSubsystem:_connect()
    RollPunkAPI.log("Присоединение GroupsSubsystem")
    self.head_group = self.character.getField(_head_group.name)
    self.action_group = self.character.getField( _action_group.name)
    self.character_group = self.character.getField( _character_group.name)
    self.update_group = self.character.getField( _update_group.name)
    self.data_group = self.character.getField( _data_group.name)
    self.stats_group = self.character.getField( _stats_group.name)
    self.skills_scroll_group = self.character.getField( _skills_scroll_group.name)
    self.skills_group = self.character.getField( _skills_group.name)
    self.parameters_group = self.character.getField( _parameters_group.name)
end

---@param character EntityFieldAPI
function GroupsSubsystem:new(character)
    RollPunkAPI.log("3")

    ---@type GroupsSubsystem
    local instance = CharacterSubsystem.new(self, "GroupsSubsystem", character)

    if instance:isCreated() == false then
        instance:_create()
        instance:markAsCreated()
    else
        instance:_connect()
    end

    return instance
end

function GroupsSubsystem:validate(updated_field)

end

return GroupsSubsystem
