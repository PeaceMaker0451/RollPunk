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

return FieldServices