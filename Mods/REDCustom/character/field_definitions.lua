---@meta

local FieldDefinitions = {}

-- Группы полей
FieldDefinitions.Groups = {
    head_group = {
        name = "HeadGroup",
        type = "FieldsGroup",
        additional_data = { container_type = "VBox", label_visible = false }
    },
    update_group = {
        name = "UpdateGroup",
        type = "FieldsGroup",
        additional_data = { container_type = "HFlow", label_visible = false }
    },
    action_group = {
        name = "ActionGroup",
        visible_name = "Действия",
        type = "FieldsGroup",
        additional_data = { container_type = "HFlow", label_visible = true }
    },
    image_and_character_group = {
        name = "ImageGroup",
        visible_name = "Персонаж",
        type = "FieldsGroup",
        additional_data = { container_type = "HBox", label_visible = false }
    },
    character_group = {
        name = "CharacterGroup",
        visible_name = "Персонаж",
        type = "FieldsGroup",
        additional_data = { container_type = "HFlow", label_visible = true }
    },
    stats_group = {
        name = "StatsGroup",
        visible_name = "Статы",
        type = "FieldsGroup",
        additional_data = { container_type = "HFlow", label_visible = true }
    },
    data_group = {
        name = "DataGroup",
        type = "FieldsGroup",
        additional_data = { container_type = "HBox", label_visible = false }
    },
    skills_scroll_group = {
        name = "SkillsScrollGroup",
        visible_name = "Навыки",
        type = "FieldsGroup",
        additional_data = {
            container_type = "Scroll",
            vertical_scroll = true,
            horizontal_scroll = false,
            label_visible = false
        }
    },
    skills_group = {
        name = "SkillsGroup",
        visible_name = "",
        type = "FieldsGroup",
        additional_data = { container_type = "VBox", label_visible = false }
    },
    parameters_group = {
        name = "ParametersGroup",
        visible_name = "Параметры",
        type = "FieldsGroup",
        additional_data = { container_type = "VBox", label_visible = true, stretch_ratio = 3.0 }
    }
}

-- Статы
FieldDefinitions.Stats = {
    int = { name = "INT", visible_name = "ИНТ" },
    ref = { name = "REF", visible_name = "РЕА" },
    dex = { name = "DEX", visible_name = "ЛВК" },
    tech = { name = "TECH", visible_name = "ТЕХ" },
    cool = { name = "COOL", visible_name = "ХАР" },
    will = { name = "WILL", visible_name = "ВОЛЯ" },
    luck = { name = "LUCK", visible_name = "УДЧ" },
    emp = { name = "EMP", visible_name = "ЭМП" },
    body = { name = "BODY", visible_name = "ТЕЛО" }
}

-- Общие поля персонажа
FieldDefinitions.Common = {
    character_image = {
        name = "CharacterImage",
        visible_name = "Изображение",
        type = "ImageField",
        line_priority = 102,
        additional_data = { display_width = 140, display_height = 190, fit_mode = "cover" }
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
        value = "",
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
        edit_access_level = 2,
        line_priority = 49,
        additional_data = { vertical = true },
    },
    inventory = {
        name = "Inventory",
        type = "StringField",
        visible_name = "Инвентарь",
        value = "В этом поле отключен авто-перенос строк \nРекомендую писать по предмету на строку\nЗапись в свободной форме",
        view_access_level = 1,
        edit_access_level = 2,
        line_priority = 21,
        additional_data = { is_multiline = true, is_wrap_enabled = false }
    },
    implants = {
        name = "Implants",
        type = "StringField",
        visible_name = "Импланты",
        value = "Аналогично полю инвентаря",
        view_access_level = 1,
        edit_access_level = 2,
        line_priority = 20,
        additional_data = { is_multiline = true, is_wrap_enabled = false }
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
    }
}

-- Поля здоровья
FieldDefinitions.Health = {
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
    }
}

-- Поля психо-подсистемы
FieldDefinitions.Psycho = {
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
    }
}

-- Поля обновления
FieldDefinitions.Update = {
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
    }
}

-- Правила
FieldDefinitions.Rules = {
    damage = {
        name = "Damage",
        type = "Rule",
        hook = "Damage"
    },
    heal = {
        name = "Heal",
        type = "Rule",
        hook = "Heal"
    },
    spend_psycho_points = {
        name = "SpendPsychoPoints",
        type = "Rule",
        hook = "SpendPsychoPoints"
    },
    add_skill_points = {
        name = "AddSkillUpdatePoints",
        type = "Rule",
        hook = "AddSkillUpdatePoints"
    }
}

-- Поля правил
FieldDefinitions.RuleFields = {
    damage = {
        name = "DamageRuleField",
        rule_name = "Damage",
        visible_name = "Нанести урон",
        type = "RuleField",
        line_priority = 69,
    },
    heal = {
        name = "HealRuleField",
        rule_name = "Heal",
        visible_name = "Лечить",
        type = "RuleField",
        line_priority = 68,
    },
    spend_psycho_points = {
        name = "SpendPsychoPointsField",
        rule_name = "SpendPsychoPoints",
        visible_name = "Использовать Псих-очки",
        type = "RuleField",
        line_priority = 67,
    }
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

return FieldDefinitions
