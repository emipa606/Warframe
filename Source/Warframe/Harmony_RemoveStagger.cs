using HarmonyLib;
using Verse;
using Verse.AI;

namespace Warframe
{
    [HarmonyPatch(typeof(Pawn_PathFollower), "CostToPayThisTick")]
    public static class Harmony_RemoveStagger
    {
        public static void Postfix(Pawn_PathFollower __instance, ref float __result)
        {
            var traverse = Traverse.Create(__instance);

            var pawn = traverse.Field("pawn").GetValue<Pawn>();
            if (!pawn.IsWarframe())
            {
                return;
            }

            if (__result < 1f)
            {
                __result = 1f;
            }
        }
    }
}