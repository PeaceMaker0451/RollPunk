---@class CharacterSubsystem
---@field name string
---@field character EntityFieldAPI
local CharacterSubsystem = {}
CharacterSubsystem.__index = CharacterSubsystem

function CharacterSubsystem:markAsCreated()
    self.character.setAdditionalDataField("is_" .. self.name .. "_created", true)
end

function CharacterSubsystem:isCreated()
    local is_created = self.character.getAdditionalDataField("is_" .. self.name .. "_created")
    if is_created == nil or type(is_created) ~= "boolean" then
        return false
    else
        return is_created
    end
end

---@param character EntityFieldAPI
function CharacterSubsystem:new(name, character)
    local instance = setmetatable({}, self)
    instance.character = character
    instance.name = name

    return instance
end

function CharacterSubsystem.initialize(self)
end

function CharacterSubsystem.validate(self, updatedField)
end

return CharacterSubsystem