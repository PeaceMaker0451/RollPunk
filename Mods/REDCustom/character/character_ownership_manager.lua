local PlayerRegistry = require("players.player_registry_controller")

local OwnershipManager = {}

function OwnershipManager.PinCharacterToPlayer(character, player)
    SessionAPI.OwnersRegistry.addEntityOwner(character, player)
    PlayerRegistry.addOrUpdateCharacterRecord(player, character)
end

function OwnershipManager.PinItemToPlayer(item, player)
    
end

return OwnershipManager
