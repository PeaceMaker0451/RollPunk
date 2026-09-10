local FieldsServices = require("fields_services")
local FieldDefs = require("field_definitions")

local PlayerRegistry = {}

local function getOrCreateRegistry()
        local registry = SessionAPI.getEntity(FieldDefs.PlayerRegistry.registry.name)

    if(registry == nil) then        
        registry = ConstructorAPI.createField(FieldDefs.PlayerRegistry.registry)
        SessionAPI.addEntity(registry)
    end

    return registry
end

local function getOrCreatePlayerGroup(player_id)
    local registry = getOrCreateRegistry()

    local player_group = registry.getField(player_id)

    if player_group == nil then
        player_group = ConstructorAPI.createField(FieldDefs.PlayerRegistry.player_group)
        RollPunkAPI.log("id " .. player_id)
        player_group.setName(player_id)
        registry.addField(player_group)
    end

    return player_group
end

local function getOrCreateCharacterRecord(group)
    local record = group.getField(FieldDefs.PlayerRegistry.character.name)

    if record == nil then
        record = FieldsServices.createAndChild(group, FieldDefs.PlayerRegistry.character)
    end

    return record
end

function PlayerRegistry.addOrUpdateCharacterRecord(player, character)
    local registry = getOrCreatePlayerGroup(player.client_id)
    local character_record = getOrCreateCharacterRecord(registry)
    character_record.setValue(character.id)
end

function PlayerRegistry.getPlayersCharacterId(player)
    local registry = getOrCreatePlayerGroup(player.client_id)
    local character_record = getOrCreateCharacterRecord(registry)
    return character_record.getValue()
end

function PlayerRegistry.getCharactersPlayerId(character)
    local registry = getOrCreateRegistry()

    for key, value in pairs(registry.children) do
        local record = getOrCreateCharacterRecord(value)

        if record.getValue() == character.id then
            return record.name
        end
    end
end

function PlayerRegistry.isCurrentCharacter(character)
    local current_player = SessionAPI.current_player
    local current_character_id = PlayerRegistry.getPlayersCharacterId(current_player)
    return current_character_id == character.id
end

return PlayerRegistry