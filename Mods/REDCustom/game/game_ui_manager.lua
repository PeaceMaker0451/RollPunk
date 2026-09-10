local GameUIManager = {}

local function testButtonYebat(create_character_func, load_character_func)
    local dialog = FormsAPI.Dialogs.showUIDocumentDialog("тест ебать")
    dialog.document.addButton("Создать персонажа", create_character_func)
    dialog.document.addButton("Загрузить персонажа", load_character_func)
    dialog.document.addLabel("ШИИИЗАААА")
    dialog.document.addButton("Создать персонажа123", create_character_func)
    dialog.document.addButton("Загрузить персонажа341521", load_character_func)
    dialog.document.addButton("Создать персонажа215224342", create_character_func)
    dialog.document.addButton("Загрузить перс43242онажа", load_character_func)
    dialog.document.addButton("Создать пе424рсонажа", create_character_func)
    dialog.document.addButton("Загр2у234зить4324 персонажа", load_character_func)
end

function GameUIManager.showSetupMenu(create_character_func, load_character_func)
    SessionAPI.PlayerSpace.Actions.clear()
    SessionAPI.PlayerSpace.setActionsTabName("Создание персонажа")
    SessionAPI.PlayerSpace.Actions.addButton("Создать персонажа", create_character_func)
    SessionAPI.PlayerSpace.Actions.addButton("Загрузить персонажа", load_character_func)
end

function GameUIManager.showAdminSetupMenu(create_character_func, load_character_func, create_npc_func, load_npc_func, create_enemy_func, load_enemy_func)
    SessionAPI.PlayerSpace.Actions.clear()
    SessionAPI.PlayerSpace.setActionsTabName("Создание бегунков (админ-панель)")
    SessionAPI.PlayerSpace.Actions.addButton("Создать персонажа", create_character_func)
    SessionAPI.PlayerSpace.Actions.addButton("Загрузить персонажа", load_character_func)
    SessionAPI.PlayerSpace.Actions.addButton("Создать NPC", create_npc_func)
    SessionAPI.PlayerSpace.Actions.addButton("Загрузить NPC", load_npc_func)
    SessionAPI.PlayerSpace.Actions.addButton("Создать противника", create_enemy_func)
    SessionAPI.PlayerSpace.Actions.addButton("Загрузить противника", load_enemy_func)

    SessionAPI.PlayerSpace.Actions.addButton("Тестовая кнопка ебать", function ()
        testButtonYebat(create_character_func, load_character_func)
    end)
end

function GameUIManager.showOrUpdateGameMenu(show_character_func, teamate_table , npcs_table, enemies_table)
    SessionAPI.PlayerSpace.Actions.clear()
    SessionAPI.PlayerSpace.setActionsTabName("Найт-Сити")
    SessionAPI.PlayerSpace.Actions.addButton("Ваш персонаж", show_character_func)

    if teamate_table ~= nil and next(teamate_table) ~= nil then
        local teamate_container = SessionAPI.PlayerSpace.Actions.addContainer("Тиммейты")
        
        for index, value in ipairs(teamate_table) do
            teamate_container.addButton(value.name, value.func)
        end
    end

    if enemies_table ~= nil and next(enemies_table) ~= nil then
        local enemies_container = SessionAPI.PlayerSpace.Actions.addContainer("Противники")
        
        for index, value in ipairs(enemies_table) do
            enemies_container.addButton(value.name, value.func)
        end
    end
end

return GameUIManager