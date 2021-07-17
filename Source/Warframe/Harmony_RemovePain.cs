using System;
using HarmonyLib;
using Verse;

namespace Warframe
{
    [HarmonyPatch(typeof(HediffSet), "CalculatePain", new Type[]
    {
    })]
    public static class Harmony_RemovePain
    {
        public static void Postfix(HediffSet __instance, ref float __result)
        {
            var tv = Traverse.Create(__instance);
            var pawn = tv.Field("pawn").GetValue<Pawn>();
            if (pawn.IsWarframe())
            {
                // HealthUtility.AdjustSeverity(pawn, hediff, -0.00033333333f);
                __result = 0f;
            }
        }
    }
}