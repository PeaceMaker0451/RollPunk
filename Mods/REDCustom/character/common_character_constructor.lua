local GroupsSubsystem = require("character.subsystems.groups_subsystem")
local CommonSubsystem = require('character.subsystems.common_subsystem')
local StatsSubsystem = require('character.subsystems.stats_subsystem')
local HealthSubsystem = require('character.subsystems.health_subsystem')
local SkillsSubsystem = require('character.subsystems.skills_subsystem')
local PsychoSubsystem = require('character.subsystems.psycho_subsystem')
local UpdateSubsystem = require('character.subsystems.update_subsystem')

local CommonCharacterConstructor = {}

function CommonCharacterConstructor.initialize(character)
    -- Инициализация в правильном порядке зависимостей
    GroupsSubsystem.initialize(character)
    CommonSubsystem.initialize(character)
    StatsSubsystem.initialize(character)
    HealthSubsystem.initialize(character)
    SkillsSubsystem.initialize(character)
    PsychoSubsystem.initialize(character)
    UpdateSubsystem.initialize(character)
    
    character.setAdditionalDataField("version", "0.5.1")
end

function CommonCharacterConstructor.validate(character, updated_field)
    RollPunkAPI.log("Валидация персонажа из-за: " .. updated_field.name .. " (" .. updated_field.id .. ")")
    
    -- Прямые вызовы валидации без циклов по экземплярам
    CommonSubsystem.validate(character, updated_field)
    StatsSubsystem.validate(character, updated_field)
    HealthSubsystem.validate(character, updated_field)
    SkillsSubsystem.validate(character, updated_field)
    PsychoSubsystem.validate(character, updated_field)
    UpdateSubsystem.validate(character, updated_field)
end

return CommonCharacterConstructor
