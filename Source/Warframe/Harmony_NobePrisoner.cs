using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace Warframe
{
    [HarmonyPatch(typeof(JobDriver_TakeToBed), "TryMakePreToilReservations", typeof(bool))]
    public static class Harmony_NobePrisoner
    {
        public static bool Prefix(JobDriver_TakeToBed __instance, ref bool __result)
        {
            var tv = Traverse.Create(__instance);
            var job = tv.Field("job").GetValue<Job>();
            var pawn = (Pawn) job.GetTarget(TargetIndex.A).Thing;
            if (!pawn.kindDef.defName.StartsWith("Warframe_"))
            {
                return true;
            }

            Messages.Message("CanBeCarryMsg".Translate(), MessageTypeDefOf.RejectInput, false);
            __result = false;
            return false;
        }
    }
}