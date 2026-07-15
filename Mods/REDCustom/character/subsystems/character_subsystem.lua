---@class CharacterSubsystem
local CharacterSubsystem = {}

function CharacterSubsystem.markAsCreated(character, subsystem_name)
    character.setAdditionalDataField("is_" .. subsystem_name .. "_created", true)
end

function CharacterSubsystem.isCreated(character, subsystem_name)
    local is_created = character.getAdditionalDataField("is_" .. subsystem_name .. "_created")
    if is_created == nil or type(is_created) ~= "boolean" then
        return false
    else
        return is_created
    end
end

return CharacterSubsystem
