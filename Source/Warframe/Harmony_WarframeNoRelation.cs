using System;
using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Warframe
{
    [HarmonyPatch(typeof(Pawn_RelationsTracker), "RelationsTrackerTick", new Type[]
    {
    })]
    public static class Harmony_WarframeNoRelation
    {
        public static bool Prefix(Pawn_RelationsTracker __instance)
        {
            var tv = Traverse.Create(__instance);
            var pawn = tv.Field("pawn").GetValue<Pawn>();
            var unused = tv.Field("directRelations").GetValue<List<DirectPawnRelation>>();
            if (!pawn.IsWarframe())
            {
                return true;
            }

            unused = new List<DirectPawnRelation>();
            return false;
        }
    }
}