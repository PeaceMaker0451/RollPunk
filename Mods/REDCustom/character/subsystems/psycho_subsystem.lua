local FieldsServices = require("fields_services")
local CharacterSubsystem = require("character.subsystems.character_subsystem")

---@class PsychoSubsystem : CharacterSubsystem
---@field humanity_field LineFieldAPI
---@field humanity_loss_field LineFieldAPI
---@field max_humanity_loss_field LineFieldAPI
---@field psycho_points_field LineFieldAPI
---@
---@field emp_field LineFieldAPI
---@field old_emp_value_name string
---@field real_emp_value_name string
---@
---@field character_group_field FieldGroupAPI
---@field parameters_group_field FieldGroupAPI
---@field action_group_field FieldGroupAPI
local PsychoSubsystem = setmetatable({}, CharacterSubsystem)
PsychoSubsystem.__index = PsychoSubsystem

---@type IntFieldData
local psycho_points_field_data =
{
    name = "PsychoPoints",
    visible_name = "Псих-очки",
    type = "IntField",
    value = 10,
    max_value = 100,
    min_value = 0,
    view_access_level = 0,
    edit_access_level = 2,
    line_priority = 60,
    additional_data = { show_max = true, vertical = true  },
}

---@type IntFieldData
local humanity_field_data =
{
    name = "Humanity",
    visible_name = "Человечность",
    type = "IntField",
    value = 0,
    max_value = 100,
    min_value = -100,
    view_access_level = 0,
    edit_access_level = 3,
    line_priority = 50,
    additional_data = { show_max = true },
}

---@type IntFieldData
local humanityt_loss_field_data =
{
    name = "HumanityLoss",
    visible_name = "Потеря человечности",
    type = "IntField",
    value = 0,
    max_value = 100,
    min_value = 0,
    view_access_level = 2,
    edit_access_level = 2,
    line_priority = 48,
    additional_data = { show_max = false },
}

---@type IntFieldData
local humanityt_max_loss_field_data =
{
    name = "HumanityMaxLoss",
    visible_name = "Потеря макс. человечности",
    type = "IntField",
    value = 0,
    max_value = 100,
    min_value = 0,
    view_access_level = 2,
    edit_access_level = 2,
    line_priority = 49,
    additional_data = { show_max = false },
}

---@type RuleData
local _spend_psycho_rule_data =
{
    name = "SpendPsychoPoints",
    type = "Rule",
    hook = "SpendPsychoPoints"
}

---@type RuleFieldData
local _spend_psycho_rule_field_data =
{
    name = "SpendPsychoPointsField",
    rule_name = _spend_psycho_rule_data.name,
    visible_name = "Использовать Псих-очки",
    type = "RuleField",
    line_priority = 67,
}

local _humanity_factor = 5
local _max_emp_value = 20 

---@param self PsychoSubsystem
local function _addRuleHooks(self)
    ModHookerAPI.addHook(_spend_psycho_rule_data.hook,function (character)
        if self == nil or character ~= self.character then
            return
        end

        UIAPI.openIntDialogue("Введите число псих-очков:", function(result)
            PsychoSubsystem.spendPsychoPoints(self,  result)
        end)
    end)
end

---@param self PsychoSubsystem
local function _handleOuterEMPChange(self)
    local empValue = self.emp_field.getValue()
    local realEmpValue = self.emp_field.getAdditionalDataField(self.real_emp_value_name)
    local oldEmpValue = self.emp_field.getAdditionalDataField(self.old_emp_value_name)
    local empDelta = empValue - oldEmpValue

    if empDelta == 0 then
        return
    end

    local newRealEmpValue = realEmpValue + empDelta

    self.emp_field.setAdditionalDataField(self.old_emp_value_name, empValue)
    self.emp_field.setAdditionalDataField(self.real_emp_value_name, newRealEmpValue)
end

---@param self PsychoSubsystem
local function _updateHumanity(self)
    local empRealValue = self.emp_field.getAdditionalDataField(self.real_emp_value_name)
    local totalHumanity = empRealValue * _humanity_factor

    local humanityLossValue = self.humanity_loss_field.getValue()
    local humanityMaxLossValue = self.max_humanity_loss_field.getValue()
    local newHumanityMaxValue = totalHumanity - humanityMaxLossValue
    local newHumanityValue = totalHumanity - humanityLossValue
    
    self.humanity_field.setMaxValue(newHumanityMaxValue)
    self.humanity_field.setValue(newHumanityValue)
end

---@param self PsychoSubsystem
local function _updateEMPFromHumanity(self)
    local humanityValue = self.humanity_field.getValue()

    self.emp_field.setMaxValue(_max_emp_value + 1)

    local humanityOffset = 300
    local EmpOffset = humanityOffset / _humanity_factor

    local newEmpValue = math.floor((humanityValue + humanityOffset) / _humanity_factor)
    local newFinalEmpValue = newEmpValue - EmpOffset

    self.emp_field.setValue(newFinalEmpValue)
end

---@param self PsychoSubsystem
local function _handleInnerEMPChange(self)
    local realEmpValue = self.emp_field.getAdditionalDataField(self.real_emp_value_name)
    local empValue = self.emp_field.getValue()
    
    local realFromCurrentEmpDelta = realEmpValue - empValue
    local newMaxEmpValue = _max_emp_value - realFromCurrentEmpDelta
    local newMinEmpValue = 1 - realFromCurrentEmpDelta
    
    self.emp_field.setMaxValue(newMaxEmpValue)
    self.emp_field.setMinValue(newMinEmpValue)

    self.emp_field.setAdditionalDataField(self.old_emp_value_name, empValue)
end

---@param self PsychoSubsystem
local function _setHumanityLossOverMaxLoss(self)
    local humanityMaxLossValue = self.max_humanity_loss_field.getValue()
    self.humanity_loss_field.setMinValue(humanityMaxLossValue)
end

---@param self PsychoSubsystem
local function _updatePsychoPointsMax(self)
    local emp = self.emp_field.getValue()
    local psycho_max = ((emp * emp) / 6) + 4

    if psycho_max < 0 then
        psycho_max = 0
    end

    self.psycho_points_field.setMaxValue(psycho_max)
end

---@param self PsychoSubsystem
local function _removeHumanityForPsychoPoints(self, psycho_points)
    self.humanity_loss_field.setValue(self.humanity_loss_field.getValue() + (psycho_points * 2))
end

function PsychoSubsystem:_create()
    self.humanity_field = FieldsServices.createAndChild(self.parameters_group_field, humanity_field_data)
    self.humanity_loss_field = FieldsServices.createAndChild(self.parameters_group_field, humanityt_loss_field_data)
    self.max_humanity_loss_field = FieldsServices.createAndChild(self.parameters_group_field, humanityt_max_loss_field_data)
    
    self.psycho_points_field = FieldsServices.createAndChild(self.character_group_field, psycho_points_field_data)

    self.emp_field.setAdditionalDataField(self.real_emp_value_name, self.emp_field.getValue())
    self.emp_field.setAdditionalDataField(self.old_emp_value_name, self.emp_field.getValue())
    self.emp_field.setMaxValue(_max_emp_value)

    FieldsServices.createAndChild(self.action_group_field, _spend_psycho_rule_field_data)
    self.character.addRule(ConstructorAPI.createRule(_spend_psycho_rule_data))
end

function PsychoSubsystem:_connect()
    self.humanity_field = self.character.getField(humanity_field_data.name)
    self.humanity_loss_field = self.character.getField(humanityt_loss_field_data.name)
    self.max_humanity_loss_field = self.character.getField(humanityt_max_loss_field_data.name)
    
    self.psycho_points_field = self.character.getField(psycho_points_field_data.name)
end

function PsychoSubsystem:new(character, action_group, parameters_group, character_group, emp_field)
    ---@type PsychoSubsystem
    local instance = CharacterSubsystem.new(self, "PsychoSubsystem", character)    
    
    instance.parameters_group_field = parameters_group
    instance.character_group_field = character_group
    instance.action_group_field = action_group
    instance.emp_field = emp_field

    instance.old_emp_value_name = "old_emp_value"
    instance.real_emp_value_name = "real_emp_value"

    if instance:isCreated() == false then
        instance:_create()
        instance:markAsCreated()
    else
        instance:_connect()
    end

    _handleOuterEMPChange(instance)
    _setHumanityLossOverMaxLoss(instance)
    _updateHumanity(instance)
    _updateEMPFromHumanity(instance)
    _handleInnerEMPChange(instance)
    _updatePsychoPointsMax(instance)

    _addRuleHooks(instance)

    return instance  
end

function PsychoSubsystem:spendPsychoPoints(psycho_points)
    local current_psycho_points = self.psycho_points_field.getValue()
    
    if current_psycho_points < psycho_points then
       self.psycho_points_field.setValue(0)
       _removeHumanityForPsychoPoints(self, psycho_points - current_psycho_points)
    else
        self.psycho_points_field.setValue(self.psycho_points_field.getValue() - psycho_points)
    end
end

function PsychoSubsystem:validate(updatedField)
    if updatedField == self.emp_field then
        RollPunkAPI.log("Обновление псих-подсистемы из-за эмпатии")
        _handleOuterEMPChange(self) 
        _setHumanityLossOverMaxLoss(self)
        _updateHumanity(self)
        _updatePsychoPointsMax(self)
    elseif (updatedField == self.humanity_field) or (updatedField == self.humanity_loss_field) or (updatedField == self.max_humanity_loss_field) then
        RollPunkAPI.log("Обновление псих-подсистемы из-за человечности")
        _updateHumanity(self)
        _setHumanityLossOverMaxLoss(self)
        _updateEMPFromHumanity(self)
        _handleInnerEMPChange(self)
        _updatePsychoPointsMax(self)
    end
end

return PsychoSubsystem
