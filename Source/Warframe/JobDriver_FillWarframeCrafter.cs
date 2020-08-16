using System;
using System.Collections.Generic;
using System.Text;
using Verse;
using Verse.AI;

namespace Warframe
{
    public class JobDriver_FillWarframeCrafter : JobDriver
    {
        // Token: 0x060000F3 RID: 243 RVA: 0x00003C68 File Offset: 0x00001E68
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.targetA, job, 1, -1, null, true);
        }

        // Token: 0x1700001B RID: 27
        // (get) Token: 0x060000F4 RID: 244 RVA: 0x000073CC File Offset: 0x000055CC
        protected Building_WarframeCrafter bwc
        {
            get
            {
                return (Building_WarframeCrafter)job.GetTarget(TargetIndex.A).Thing;
            }
        }

        // Token: 0x1700001C RID: 28
        // (get) Token: 0x060000F5 RID: 245 RVA: 0x000073F4 File Offset: 0x000055F4
        protected Thing part
        {
            get
            {
                return job.GetTarget(TargetIndex.B).Thing;
            }
        }

        // Token: 0x060000F6 RID: 246 RVA: 0x00007415 File Offset: 0x00005615
        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            this.FailOnBurningImmobile(TargetIndex.A);
            yield return Toils_Reserve.Reserve(TargetIndex.A, 1, -1, null);
            Toil reservefuel = Toils_Reserve.Reserve(TargetIndex.B, 1, -1, null);
            yield return reservefuel;
            yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.ClosestTouch).FailOnDespawnedNullOrForbidden(TargetIndex.B).FailOnSomeonePhysicallyInteracting(TargetIndex.B);
            yield return Toils_Haul.StartCarryThing(TargetIndex.B, false, true, false).FailOnDestroyedNullOrForbidden(TargetIndex.B);
            yield return Toils_Haul.CheckForGetOpportunityDuplicate(reservefuel, TargetIndex.B, TargetIndex.None, true, null);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            yield return Toils_General.Wait(100, TargetIndex.None).FailOnDestroyedNullOrForbidden(TargetIndex.B).FailOnDestroyedNullOrForbidden(TargetIndex.A).WithProgressBarToilDelay(TargetIndex.A, false, -0.5f);
            yield return new Toil
            {
                initAction = delegate
                {
                    bwc.AddPart(part);
                    if (bwc.allPartAlready()) bwc.curState = Building_WarframeCrafter.CraftState.Crafting;
                    pawn.carryTracker.innerContainer.Remove(part);
                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };
            yield break;
           // yield break;
        }

        // Token: 0x04000090 RID: 144
        private const int Duration = 200;
    }
}
