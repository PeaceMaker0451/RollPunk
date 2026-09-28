local FieldsServices = require("fields_services")
local CharacterSubsystem = require("character.subsystems.character_subsystem")
local FieldDefs = require("field_definitions")

local InventorySubsystem = {}

local function getItemName(item)
    local name_field = item.getField(FieldDefs.Character.Inventory.item_name_field_name)

    if name_field == nil then
        return nil 
    end

    return name_field.getValue()
end

local function addItem(item, character)
    local item_reference = ConstructorAPI.createField(FieldDefs.Character.Inventory.inventory_item_reference_sample)
    item_reference.setName(item.id)
    item_reference.setVisibleName(getItemName(item))
    SessionAPI.OwnersRegistry.addEntityOwner(item, SessionAPI.current_player)

    local inventory_group = character.getField(FieldDefs.Character.Inventory.inventory_group.name)
    inventory_group.addField(item_reference)
end

local function verifyIsItem(item)
    local tag = item.getAdditionalDataField(FieldDefs.Character.Inventory.takeable_item_tag_name)
    local name_field = item.getField(FieldDefs.Character.Inventory.item_name_field_name)

    if name_field == nil then
        return false
    else 
        return true
    end
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

    character.addRule(ConstructorAPI.createRule(FieldDefs.Character.Inventory.Rules.add_item))
    character.addRule(ConstructorAPI.createRule(FieldDefs.Character.Inventory.Rules.remove_item))

    character.addRule(ConstructorAPI.createRule(FieldDefs.Character.Inventory.Rules.add_item_ui))
    FieldsServices.createAndChild(parameters_group, FieldDefs.Character.Inventory.RuleFields.add_item)
    character.addRule(ConstructorAPI.createRule(FieldDefs.Character.Inventory.Rules.remove_item_ui))
    FieldsServices.createAndChild(parameters_group, FieldDefs.Character.Inventory.RuleFields.remove_item)
end

local function _onAddItem(character, item_id)
    local item = SessionAPI.getEntityById(item_id)

    if item == nil then
        RollPunkAPI.logError("Попытка добавить с помощью правила предмет с некорректным ID")
        return
    end

    addItem(item, character)
end

local function _onRemoveItem(character, item_id)
    
end

local function _onAddItem_UI(character)
    local items = {}
    
    for _, entity in ipairs(SessionAPI.entities) do
        if verifyIsItem(entity) then
            table.insert(items, entity)
        end
    end

    if #items == 0 then
        FormsAPI.Dialogs.showInformationDialog("Подбор предмета", {message = "В сцене нет свободных предметов!"})
        return
    end
    
    local content = FormsAPI.Dialogs.showUIDocumentDialog("Подбор предмета")
    
    for _, item in ipairs(items) do
        local item_name = getItemName(item)
        
        content.Document.addButton(item_name, function ()
            character.getRule(FieldDefs.Character.Inventory.Rules.add_item.name).execute({item.id})
            content.close()
        end)
    end
end

local function _onRemoveItem_UI(character)
    
end

ModHookerAPI.addHook(FieldDefs.Character.Inventory.Rules.add_item.hook, _onAddItem)
ModHookerAPI.addHook(FieldDefs.Character.Inventory.Rules.remove_item.hook, _onRemoveItem)
ModHookerAPI.addHook(FieldDefs.Character.Inventory.Rules.add_item_ui.hook, _onAddItem_UI)
ModHookerAPI.addHook(FieldDefs.Character.Inventory.Rules.remove_item_ui.hook, _onRemoveItem_UI)

return InventorySubsystem