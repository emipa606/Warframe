using HarmonyLib;
using RimWorld;
using Verse;

namespace Warframe
{
    [HarmonyPatch(typeof(ThinkNode_ConditionalNeedPercentageAbove), "Satisfied", typeof(Pawn))]
    public static class Harmony_RemoveSatisThink
    {
        public static bool Prefix(Pawn pawn, bool __result)
        {
            if (!pawn.IsWarframe())
            {
                return true;
            }

            __result = true;
            return false;
        }
    }
}