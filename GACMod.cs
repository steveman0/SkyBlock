using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

public static class GACMod
{
    public static bool UpdateLookingForResources(GenericAutoCrafterNew __instance)
    {
        if (__instance.mRecipe.Costs.Count == 0)
        {
            // Harmony caches the reflection internally so it is *supposed* to be fast
            Harmony.Traverse.Create(__instance).Method("SetNewState", GenericAutoCrafterNew.eState.eCrafting).GetValue();

            return false; // We're done skip the rest of the method
        }

        return true;
    }
}
