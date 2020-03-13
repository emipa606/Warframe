using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace Warframe
{
    public class WarframeHead : Apparel
    {

        public float jumpCD=0;
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<float>(ref this.jumpCD, "jumpCD", 0, false);
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
            if (Find.Selector.SingleSelectedThing != base.Wearer)
            {
                yield break;
            }
            Pawn pawn = this.Wearer;
            if (pawn == null || !pawn.isWarframe()) yield break;

            Command_CastSkillTargetingFloor ck1 = new Command_CastSkillTargetingFloor();
            ck1.self = this.Wearer;
            ck1.targetingParams = WarframeStaticMethods.jumpTP();
            ck1.defaultLabel = "WarframeJumpGizmo.name".Translate();
            ck1.defaultDesc = "WarframeJumpGizmo.desc".Translate();
            ck1.range = 14f;
            ck1.icon = ContentFinder<Texture2D>.Get("Skills/Jump");
            ck1.cooldownTime = 60;
            ck1.hotKey = KeyBindingDefOf.Command_ItemForbid;
            ck1.disabled = !this.Wearer.Drafted || this.jumpCD > 0 || this.Wearer.stances.stunner.Stunned;
            ck1.action = delegate (Pawn self, LocalTargetInfo target)
            {
                if (WarframeStaticMethods.outRange(ck1.range,self,target.Cell.ToVector3()))
                {
                    SoundDefOf.ClickReject.PlayOneShotOnCamera();
                    return;
                }

                if (!target.Cell.Walkable(self.Map))
                    {
                        Messages.Message("WFCantJumpToThere".Translate(), MessageTypeDefOf.RejectInput, false);
                        return;
                    }

                int jtype = WarframeStaticMethods.getJumpType(self,target.Cell);


                if (jtype == 0)
                {
                    RoofDef wfroof = self.Map.roofGrid.RoofAt(self.Position);
                    if (wfroof != null)
                    {
                        if (wfroof != RoofDefOf.RoofConstructed)
                        {
                            Messages.Message("WFJumpRockRoof".Translate(), MessageTypeDefOf.RejectInput, false);
                            return;
                        }
                        if (!wfroof.soundPunchThrough.NullOrUndefined())
                        {
                            wfroof.soundPunchThrough.PlayOneShot(new TargetInfo(self.Position, self.Map, false));
                            CellRect.CellRectIterator iterator = CellRect.CenteredOn(self.Position, 1).GetIterator();
                            while (!iterator.Done())
                            {
                                Find.CurrentMap.roofGrid.SetRoof(iterator.Current, null);
                                iterator.MoveNext();
                            }
                        }

                    }
                    RoofDef locroof = self.Map.roofGrid.RoofAt(target.Cell);
                    if (locroof != null)
                    {
                        if (locroof != RoofDefOf.RoofConstructed)
                        {
                            Messages.Message("WFJumpRockRoof".Translate(), MessageTypeDefOf.RejectInput, false);
                            return;
                        }
                        if (!locroof.soundPunchThrough.NullOrUndefined())
                        {
                            locroof.soundPunchThrough.PlayOneShot(new TargetInfo(self.Position, self.Map, false));
                            CellRect.CellRectIterator iterator = CellRect.CenteredOn(target.Cell, 1).GetIterator();
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
                this.jumpCD = ck1.cooldownTime;
            };
            ck1.finishAction = delegate {
                GenDraw.DrawRadiusRing(ck1.self.Position,ck1.range);//DrawFieldEdges(WarframeStaticMethods.getCellsAround(ck1.self.Position, ck1.self.Map, ck1.range));
            };
            yield return ck1;
            yield break;

        }


    }
}
