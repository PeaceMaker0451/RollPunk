---@meta

---@class ModHookerAPI
---@field addHook fun(hookName:string, luaFunction:fun())

---@type ModHookerAPI
ModHookerAPI = ModHookerAPI

---@class ConstructorAPI
---@field createField fun(fieldData:FieldData):FieldAPI
---@field createRule fun(ruleData:RuleData):RuleAPI

---@type ConstructorAPI
ConstructorAPI = ConstructorAPI

---@class RollPunkAPI
---@field log fun(logData:string)
---@field logError fun(logData:string)
---@field public print fun(logData:string)

---@type RollPunkAPI
RollPunkAPI = RollPunkAPI

---@class SessionAPI
---@field OwnersRegistry OwnersRegistryAPI
---@field current_player PlayerAPI
---@field addEntityField fun(field:EntityFieldAPI)
---@field removeEntityField fun(field:EntityFieldAPI)

---@type SessionAPI
SessionAPI = SessionAPI

---@class UIAPI
---@field openStringDialogue fun(title:string, callback:function, parameter1:any?, parameter2:any?, parameter3:any?)
---@field openIntDialogue fun(title:string, callback:function, parameter1:any?, parameter2:any?, parameter3:any?)
---@field openInformationDialogue fun(title:string, message:string, callback:function, parameter1:any?, parameter2:any?, parameter3:any?)

---@type UIAPI
UIAPI = UIAPI

---@class OwnersRegistryAPI
---@field setEntityOwner fun(entity:EntityFieldAPI, player:PlayerAPI)
---@field removeEntityOwner fun(entity:EntityFieldAPI, player:PlayerAPI)

---@class PlayerAPI
---@field name string,
---@field player_id string,
---@field is_admin boolean

---@class FieldAPI
---@field name string
---@field id string
---@field parent FieldAPI
---@field children FieldAPI[]
---@field setAdditionalDataField fun(name:string, value:any)
---@field getAdditionalDataField fun(name:string):any
---@field addField fun(field:FieldAPI)
---@field removeField fun(field:FieldAPI):boolean
---@field getField fun(name:string):FieldAPI

---@class ValueFieldAPI : FieldAPI
---@field getValue fun():any

---@class EntityFieldAPI : FieldAPI
---@field addRule fun(rule:RuleAPI)

---@class LineFieldAPI : ValueFieldAPI
---@field line_priority integer 
---@field view_access_level integer
---@field edit_access_level integer
---@field setLinePriority fun(priority:integer)
---@field setViewAccessLevel fun(newLevel:integer)
---@field setEditAccessLevel fun(newLevel:integer)

---@class RuleFieldAPI : LineFieldAPI
---@field rule_name string,
---@field execute fun()

---@class FieldGroupAPI : LineFieldAPI
---@field addField fun(field:LineFieldAPI)
---@field removeField fun(field:LineFieldAPI) : boolean
---@field getField fun(name:string):LineFieldAPI
---@field getFields fun():LineFieldAPI[]
---@field getFieldByID fun(d:string):LineFieldAPI

---@class RuleAPI
---@field name string
---@field description string
---@field hook string
---@field id string
---@field handler any
---@field arguments string[]
---@field return_parameters string[]
---@field execute fun()

---@alias FieldType "BoolField"|"IntField"|"StringField"|"FieldsGroup"

---@class FieldData
---@field name string
---@field type FieldType
---@field additional_data table?

---@class EntityFieldData : FieldData
---@field type "EntityField"

---@class LineFieldData : FieldData
---@field visible_name string?
---@field line_priority integer?
---@field edit_access_level integer?
---@field view_access_level integer?

---@class IntFieldData : LineFieldData
---@field type "IntField"
---@field value integer
---@field min_value integer?
---@field max_value integer?

---@class StringFieldData : LineFieldData
---@field type "StringField"
---@field value string

---@class BoolFieldData : LineFieldData
---@field type "BoolField"
---@field value boolean

---@class RuleFieldData : LineFieldData
---@field type "RuleField"
---@field rule_name string

---@class FieldsGroupData : LineFieldData
---@field type "FieldsGroup"

---@class RuleData
---@field type "Rule",
---@field name string,
---@field hook string
