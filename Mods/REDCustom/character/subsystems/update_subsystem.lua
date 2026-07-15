local FieldsServices = require("fields_services")
local CharacterSubsystem = require("character.subsystems.character_subsystem")
local FieldDefs = require("character.field_definitions")

local UpdateSubsystem = {}

local function _calculateCurrentSkillsTotalValue(character)
    local currentTotalValue = 0
    local skills_group = character.getField(FieldDefs.Groups.skills_group.name)

    for _, skills_subgroup in pairs(skills_group.children) do
        for _, skill in pairs(skills_subgroup.children) do
            if skill.getAdditionalDataField("type") == "skill" then
                local cost = skill.getAdditionalDataField("cost") or 1
                currentTotalValue = currentTotalValue + skill.getValue() * cost
            else
                RollPunkAPI.log(skill.name .. " - не скилл по типу")
            end
        end
    end

    return currentTotalValue
end

local function _refreshSkillsUpdateField(character)
    local skills_points_field = character.getField(FieldDefs.Update.skills_update_points.name)
    local pool_offset = skills_points_field.getAdditionalDataField(FieldDefs.AdditionalDataKeys.update_pool_offset)
    local pool_value = skills_points_field.getAdditionalDataField(FieldDefs.AdditionalDataKeys.update_pool)
    local current_total_value = _calculateCurrentSkillsTotalValue(character)

    local available_points = pool_value + pool_offset - current_total_value
    skills_points_field.setValue(available_points)

    local confirm_field = character.getField(FieldDefs.Update.update_confirm.name)
    confirm_field.setAdditionalDataField(FieldDefs.AdditionalDataKeys.skills_updated, available_points == 0)
end

local function _calculate_current_stats_total_value(character)
    local current_total_value = 0
    local stats_group = character.getField(FieldDefs.Groups.stats_group.name)

    for _, stat in pairs(stats_group.children) do
        if stat.getAdditionalDataField("type") == "stat" then
            local emp_field = character.getField(FieldDefs.Stats.emp.name)
            if stat.name == FieldDefs.Stats.emp.name and emp_field.getAdditionalDataField(FieldDefs.AdditionalDataKeys.real_emp_value) ~= nil then
                current_total_value = current_total_value + emp_field.getAdditionalDataField(FieldDefs.AdditionalDataKeys.real_emp_value)
            else
                current_total_value = current_total_value + stat.getValue()
            end
        end
    end

    return current_total_value
end

local function _refreshStatsUpdateField(character)
    local stats_points_field = character.getField(FieldDefs.Update.stats_update_points.name)
    local poolOffset = stats_points_field.getAdditionalDataField(FieldDefs.AdditionalDataKeys.update_pool_offset)
    local poolValue = stats_points_field.getAdditionalDataField(FieldDefs.AdditionalDataKeys.update_pool)
    local currentTotalValue = _calculate_current_stats_total_value(character)

    local available_points = poolValue + poolOffset - currentTotalValue
    stats_points_field.setValue(available_points)

    local confirm_field = character.getField(FieldDefs.Update.update_confirm.name)
    confirm_field.setAdditionalDataField(FieldDefs.AdditionalDataKeys.stats_updated, available_points == 0)
end

local function _setUpdateGroupVisible(character, visible)
    local update_group = character.getField(FieldDefs.Groups.update_group.name)
    if visible then
        update_group.setViewAccessLevel(2)
    else
        update_group.setViewAccessLevel(3)
    end
end

local function _refreshUpdateGroup(character)
    local confirm_field = character.getField(FieldDefs.Update.update_confirm.name)
    local stats_updated = confirm_field.getAdditionalDataField(FieldDefs.AdditionalDataKeys.stats_updated)
    local skills_updated = confirm_field.getAdditionalDataField(FieldDefs.AdditionalDataKeys.skills_updated)
    
    if stats_updated == false or skills_updated == false then
        _setUpdateGroupVisible(character, true)
        confirm_field.setEditAccessLevel(3)
        UpdateSubsystem.setSkillsEditingEnabled(character, true)
        UpdateSubsystem.setStatsEditingEnabled(character, true)
    else
        confirm_field.setEditAccessLevel(2)
    end
end

local function _handleConfirmation(character)
    local confirm_field = character.getField(FieldDefs.Update.update_confirm.name)
    local stats_updated = confirm_field.getAdditionalDataField(FieldDefs.AdditionalDataKeys.stats_updated)
    local skills_updated = confirm_field.getAdditionalDataField(FieldDefs.AdditionalDataKeys.skills_updated)
    
    if stats_updated and skills_updated then
        _setUpdateGroupVisible(character, false)
        UpdateSubsystem.setSkillsEditingEnabled(character, false)
        UpdateSubsystem.setStatsEditingEnabled(character, false)
    end
end

function UpdateSubsystem.initialize(character)
    if not CharacterSubsystem.isCreated(character, "UpdateSubsystem") then
        UpdateSubsystem.create(character)
        CharacterSubsystem.markAsCreated(character, "UpdateSubsystem")
    end
    
    _refreshSkillsUpdateField(character)
    _refreshStatsUpdateField(character)
    _refreshUpdateGroup(character)
end

function UpdateSubsystem.create(character)
    local update_group = character.getField(FieldDefs.Groups.update_group.name)
    FieldsServices.createAndChild(update_group, FieldDefs.Update.stats_update_points)
    FieldsServices.createAndChild(update_group, FieldDefs.Update.skills_update_points)
    FieldsServices.createAndChild(update_group, FieldDefs.Update.update_confirm)
end

function UpdateSubsystem.setSkillUpdatePoints(character, value, should_update_offset)
    RollPunkAPI.log("Добавляем очки прокачки скиллов - " .. value)
    
    local skills_points_field = character.getField(FieldDefs.Update.skills_update_points.name)
    
    if should_update_offset then
        local currentTotalValue = _calculateCurrentSkillsTotalValue(character)
        skills_points_field.setAdditionalDataField(FieldDefs.AdditionalDataKeys.update_pool_offset, currentTotalValue)
        skills_points_field.setAdditionalDataField(FieldDefs.AdditionalDataKeys.update_pool, currentTotalValue + value)
    else
        skills_points_field.setAdditionalDataField(FieldDefs.AdditionalDataKeys.update_pool, value)
    end

    _refreshSkillsUpdateField(character)
    _refreshUpdateGroup(character)
end

function UpdateSubsystem.setStatsUpdatePoints(character, value, should_update_offset)
    RollPunkAPI.log("Добавляем очки прокачки статов - " .. value)
    
    local stats_points_field = character.getField(FieldDefs.Update.stats_update_points.name)
    
    if should_update_offset then
        local currentTotalValue = _calculate_current_stats_total_value(character)
        stats_points_field.setAdditionalDataField(FieldDefs.AdditionalDataKeys.update_pool_offset, currentTotalValue)
        stats_points_field.setAdditionalDataField(FieldDefs.AdditionalDataKeys.update_pool, currentTotalValue + value)
    else
        stats_points_field.setAdditionalDataField(FieldDefs.AdditionalDataKeys.update_pool, value)
    end

    _refreshStatsUpdateField(character)
    _refreshUpdateGroup(character)
end

function UpdateSubsystem.setSkillsEditingEnabled(character, enabled)
    local skills_group = character.getField(FieldDefs.Groups.skills_group.name)
    for _, skill in pairs(skills_group.children) do
        if skill.getAdditionalDataField("type") == "skill" then
            if enabled then
                skill.setEditAccessLevel(2)
                skill.setViewAccessLevel(2)
            else
                skill.setEditAccessLevel(3)
                if skill.getValue() == 0 then
                    skill.setViewAccessLevel(3)
                end
            end
        end
    end
end

function UpdateSubsystem.setStatsEditingEnabled(character, enabled)
    local stats_group = character.getField(FieldDefs.Groups.stats_group.name)
    for _, stat in pairs(stats_group.children) do
        if stat.getAdditionalDataField("type") == "stat" then
            if enabled then
                stat.setEditAccessLevel(2)
            else
                stat.setEditAccessLevel(3)
            end
        end
    end
end

function UpdateSubsystem.validate(character, updated_field)
    if updated_field.getAdditionalDataField("type") == "stat" then
        _refreshStatsUpdateField(character)
        _refreshUpdateGroup(character)
    end

    if updated_field.getAdditionalDataField("type") == "skill" then
        _refreshSkillsUpdateField(character)
        _refreshUpdateGroup(character)
    end

    local confirm_field = character.getField(FieldDefs.Update.update_confirm.name)
    if updated_field == confirm_field then
        _handleConfirmation(character)
    end
end

return UpdateSubsystem
