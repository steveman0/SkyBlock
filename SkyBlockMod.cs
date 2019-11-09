using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Harmony;
using System.Reflection;

public class SkyBlockMod : FortressCraftMod
{
    public override ModRegistrationData Register()
    {
        ModRegistrationData data = new ModRegistrationData();
        PatchMethods();

        BiomeLayer.Layers = new List<BiomeLayer>()
        {
            new BiomeLayer(BiomeLayerType.SkyRealm, 0, 5000, new AirGenerator()),
            new BiomeLayer(BiomeLayerType.Surface, -16, 0, new SurvivalSurfaceGenerator()),
            new BiomeLayer(BiomeLayerType.SkyRealm, -4000, -16, new AirGenerator()),
            new BiomeLayer(BiomeLayerType.MagmaOcean, -4160, -4000, new MagmaOceanGenerator()),
            new BiomeLayer(BiomeLayerType.HotLayers, -5000, -4160, new CaveGenerator()),
            new BiomeLayer(BiomeLayerType.Hell, -5500, -5000, new RockGenerator()),
            new BiomeLayer(BiomeLayerType.BelowHell, -6500, -5500, new RockGenerator()),
            new BiomeLayer(BiomeLayerType.Bedrock, -7000, -6500, new RockGenerator()),
            new BiomeLayer(BiomeLayerType.Void, long.MinValue, -7000, new RockGenerator()),
        };

        Debug.Log("FCE Sky Factory V1 registered");

        return data;
    }

    private static void PatchMethods()
    {
        HarmonyInstance harmony = HarmonyInstance.Create("steveman0.SkyBlockMod");
        // Core Sky Block world gen patch
        var original = typeof(SurvivalSurfaceGenerator).GetMethod("GenerateCubes");
        var prefix = typeof(SurvivalSurfaceGeneratorMod).GetMethod("Prefix");
        var suffix = typeof(SurvivalSurfaceGeneratorMod).GetMethod("Suffix");

        // GAC patch to support empty ingredient lists
        var GACResources = AccessTools.Method(typeof(GenericAutoCrafterNew), "UpdateLookingForResources");
        if (GACResources == null) Debug.LogWarning("GACResources is null");
        var GACResourcePre = typeof(GACMod).GetMethod("UpdateLookingForResources");

        try
        {
            Debug.Log("Sky Block Mod patching SurvivalSurfaceGenerator.");
            harmony.Patch(original, new HarmonyMethod(prefix), new HarmonyMethod(suffix));

            Debug.Log("Sky Block Mod patching GenericAutoCrafterNew.");
            harmony.Patch(GACResources, new HarmonyMethod(GACResourcePre));
        }
        catch (Exception ex)
        {
            if (ex.InnerException != null)
            {
                Debug.LogWarning("Harmony inner exception: " + ex.InnerException.Message);
            }
            else
            {
                Debug.LogWarning("Harmony exception: " + ex.Message);
            }

            throw;
        }
    }
}
