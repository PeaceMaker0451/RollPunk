local FieldsServices = require("fields_services")
local CharacterSubsystem = require("character.subsystems.character_subsystem")

---@class CommonSubsystem : CharacterSubsystem
---@
---@field character_group FieldGroupAPI
---@field parameters_group FieldGroupAPI
---@
---@field name_field LineFieldAPI
---@field nickname_field LineFieldAPI
---@field class_field LineFieldAPI
---@field level_field LineFieldAPI
---@
---@field armor_field LineFieldAPI
---@field bio_field LineFieldAPI
---@field inventory_field LineFieldAPI
---@field implants_field LineFieldAPI
---@field notes_field LineFieldAPI
---@
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

function CommonSubsystem:_create()
    RollPunkAPI.log("Создание CommonSubsystem")
    self.name_field = FieldsServices.createAndChild(self.character_group, _name_field_data)
    self.nickname_field = FieldsServices.createAndChild(self.character_group, _nick_name_field_data)
    self.class_field = FieldsServices.createAndChild(self.character_group, _class_field_data)
    self.level_field = FieldsServices.createAndChild(self.character_group, _level_field_data)
    
    --self.armor_field = FieldsServices.createAndChild(self.parameters_group, _armor_field_data)
    --self.bio_field = FieldsServices.createAndChild(self.parameters_group, _bio)
    self.inventory_field = FieldsServices.createAndChild(self.parameters_group, _inventory)
    self.implants_field = FieldsServices.createAndChild(self.parameters_group, _implants)
    self.notes_field = FieldsServices.createAndChild(self.parameters_group, _notes)
end

function CommonSubsystem:_connect()
    RollPunkAPI.log("Присоединение CommonSubsystem")
    self.name_field = self.character.getField(_name_field_data.name)
    self.nickname_field = self.character.getField( _nick_name_field_data.name)
    self.class_field = self.character.getField( _class_field_data.name)
    self.level_field = self.character.getField( _level_field_data.name)
    
    --self.armor_field = self.character.getField( _armor_field_data.name)
    --self.bio_field = self.character.getField( _bio.name)
    self.inventory_field = self.character.getField( _inventory.name)
    self.implants_field = self.character.getField( _implants.name)
    self.notes_field = self.character.getField( _notes.name)
end

---@param character EntityFieldAPI
---@param character_group FieldGroupAPI
---@param parameters_group FieldGroupAPI
function CommonSubsystem:new(character, character_group, parameters_group)
    ---@type CommonSubsystem
    local instance = CharacterSubsystem.new(self, "CommonSubsystem", character)

    instance.character_group = character_group
    instance.parameters_group = parameters_group

    if instance:isCreated() == false then
        instance:_create()
        instance:markAsCreated()
    else
        instance:_connect()
    end

    return instance
end

function CommonSubsystem:SetLevel(level)
    self.level_field.setValue(level)
end

function CommonSubsystem:validate(updated_field)
    if updated_field == self.nickname_field then
        self.character.setName(self.nickname_field.getValue())
    end
end

return CommonSubsystem
