local FieldsServices = require("fields_services")
local CharacterSubsystem = require("character.subsystems.character_subsystem")
local FieldDefs = require("character.field_definitions")

local PsychoSubsystem = {}

local _humanity_factor = 5
local _max_emp_value = 20 

local function _handleOuterEMPChange(character)
    local emp_field = character.getField(FieldDefs.Stats.emp.name)
    local empValue = emp_field.getValue()
    local realEmpValue = emp_field.getAdditionalDataField(FieldDefs.AdditionalDataKeys.real_emp_value)
    local oldEmpValue = emp_field.getAdditionalDataField(FieldDefs.AdditionalDataKeys.old_emp_value)
    local empDelta = empValue - oldEmpValue

    if empDelta == 0 then
        return
    end

    local newRealEmpValue = realEmpValue + empDelta

    emp_field.setAdditionalDataField(FieldDefs.AdditionalDataKeys.old_emp_value, empValue)
    emp_field.setAdditionalDataField(FieldDefs.AdditionalDataKeys.real_emp_value, newRealEmpValue)
end

local function _updateHumanity(character)
    local emp_field = character.getField(FieldDefs.Stats.emp.name)
    local empRealValue = emp_field.getAdditionalDataField(FieldDefs.AdditionalDataKeys.real_emp_value)
    local totalHumanity = empRealValue * _humanity_factor

    local humanity_field = character.getField(FieldDefs.Psycho.humanity.name)
    local humanity_loss_field = character.getField(FieldDefs.Psycho.humanity_loss.name)
    local max_humanity_loss_field = character.getField(FieldDefs.Psycho.humanity_max_loss.name)
    
    local humanityLossValue = humanity_loss_field.getValue()
    local humanityMaxLossValue = max_humanity_loss_field.getValue()
    local newHumanityMaxValue = totalHumanity - humanityMaxLossValue
    local newHumanityValue = totalHumanity - humanityLossValue
    
    humanity_field.setMaxValue(newHumanityMaxValue)
    humanity_field.setValue(newHumanityValue)
end

local function _updateEMPFromHumanity(character)
    local humanity_field = character.getField(FieldDefs.Psycho.humanity.name)
    local emp_field = character.getField(FieldDefs.Stats.emp.name)
    local humanityValue = humanity_field.getValue()

    emp_field.setMaxValue(_max_emp_value + 1)

    local humanityOffset = 300
    local EmpOffset = humanityOffset / _humanity_factor

    local newEmpValue = math.floor((humanityValue + humanityOffset) / _humanity_factor)
    local newFinalEmpValue = newEmpValue - EmpOffset

    emp_field.setValue(newFinalEmpValue)
end

local function _handleInnerEMPChange(character)
    local emp_field = character.getField(FieldDefs.Stats.emp.name)
    local realEmpValue = emp_field.getAdditionalDataField(FieldDefs.AdditionalDataKeys.real_emp_value)
    local empValue = emp_field.getValue()
    
    local realFromCurrentEmpDelta = realEmpValue - empValue
    local newMaxEmpValue = _max_emp_value - realFromCurrentEmpDelta
    local newMinEmpValue = 1 - realFromCurrentEmpDelta
    
    emp_field.setMaxValue(newMaxEmpValue)
    emp_field.setMinValue(newMinEmpValue)

    emp_field.setAdditionalDataField(FieldDefs.AdditionalDataKeys.old_emp_value, empValue)
end

local function _setHumanityLossOverMaxLoss(character)
    local humanity_loss_field = character.getField(FieldDefs.Psycho.humanity_loss.name)
    local max_humanity_loss_field = character.getField(FieldDefs.Psycho.humanity_max_loss.name)
    local humanityMaxLossValue = max_humanity_loss_field.getValue()
    humanity_loss_field.setMinValue(humanityMaxLossValue)
end

local function _updatePsychoPointsMax(character)
    local emp_field = character.getField(FieldDefs.Stats.emp.name)
    local psycho_points_field = character.getField(FieldDefs.Psycho.psycho_points.name)
    local emp = emp_field.getValue()
    local psycho_max = ((emp * emp) / 6) + 4

    if psycho_max < 0 then
        psycho_max = 0
    end

    psycho_points_field.setMaxValue(psycho_max)
end

local function _removeHumanityForPsychoPoints(character, psycho_points)
    local humanity_loss_field = character.getField(FieldDefs.Psycho.humanity_loss.name)
    humanity_loss_field.setValue(humanity_loss_field.getValue() + (psycho_points * 2))
end

local function _upgradeTo051(character)
    RollPunkAPI.log("Обновление псих-подсистемы до версии 0.5.1...")
    
    local psychoButton = character.getField(FieldDefs.RuleFields.spend_psycho_points.name)
    psychoButton.setRuleName(FieldDefs.Rules.spend_psycho_points.name)

    local psychoRule = character.getRule(FieldDefs.Rules.spend_psycho_points.name)
    psychoRule.setHook(FieldDefs.Rules.spend_psycho_points.hook)
end

function PsychoSubsystem.initialize(character)
    if not CharacterSubsystem.isCreated(character, "PsychoSubsystem") then
        PsychoSubsystem.create(character)
        CharacterSubsystem.markAsCreated(character, "PsychoSubsystem")
    else
        PsychoSubsystem.connect(character)
    end
    
    _handleOuterEMPChange(character)
    _setHumanityLossOverMaxLoss(character)
    _updateHumanity(character)
    _updateEMPFromHumanity(character)
    _handleInnerEMPChange(character)
    _updatePsychoPointsMax(character)
end

function PsychoSubsystem.create(character)
    RollPunkAPI.log("Создание PsychoSubsystem")
    
    local parameters_group = character.getField(FieldDefs.Groups.parameters_group.name)
    local character_group = character.getField(FieldDefs.Groups.character_group.name)
    local action_group = character.getField(FieldDefs.Groups.action_group.name)
    
    FieldsServices.createAndChild(parameters_group, FieldDefs.Psycho.humanity)
    FieldsServices.createAndChild(parameters_group, FieldDefs.Psycho.humanity_loss)
    FieldsServices.createAndChild(parameters_group, FieldDefs.Psycho.humanity_max_loss)
    FieldsServices.createAndChild(character_group, FieldDefs.Psycho.psycho_points)

    local emp_field = character.getField(FieldDefs.Stats.emp.name)
    emp_field.setAdditionalDataField(FieldDefs.AdditionalDataKeys.real_emp_value, emp_field.getValue())
    emp_field.setAdditionalDataField(FieldDefs.AdditionalDataKeys.old_emp_value, emp_field.getValue())
    emp_field.setMaxValue(_max_emp_value)

    FieldsServices.createAndChild(action_group, FieldDefs.RuleFields.spend_psycho_points)
    character.addRule(ConstructorAPI.createRule(FieldDefs.Rules.spend_psycho_points))
end

function PsychoSubsystem.connect(character)
    local version = character.getAdditionalDataField("version")

    if version == nil then
        _upgradeTo051(character)
    end
end

function PsychoSubsystem.spendPsychoPoints(character, psycho_points)
    local psycho_points_field = character.getField(FieldDefs.Psycho.psycho_points.name)
    local current_psycho_points = psycho_points_field.getValue()
    
    if current_psycho_points < psycho_points then
       psycho_points_field.setValue(0)
       _removeHumanityForPsychoPoints(character, psycho_points - current_psycho_points)
    else
        psycho_points_field.setValue(psycho_points_field.getValue() - psycho_points)
    end
end

function PsychoSubsystem.validate(character, updatedField)
    local emp_field = character.getField(FieldDefs.Stats.emp.name)
    local humanity_field = character.getField(FieldDefs.Psycho.humanity.name)
    local humanity_loss_field = character.getField(FieldDefs.Psycho.humanity_loss.name)
    local max_humanity_loss_field = character.getField(FieldDefs.Psycho.humanity_max_loss.name)
    
    if updatedField == emp_field then
        RollPunkAPI.log("Обновление псих-подсистемы из-за эмпатии")
        _handleOuterEMPChange(character) 
        _setHumanityLossOverMaxLoss(character)
        _updateHumanity(character)
        _updatePsychoPointsMax(character)
    elseif (updatedField == humanity_field) or (updatedField == humanity_loss_field) or (updatedField == max_humanity_loss_field) then
        RollPunkAPI.log("Обновление псих-подсистемы из-за человечности")
        _updateHumanity(character)
        _setHumanityLossOverMaxLoss(character)
        _updateEMPFromHumanity(character)
        _handleInnerEMPChange(character)
        _updatePsychoPointsMax(character)
    end
end

local function _onSpendPsychoPoints(character)
    UIAPI.openIntDialogue("Введите число псих-очков:", function(result)
        PsychoSubsystem.spendPsychoPoints(character, result)
    end)
end

ModHookerAPI.addHook(FieldDefs.Rules.spend_psycho_points.hook, _onSpendPsychoPoints)

return PsychoSubsystem
