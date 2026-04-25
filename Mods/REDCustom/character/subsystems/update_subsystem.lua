local FieldsServices = require("fields_services")
local CharacterSubsystem = require("character.subsystems.character_subsystem")

---@class UpdateSubsystem : CharacterSubsystem
---@field update_group FieldGroupAPI
---@field stats_points_field LineFieldAPI
---@field skills_points_field LineFieldAPI
---@field confirm_field LineFieldAPI
---@
---@field skills_ss SkillSubsystem
---@field stats_ss StatsSubsystem
---@field emp_field_name string
---@field real_emp_value_name string
local UpdateSubsystem = setmetatable({}, CharacterSubsystem)
UpdateSubsystem.__index = UpdateSubsystem

local _update_pool_name = "update_pool"
local _update_pool_offset_name = "update_pool_offset"
local _stats_updated_value_name = "stats_updated"
local _skills_updated_value_name = "skills_updated"

local _updateFields = 
{
    ---@type IntFieldData
    stats_update_points_field_data = 
    {
        name = "StatUpdate",
        visible_name = "Очки обновления Статов",
        type = "IntField",
        value = 0,
        max_value = 3000,
        min_value = -3000,
        view_access_level = 2,
        edit_access_level = 3,
        additional_data = { [_update_pool_name] = 0, [_update_pool_offset_name] = 0},
    },

    ---@type IntFieldData
    skills_update_points_field_data = 
    {
        name = "SkillUpdate",
        visible_name = "Очки обновления Навыков",
        type = "IntField",
        value = 0,
        max_value = 3000,
        min_value = -3000,
        view_access_level = 2,
        edit_access_level = 3,
        additional_data = { [_update_pool_name] = 0, [_update_pool_offset_name] = 0 },
    },
    ---@type BoolFieldData
    update_confirm = 
    {
        name = "UpdatedConfirm",
        visible_name = "Подтвердить",
        type = "BoolField",
        value = false,
        view_access_level = 2,
        edit_access_level = 3,
        additional_data = { [_stats_updated_value_name] = false, [_skills_updated_value_name] = false },
    }
}

---@param self UpdateSubsystem
local function _calculateCurrentSkillsTotalValue(self)
    local currentTotalValue = 0

    for _, skill in pairs(self.skills_ss.skills) do
            local cost = skill.getAdditionalDataField("cost") or 1
            currentTotalValue = currentTotalValue + skill.getValue() * cost
        end

    return currentTotalValue
end

---@param self UpdateSubsystem
local function _refreshSkillsUpdateField(self)
    local pool_offset = self.skills_points_field.getAdditionalDataField(_update_pool_offset_name)
    local pool_value = self.skills_points_field.getAdditionalDataField(_update_pool_name)
    local current_total_value = _calculateCurrentSkillsTotalValue(self)

    local available_points = pool_value + pool_offset - current_total_value

    self.skills_points_field.setValue(available_points);

    self.confirm_field.setAdditionalDataField(_skills_updated_value_name, available_points == 0)
end

---@param self UpdateSubsystem
local function _calculate_current_stats_total_value(self)
    local current_total_value = 0

    for _, stat in pairs(self.stats_ss.stats_group.children) do
        if(stat.name == self.emp_field_name) and (stat.getAdditionalDataField(self.real_emp_value_name) ~= nil) then
            current_total_value = current_total_value + stat.getAdditionalDataField(self.real_emp_value_name)
        else
            current_total_value = current_total_value + stat.getValue()
        end
    end

    return current_total_value
end

---@param self UpdateSubsystem
local function _refreshStatsUpdateField(self)
    local poolOffset = self.stats_points_field.getAdditionalDataField(_update_pool_offset_name)
    local poolValue = self.stats_points_field.getAdditionalDataField(_update_pool_name)
    local currentTotalValue = _calculate_current_stats_total_value(self)

    local available_points = poolValue + poolOffset - currentTotalValue

    self.stats_points_field.setValue(available_points);

    self.confirm_field.setAdditionalDataField(_stats_updated_value_name, available_points == 0)
end

---@param self UpdateSubsystem
local function _setUpdateGroupVisible(self, visible)
    if visible then
        self.update_group.setViewAccessLevel(2)
    else
        self.update_group.setViewAccessLevel(3)
    end
end

---@param self UpdateSubsystem
local function _refreshUpdateGroup(self)
    local stats_updated = self.confirm_field.getAdditionalDataField(_stats_updated_value_name)
    local skills_updated = self.confirm_field.getAdditionalDataField(_skills_updated_value_name)
    
    if stats_updated == false or skills_updated == false then
        _setUpdateGroupVisible(self, true)
        self.confirm_field.setEditAccessLevel(3)
        self.skills_ss:setEditingEnabled(true)
        self.stats_ss:setEditingEnabled(true)
    else
        self.confirm_field.setEditAccessLevel(2)
    end
end

---@param self UpdateSubsystem
local function _handleConfirmation(self)
    local stats_updated = self.confirm_field.getAdditionalDataField(_stats_updated_value_name)
    local skills_updated = self.confirm_field.getAdditionalDataField(_skills_updated_value_name)
    
    if stats_updated and skills_updated then
        _setUpdateGroupVisible(self, false)
        self.skills_ss:setEditingEnabled(false)
        self.stats_ss:setEditingEnabled(false)
    end
end

function UpdateSubsystem:setSkillUpdatePoints(value, should_update_offset)
    RollPunkAPI.log("Добавляем очки прокачки скиллов - " .. value)    

    if(should_update_offset) then
        local currentTotalValue = _calculateCurrentSkillsTotalValue(self)
        self.skills_points_field.setAdditionalDataField(_update_pool_offset_name, currentTotalValue)
        self.skills_points_field.setAdditionalDataField(_update_pool_name, currentTotalValue + value)
    else
        self.skills_points_field.setAdditionalDataField(_update_pool_name, value)
    end

    _refreshSkillsUpdateField(self)
    _refreshUpdateGroup(self)
end

function UpdateSubsystem:setStatsUpdatePoints(value, should_update_offset)
    RollPunkAPI.log("Добавляем очки прокачки статов - " .. value)

    if(should_update_offset) then
        local currentTotalValue = _calculate_current_stats_total_value(self)
        self.stats_points_field.setAdditionalDataField(_update_pool_offset_name, currentTotalValue)
        self.stats_points_field.setAdditionalDataField(_update_pool_name, currentTotalValue + value)
    else
        self.stats_points_field.setAdditionalDataField(_update_pool_name, value)
    end

    _refreshStatsUpdateField(self)
    _refreshUpdateGroup(self)
end

function UpdateSubsystem:_create()
    self.stats_points_field = FieldsServices.createAndChild(self.update_group, _updateFields.stats_update_points_field_data)
    self.skills_points_field = FieldsServices.createAndChild(self.update_group, _updateFields.skills_update_points_field_data)
    self.confirm_field = FieldsServices.createAndChild(self.update_group, _updateFields.update_confirm)
end

function UpdateSubsystem:_connect()
    self.stats_points_field = self.character.getField(_updateFields.stats_update_points_field_data.name)
    self.skills_points_field = self.character.getField(_updateFields.skills_update_points_field_data.name)
    self.confirm_field = self.character.getField(_updateFields.update_confirm.name)
end

function UpdateSubsystem:new(character, update_group, skills_ss, stats_ss, emp_field_name, real_emp_value_name)
    ---@type UpdateSubsystem
    local instance = CharacterSubsystem.new(self, "UpdateSubsystem", character)    
    
    instance.skills_ss = skills_ss
    instance.stats_ss = stats_ss
    instance.update_group = update_group
    instance.emp_field_name = emp_field_name
    instance.real_emp_value_name = real_emp_value_name

    if instance:isCreated() == false then
        instance:_create()
        instance:markAsCreated()
    else
        instance:_connect()
    end

    _refreshSkillsUpdateField(instance)
    _refreshStatsUpdateField(instance)
    _refreshUpdateGroup(instance)

    return instance
end

function UpdateSubsystem:validate(updated_field)
    if updated_field.getAdditionalDataField("type") == "stat" then
        _refreshStatsUpdateField(self)
        _refreshUpdateGroup(self)
    end

    if updated_field.getAdditionalDataField("type") == "skill" then
        _refreshSkillsUpdateField(self)
        _refreshUpdateGroup(self)
    end

    if updated_field == self.confirm_field then
        _handleConfirmation(self)
    end
end

return UpdateSubsystem