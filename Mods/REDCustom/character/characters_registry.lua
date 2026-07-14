local CommonCharacterConstructor = require('character.common_character_constructor')

local CharacterFactory = {}

---@type EntityFieldData
local _character_entity_data =
{
  name = "New Character",
  type = "EntityField",
  additional_data = {type = "CP_Character"}
}

function CharacterFactory.createCharacter()
    RollPunkAPI.log("Создание нового персонажа...")   
    local character_field = ConstructorAPI.createField(_character_entity_data)
    CharacterFactory.handleCharacter(character_field)
    return character_field
end

function CharacterFactory.handleCharacter(character)
    RollPunkAPI.log("Обработка систем для поля персонажа " .. character.id .. "...")
    CommonCharacterConstructor.initialize(character)
end

local function _onValidate(entity_field, field_API)
    -- Проверяем, что это персонаж по типу
    if entity_field.getAdditionalDataField("type") == "CP_Character" then
        RollPunkAPI.log("Валидация поля персонажа " .. entity_field.id .. "...")
        CommonCharacterConstructor.validate(entity_field, field_API)
    end
end

ModHookerAPI.addHook("Validate",_onValidate)

return CharacterFactory
