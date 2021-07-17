using System;
using HarmonyLib;
using Verse;

namespace Warframe
{
    [HarmonyPatch(typeof(Pawn), "GetInspectString", new Type[]
    {
    })]
    public static class Harmony_AddLevelShow
    {
        public static void Postfix(Pawn __instance, ref string __result)
        {
            if (__instance.IsWarframe() && !__instance.Dead)
            {
                __result += "\n" + "Lv." + __instance.GetLevel().ToString("f0");
            }
        }
    }
}