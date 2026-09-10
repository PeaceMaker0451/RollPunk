local FieldDefs = require("field_definitions")
local CommonCharacterConstructor = require('character.common_character_constructor')
local OwnershipManager = require('character.character_ownership_manager')

local CharacterFactory = {}

function CharacterFactory.createCharacter()
    RollPunkAPI.log("Создание нового персонажа...")   
    local character_field = ConstructorAPI.createField(FieldDefs.Character.character_entity_data)
    CharacterFactory.handleCharacter(character_field)
    return character_field
end

function CharacterFactory.handleCharacter(character)
    RollPunkAPI.log("Обработка систем для поля персонажа " .. character.id .. "...")
    CommonCharacterConstructor.initialize(character)
end

return CharacterFactory
