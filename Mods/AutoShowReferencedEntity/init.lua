ModHookerAPI.addHook("ReferenceFieldSelected", function (reference)
        SessionAPI.PlayerSpace.setDisplayedEntity(reference.getEntity())
    end)