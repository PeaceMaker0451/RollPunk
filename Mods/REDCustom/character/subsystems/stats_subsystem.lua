local FieldsServices = require("fields_services")
local CharacterSubsystem = require("character.subsystems.character_subsystem")
local FieldDefs = require("character.field_definitions")

local StatsSubsystem = {}

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

function StatsSubsystem.initialize(character)
    if not CharacterSubsystem.isCreated(character, "StatsSubsystem") then
        StatsSubsystem.create(character)
        CharacterSubsystem.markAsCreated(character, "StatsSubsystem")
    end
end

function StatsSubsystem.create(character)
    RollPunkAPI.log("Создание StatsSubsystem")
    
    local stats_group = character.getField(FieldDefs.Groups.stats_group.name)
    
    FieldsServices.createAndChild(stats_group, _createStatsFieldData(FieldDefs.Stats.int, 10))
    FieldsServices.createAndChild(stats_group, _createStatsFieldData(FieldDefs.Stats.ref, 9))
    FieldsServices.createAndChild(stats_group, _createStatsFieldData(FieldDefs.Stats.dex, 8))
    FieldsServices.createAndChild(stats_group, _createStatsFieldData(FieldDefs.Stats.tech, 7))
    FieldsServices.createAndChild(stats_group, _createStatsFieldData(FieldDefs.Stats.cool, 6))
    FieldsServices.createAndChild(stats_group, _createStatsFieldData(FieldDefs.Stats.will, 5))
    FieldsServices.createAndChild(stats_group, _createStatsFieldData(FieldDefs.Stats.luck, 4))
    FieldsServices.createAndChild(stats_group, _createStatsFieldData(FieldDefs.Stats.emp, 3))
    FieldsServices.createAndChild(stats_group, _createStatsFieldData(FieldDefs.Stats.body, 2))
end

function StatsSubsystem.setEditingEnabled(character, enabled)
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

function StatsSubsystem.validate(character, updatedField)
    -- Пока пустая, но готова для будущих валидаций
end

return StatsSubsystem
