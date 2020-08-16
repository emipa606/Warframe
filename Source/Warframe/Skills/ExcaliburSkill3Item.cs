using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Warframe.Skills
{
    public class ExcaliburSkill3Item:ThingWithComps
    {
        public int createdTick;
        public Pawn self;
        public float range;
        /*
        public ExcaliburSkill3Item(int tick,Pawn self,float range){
            this.createdTick = tick;
            this.self = self;
            this.range = range;

        }
        */
        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look<int>(ref createdTick, "createdTick",0, false);
            Scribe_Values.Look<float>(ref range, "range", 0, false);
            Scribe_References.Look<Pawn>(ref self,"self",false);

        }
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);

        }
        public override void Draw()
        {
            //base.Draw();
        }
        public override void Tick()
        {
            base.Tick();
            if(Find.TickManager.TicksGame -60>= createdTick)
            {
                float damage = 120 + (8 * WarframeStaticMethods.GetWFLevel(self) / 5);


                foreach (IntVec3 iv in WarframeStaticMethods.GetCellsAround(self.Position, self.Map, range))
                {
                    foreach (Thing t in self.Map.thingGrid.ThingsAt(iv))
                    {
                        if (t is Pawn)
                        {
                            if ((t as Pawn) != self && (t as Pawn).Faction != self.Faction)
                            {
                                WarframeStaticMethods.ShowDamageAmount(t, damage.ToString("f0"));
                                // float totaldamage = 0;
                                DamageInfo dinfo = new DamageInfo(DamageDefOf.Cut, damage, 1, -1, self, null, null, DamageInfo.SourceCategory.ThingOrUnknown, null);
                                /*
                                foreach (BodyPartRecord bpr in (t as Pawn).health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Outside))
                                {
                                    (t as Pawn).TakeDamage(dinfo);
                                    totaldamage += bpr.def.hitPoints;
                                    dinfo.SetAmount(damage - totaldamage);
                                    if (totaldamage > damage)
                                    {
                                        break;
                                    }
                                }
                                */
                                (t as Pawn).TakeDamage(dinfo);
                                {
                                    Mote mote = (Mote)ThingMaker.MakeThing(ThingDef.Named("Mote_2ExFlash"), null);
                                    mote.exactPosition = t.Position.ToVector3Shifted();
                                    mote.Scale = (float)Mathf.Max(10f, 15f);
                                    mote.rotationRate = 1.2f;
                                    // mote.Scale = 0.2f;
                                    GenSpawn.Spawn(mote, t.Position + new IntVec3(0, 1, 0), self.Map, WipeMode.Vanish);
                                }

                            }
                        }
                    }
                }
                Destroy(DestroyMode.Vanish);


            }
        }





    }
}
