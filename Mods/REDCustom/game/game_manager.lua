local Logger = require("console_logger")
local FieldDefs = require("field_definitions")
local CharacterFactory = require("character.characters_factory")
local OwnershipManager = require("character.character_ownership_manager")
local UIManager = require("game.game_ui_manager")
local PlayerRegistry = require("players.player_registry_controller")
local Tagger = require("character.character_tagger")

local GameManager = {}
local _is_player_initialized = false

local function openPlayersCharacter(player)
    local character_id = PlayerRegistry.getPlayersCharacterId(player)
    local character = SessionAPI.getEntityById(character_id)
    SessionAPI.PlayerSpace.setDisplayedEntity(character)
end

local function createCharacterSelectorName(character)
    if Tagger.isPlayer(character) then
        return "(" .. character.name .. ") " .. character.getField(FieldDefs.Character.Common.nick_name.name).getValue()
    end
end

local function updateGameView()
    if _is_player_initialized == false then
        return
    end

    local teamates = {}

    Logger.log("ищем тиммейтов")
    
    for index, entity in ipairs(SessionAPI.entities) do
        if entity.getAdditionalDataField("type") == FieldDefs.Character.character_entity_data.additional_data.type
        and Tagger.isPlayer(entity)
        and PlayerRegistry.isCurrentCharacter(entity) == false then
            Logger.log("Нашли игрока " .. entity.name)
            
            table.insert(teamates, {name = createCharacterSelectorName(entity), func = function ()
                SessionAPI.PlayerSpace.setDisplayedEntity(entity)
            end})
        end
    end

    UIManager.showOrUpdateGameMenu(function ()
        openPlayersCharacter(SessionAPI.current_player)
    end, teamates )
end

local function initializePlayerWithCharacter(character)
    character.setName(SessionAPI.current_player.name)
    OwnershipManager.PinCharacterToPlayer(character, SessionAPI.current_player)
    SessionAPI.addEntity(character)

    SessionAPI.addSystemLog("RED", SessionAPI.current_player.name .. " присоединился к игре!")
    _is_player_initialized = true

    updateGameView()
end

local function createCharacter()
    local character = CharacterFactory.createCharacter()
    Tagger.setCharacterTag(character)
    initializePlayerWithCharacter(character)
end

local function loadCharacter()
    SessionAPI.loadString(function(data)
        local field = SerializatorAPI.deserializeField(data)
        Tagger.setCharacterTag(field)
        CharacterFactory.handleCharacter(field)
        initializePlayerWithCharacter(field)
    end)
end

function GameManager.start()
    Logger.log("старт гейм-менеджера")
    SessionAPI.PlayerSpace.openSessionView(false)
    
    ModHookerAPI.addHook("EntityInitialized", updateGameView)
    ModHookerAPI.addHook("Validate", updateGameView)
    ModHookerAPI.addHook("PatchApplied", updateGameView)
    ModHookerAPI.addHook("StateApplied", updateGameView)

    if(PlayerRegistry.getPlayersCharacterId(SessionAPI.current_player) ~= "") then
        _is_player_initialized = true
        updateGameView()
        SessionAPI.addSystemLog("RED", "Сессия игрока " .. SessionAPI.current_player.name .. " восстановлена.")
        return
    end

    if(SessionAPI.current_player.is_admin) then
        UIManager.showAdminSetupMenu(createCharacter, loadCharacter)
        return
    end

    FormsAPI.Dialogs.showInformationDialog("Привет, чумба!", 
            {
                message = "Это набор правил, вдохновленный киберпанк редом и воссоздающий игровой опыт ролевых настолок.\nСейчас тебе нужно будет создать или загрузить своего персонажа.\nНадеюсь что во время игры ничего не сломается (что-то всегда ломается) и что вы получите удовольствие.\nУдачи!",
                okText = "Вперед!",
                --allowCancel = false,
            })

    UIManager.showSetupMenu(createCharacter, loadCharacter)
end

return GameManager 