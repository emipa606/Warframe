using System;
using HarmonyLib;
using Verse;

namespace Warframe
{
    [HarmonyPatch(typeof(Pawn_HealthTracker), "DropBloodFilth", new Type[]
    {
    })]
    public static class Harmony_NoBloodDrop
    {
        public static bool Prefix(Pawn_HealthTracker __instance)
        {
            var traverse = Traverse.Create(__instance);
            var pawn = traverse.Field("pawn").GetValue<Pawn>();
            return !pawn.IsWarframe();
        }
    }
}