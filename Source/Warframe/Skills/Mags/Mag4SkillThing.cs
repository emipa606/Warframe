using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;

namespace Warframe.Skills.Mags
{
    public class Mag4SkillThing:ThingWithComps
    {
        public int range;
        public Pawn self;
        public int ticks;
       // public List<Pawn> affected;
        public float damage;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look<Pawn>(ref self,"self",false);
            Scribe_Values.Look<int>(ref range,"range",0,false);
            Scribe_Values.Look<int>(ref ticks, "ticks", 0, false);
            Scribe_Values.Look<float>(ref damage, "damage", 0, false);
          //  Scribe_Collections.Look<Pawn>(ref this.affected,"affected",LookMode.Reference, new object[0]);


        }
        public override void Draw()
        {
            //base.Draw();
        }
        public override void Tick()
        {
            base.Tick();
            if (ticks > 180) Destroy();
            if (!Spawned) return;

            ticks++;

            if (ticks == 1 || ticks==60)
            {
                foreach (IntVec3 iv in WarframeStaticMethods.GetCellsAround(self.Position, self.Map, range))
                {
                    foreach (Thing t in self.Map.thingGrid.ThingsAt(iv))
                    {
                        if (t is Pawn)
                        {
                            if ((t as Pawn) != self && (t as Pawn).Faction != self.Faction)
                            {
                                (t as Pawn).stances.stunner.StunFor(180,self);
                                {
                                    Mote mote = (Mote)ThingMaker.MakeThing(ThingDef.Named("Mote_2ExFlash"), null);
                                    mote.exactPosition = t.Position.ToVector3Shifted();
                                    mote.Scale = (float)Mathf.Max(6f,11f);
                                    mote.rotationRate = 1.2f;
                                    GenSpawn.Spawn(mote, t.Position + new IntVec3(0, 1, 0), self.Map, WipeMode.Vanish);
                                }
                                WarframeStaticMethods.ShowDamageAmount(t, (damage*0.3f).ToString("f0"));
                                
                                DamageInfo dinfo = new DamageInfo(DamageDefOf.Crush, (damage * 0.3f), 1, -1, self, null, null, DamageInfo.SourceCategory.ThingOrUnknown, null);
                                IEnumerable<BodyPartRecord> bprs= (t as Pawn).health.hediffSet.GetNotMissingParts();
                                List<BodyPartRecord> canHitPart = new List<BodyPartRecord>();
                                if((t as Pawn).RaceProps.IsFlesh)
                                foreach (BodyPartRecord bpr in bprs)
                                {
                                    if(bpr.groups!=null && bpr.groups.Contains(BodyPartGroupDefOf.Torso))
                                        {
                                            canHitPart.Add(bpr);
                                        }
                                }
                                else
                                    foreach (BodyPartRecord bpr in bprs)
                                    {
                                        if (bpr.def.tags!=null &&bpr.def.tags.Contains(BodyPartTagDefOf.MovingLimbCore))
                                        {
                                            canHitPart.Add(bpr);
                                        }
                                    }
                                if (canHitPart.Count > 0)
                                {
                                    dinfo.SetHitPart(canHitPart.RandomElement());
                                    (t as Pawn).TakeDamage(dinfo);
                                }


                            }
                        }
                    }
                }
            }else if (ticks == 110)//last attack
            {
                foreach (IntVec3 iv in WarframeStaticMethods.GetCellsAround(self.Position, self.Map, range))
                {
                    foreach (Thing t in self.Map.thingGrid.ThingsAt(iv))
                    {
                        if (t is Pawn)
                        {
                            if ((t as Pawn) != self && (t as Pawn).Faction != self.Faction)
                            {
                                (t as Pawn).stances.stunner.StunFor(180, self);
                                {
                                    Mote mote = (Mote)ThingMaker.MakeThing(ThingDef.Named("Mote_ExFlash"), null);
                                    mote.exactPosition = t.Position.ToVector3Shifted();
                                    mote.Scale = (float)Mathf.Max(10f, 15f);
                                    mote.rotationRate = 1.2f;
                                    GenSpawn.Spawn(mote, t.Position + new IntVec3(0, 1, 0), self.Map, WipeMode.Vanish);
                                }
                                WarframeStaticMethods.ShowDamageAmount(t, (damage).ToString("f0"));

                                DamageInfo dinfo = new DamageInfo(DamageDefOf.Crush, (damage), 1, -1, self, null, null, DamageInfo.SourceCategory.ThingOrUnknown, null);
                                IEnumerable<BodyPartRecord> bprs = (t as Pawn).health.hediffSet.GetNotMissingParts();
                                List<BodyPartRecord> canHitPart = new List<BodyPartRecord>();
                                if ((t as Pawn).RaceProps.IsFlesh)
                                    foreach (BodyPartRecord bpr in bprs)
                                    {
                                        if (bpr.groups != null && bpr.groups.Contains(BodyPartGroupDefOf.Torso))
                                        {
                                            canHitPart.Add(bpr);
                                        }
                                    }
                                foreach (BodyPartRecord bpr in bprs)
                                {
                                    if (bpr.def.tags != null && bpr.def.tags.Contains(BodyPartTagDefOf.MovingLimbCore))
                                    {
                                        canHitPart.Add(bpr);
                                    }
                                }
                                if (canHitPart.Count > 0)
                                {
                                    dinfo.SetHitPart(canHitPart.RandomElement());
                                    (t as Pawn).TakeDamage(dinfo);
                                }



                            }
                        }
                    }
                }
            }


        }


      
    }
}
