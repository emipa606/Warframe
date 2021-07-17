using HarmonyLib;
using Verse;
using Verse.AI;

namespace Warframe
{
    [HarmonyPatch(typeof(Pawn_PathFollower), "CostToMoveIntoCell", typeof(Pawn), typeof(IntVec3))]
    public static class Harmony_RemovePathCost
    {
        public static bool Prefix(Pawn_PathFollower __instance, Pawn pawn, IntVec3 c, ref int __result)
        {
            // Traverse traverse = Traverse.Create(__instance);

            // Pawn pawn = traverse.Field("pawn").GetValue<Pawn>();
            if (!pawn.IsWarframe())
            {
                return true;
            }

            int num;
            if (c.x == pawn.Position.x || c.z == pawn.Position.z)
            {
                num = pawn.TicksPerMoveCardinal;
            }
            else
            {
                num = pawn.TicksPerMoveDiagonal;
            }

            __result = num;
            return false;
        }
    }
}