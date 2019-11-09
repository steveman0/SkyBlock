using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using UnityEngine;

//[HarmonyPatch(typeof(SurvivalSurfaceGenerator), "GenerateCubes")]
public static class SurvivalSurfaceGeneratorMod
{
    //[HarmonyPrefix]
    public static bool Prefix(Segment s, PerlinCache cache)
    {
        long realCubeX = s.baseX;
        long realCubeZ = s.baseZ;

        long offsetX = realCubeX - WorldScript.mDefaultOffset;
        long offsetZ = realCubeZ - WorldScript.mDefaultOffset;

        if (Math.Abs(offsetX) < 17 && Math.Abs(offsetZ) < 17)
        {
            return true;
        }
        return false;
    }

    public static void Suffix(Segment s, PerlinCache cache, ushort[,]  ___SurfaceValCache)
    {
        for (int x = 0; x < WorldHelper.SegmentX + TerrainGenerator.doublePerlinBorder; x++)
        {
            for (int z = 0; z < WorldHelper.SegmentZ + TerrainGenerator.doublePerlinBorder; z++)
            {
                for (int y = 0; y < WorldHelper.SegmentY + TerrainGenerator.doublePerlinBorder; y++)
                {
                    long realCubeX = s.baseX + x - PerlinCache.STANDARD_PERLIN_BORDER;
                    long realCubeZ = s.baseZ + z - PerlinCache.STANDARD_PERLIN_BORDER;

                    long offsetX = realCubeX - WorldScript.mDefaultOffset;
                    long offsetZ = realCubeZ - WorldScript.mDefaultOffset;

                    // Pythag
                    float offsetSquared = Mathf.Pow(offsetX, 2) + Mathf.Pow(offsetZ, 2);

                    if (offsetSquared > 100)
                    {
                        cache.maCubes[x, y, z] = eCubeTypes.Air;
                        cache.maCubeValues[x, y, z] = 0;
                    }
                    else
                    {
                        // Top layer is organic detritus with several meters of rock
                        if (y == WorldHelper.SegmentY + 1)
                        {
                            cache.maCubes[x, y, z] = eCubeTypes.NewSurvivalSurface;
                            cache.maCubeValues[x, y, z] = ___SurfaceValCache[x, z];
                        }
                        else if (y > 8 && y < WorldHelper.SegmentY + 1)
                        {
                            cache.maCubes[x, y, z] = eCubeTypes.RoughHewnRock;
                        }
                        else
                        {
                            cache.maCubes[x, y, z] = eCubeTypes.Air;
                        }
                    }
                }
            }
        }
    }
}
