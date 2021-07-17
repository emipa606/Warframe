using System;
using HarmonyLib;
using Verse;

namespace Warframe
{
    [HarmonyPatch(typeof(Pawn_HealthTracker), "ShouldBeDead", new Type[]
    {
    })]
    public static class Harmony_ShouldDie
    {
        public static void Postfix(Pawn_HealthTracker __instance, ref bool __result)
        {
            var traverse = Traverse.Create(__instance);
            var pawn = traverse.Field("pawn").GetValue<Pawn>();
            if (!pawn.IsWarframe())
            {
                return;
            }

            var num = WarframeStaticMethods.GetHP(pawn);
            __result = WarframeStaticMethods.ShouldDie(num, pawn);
        }
    }
}