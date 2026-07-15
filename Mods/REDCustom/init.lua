local _characterConfig = require("character.characters_registry")
local _baseActions = require("setup")
local UpdateSubsystem = require("character.subsystems.update_subsystem")

local function _createCharacter()
    local character_field = _characterConfig.createCharacter()
    RollPunkAPI.log("[color=deep_pink]ADDING CHARACTER TO SESSION!!![/color]")

    UpdateSubsystem.setStatsUpdatePoints(character_field, 115, false)
    UpdateSubsystem.setSkillUpdatePoints(character_field, 150, false)

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

local function _loadField()
    SessionAPI.loadString(function(data)
        local field = SerializatorAPI.deserializeField(data)
        SessionAPI.addEntityField(field)
        SessionAPI.OwnersRegistry.setEntityOwner(field, SessionAPI.current_player)
    end)
end

local function _onSessionInitialized()
    RollPunkAPI.log("RollPunk default rules initialization...")
    _baseActions.create()
    
end

_baseActions.initialize(_createCharacter, _loadCharacter, _loadField)
ModHookerAPI.addHook("SessionInitialized", _onSessionInitialized)
