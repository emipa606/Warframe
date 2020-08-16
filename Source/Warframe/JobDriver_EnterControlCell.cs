using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace Warframe
{
    // Token: 0x02000066 RID: 102
    public class JobDriver_EnterControlCell : JobDriver
    {
        // Token: 0x060002DF RID: 735 RVA: 0x0001C144 File Offset: 0x0001A544
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            Pawn pawn = this.pawn;
            LocalTargetInfo targetA = this.job.targetA;
            Job job = this.job;
            return pawn.Reserve(targetA, job, 1, -1, null, errorOnFailed);
        }

        // Token: 0x060002E0 RID: 736 RVA: 0x0001C17C File Offset: 0x0001A57C
        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedOrNull(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell);
            Toil prepare = Toils_General.Wait(60, TargetIndex.None);
            prepare.FailOnCannotTouch(TargetIndex.A, PathEndMode.InteractionCell);
            prepare.WithProgressBarToilDelay(TargetIndex.A, false, -0.5f);
            yield return prepare;
            Toil enter = new Toil();
            enter.initAction = delegate
            {
                Pawn actor = enter.actor;
                Building_ControlCell pod = (Building_ControlCell)actor.CurJob.targetA.Thing;
                Action action = delegate
                {
                    actor.DeSpawn(DestroyMode.Vanish);
                    pod.TryAcceptThing(actor, true);
                };
                if (!pod.def.building.isPlayerEjectable)
                {
                    int freeColonistsSpawnedOrInPlayerEjectablePodsCount = Map.mapPawns.FreeColonistsSpawnedOrInPlayerEjectablePodsCount;
                    if (freeColonistsSpawnedOrInPlayerEjectablePodsCount <= 1)
                    {
                        Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("CasketWarning".Translate().AdjustedFor(actor, "PAWN"), action, false, null));
                    }
                    else
                    {
                        action();
                    }
                }
                else
                {
                    action();
                }
            };
            enter.defaultCompleteMode = ToilCompleteMode.Instant;
            yield return enter;
            yield break;
        }
    }
}
