---@class CommonPlayerConstructor
---@field subsystems {}
---@
---@field groups_subsystem GroupsSubsystem
---@field common_subsystem CommonSubsystem
---@field stats_subsystem StatsSubsystem
---@field health_subsystem HealthSubsystem
---@field skill_subsystem SkillSubsystem
---@field psycho_subsystem PsychoSubsystem
---@field update_subsystem UpdateSubsystem
---@
---@field character EntityFieldAPI
local CommonPlayerConstructor = {}
CommonPlayerConstructor.__index = CommonPlayerConstructor

local FieldsServices = require("fields_services")

local GroupsSubsystem = require("character.subsystems.groups_subsystem")
local CommonSubsystem = require('character.subsystems.common_subsystem')
local StatsSubsystem = require('character.subsystems.stats_subsystem')
local HealthSubsystem = require('character.subsystems.health_subsystem')
local SkillSubsystem = require('character.subsystems.skills_subsystem')
local PsychoSubsystem = require('character.subsystems.psycho_subsystem')
local UpdateSubsystem = require('character.subsystems.update_subsystem')


function CommonPlayerConstructor:new(character)
    local instance = setmetatable({}, self)
    instance.character = character
    instance.subsystems = {}

    instance.groups_subsystem = GroupsSubsystem:new(character)
    table.insert(instance.subsystems, instance.groups_subsystem)

    instance.common_subsystem = CommonSubsystem:new(character, 
    instance.groups_subsystem.character_group, instance.groups_subsystem.parameters_group)
    table.insert(instance.subsystems, instance.common_subsystem)

    instance.stats_subsystem = StatsSubsystem:new(character, instance.groups_subsystem.stats_group)
    table.insert(instance.subsystems, instance.stats_subsystem)

    instance.health_subsystem = HealthSubsystem:new(character, instance.groups_subsystem.action_group,instance.groups_subsystem.character_group,
    instance.groups_subsystem.parameters_group, instance.stats_subsystem.body_stat, instance.stats_subsystem.will_stat)
    table.insert(instance.subsystems, instance.health_subsystem)

    instance.skill_subsystem = SkillSubsystem:new(character, instance.groups_subsystem.skills_group)
    table.insert(instance.subsystems, instance.skill_subsystem)

    instance.psycho_subsystem = PsychoSubsystem:new(character, instance.groups_subsystem.action_group, instance.groups_subsystem.parameters_group, instance.groups_subsystem.character_group, instance.stats_subsystem.emp_stat)
    table.insert(instance.subsystems, instance.psycho_subsystem)
    
    instance.update_subsystem = UpdateSubsystem:new(character, instance.groups_subsystem.update_group, instance.skill_subsystem, 
    instance.stats_subsystem, instance.stats_subsystem.emp_stat.name, instance.psycho_subsystem.real_emp_value_name)
    table.insert(instance.subsystems, instance.update_subsystem)

    return instance
end

-- function CommonPlayerConstructor:initializeCharacter()
--     for _, subsystem in pairs(self.subsystems) do
--         if subsystem ~= nil and subsystem.name ~= nil then
--             RollPunkAPI.log("Инициализация подсистемы: " .. subsystem.name or "Unknown Name" .. "...")
--             subsystem:initialize()
--         else
--             RollPunkAPI.logError("Невозможно инициализировать подсистему " .. _)
--         end
--     end 
-- end

function  CommonPlayerConstructor:validateCharacter(UpdatedField)
    RollPunkAPI.log("Валидация персонажа из-за: " .. UpdatedField.name .. " (" .. UpdatedField.id .. ")")
    for _, subsystem in pairs(self.subsystems) do
        RollPunkAPI.log("Валидация подсистемы: " .. subsystem.name .. "...")
        subsystem:validate(UpdatedField)
    end 
end

return CommonPlayerConstructor