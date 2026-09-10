local FieldServices = {}

---@param parent FieldAPI
---@param field_data LineFieldData
---@return FieldAPI
function FieldServices.createAndChild(parent, field_data)
    if field_data.type == nil or field_data.name == nil  then
        error("FieldData is incorrect!",3)
    end

    if parent == nil then
        error("Parent is nil!",3)
    end

    local field = ConstructorAPI.createField(field_data)
    parent.addField(field)
    return field
end

---@param rule_data RuleData
---@return RuleFieldAPI
function FieldServices.createRuleField(rule_data)
    ---@type RuleFieldData
    local rule_field_data = 
    {
        type = "RuleField",
        name = rule_data.name,
        visible_name = rule_data.visible_name,
        view_access_level = 0,
        edit_access_level = 0,
        rule_name = rule_data.hook
    }
    
    ---@type RuleFieldAPI
    local rule_field = ConstructorAPI.createField(rule_field_data)
    return rule_field
end

return FieldServices