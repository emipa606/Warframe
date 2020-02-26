using RimWorld;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using Verse;
using Warframe.Skills.Volts;

namespace Warframe.Skills.Volts
{
    public class Volt4SkillThing:ThingWithComps
    {
        public int range;
        public Pawn self;
        public int ticks;
        public List<Pawn> affected;
        public float damage;

        private bool startBomb
        {
            get
            {
                return this.ticks >= 60;
            }
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look<Pawn>(ref this.self,"self",false);
            Scribe_Values.Look<int>(ref this.range,"range",0,false);
            Scribe_Values.Look<int>(ref this.ticks, "ticks", 0, false);
            Scribe_Values.Look<float>(ref this.damage, "damage", 0, false);
            Scribe_Collections.Look<Pawn>(ref this.affected,"affected",LookMode.Reference, new object[0]);


        }
        public override void Draw()
        {
            //base.Draw();
        }
        public override void Tick()
        {
            base.Tick();
            if (ticks > 240) this.Destroy(DestroyMode.Vanish);
            if (!this.Spawned) return;

            this.ticks++;
            if (ticks == 59) { 
                this.affected = new List<Pawn>();
            }

           

            
            if (startBomb )
            {
                GenDraw.DrawFieldEdges(this.MagNowCellsAround(this.self.Position, this.Map, this.range * ((this.ticks - 60) * 1f / 180f)),new Color(0.4f,0.4f,0.8f));

                foreach(IntVec3 ic in this.MagNowCellsAround(this.self.Position, this.Map, this.range * ((this.ticks-60) * 1f / 180f)))
                {
                    foreach(Thing th in this.Map.thingGrid.ThingsAt(ic))
                    {
                        if(th is Pawn)
                        {
                            Pawn pa = th as Pawn;
                            if (this.affected.Contains(pa)||pa==self) continue;

                            if (pa.Faction.HostileTo(self.Faction))
                            {
                                DamageInfo dinfo = new DamageInfo(DefDatabase<DamageDef>.GetNamed("Mag",true),damage,0,-1,self,null,null,DamageInfo.SourceCategory.ThingOrUnknown,pa);
                                pa.TakeDamage(dinfo);
                                Hediff_Volt4Skill hediff_Magnetize = (Hediff_Volt4Skill)HediffMaker.MakeHediff(HediffDef.Named("Volt4Skill"), self, null);
                                hediff_Magnetize.level = (int)self.getLevel();
                                hediff_Magnetize.damage = 3;
                                (th as Pawn).health.AddHediff(hediff_Magnetize, null, null, null);
                                WarframeStaticMethods.showDamageAmount(pa, damage.ToString("f0"));
                                pa.stances.stunner.StunFor((int)(180 * (1+(self.getLevel()*1f/30f))),self);
                            }

                            this.affected.Add(pa);


                        }
                    }
                }
            }
        }


        public List<IntVec3> MagNowCellsAround(IntVec3 pos, Map map,float nowrange)
        {
            List<IntVec3> result = new List<IntVec3>();
            if (!pos.InBounds(map))
            {
                return result;
            }
            Region region = pos.GetRegion(map, RegionType.Set_Passable);
            if (region == null)
            {
                return result;
            }
            RegionTraverser.BreadthFirstTraverse(region, (Region from, Region r) => r.door == null, delegate (Region r)
            {
                foreach (IntVec3 item in r.Cells)
                {
                    if (item.InHorDistOf(pos, nowrange) && !item.InHorDistOf(pos, nowrange - 1))
                    {
                        result.Add(item);
                    }
                }
                return false;
            }, (int)nowrange, RegionType.Set_Passable);
            return result;
        }
    }
}
