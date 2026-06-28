---@class CharacterSubsystem
---@field name string
local CharacterSubsystem = {}
CharacterSubsystem.__index = CharacterSubsystem

function CharacterSubsystem:markAsCreated(character)
    character.setAdditionalDataField("is_" .. self.name .. "_created", true)
end

function CharacterSubsystem:isCreated(character)
    local is_created = character.getAdditionalDataField("is_" .. self.name .. "_created")
    if is_created == nil or type(is_created) ~= "boolean" then
        return false
    else
        return is_created
    end
end

function CharacterSubsystem.initialize()
end

---@param character EntityFieldAPI
function CharacterSubsystem.create(character)
end

---@param character EntityFieldAPI
function CharacterSubsystem.connect(character)
end

return CharacterSubsystem