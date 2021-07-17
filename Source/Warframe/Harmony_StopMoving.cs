using System;
using HarmonyLib;
using Verse;
using Verse.AI;

namespace Warframe
{
    [HarmonyPatch(typeof(Pawn_HealthTracker), "HealthTick", new Type[]
    {
    })]
    public static class Harmony_StopMoving
    {
        public static void Postfix(Pawn_HealthTracker __instance)
        {
            var traverse = Traverse.Create(__instance);
            var pp = traverse.Field("pawn").GetValue<Pawn>();
            if (!pp.IsWarframe())
            {
                return;
            }

            if (!pp.Spawned)
            {
                return;
            }

            if (!WarframeStaticMethods.PawnInControlCell(pp))
            {
                var job = new Job(DefDatabase<JobDef>.GetNamed("WFWait"));
                pp.jobs.TryTakeOrderedJob(job, JobTag.Misc);
            }
            else
            {
                if (pp.jobs.curJob.def == DefDatabase<JobDef>.GetNamed("WFWait"))
                {
                    pp.jobs.EndCurrentJob(JobCondition.None);
                }
            }
        }
    }
}