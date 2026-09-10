FieldDefs = require("field_definitions")

local Tagger = {}

function Tagger.setCharacterTag(character)
    character.setAdditionalDataField("tag", FieldDefs.Character.player_character_tag)
end

function Tagger.setNpcTag(character)
    character.setAdditionalDataField("tag", FieldDefs.Character.npc_character_tag)
end

function Tagger.setEnemyTag(character)
    character.setAdditionalDataField("tag", FieldDefs.Character.enemy_character_tag)
end

function Tagger.getTag(character)
    return character.getAdditionalDataField("tag")
end

function Tagger.isPlayer(character)
    return Tagger.getTag(character) == FieldDefs.Character.player_character_tag
end

function Tagger.isNpc(character)
    return Tagger.getTag(character) == FieldDefs.Character.npc_character_tag
end

function Tagger.isEnemy(character)
    return Tagger.getTag(character) == FieldDefs.Character.enemy_character_tag
end

return Tagger