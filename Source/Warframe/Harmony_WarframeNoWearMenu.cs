using HarmonyLib;
using RimWorld;
using Verse;

namespace Warframe
{
    [HarmonyPatch(typeof(ReachabilityUtility), "CanReach")]
    public static class Harmony_WarframeNoWearMenu
    {
        public static bool Prefix(Pawn pawn, LocalTargetInfo dest, ref bool __result)
        {
            if (!pawn.IsWarframe() || pawn.Downed)
            {
                return true;
            }

            if (dest.Thing is not Apparel)
            {
                return true;
            }

            __result = false;
            return false;
        }
    }
}