using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace Warframe
{
    public class WarframeHead : Apparel
    {
        public float jumpCD;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref jumpCD, "jumpCD");
        }

        public override void Tick()
        {
            base.Tick();
            if (jumpCD > 0)
            {
                jumpCD -= 1;
            }
        }

        public override IEnumerable<Gizmo> GetWornGizmos()
        {
            if (Find.Selector.SingleSelectedThing != Wearer)
            {
                yield break;
            }

            var pawn = Wearer;
            if (pawn == null || !pawn.IsWarframe())
            {
                yield break;
            }

            var ck1 = new Command_CastSkillTargetingFloor
            {
                self = Wearer,
                targetingParams = WarframeStaticMethods.JumpTP(),
                defaultLabel = "WarframeJumpGizmo.name".Translate(),
                defaultDesc = "WarframeJumpGizmo.desc".Translate(),
                range = 14f,
                icon = ContentFinder<Texture2D>.Get("Skills/Jump"),
                cooldownTime = 60,
                hotKey = KeyBindingDefOf.Command_ItemForbid,
                disabled = !Wearer.Drafted || jumpCD > 0 || Wearer.stances.stunner.Stunned
            };
            ck1.action = delegate(Pawn self, LocalTargetInfo target)
            {
                if (WarframeStaticMethods.OutRange(ck1.range, self, target.Cell.ToVector3()))
                {
                    SoundDefOf.ClickReject.PlayOneShotOnCamera();
                    return;
                }

                if (!target.Cell.Walkable(self.Map))
                {
                    Messages.Message("WFCantJumpToThere".Translate(), MessageTypeDefOf.RejectInput, false);
                    return;
                }

                var jtype = WarframeStaticMethods.GetJumpType(self, target.Cell);


                if (jtype == 0)
                {
                    var wfroof = self.Map.roofGrid.RoofAt(self.Position);
                    if (wfroof != null)
                    {
                        if (wfroof != RoofDefOf.RoofConstructed)
                        {
                            Messages.Message("WFJumpRockRoof".Translate(), MessageTypeDefOf.RejectInput, false);
                            return;
                        }

                        if (!wfroof.soundPunchThrough.NullOrUndefined())
                        {
                            wfroof.soundPunchThrough.PlayOneShot(new TargetInfo(self.Position, self.Map));
                            var iterator = CellRect.CenteredOn(self.Position, 1).GetIterator();
                            while (!iterator.Done())
                            {
                                Find.CurrentMap.roofGrid.SetRoof(iterator.Current, null);
                                iterator.MoveNext();
                            }
                        }
                    }

                    var locroof = self.Map.roofGrid.RoofAt(target.Cell);
                    if (locroof != null)
                    {
                        if (locroof != RoofDefOf.RoofConstructed)
                        {
                            Messages.Message("WFJumpRockRoof".Translate(), MessageTypeDefOf.RejectInput, false);
                            return;
                        }

                        if (!locroof.soundPunchThrough.NullOrUndefined())
                        {
                            locroof.soundPunchThrough.PlayOneShot(new TargetInfo(self.Position, self.Map));
                            var iterator = CellRect.CenteredOn(target.Cell, 1).GetIterator();
                            while (!iterator.Done())
                            {
                                Find.CurrentMap.roofGrid.SetRoof(iterator.Current, null);
                                iterator.MoveNext();
                            }
                        }
                    }
                }

                self.pather.StartPath(target, PathEndMode.Touch);
                self.Position = target.Cell;

                self.pather.StopDead();
                if (self.jobs.curJob != null)
                {
                    self.jobs.curDriver.Notify_PatherArrived();
                }

                SoundDef.Named("Warframe_Jump").PlayOneShot(self);
                jumpCD = ck1.cooldownTime;
            };
            ck1.finishAction = delegate
            {
                GenDraw.DrawRadiusRing(ck1.self.Position,
                    ck1.range); //DrawFieldEdges(WarframeStaticMethods.getCellsAround(ck1.self.Position, ck1.self.Map, ck1.range));
            };
            yield return ck1;
        }
    }
}