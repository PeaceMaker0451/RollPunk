local CommonCharacterConstructor = require('character.common_character_constructor')

local CharacterFactory = {}

local _characters = {}

---@type EntityFieldData
local _character_entity_data =
{
  name = "CP_Charater",
  type = "EntityField",
  additional_data = {type = "CP_Character"}
}

---@return CommonPlayerConstructor
function CharacterFactory.getCharacterConstructor(character)
    return _characters[character]
end

function CharacterFactory.createCharacter()
    RollPunkAPI.log("Создание нового персонажа...")   
    local character_field = ConstructorAPI.createField(_character_entity_data)
    CharacterFactory.handleCharacter(character_field)
    return character_field
end

function CharacterFactory.handleCharacter(character)
    RollPunkAPI.log("Обработка систем для поля персонажа " .. character.id .. "...")
    local character_constructor = CommonCharacterConstructor:new(character)
    _characters[character] = character_constructor
end

-- function CharacterFactory.initializeCharacter(character)
--     if _characters[character] ~= nil then
--         _characters[character]:initializeCharacter()
--     end
-- end

local function _onValidate(entity_field, field_API)
    if _characters[entity_field] ~= nil then
        RollPunkAPI.log("Валидация поля персонажа " .. entity_field.id .. "...")
        _characters[entity_field]:validateCharacter(field_API)
    end
end

ModHookerAPI.addHook("Validate",_onValidate)

return CharacterFactory