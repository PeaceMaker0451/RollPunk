local FieldsServices = require("fields_services")
local CharacterSubsystem = require("character.subsystems.character_subsystem")
local FieldDefs = require("field_definitions")

local InventorySubsystem = {}

local function addItem(item)
    local item_reference = ConstructorAPI.createField(FieldDefs.Character.Inventory.inventory_item_reference_sample)
    item_reference.setName(item.id)
    item_reference.setVisibleName(item.getField(FieldDefs.Character.Inventory.item_name_field_name).getValue())
end

local function verifyIsItem(item)
    item.getAdditionalDataField(FieldDefs.)
    
    item.getField(FieldDefs.Character.Inventory.item_name_field_name)
end

function InventorySubsystem.initialize(character)
    if not CharacterSubsystem.isCreated(character, "InventorySubsystem") then
        InventorySubsystem.create(character)
        CharacterSubsystem.markAsCreated(character, "InventorySubsystem")
    end
end

function InventorySubsystem.create(character)
    RollPunkAPI.log("Создание InventorySubsystem")

    local parameters_group = character.getField(FieldDefs.Character.Groups.parameters_group.name)
    local inventory_group = FieldsServices.createAndChild(parameters_group, FieldDefs.Character.Inventory.inventory_group)

    character.addRule(FieldDefs.Character.Inventory.Rules.add_item)
    character.addRule(FieldDefs.Character.Inventory.Rules.remove_item)

    character.addRule(FieldDefs.Character.Inventory.Rules.add_item_ui)
    FieldsServices.createAndChild(parameters_group, FieldDefs.Character.Inventory.RuleFields.add_item)
    character.addRule(FieldDefs.Character.Inventory.Rules.remove_item_ui)
    FieldsServices.createAndChild(parameters_group, FieldDefs.Character.Inventory.RuleFields.remove_item)
end

local function _onAddItem(item_id)
    
end

local function _onRemoveItem(item_id)
    
end

local function _onAddItem_UI()
    
end

local function _onRemoveItem_UI()
    
end

ModHookerAPI.addHook(FieldDefs.Character.Inventory.Rules.add_item.hook, _onAddItem)
ModHookerAPI.addHook(FieldDefs.Character.Inventory.Rules.remove_item.hook, _onRemoveItem)
ModHookerAPI.addHook(FieldDefs.Character.Inventory.Rules.add_item_ui.hook, _onAddItem_UI)
ModHookerAPI.addHook(FieldDefs.Character.Inventory.Rules.remove_item_ui.hook, _onRemoveItem_UI)

return InventorySubsystem