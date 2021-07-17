using HarmonyLib;
using RimWorld;
using Verse;

namespace Warframe
{
    [HarmonyPatch(typeof(Pawn_InteractionsTracker), "TryInteractWith")]
    public static class Harmony_WarframeNoTalk
    {
        public static bool Prefix(Pawn_InteractionsTracker __instance, Pawn recipient, ref bool __result)
        {
            var tv = Traverse.Create(__instance);
            var pawn = tv.Field("pawn").GetValue<Pawn>();
            if (!pawn.IsWarframe() && !recipient.IsWarframe())
            {
                return true;
            }

            __result = true;
            return false;
        }
    }
}