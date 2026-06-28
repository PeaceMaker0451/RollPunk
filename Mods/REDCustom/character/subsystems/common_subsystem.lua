local FieldsServices = require("fields_services")
local CharacterSubsystem = require("character.subsystems.character_subsystem")

---@class CommonSubsystem : CharacterSubsystem
local CommonSubsystem = setmetatable({}, CharacterSubsystem)
CommonSubsystem.__index = CommonSubsystem

---@type StringFieldData
local _name_field_data =
{
    name = "Name",
    type = "StringField",
    visible_name = "Полное имя",
    value = "",
    view_access_level = 0,
    edit_access_level = 2,
    line_priority = 101,
}

---@type StringFieldData
local _nick_name_field_data =
{
    name = "NickName",
    type = "StringField",
    visible_name = "Прозвище",
    value = "",
    view_access_level = 0,
    edit_access_level = 2,
    line_priority = 100,
}

---@type StringFieldData
local _class_field_data =
{
    name = "Class",
    type = "StringField",
    visible_name = "Класс",
    value = "",
    view_access_level = 0,
    edit_access_level = 2,
    line_priority = 50,
}

---@type IntFieldData
local _level_field_data =
{
    name = "Level",
    type = "IntField",
    visible_name = "Уровень",
    value = 10,
    max_value = 20,
    min_value = 0,
    view_access_level = 0,
    edit_access_level = 2,
    line_priority = 49,
    additional_data = { vertical = true },
}

-- ---@type StringFieldData
-- local _bio =
-- {
--     name = "Bio",
--     type = "StringField",
--     visible_name = "Биография",
--     value = "Информация, которую вы, возможно, хотели рассказать о себе",
--     view_access_level = 0,
--     edit_access_level = 2,
--     line_priority = 101,
--     additional_data = { is_multiline = true, is_wrap_enabled = true }
-- }

---@type StringFieldData
local _inventory =
{
    name = "Inventory",
    type = "StringField",
    visible_name = "Инвентарь",
    value =
    "В этом поле отключен авто-перенос строк \nРекомендую писать по предмету на строку\nЗапись в свободной форме",
    view_access_level = 1,
    edit_access_level = 2,
    line_priority = 21,
    additional_data = { is_multiline = true, is_wrap_enabled = false }
}

---@type StringFieldData
local _implants =
{
    name = "Implants",
    type = "StringField",
    visible_name = "Импланты",
    value = "Аналогично полю инвентаря",
    view_access_level = 1,
    edit_access_level = 2,
    line_priority = 20,
    additional_data = { is_multiline = true, is_wrap_enabled = false }
}

---@type StringFieldData
local _notes =
{
    name = "Notes",
    type = "StringField",
    visible_name = "Заметки",
    value = "",
    view_access_level = 2,
    edit_access_level = 2,
    line_priority = 0,
    additional_data = { is_multiline = true, is_wrap_enabled = true }
}

local _character_group_name
local _parameters_group_name

function CommonSubsystem.initialize(character_group_name, parameters_group_name)
    CommonSubsystem.name = "CommonSubsystem"
    
    _character_group_name = character_group_name
    _parameters_group_name = parameters_group_name
end

function CommonSubsystem.create(character)
    RollPunkAPI.log("Создание CommonSubsystem")
    local character_group = character.getField(_character_group_name)
    local parameters_group = character.getField(_parameters_group_name)
    
    FieldsServices.createAndChild(character_group, _name_field_data)
    FieldsServices.createAndChild(character_group, _nick_name_field_data)
    FieldsServices.createAndChild(character_group, _class_field_data)
    FieldsServices.createAndChild(character_group, _level_field_data)
    
    FieldsServices.createAndChild(parameters_group, _inventory)
    FieldsServices.createAndChild(parameters_group, _implants)
    FieldsServices.createAndChild(parameters_group, _notes)
end

function CommonSubsystem.connect()
end

-- function CommonSubsystem.SetLevel(character, level)
--     local level_field = character.getField(_level_field_data.name) 
--     level_field.setValue(level)
-- end

local function _onValidate(character, updated_field)
    if updated_field.name == _name_field_data.name then
        character.setName(character.getField(_name_field_data.name).getValue())
    end
end

ModHookerAPI.addHook("Validate", _onValidate)

return CommonSubsystem
