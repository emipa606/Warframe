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
            ticks = 0;

        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref ticks,"ticks",0,false);
        }
        public override void Tick()
        {
            base.Tick();
            if (!this.TryGetComp<CompPowerTrader>().PowerOn) return;

            if (ticks < 600)
            {

                ticks++;
            }else
            {
                healWarframe();
                ticks = 0;

            }
        }
        private void healWarframe() {
           foreach(Thing thing in Map.thingGrid.ThingsAt(Position))
            {
                if(thing is Pawn)
                {
                    if((thing as Pawn).IsWarframe())
                    {
                        
                            Pawn p = thing as Pawn;
                            
                            Hediff_Injury hediff_Injury;
                            if ((from x in p.health.hediffSet.GetHediffs<Hediff_Injury>()
                                 where x.CanHealNaturally() || x.CanHealFromTending()
                                 select x).TryRandomElement(out hediff_Injury))
                            {
                                hediff_Injury.Heal(10f);
                                WarframeStaticMethods.ShowColorText(p, "HP+10", new Color(0.2f, 1, 0.1f), GameFont.Medium);
                            }
                            
                            break;
                        
                    }
                }
            }
        }
    }
}
