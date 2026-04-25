local FieldsServices = require("fields_services")
local CharacterSubsystem = require("character.subsystems.character_subsystem")

---@class StatsSubsystem : CharacterSubsystem
---@field int_stat LineFieldAPI
---@field ref_stat LineFieldAPI
---@field dex_stat LineFieldAPI
---@field tech_stat LineFieldAPI
---@field cool_stat LineFieldAPI
---@field will_stat LineFieldAPI
---@field luck_stat LineFieldAPI
---@field emp_stat LineFieldAPI
---@field body_stat LineFieldAPI
---@
---@field stats_group FieldGroupAPI
local StatSubsystem = setmetatable({}, CharacterSubsystem)
StatSubsystem.__index = StatSubsystem

local _stats = 
{
    int = 
    {
        name = "INT",
        visible_name = "ИНТ"
    },

    ref = 
    {
        name = "REF",
        visible_name = "РЕА"
    },

    dex = 
    {
        name = "DEX",
        visible_name = "ЛВК"
    },

    tech = 
    {
        name = "TECH",
        visible_name = "ТЕХ"
    },

    cool = 
    {
        name = "COOL",
        visible_name = "ХАР"
    },

    will = 
    {
        name = "WILL",
        visible_name = "ВОЛЯ"
    },

    luck = 
    {
        name = "LUCK",
        visible_name = "УДЧ"
    },

    emp = 
    {
        name = "EMP",
        visible_name = "ЭМП"
    },

    body = 
    {
        name = "BODY",
        visible_name = "ТЕЛО"
    },
}

local function _createStatsFieldData(stats_data, priority)
    ---@type IntFieldData
    local fieldData = {
        name = stats_data.name,
        visible_name = stats_data.visible_name,
        type = "IntField",
        value = 10,
        min_value = 2,
        max_value = 20,
        view_access_level = 0,
        edit_access_level = 2,
        line_priority = priority,
        additional_data = { is_stat = true, type = "stat" }
    }
    return fieldData
end

function StatSubsystem:_create()
    RollPunkAPI.log("Создание StatSubsystem")

    self.int_stat = FieldsServices.createAndChild(self.stats_group, _createStatsFieldData(_stats.int, 10))
    self.ref_stat = FieldsServices.createAndChild(self.stats_group, _createStatsFieldData(_stats.ref, 9))
    self.dex_stat = FieldsServices.createAndChild(self.stats_group, _createStatsFieldData(_stats.dex, 8))
    self.tech_stat = FieldsServices.createAndChild(self.stats_group, _createStatsFieldData(_stats.tech, 7))
    self.cool_stat = FieldsServices.createAndChild(self.stats_group, _createStatsFieldData(_stats.cool, 6))
    self.will_stat = FieldsServices.createAndChild(self.stats_group, _createStatsFieldData(_stats.will, 5))
    self.luck_stat = FieldsServices.createAndChild(self.stats_group, _createStatsFieldData(_stats.luck, 4))
    self.emp_stat = FieldsServices.createAndChild(self.stats_group, _createStatsFieldData(_stats.emp, 3 ))
    self.body_stat = FieldsServices.createAndChild(self.stats_group, _createStatsFieldData(_stats.body, 2))
end

function StatSubsystem:_connect()
    RollPunkAPI.log("Присоединение StatSubsystem")

    self.int_stat = self.character.getField(_stats.int.name)
    self.ref_stat = self.character.getField(_stats.ref.name)
    self.dex_stat = self.character.getField(_stats.dex.name)
    self.tech_stat = self.character.getField(_stats.tech.name)
    self.cool_stat = self.character.getField(_stats.cool.name)
    self.will_stat = self.character.getField(_stats.will.name)
    self.luck_stat = self.character.getField(_stats.luck.name)
    self.emp_stat = self.character.getField(_stats.emp.name)
    self.body_stat = self.character.getField(_stats.body.name)
end

---@param character EntityFieldAPI
function StatSubsystem:new(character, stats_group)
    ---@type StatsSubsystem
    local instance = CharacterSubsystem.new(self, "StatSubsystem", character)

    instance.stats_group = stats_group

    if instance:isCreated() == false then
        instance:_create()
        instance:markAsCreated()
    else
        instance:_connect()
    end

    return instance
end

function StatSubsystem:setEditingEnabled(enabled)
    for _, stat in pairs(self.stats_group.children) do
        if enabled then
            stat.setEditAccessLevel(2)
        else
            stat.setEditAccessLevel(3)
        end
    end
end

function StatSubsystem:validate(updatedField)
    
end

return StatSubsystem