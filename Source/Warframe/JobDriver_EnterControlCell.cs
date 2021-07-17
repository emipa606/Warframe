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
            var pawn1 = pawn;
            var targetA = job.targetA;
            var job1 = job;
            return pawn1.Reserve(targetA, job1, 1, -1, null, errorOnFailed);
        }

        // Token: 0x060002E0 RID: 736 RVA: 0x0001C17C File Offset: 0x0001A57C
        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedOrNull(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell);
            var prepare = Toils_General.Wait(60);
            prepare.FailOnCannotTouch(TargetIndex.A, PathEndMode.InteractionCell);
            prepare.WithProgressBarToilDelay(TargetIndex.A);
            yield return prepare;
            var enter = new Toil();
            enter.initAction = delegate
            {
                var actor = enter.actor;
                var pod = (Building_ControlCell) actor.CurJob.targetA.Thing;

                void Action()
                {
                    actor.DeSpawn();
                    pod.TryAcceptThing(actor);
                }

                if (!pod.def.building.isPlayerEjectable)
                {
                    var freeColonistsSpawnedOrInPlayerEjectablePodsCount =
                        Map.mapPawns.FreeColonistsSpawnedOrInPlayerEjectablePodsCount;
                    if (freeColonistsSpawnedOrInPlayerEjectablePodsCount <= 1)
                    {
                        Find.WindowStack.Add(
                            Dialog_MessageBox.CreateConfirmation("CasketWarning".Translate().AdjustedFor(actor),
                                Action));
                    }
                    else
                    {
                        Action();
                    }
                }
                else
                {
                    Action();
                }
            };
            enter.defaultCompleteMode = ToilCompleteMode.Instant;
            yield return enter;
        }
    }
}