using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace Warframe.Skills.Mags
{
    public class Mag4SkillThing : ThingWithComps
    {
        // public List<Pawn> affected;
        public float damage;
        public int range;
        public Pawn self;
        public int ticks;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref self, "self");
            Scribe_Values.Look(ref range, "range");
            Scribe_Values.Look(ref ticks, "ticks");
            Scribe_Values.Look(ref damage, "damage");
            //  Scribe_Collections.Look<Pawn>(ref this.affected,"affected",LookMode.Reference, new object[0]);
        }

        public override void Draw()
        {
            //base.Draw();
        }

        public override void Tick()
        {
            base.Tick();
            if (ticks > 180)
            {
                Destroy();
            }

            if (!Spawned)
            {
                return;
            }

            ticks++;

            if (ticks == 1 || ticks == 60)
            {
                foreach (var iv in WarframeStaticMethods.GetCellsAround(self.Position, self.Map, range))
                {
                    foreach (var t in self.Map.thingGrid.ThingsAt(iv))
                    {
                        if (t is not Pawn pawn)
                        {
                            continue;
                        }

                        if (pawn == self || pawn.Faction == self.Faction)
                        {
                            continue;
                        }

                        pawn.stances.stunner.StunFor(180, self);
                        {
                            var mote = (Mote) ThingMaker.MakeThing(ThingDef.Named("Mote_2ExFlash"));
                            mote.exactPosition = pawn.Position.ToVector3Shifted();
                            mote.Scale = Mathf.Max(6f, 11f);
                            mote.rotationRate = 1.2f;
                            GenSpawn.Spawn(mote, pawn.Position + new IntVec3(0, 1, 0), self.Map);
                        }
                        WarframeStaticMethods.ShowDamageAmount(pawn, (damage * 0.3f).ToString("f0"));

                        var dinfo = new DamageInfo(DamageDefOf.Crush, damage * 0.3f, 1, -1, self);
                        var bprs = pawn.health.hediffSet.GetNotMissingParts();
                        var canHitPart = new List<BodyPartRecord>();
                        if (pawn.RaceProps.IsFlesh)
                        {
                            foreach (var bpr in bprs)
                            {
                                if (bpr.groups != null && bpr.groups.Contains(BodyPartGroupDefOf.Torso))
                                {
                                    canHitPart.Add(bpr);
                                }
                            }
                        }
                        else
                        {
                            foreach (var bpr in bprs)
                            {
                                if (bpr.def.tags != null &&
                                    bpr.def.tags.Contains(BodyPartTagDefOf.MovingLimbCore))
                                {
                                    canHitPart.Add(bpr);
                                }
                            }
                        }

                        if (canHitPart.Count <= 0)
                        {
                            continue;
                        }

                        dinfo.SetHitPart(canHitPart.RandomElement());
                        pawn.TakeDamage(dinfo);
                    }
                }
            }
            else if (ticks == 110) //last attack
            {
                foreach (var iv in WarframeStaticMethods.GetCellsAround(self.Position, self.Map, range))
                {
                    foreach (var t in self.Map.thingGrid.ThingsAt(iv))
                    {
                        if (t is not Pawn pawn)
                        {
                            continue;
                        }

                        if (pawn == self || pawn.Faction == self.Faction)
                        {
                            continue;
                        }

                        pawn.stances.stunner.StunFor(180, self);
                        {
                            var mote = (Mote) ThingMaker.MakeThing(ThingDef.Named("Mote_ExFlash"));
                            mote.exactPosition = pawn.Position.ToVector3Shifted();
                            mote.Scale = Mathf.Max(10f, 15f);
                            mote.rotationRate = 1.2f;
                            GenSpawn.Spawn(mote, pawn.Position + new IntVec3(0, 1, 0), self.Map);
                        }
                        WarframeStaticMethods.ShowDamageAmount(pawn, damage.ToString("f0"));

                        var dinfo = new DamageInfo(DamageDefOf.Crush, damage, 1, -1, self);
                        var bprs = pawn.health.hediffSet.GetNotMissingParts();
                        var canHitPart = new List<BodyPartRecord>();
                        if (pawn.RaceProps.IsFlesh)
                        {
                            foreach (var bpr in bprs)
                            {
                                if (bpr.groups != null && bpr.groups.Contains(BodyPartGroupDefOf.Torso))
                                {
                                    canHitPart.Add(bpr);
                                }
                            }
                        }

                        foreach (var bpr in bprs)
                        {
                            if (bpr.def.tags != null && bpr.def.tags.Contains(BodyPartTagDefOf.MovingLimbCore))
                            {
                                canHitPart.Add(bpr);
                            }
                        }

                        if (canHitPart.Count <= 0)
                        {
                            continue;
                        }

                        dinfo.SetHitPart(canHitPart.RandomElement());
                        pawn.TakeDamage(dinfo);
                    }
                }
            }
        }
    }
}