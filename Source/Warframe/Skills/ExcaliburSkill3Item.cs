using RimWorld;
using UnityEngine;
using Verse;

namespace Warframe.Skills
{
    public class ExcaliburSkill3Item : ThingWithComps
    {
        public int createdTick;
        public float range;

        public Pawn self;

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

            Scribe_Values.Look(ref createdTick, "createdTick");
            Scribe_Values.Look(ref range, "range");
            Scribe_References.Look(ref self, "self");
        }

        public override void Draw()
        {
            //base.Draw();
        }

        public override void Tick()
        {
            base.Tick();
            if (Find.TickManager.TicksGame - 60 < createdTick)
            {
                return;
            }

            float damage = 120 + (8 * WarframeStaticMethods.GetWFLevel(self) / 5);


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

                    WarframeStaticMethods.ShowDamageAmount(pawn, damage.ToString("f0"));
                    // float totaldamage = 0;
                    var dinfo = new DamageInfo(DamageDefOf.Cut, damage, 1, -1, self);
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
                    pawn.TakeDamage(dinfo);
                    {
                        var mote = (Mote) ThingMaker.MakeThing(ThingDef.Named("Mote_2ExFlash"));
                        mote.exactPosition = pawn.Position.ToVector3Shifted();
                        mote.Scale = Mathf.Max(10f, 15f);
                        mote.rotationRate = 1.2f;
                        // mote.Scale = 0.2f;
                        GenSpawn.Spawn(mote, pawn.Position + new IntVec3(0, 1, 0), self.Map);
                    }
                }
            }

            Destroy();
        }
    }
}