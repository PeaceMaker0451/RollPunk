local FieldsServices = require("fields_services")
local CharacterSubsystem = require("character.subsystems.character_subsystem")
local FieldDefs = require("field_definitions")

local SkillsSubsystem = {}

local function _createSkillFieldData(SkillData, priority)
    ---@type IntFieldData
    local custom_min_value = 0

    if SkillData.Base then
        custom_min_value = 2
    end
    
    local fieldData = {
        name = SkillData.Name,
        visible_name = (SkillData.DisplayName) .. " ({stat})",
        type = "IntField",
        value = 0,
        min_value = custom_min_value,
        max_value = 20,
        view_access_level = 0,
        edit_access_level = 2,
        line_priority = priority,
        additional_data = { stat = SkillData.Stat, type = "skill" }
    }

    return fieldData
end

local function create_skills_category(skills, skills_group)
    ---@type FieldsGroupData
    local new_category_group_data = 
    {
        type = "FieldsGroup",
        name = skills.name,
        visible_name = skills.visible_name
    }

    local new_category_field = FieldsServices.createAndChild(skills_group, new_category_group_data)
    
    local i = 0
    for _, skill_info in pairs(skills) do
        if type(skill_info) == "table" then
            local field_data = _createSkillFieldData(skill_info, i)
            FieldsServices.createAndChild(new_category_field, field_data)
            RollPunkAPI.log("Создали скилл " .. skill_info.Name)
            i = i + 1
        end
    end
end

function SkillsSubsystem.initialize(character)
    if not CharacterSubsystem.isCreated(character, "SkillsSubsystem") then
        SkillsSubsystem.create(character)
        CharacterSubsystem.markAsCreated(character, "SkillsSubsystem")
    end
end

function SkillsSubsystem.create(character)
    RollPunkAPI.log("Создание SkillsSubsystem")

    local data_group = character.getField(FieldDefs.Character.Groups.data_group.name)
    local skill_scroll_group = FieldsServices.createAndChild(data_group, FieldDefs.Character.Skills.skills_scroll_group)
    local skills_group = FieldsServices.createAndChild(skill_scroll_group, FieldDefs.Character.Skills.skills_group)

    create_skills_category(FieldDefs.Character.Skills.perception_skills, skills_group)
    create_skills_category(FieldDefs.Character.Skills.physical_skills, skills_group)
    create_skills_category(FieldDefs.Character.Skills.control_skills, skills_group)
    create_skills_category(FieldDefs.Character.Skills.educational_skills, skills_group)
    create_skills_category(FieldDefs.Character.Skills.melee_skills, skills_group)
    create_skills_category(FieldDefs.Character.Skills.ranged_skills, skills_group)
    create_skills_category(FieldDefs.Character.Skills.performance_skills, skills_group)
    create_skills_category(FieldDefs.Character.Skills.social_skills, skills_group)
    create_skills_category(FieldDefs.Character.Skills.technical_skills, skills_group)
end

function SkillsSubsystem.setEditingEnabled(character, enabled)
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

return SkillsSubsystem
