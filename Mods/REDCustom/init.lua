local _characterConfig = require("character.characters_registry")
local _baseActions = require("setup")

local function _createCharacter()
    local character_field = _characterConfig.createCharacter()
    RollPunkAPI.log("[color=deep_pink]ADDING CHARACTER TO SESSION!!![/color]")

    _characterConfig.getCharacterConstructor(character_field).update_subsystem:setStatsUpdatePoints(115, false)
    _characterConfig.getCharacterConstructor(character_field).update_subsystem:setSkillUpdatePoints(150, false)

    SessionAPI.addEntityField(character_field)
    SessionAPI.OwnersRegistry.setEntityOwner(character_field, SessionAPI.current_player)
end

local function _loadCharacter()
    SessionAPI.loadString(function(data)
        local field = SerializatorAPI.deserializeField(data)
        RollPunkAPI.log("[color=deep_pink]ADDING CHARACTER TO SESSION!!![/color]")
        _characterConfig.handleCharacter(field)
        SessionAPI.addEntityField(field)
        SessionAPI.OwnersRegistry.setEntityOwner(field, SessionAPI.current_player)
    end)
end

local function _onSessionInitialized()
    RollPunkAPI.log("RollPunk default rules initialization...")
    _baseActions.setup(_createCharacter, _loadCharacter)
end

ModHookerAPI.addHook("SessionInitialized", _onSessionInitialized)
