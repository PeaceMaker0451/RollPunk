local FieldDefs = require("field_definitions")
local FieldsServices= require("fields_services")
local WeaponFactory = {}

function WeaponFactory.createBaseWeapon()
    local weapon = ConstructorAPI.createField(FieldDefs.Weapons.weapon_base)
    local image_group = FieldsServices.createAndChild(weapon, FieldDefs.Weapons.Groups.image_group)
    local parameters_group = FieldsServices.createAndChild(image_group, FieldDefs.Weapons.Groups.parameters_group)

    FieldsServices.createAndChild(image_group, FieldDefs.Weapons.image_field)

    FieldsServices.createAndChild(parameters_group, FieldDefs.Weapons.Parameters.name)
    FieldsServices.createAndChild(parameters_group, FieldDefs.Weapons.Parameters.damage)

    return weapon
end

return WeaponFactory