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
        Description = "Навык сосредаточения, памяти, запоминания информации и игнорирования отвлекающих факторов",
        Stat = "ВОЛЯ",
        Cost = 1,
        Base = false
    },

    concealReveal = 
    { 
        Name = "ConcealReveal",
        DisplayName = "Скрытие/Раскрытие",
        Description = "Умение прятать предметы и находить спрятанные предметы. \
        Этот навык используется для скрытия или обнурежения скрытого предмета под одеждой",
        Stat = "ИНТ",
        Cost = 1,
        Base = false
    },

    perception = 
    { 
        Name = "Perception",
        DisplayName = "Внимательность",
        Description = "Навык обнаружения скрытых вещей (кроме предметов скрытых через Скрытие)",
        Stat = "ИНТ",
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
        Description = "Умение лазать, метать, прыгать, плавать, поднимать тяжести и т.д.",
        Stat = "ЛВК",
        Cost = 1,
        Base = true
    },

    contortionist = 
    { 
        Name = "Contortionist",
        DisplayName = "Акробатика",
        Description = "Способность избавиться от наручников или других пут, пролезть в недоступные места или пространства",
        Stat = "ЛВК",
        Cost = 1,
        Base = false
    },

    endurance = 
    { 
        Name = "Endurance",
        DisplayName = "Выносливость",
        Description = "Превозмогание суровых условий, тягостей и лишений \
        + сопротевление болезненным воздействиям допросов, пыток, наркотиков и других веществ",
        Stat = "ВОЛЯ",
        Cost = 1,
        Base = false
    },

    stealth = 
    { 
        Name = "Stealth",
        DisplayName = "Скрытность",
        Description = "Умение незаметно двигаться, прятаться, действовать скрытно.\
        Вас могут найти с помощью навыка внимательности.",
        Stat = "ЛВК",
        Cost = 1,
        Base = true
    },

    evasion = 
    { 
        Name = "Evasion",
        DisplayName = "Уклонение",
        Description = "Умение уходить с пути опасности",
        Stat = "ЛВК",
        Cost = 1,
        Base = true
    }
}

-- Навыки управления
local _control_skills = 
{
    name = "ControlSkills",
    visible_name = "Навыки управления", 

    driveLandVehicle = 
    {
        Name = "DriveLandVehicle",
        DisplayName = "Вождение",
        Description = "Навык вождения и маневрирования наземным транспортом",
        Stat = "РЕА",
        Cost = 1,
        Base = false
    },

    pilotAirVehicle = 
    { 
        Name = "PilotAirVehicle",
        DisplayName = "Пилотирование (x2)",
        Description = "Навык управления летательными аппаратами",
        Stat = "РЕА",
        Cost = 2,
        Base = false
    },

    pilotSeaVehicle = 
    { 
        Name = "PilotSeaVehicle",
        DisplayName = "Судоходство",
        Description = "Навык обнаружения скрытых вещей (кроме предметов скрытых через Скрытие)",
        Stat = "РЕА",
        Cost = 1,
        Base = false
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
        Description = "Умение сделать неочевидный вывод из зацепок",
        Stat = "ИНТ",
        Cost = 1,
        Base = false
    },

    education = 
    { 
        Name = "Education",
        DisplayName = "Образование",
        Description = "Навык общих знаний, эквивалентный школьному образованию, \
        позволяющий читать, писать и знать достаточно, чтобы свести концы с концами",
        Stat = "ИНТ",
        Cost = 1,
        Base = true
    },

    criminology = 
    { 
        Name = "Criminology",
        DisplayName = "Криминология",
        Description = "Поиск отпечатков пальцев, проведение баллистических тестов и чтение полицейских файлов и записей",
        Stat = "ИНТ",
        Cost = 1,
        Base = false
    },

    cryptography = 
    { 
        Name = "Cryptography",
        DisplayName = "Криптография",
        Description = "Умение шифровать и расшифровывать сообщения",
        Stat = "ИНТ",
        Cost = 1,
        Base = false
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
        Description = "Умение драться и бороться с помощью грубой силы",
        Stat = "ЛВК",
        Cost = 1,
        Base = true
    },

    meleeweapon = 
    { 
        Name = "MeleeWeapon",
        DisplayName = "Оружие ближнего боя",
        Description = "Умение драться холодным оружием",
        Stat = "ЛВК",
        Cost = 1,
        Base = false
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
        Description = "Навык стрельбы из носимого оружия с плечевым упором",
        Stat = "РЕА",
        Cost = 1,
        Base = false
    },

    handguns = 
    { 
        Name = "HandGuns",
        DisplayName = "Пистолеты",
        Description = "Навык стрельбы из легкого ручного оружия",
        Stat = "РЕА",
        Cost = 1,
        Base = false
    },

    heavyweapons = 
    { 
        Name = "HeavyWeapons",
        DisplayName = "Тяжелое оружие",
        Description = "Навык точной стрельбы из оружия крупного калибра, например гранотометы и ракетницы",
        Stat = "РЕА",
        Cost = 1,
        Base = false
    },

    autofire = 
    { 
        Name = "AutoFire",
        DisplayName = "Автоматический огонь (x2)",
        Description = "Навык удержания цеои в прицеле в режиме автоматического огня, вопреки отдаче",
        Stat = "РЕА",
        Cost = 2,
        Base = false
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
        Description = "Умение профессионально играть на музыкальном инструменте",
        Stat = "ТЕХ",
        Cost = 1,
        Base = false
    },

    acting = 
    { 
        Name = "Acting",
        DisplayName = "Исполнение",
        Description = "Умение играть роли, маскироваться под кого-то реального или вымышленного, имитировать эмоции и настроения",
        Stat = "ХАР",
        Cost = 1,
        Base = false
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
        Description = "Умение читать выражение лица, язык тела, \
        различать эмоциональное состояние и обнаруживать ложь и обман",
        Stat = "ЭМП",
        Cost = 1,
        Base = true
    },

    conversation = 
    { 
        Name = "Conversation",
        DisplayName = "Общение",
        Description = "Умение вытягивать информацию из людей, не вызывая подозрений при разговоре",
        Stat = "ЭМП",
        Cost = 1,
        Base = true
    },

    trading = 
    { 
        Name = "Trading",
        DisplayName = "Торговля",
        Description = "Умение заключать выгодные сделки и торговаться",
        Stat = "ХАР",
        Cost = 1,
        Base = false
    },

    interrogation = 
    { 
        Name = "Interrogation",
        DisplayName = "Допрос",
        Description = "Навык насильственного извлечения информации",
        Stat = "ХАР",
        Cost = 1,
        Base = false
    },

    persuasion = 
    { 
        Name = "Persuasion",
        DisplayName = "Убеждение",
        Description = "Умение убеждать, уговаривать или оказывать влияние на людей",
        Stat = "ХАР",
        Cost = 1,
        Base = true
    },

    streetwise = 
    { 
        Name = "StreetWise",
        DisplayName = "Знаток улиц",
        Description = "Умение устанавливать контакты для получения незаконных товаров и контрабанды, \
        ведение переговоров с криминальными структурами. Так же позволяет избегать плохих ситуаций в плохих районах",
        Stat = "ХАР",
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
        DisplayName = "Базовая техника",
        Description = "Умение определять, понимать и ремонтировать простые электронные и механические устройства, \
        а также все другие предметы, которые не относятся к другим техническим навыкам.",
        Stat = "ТЕХ",
        Cost = 1,
        Base = false
    },

    cybertech = 
    { 
        Name = "CyberTech",
        DisplayName = "Кибернетика",
        Description = "Навык определения, понимания и ремонта кибернетики",
        Stat = "ТЕХ",
        Cost = 1,
        Base = false
    },

    electronics = 
    { 
        Name = "Electronics",
        DisplayName = "Электротехника (x2)",
        Description = "Знание, понимание, консультирование, установка и взлом сложных электронных \
        устройств и систем безопасности (компьютеры, кибердеки, нажимные плиты, камеры, лазерные ловушки и т.п.)",
        Stat = "ТЕХ",
        Cost = 2,
        Base = false
    },

    forgery = 
    { 
        Name = "Forgery",
        DisplayName = "Фальсификация",
        Description = "Умение создавать и обнаруживать фальшивые документы и ID-карты",
        Stat = "ТЕХ",
        Cost = 2,
        Base = false
    },

    firstaid = 
    { 
        Name = "FirstAid",
        DisplayName = "Первая помощь",
        Description = "Навык применения медецинских процедур к раненому человеку \
        для лечения наиболее распространенных критических ранений и предотвращения смерти",
        Stat = "ТЕХ",
        Cost = 1,
        Base = true
    },

    paramedic = 
    { 
        Name = "Paramedic",
        DisplayName = "Парамедик (x2)",
        Description = "Навык лечения раненых людей для устранения всех критическихранений \
        и предотвращения смерти, не требующих хирургии",
        Stat = "ТЕХ",
        Cost = 2,
        Base = false
    },

    vehicletech = 
    { 
        Name = "VehicleTech",
        DisplayName = "Автомеханик",
        Description = "Умение ремонтировать автомобили и мотоциклы",
        Stat = "ТЕХ",
        Cost = 1,
        Base = false
    },

    airvehicletech = 
    { 
        Name = "AirVehicleTech",
        DisplayName = "Воздушный транспорт",
        Description = "Умение ремонтировать авиатехнику",
        Stat = "ТЕХ",
        Cost = 1,
        Base = false
    },

    seavehicletech = 
    { 
        Name = "SeaVehicleTech",
        DisplayName = "Водный транспорт",
        Description = "Умение ремонтировать водную технику",
        Stat = "ТЕХ",
        Cost = 1,
        Base = false
    },

    weaponstech = 
    { 
        Name = "WeaponsTech",
        DisplayName = "Оружейник",
        Description = "Навык ремонта и обслуживания оружия всх типов",
        Stat = "ТЕХ",
        Cost = 1,
        Base = false
    },

    picklock = 
    { 
        Name = "PickLock",
        DisplayName = "Взлом замков",
        Description = "Навык взлома не электронных замков",
        Stat = "ТЕХ",
        Cost = 1,
        Base = false
    },

    pickpocket = 
    { 
        Name = "PickPocket",
        DisplayName = "Карманник",
        Description = "Навык кражи предметов, закрепленных на другом человеке, \
        кражи мелких вещей в магазине, оставаясь незамеченным",
        Stat = "ТЕХ",
        Cost = 1,
        Base = false
    }
}

local _skills_group_name

function SkillSubsystem.construct(skills_group_name)
    _skills_group_name = skills_group_name
end

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
    create_skills_category(self,_control_skills, self.skills_group)
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
    connect_skills_category(self,_control_skills)
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
            skill.setViewAccessLevel(2)
        else
            skill.setEditAccessLevel(3)

            if skill.getValue() == 0 then
                skill.setViewAccessLevel(3)
            end
        end
    end
end

function SkillSubsystem:validate(character, updatedField)

end

return SkillSubsystem