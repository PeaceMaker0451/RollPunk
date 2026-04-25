local FieldsServices = require("fields_services")
local CharacterSubsystem = require("character.subsystems.character_subsystem")

---@class SkillSubsystem : CharacterSubsystem
---@field skills table
---@field skills_group FieldGroupAPI
local SkillSubsystem = setmetatable({}, CharacterSubsystem)
SkillSubsystem.__index = SkillSubsystem

-- Навыки восприятия   
local _perception_skills = 
{
    name = "PerceptionSkills",
    visible_name = "Навыки восприятия", 

    concentration = 
    {
        Name = "Concentration",
        DisplayName = "Концентрация", 
        Stat = "WILL",
        Cost = 1,
        Base = true
    },

    perception = 
    { 
        Name = "Perception",
        DisplayName = "Внимательность",
        Stat = "INT",
        Cost = 1,
        Base = true
    }
}

-- Физические навыки
local _physical_skills = 
{
    name = "PhysicalSkills",
    visible_name = "Физические навыки",  

    athletics = 
    { 
        Name = "Athletics",
        DisplayName = "Атлетика",
        Stat = "DEX",
        Cost = 1,
        Base = true
    },

    contortionist = 
    { 
        Name = "Contortionist",
        DisplayName = "Акробатика",
        Stat = "DEX",
        Cost = 1,
        Base = true
    },

    endurance = 
    { 
        Name = "Endurance",
        DisplayName = "Выносливость",
        Stat = "WILL",
        Cost = 1,
        Base = true
    },

    stealth = 
    { 
        Name = "Stealth",
        DisplayName = "Скрытность",
        Stat = "DEX",
        Cost = 1,
        Base = true
    },

    evasion = 
    { 
        Name = "Evasion",
        DisplayName = "Уклонение",
        Stat = "DEX",
        Cost = 1,
        Base = true
    }
}

-- Образовательные навыки
local _educational_skills = 
{
    name = "EducationalSkills",
    visible_name = "Образовательные навыки",  

    deduction = 
    { 
        Name = "Deduction",
        DisplayName = "Дедукция",
        Stat = "INT",
        Cost = 1,
        Base = true
    },

    education = 
    { 
        Name = "Education",
        DisplayName = "Образование",
        Stat = "INT",
        Cost = 1,
        Base = true
    },

    science = 
    { 
        Name = "Science",
        DisplayName = "Наука",
        Stat = "INT",
        Cost = 1,
        Base = true
    }
}

-- Навыки ближнего боя
local _melee_skills = 
{
    name = "MeleeSkills",
    visible_name = "Навыки ближнего боя",  

    brawling = 
    { 
        Name = "Brawling",
        DisplayName = "Рукопашный бой",
        Stat = "DEX",
        Cost = 1,
        Base = true
    },

    meleeweapon = 
    { 
        Name = "MeleeWeapon",
        DisplayName = "Оружие ближнего боя",
        Stat = "DEX",
        Cost = 1,
        Base = true
    }
}

-- Навыки дальнего боя
local _ranged_skills = 
{
    name = "RangedSkills",
    visible_name = "Навыки дальнего боя",    

    shoulderarms = 
    { 
        Name = "ShoulderArms",
        DisplayName = "Тактическое оружие",
        Stat = "REF",
        Cost = 1,
        Base = true
    },

    handguns = 
    { 
        Name = "HandGuns",
        DisplayName = "Пистолеты",
        Stat = "REF",
        Cost = 1,
        Base = true
    },

    heavyweapons = 
    { 
        Name = "HeavyWeapons",
        DisplayName = "Тяжелое оружие",
        Stat = "REF",
        Cost = 1,
        Base = true
    },

    autofire = 
    { 
        Name = "AutoFire",
        DisplayName = "Автоматический огонь",
        Stat = "REF",
        Cost = 1,
        Base = true
    }
}

-- Навыки исполнения
local _performance_skills = 
{
    name = "PerformanceSkills",
    visible_name = "Навыки исполнения",    

    playinstrument = 
    { 
        Name = "PlayInstrument",
        DisplayName = "Игра на инструменте",
        Stat = "TECH",
        Cost = 1,
        Base = true
    },

    acting = 
    { 
        Name = "Acting",
        DisplayName = "Исполнение",
        Stat = "COOL",
        Cost = 1,
        Base = true
    }
}

-- Социальные навыки
local _social_skills = 
{
    name = "SocialSkills",
    visible_name = "Социальные навыки",

    humanperception = 
    { 
        Name = "HumanPerception",
        DisplayName = "Проницательность",
        Stat = "EMP",
        Cost = 1,
        Base = true
    },

    conversation = 
    { 
        Name = "Conversation",
        DisplayName = "Общение",
        Stat = "EMP",
        Cost = 1,
        Base = true
    },

    trading = 
    { 
        Name = "Trading",
        DisplayName = "Торговля",
        Stat = "COOL",
        Cost = 1,
        Base = true
    },

    interrogation = 
    { 
        Name = "Interrogation",
        DisplayName = "Допрос",
        Stat = "COOL",
        Cost = 1,
        Base = true
    },

    persuasion = 
    { 
        Name = "Persuasion",
        DisplayName = "Убеждение",
        Stat = "COOL",
        Cost = 1,
        Base = true
    },

    streetwise = 
    { 
        Name = "StreetWise",
        DisplayName = "Знаток улиц",
        Stat = "COOL",
        Cost = 1,
        Base = true
    }
}

-- Технические навыки
local _technical_skills = 
{
    name = "TechSkills",
    visible_name = "Технические навыки",

    basictech = 
    { 
        Name = "BasicTech",
        DisplayName = "Техника",
        Stat = "TECH",
        Cost = 1,
        Base = true
    },

    cybertech = 
    { 
        Name = "CyberTech",
        DisplayName = "Кибернетика",
        Stat = "TECH",
        Cost = 1,
        Base = true
    },

    electronics = 
    { 
        Name = "Electronics",
        DisplayName = "Электротехника",
        Stat = "TECH",
        Cost = 1,
        Base = true
    },

    firstaid = 
    { 
        Name = "FirstAid",
        DisplayName = "Первая помощь",
        Stat = "TECH",
        Cost = 1,
        Base = true
    },

    paramedic = 
    { 
        Name = "Paramedic",
        DisplayName = "Парамедик",
        Stat = "TECH",
        Cost = 1,
        Base = true
    },

    vehicletech = 
    { 
        Name = "VehicleTech",
        DisplayName = "Автомеханик",
        Stat = "TECH",
        Cost = 1,
        Base = true
    },

    specialvehicletech = 
    { 
        Name = "SpecialVehicleTech",
        DisplayName = "Особый транспорт",
        Stat = "TECH",
        Cost = 1,
        Base = true
    },

    weaponstech = 
    { 
        Name = "WeaponsTech",
        DisplayName = "Оружейник",
        Stat = "TECH",
        Cost = 1,
        Base = true
    },

    picklock = 
    { 
        Name = "PickLock",
        DisplayName = "Взлом замков",
        Stat = "TECH",
        Cost = 1,
        Base = true
    },

    pickpocket = 
    { 
        Name = "PickPocket",
        DisplayName = "Карманник",
        Stat = "TECH",
        Cost = 1,
        Base = true
    }
}

local _skills_group_name

function SkillSubsystem.construct(skills_group_name)
    _skills_group_name = skills_group_name
end

local function _createSkillFieldData(SkillData, priority)
    ---@type IntFieldData
    local fieldData = {
        name = SkillData.Name,
        visible_name = (SkillData.DisplayName) .. " ({stat})",
        type = "IntField",
        value = 0,
        min_value = 0,
        max_value = 20,
        view_access_level = 0,
        edit_access_level = 2,
        line_priority = priority,
        additional_data = { stat = SkillData.Stat, type = "skill" }
    }

    return fieldData
end

local function create_skills_category(self, skills, skiils_group)
    ---@type FieldsGroupData
    local new_category_group_data = 
    {
        type = "FieldsGroup",
        name = skills.name,
        visible_name = skills.visible_name
    }

    local new_category_field = FieldsServices.createAndChild(skiils_group,new_category_group_data)
    
    local i = 0
    for _, skill_info in pairs(skills) do
        if type(skill_info) == "table" then
            local field_data = _createSkillFieldData(skill_info, i)
            self.skills[skill_info.Name] = FieldsServices.createAndChild(new_category_field, field_data)
            RollPunkAPI.log("Создали скилл " .. skill_info.Name)
            i = i + 1
        end
    end
end

local function connect_skills_category(self, skills)
    for _, skill_info in pairs(skills) do
        if type(skill_info) == "table" then
            if self.skills[skill_info.name] ~= nil then
                RollPunkAPI.log("Повторное добавление скилла " .. skill_info.name)
            end

            self.skills[skill_info.Name] = self.character.getField(skill_info.Name)
            RollPunkAPI.log("подключили скилл " .. skill_info.Name)
        end
    end
end

function SkillSubsystem:_create(character)
    RollPunkAPI.log("Создание SkillsSubsystem")

    create_skills_category(self, _perception_skills, self.skills_group)
    create_skills_category(self,_physical_skills, self.skills_group)
    create_skills_category(self,_educational_skills, self.skills_group)
    create_skills_category(self,_melee_skills, self.skills_group)
    create_skills_category(self,_ranged_skills, self.skills_group)
    create_skills_category(self,_performance_skills, self.skills_group)
    create_skills_category(self,_social_skills, self.skills_group)
    create_skills_category(self,_technical_skills, self.skills_group)
end

function SkillSubsystem:_connect()
    RollPunkAPI.log("Присоединение SkillsSubsystem")
    
    connect_skills_category(self, _perception_skills)
    connect_skills_category(self,_physical_skills)
    connect_skills_category(self,_educational_skills)
    connect_skills_category(self,_melee_skills)
    connect_skills_category(self,_ranged_skills)
    connect_skills_category(self,_performance_skills)
    connect_skills_category(self,_social_skills)
    connect_skills_category(self,_technical_skills)
end

function SkillSubsystem:new(character, skills_group)
    ---@type SkillSubsystem
    local instance = CharacterSubsystem.new(self, "SkillsSubsystem", character)    
    
    instance.skills_group = skills_group
    instance.skills = {}

    if instance:isCreated() == false then
        instance:_create()
        instance:markAsCreated()
    else
        instance:_connect()
    end

    return instance    
end

function SkillSubsystem:setEditingEnabled(enabled)
    for _, skill in pairs(self.skills) do
        if enabled then
            skill.setEditAccessLevel(2)
        else
            skill.setEditAccessLevel(3)
        end
    end
end

function SkillSubsystem:validate(character, updatedField)

end

return SkillSubsystem