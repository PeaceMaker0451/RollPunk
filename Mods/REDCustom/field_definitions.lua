---@meta

local FieldDefinitions = {}

FieldDefinitions.Root = {
  ---@type EntityFieldData
    root_entity_data = 
    {
        type = "EntityField",
        name = "Меню",
    },

    ---@type RuleData
    create_character_rule_data = 
    {
        type = "Rule",
        name = "CreateCharacter",
        visible_name = "Создать персонажа",
        hook = "CreateCharacter"
    },

    ---@type RuleData
    load_character_rule_data = 
    {
        type = "Rule",
        name = "LoadCharacter",
        visible_name = "Загрузить персонажа",
        hook = "LoadCharacter"
    },

    ---@type RuleData
    load_field_rule_data = 
    {
        type = "Rule",
        name = "LoadField",
        visible_name = "Загрузить поле",
        hook = "LoadField"
    }  
}

FieldDefinitions.Character = {
    
    Groups = {
        head_group = {
            name = "HeadGroup",
            type = "FieldsGroup",
            additional_data = { container_type = "VBox", label_visible = false },
            line_priority = 100000,
        },
        action_group = {
            name = "ActionGroup",
            visible_name = "Действия",
            type = "FieldsGroup",
            additional_data = { container_type = "HFlow", label_visible = true },
            line_priority = 1000000000,
        },
        data_group = {
            name = "DataGroup",
            type = "FieldsGroup",
            additional_data = { container_type = "HBox", label_visible = false }
        },
        parameters_group = {
            name = "ParametersGroup",
            visible_name = "Параметры",
            type = "FieldsGroup",
            view_access_level = 2,
            additional_data = { container_type = "VBox", label_visible = true, stretch_ratio = 3.0 },
            line_priority = 0,
        },
    },
    Implants = {
        Groups = {
                implants_group = {
                name = "ImplantsGroup",
                visible_name = "Импланты",
                type = "FieldsGroup",
                additional_data = { container_type = "VBox", label_visible = true }
            },

            style_implants_group = {
                name = "StyleImplantsGroup",
                visible_name = "Стилевые",
                type = "FieldsGroup",
                additional_data = { container_type = "VBox", label_visible = true }
            },

            head_implants_group = {
                name = "HeadImplantsGroup",
                visible_name = "Голова",
                type = "FieldsGroup",
                additional_data = { container_type = "VBox", label_visible = true }
            },
        },
        
        Rules = {
            add_style = {
                name = "AddStyle",
                type = "Rule",
                hook = "RED_AddStyle",
                arguments = {"string"}
            },
            remove_style = {
                name = "RemoveStyle",
                type = "Rule",
                hook = "RED_RemoveStyle",
                arguments = {"string"}
            },
            add_head = {
                name = "AddHead",
                type = "Rule",
                hook = "RED_AddStyle",
                arguments = {"string"}
            },
            remove_head = {
                name = "RemoveHead",
                type = "Rule",
                hook = "RED_RemoveHead",
                arguments = {"string"}
            },
        },

        implant_field_base = {
            name = "ImplantReference_",
            visible_name = "Неназванный имплант",
            type = "EntityReferenceField",
            line_priority = 0,
            edit_access_level = 1,
            view_access_level = 0,
        }
    },
    Weapons = {
        current_weapon = {
            name = "CurrentWeaponReference",
            visible_name = "Нет оружия",
            type = "EntityReferenceField",
            line_priority = 0,
            edit_access_level = 0,
            view_access_level = 0,
        },

        Rules = {
            set_weapon = {
                name = "SetWeapon",
                type = "Rule",
                hook = "RED_SetWeapon",
                arguments = {weapon_id = "string"}
            },
            remove_weapon = {
                name = "RemoveWeapon",
                type = "Rule",
                hook = "RED_RemoveWeapon"
            },
            set_weapon_UI = {
                name = "SetWeapon_UI",
                type = "Rule",
                hook = "RED_SetWeapon_UI"
            },
            remove_weapon_UI = {
                name = "RemoveWeapon_UI",
                type = "Rule",
                hook = "RED_RemoveWeapon_UI"
            },
        },

        RuleFields = {
            set_weapon_UI = {
                name = "SetWeaponField",
                rule_name = "SetWeapon_UI",
                visible_name = "Установить оружие",
                type = "RuleField",
                line_priority = 67,
            },
            remove_weapon_UI = {
                name = "RemoveWeaponField",
                rule_name = "RemoveWeapon_UI",
                visible_name = "Убрать оружие",
                type = "RuleField",
                line_priority = 66,
            },
        }
    },
    Inventory = {
        inventory_group = {
            name = "InventoryGroup",
            visible_name = "Инвентарь",
            type = "FieldsGroup",
            additional_data = { container_type = "VBox", label_visible = true },
            line_priority = 65,
            view_access_level = 2,
        },

        inventory_item_reference_sample = {
            name = "<item_id>",
            visible_name = "<Название предмета>",
            type = "EntityReferenceField",
            line_priority = 0,
            edit_access_level = 2,
            view_access_level = 0,
        },

        takeable_item_tag_name = "takeable",
        item_name_field_name = "Name",

        Rules = {
            add_item = {
                name = "AddItem",
                type = "Rule",
                hook = "RED_AddItem",
                arguments = {item_id = "string"}
            },

            remove_item = {
                name = "RemoveItem",
                type = "Rule",
                hook = "RED_RemoveItem",
                arguments = {item_id = "string"}
            },

            add_item_ui = {
                name = "AddItem_UI",
                type = "Rule",
                hook = "RED_AddItem_UI",
            },

            remove_item_ui = {
                name = "RemoveItem_UI",
                type = "Rule",
                hook = "RED_RemoveItem_UI",
            },
        },

        RuleFields = {
            add_item = {
                name = "AddItemField",
                rule_name = "AddItem_UI",
                visible_name = "Добавить предмет",
                type = "RuleField",
                line_priority = 67,
            },
            remove_item = {
                name = "RemoveItemField",
                rule_name = "RemoveItem_UI",
                visible_name = "Выбросить предмет",
                type = "RuleField",
                line_priority = 66,
            },
        }
    },
    -- Статы
    Stats = {
        int = { name = "INT", visible_name = "ИНТ" },
        ref = { name = "REF", visible_name = "РЕА" },
        dex = { name = "DEX", visible_name = "ЛВК" },
        tech = { name = "TECH", visible_name = "ТЕХ" },
        cool = { name = "COOL", visible_name = "ХАР" },
        will = { name = "WILL", visible_name = "ВОЛЯ" },
        luck = { name = "LUCK", visible_name = "УДЧ" },
        emp = { name = "EMP", visible_name = "ЭМП" },
        body = { name = "BODY", visible_name = "ТЕЛО" },

        stats_group = {
            name = "StatsGroup",
            visible_name = "Статы",
            type = "FieldsGroup",
            additional_data = { container_type = "HFlow", label_visible = true },
            line_priority = 70,
        },
    },

    Skills = {
        skills_scroll_group = {
            name = "SkillsScrollGroup",
            visible_name = "Навыки",
            type = "FieldsGroup",
            additional_data = {
                container_type = "Scroll",
                vertical_scroll = true,
                horizontal_scroll = false,
                label_visible = false,
            },
            line_priority = 101,
        },
        skills_group = {
            name = "SkillsGroup",
            visible_name = "",
            type = "FieldsGroup",
            additional_data = { container_type = "VBox", label_visible = false },
            line_priority = 101,
        },

        -- Навыки восприятия   
        perception_skills = 
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
        },

        -- Физические навыки
        physical_skills = 
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
        },

        -- Навыки управления
        control_skills = 
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
        },

        -- Образовательные навыки
        educational_skills = 
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
        },

        -- Навыки ближнего боя
        melee_skills = 
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
        },

        -- Навыки дальнего боя
        ranged_skills = 
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
        },

        -- Навыки исполнения
        performance_skills = 
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
        },

        -- Социальные навыки
        social_skills = 
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
        },

        -- Технические навыки
        technical_skills = 
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
    },

    -- Общие поля персонажа
    Common = {
        character_image = {
            name = "CharacterImage",
            visible_name = "Изображение",
            type = "ImageField",
            line_priority = 102,
            additional_data = { display_width = 140, display_height = 190, fit_mode = "cover" },
            view_access_level = 0,
            edit_access_level = 2,
        },
        name = {
            name = "Name",
            type = "StringField",
            visible_name = "Полное имя",
            value = "",
            view_access_level = 0,
            edit_access_level = 2,
            line_priority = 101,
        },
        nick_name = {
            name = "NickName",
            type = "StringField",
            visible_name = "Прозвище",
            value = "Прозвище не задано",
            view_access_level = 0,
            edit_access_level = 2,
            line_priority = 100,
        },
        class = {
            name = "Class",
            type = "StringField",
            visible_name = "Класс",
            value = "",
            view_access_level = 0,
            edit_access_level = 2,
            line_priority = 50,
        },
        level = {
            name = "Level",
            type = "IntField",
            visible_name = "Уровень",
            value = 10,
            max_value = 20,
            min_value = 0,
            view_access_level = 0,
            edit_access_level = 3,
            line_priority = 49,
            additional_data = { vertical = true },
        },

        notes = {
            name = "Notes",
            type = "StringField",
            visible_name = "Заметки",
            value = "",
            view_access_level = 2,
            edit_access_level = 2,
            line_priority = 0,
            additional_data = { is_multiline = true, is_wrap_enabled = true }
        },

        Groups = {
            image_and_character_group = {
                name = "ImageGroup",
                visible_name = "Персонаж",
                type = "FieldsGroup",
                additional_data = { container_type = "HBox", label_visible = false },
                line_priority = 100,
            },
            character_group = {
                name = "CharacterGroup",
                visible_name = "Персонаж",
                type = "FieldsGroup",
                additional_data = { container_type = "HFlow", label_visible = true }
            },
        }
    },

    -- Поля здоровья
    Health = {
        hp = {
            name = "HP",
            visible_name = "Очки здоровья",
            type = "IntField",
            value = 200,
            max_value = 200,
            min_value = 0,
            view_access_level = 0,
            edit_access_level = 2,
            line_priority = 70,
            additional_data = { show_max = true, vertical = true }
        },
        armor = {
            name = "Armor",
            type = "IntField",
            visible_name = "Броня",
            value = 10,
            max_value = 1000,
            min_value = 0,
            view_access_level = 0,
            edit_access_level = 2,
            line_priority = 51,
            additional_data = { vertical = true }
        },

        Rules = {
            damage = {
                name = "Damage",
                type = "Rule",
                hook = "RED_Damage",
                arguments = {value = "int"}
            },
            heal = {
                name = "Heal",
                type = "Rule",
                hook = "RED_Heal",
                arguments = {value = "int"}
            },
            damage_ui = {
                name = "DamageUI",
                type = "Rule",
                hook = "RED_Damage_UI",
            },
            heal_ui = {
                name = "HealUI",
                type = "Rule",
                hook = "RED_Heal_UI",
            },
        },

        RuleFields = 
        {
            damage_ui = {
                name = "DamageRuleField",
                rule_name = "DamageUI",
                visible_name = "Нанести урон",
                type = "RuleField",
                line_priority = 69,
            },
            heal_ui = {
                name = "HealRuleField",
                rule_name = "HealUI",
                visible_name = "Лечить",
                type = "RuleField",
                line_priority = 68,
            },
        }
    },

    -- Поля психо-подсистемы
    Psycho = {
        psycho_points = {
            name = "PsychoPoints",
            visible_name = "Псих-очки",
            type = "IntField",
            value = 10,
            max_value = 100,
            min_value = 0,
            view_access_level = 0,
            edit_access_level = 2,
            line_priority = 60,
            additional_data = { show_max = true, vertical = true  },
        },
        humanity = {
            name = "Humanity",
            visible_name = "Человечность",
            type = "IntField",
            value = 0,
            max_value = 100,
            min_value = -100,
            view_access_level = 0,
            edit_access_level = 3,
            line_priority = 50,
            additional_data = { show_max = true, vertical = true },
        },
        humanity_loss = {
            name = "HumanityLoss",
            visible_name = "Потеря человечности",
            type = "IntField",
            value = 0,
            max_value = 100,
            min_value = 0,
            view_access_level = 2,
            edit_access_level = 2,
            line_priority = 48,
            additional_data = { show_max = false },
        },
        humanity_max_loss = {
            name = "HumanityMaxLoss",
            visible_name = "Потеря макс. человечности",
            type = "IntField",
            value = 0,
            max_value = 100,
            min_value = 0,
            view_access_level = 2,
            edit_access_level = 2,
            line_priority = 49,
            additional_data = { show_max = false },
        },

        Rules = {
            spend_psycho_points = {
                name = "SpendPsycho",
                type = "Rule",
                hook = "RED_SpendPsycho",
                arguments = {value = "int"}
            },
            add_psycho_points = {
                name = "AddPsycho",
                type = "Rule",
                hook = "RED_AddPsycho",
                arguments = {value = "int"}
            },
            spend_psycho_points_ui = {
                name = "SpendPsycho_UI",
                type = "Rule",
                hook = "RED_SpendPsycho_UI"
            },
            add_psycho_points_ui = {
                name = "AddPsycho_UI",
                type = "Rule",
                hook = "RED_AddPsycho_UI"
            },
            increase_humanity_loss_ui = {
                name = "IncreaseHumanityLoss_UI",
                type = "Rule",
                hook = "RED_IncreaseHumanityLoss_UI"
            },
            decrease_humanity_loss_ui = {
                name = "DecreaseHumanityLoss_UI",
                type = "Rule",
                hook = "RED_DecreaseHumanityLoss_UI"
            },
            increase_max_humanity_loss_ui = {
                name = "IncreaseMaxHumanityLoss_UI",
                type = "Rule",
                hook = "RED_IncreaseMaxHumanityLoss_UI"
            },
            decrease_max_humanity_loss_ui = {
                name = "DecreaseMaxHumanityLoss_UI",
                type = "Rule",
                hook = "RED_DecreaseMaxHumanityLoss_UI"
            }
        },

        RuleFields = {
            spend_psycho_points_ui = {
                name = "SpendPsychoPointsField",
                rule_name = "SpendPsycho_UI",
                visible_name = "Использовать Псих-очки",
                type = "RuleField",
                line_priority = 67,
                view_access_level = 2,
                edit_access_level = 2,
            },
            add_psycho_points_ui = {
                name = "AddPsychoPointsField",
                rule_name = "AddPsycho_UI",
                visible_name = "Восстановить Псих-очки",
                type = "RuleField",
                line_priority = 68,
                view_access_level = 2,
                edit_access_level = 2,
            },
            increase_humanity_loss_ui = {
                name = "IncreaseHumanityLossField",
                rule_name = "IncreaseHumanityLoss_UI",
                visible_name = "Увеличить потерю человечности",
                type = "RuleField",
                line_priority = 67,
                view_access_level = 3,
                edit_access_level = 3,
            },
            decrease_humanity_loss_ui = {
                name = "DecreaseHumanityLossField",
                rule_name = "DecreaseHumanityLoss_UI",
                visible_name = "Уменьшить потерю человечности",
                type = "RuleField",
                line_priority = 67,
                view_access_level = 3,
                edit_access_level = 3,
            },
            increase_max_humanity_loss_ui = {
                name = "IncreaseMaxHumanityLossField",
                rule_name = "IncreaseMaxHumanityLoss_UI",
                visible_name = "Увеличить потерю макс. человечности",
                type = "RuleField",
                line_priority = 67,
                view_access_level = 3,
                edit_access_level = 3,
            },
            decrease_max_humanity_loss_ui = {
                name = "DecreaseMaxHumanityLossField",
                rule_name = "DecreaseMaxHumanityLoss_UI",
                visible_name = "Уменьшить потерю макс. человечности",
                type = "RuleField",
                line_priority = 67,
                view_access_level = 3,
                edit_access_level = 3,
            },
        }
    },

    -- Поля обновления
    Update = {
        stats_update_points = {
            name = "StatUpdate",
            visible_name = "Очки обновления Статов",
            type = "IntField",
            value = 0,
            max_value = 3000,
            min_value = -3000,
            view_access_level = 2,
            edit_access_level = 3,
            additional_data = { update_pool = 0, update_pool_offset = 0},
        },
        skills_update_points = {
            name = "SkillUpdate",
            visible_name = "Очки обновления Навыков",
            type = "IntField",
            value = 0,
            max_value = 3000,
            min_value = -3000,
            view_access_level = 2,
            edit_access_level = 3,
            additional_data = { update_pool = 0, update_pool_offset = 0 },
        },
        update_confirm = {
            name = "UpdatedConfirm",
            visible_name = "Подтвердить",
            type = "BoolField",
            value = false,
            view_access_level = 2,
            edit_access_level = 3,
            additional_data = { stats_updated = false, skills_updated = false },
        },

        update_group = {
            name = "UpdateGroup",
            type = "FieldsGroup",
            additional_data = { container_type = "HFlow", label_visible = false }
        },

        Rules = {
            add_stat_points = {
                name = "AddStatPoints",
                type = "Rule",
                hook = "RED_AddStatPoints",
                arguments = {"int"}
            },
            add_skill_points = {
                name = "AddSkillPoints",
                type = "Rule",
                hook = "RED_AddSkillPoints",
                arguments = {"int"}
            },
            add_stat_points_UI = {
                name = "AddStatPoints_UI",
                type = "Rule",
                hook = "RED_AddStatPoints_UI"
            },
            add_skill_points_UI = {
                name = "AddSkillPoints_UI",
                type = "Rule",
                hook = "RED_AddSkillPoints_UI"
            },
        },

        RuleFields = {
            add_stat_points_UI = {
                name = "AddStatPointsField",
                rule_name = "AddStatPoints_UI",
                visible_name = "Добавить очки статов",
                type = "RuleField",
                line_priority = 67,
            },
            add_skill_points_UI = {
                name = "AddSkillPointsField",
                rule_name = "AddSkillPoints_UI",
                visible_name = "Добавить очки скиллов",
                type = "RuleField",
                line_priority = 67,
            },
        }
    },
    character_entity_data =
    {
    name = "New Character",
    type = "EntityField",
    additional_data = {type = "RED_character"}
    },

    enemy_character_tag = "enemy",
    npc_character_tag = "npc",
    player_character_tag = "teamate"
}

-- Константы для additional_data ключей
FieldDefinitions.AdditionalDataKeys = {
    update_pool = "update_pool",
    update_pool_offset = "update_pool_offset",
    stats_updated = "stats_updated",
    skills_updated = "skills_updated",
    real_emp_value = "real_emp_value",
    old_emp_value = "old_emp_value"
}

FieldDefinitions.PlayerRegistry =
{
    registry = 
    {
        type = "EntityField",
        name = "RED_PlayersRegistry"
    },
    
    player_group = {
        type = "FieldsGroup",
        name = "<player_ID>"
    },
    
    character = 
    {
        name = "Character",
        type = "StringField",
        value = nil
    }
}

FieldDefinitions.Weapons = {
    weapon_base = {
        type = "EntityField",
        name = "RED_Weapon",
        additional_data = {type = "RED_weapon"}
    },

    Groups = {
        image_group = {
            name = "ImageGroup",
        visible_name = "",
        type = "FieldsGroup",
        additional_data = { container_type = "VBox", label_visible = false }
        },

        parameters_group = {
            name = "ParametersGroup",
            visible_name = "Параметры",
            type = "FieldsGroup",
            additional_data = { container_type = "HFlow", label_visible = true }
        }
    },

    image_field = {
        name = "Image",
        visible_name = "Изображение",
        type = "ImageField",
        line_priority = 102,
        additional_data = { display_width = 190, display_height = 140, fit_mode = "cover" }
    },

    Parameters = {
        name = {
            name = "Name",
            type = "StringField",
            visible_name = "Название",
            value = "",
            view_access_level = 0,
            edit_access_level = 2,
            line_priority = 101,
        },

        damage = {
            name = "Damage",
            type = "IntField",
            visible_name = "Урон",
            value = 10,
            max_value = 20,
            min_value = 0,
            view_access_level = 0,
            edit_access_level = 2,
            line_priority = 49,
            additional_data = { vertical = true },
        },
    }
}

return FieldDefinitions
