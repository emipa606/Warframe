using System;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Warframe
{
    [HarmonyPatch(typeof(Targeter), "TargeterUpdate")]
    public static class Harmony_AddRadius
    {
        public static void Postfix(Targeter __instance)
        {
            var traverse = Traverse.Create(__instance);

            var pawn = traverse.Field("caster").GetValue<Pawn>();
            var fiaction = traverse.Field("actionWhenFinished").GetValue<Action>();
            if (fiaction == null)
            {
                return;
            }

            if (__instance.IsTargeting && pawn != null && pawn.IsWarframe())
            {
                fiaction();
            }
        }
    }
}