using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace Warframe
{
    public class Building_WarframeHeal:Building
    {
        private int ticks;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            this.ticks = 0;

        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.ticks,"ticks",0,false);
        }
        public override void Tick()
        {
            base.Tick();
            if (!this.TryGetComp<CompPowerTrader>().PowerOn) return;

            if (this.ticks < 600)
            {

                this.ticks++;
            }else
            {
                healWarframe();
                this.ticks = 0;

            }
        }
        private void healWarframe() {
           foreach(Thing thing in this.Map.thingGrid.ThingsAt(this.Position))
            {
                if(thing is Pawn)
                {
                    if((thing as Pawn).isWarframe())
                    {
                        
                            Pawn p = thing as Pawn;
                            
                            Hediff_Injury hediff_Injury;
                            if ((from x in p.health.hediffSet.GetHediffs<Hediff_Injury>()
                                 where x.CanHealNaturally() || x.CanHealFromTending()
                                 select x).TryRandomElement(out hediff_Injury))
                            {
                                hediff_Injury.Heal(10f);
                                WarframeStaticMethods.showColorText(p, "HP+10", new Color(0.2f, 1, 0.1f), GameFont.Medium);
                            }
                            
                            break;
                        
                    }
                }
            }
        }
    }
}
