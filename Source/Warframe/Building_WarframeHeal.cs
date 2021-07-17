using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace Warframe
{
    public class Building_WarframeHeal : Building
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
            Scribe_Values.Look(ref ticks, "ticks");
        }

        public override void Tick()
        {
            base.Tick();
            if (!this.TryGetComp<CompPowerTrader>().PowerOn)
            {
                return;
            }

            if (ticks < 600)
            {
                ticks++;
            }
            else
            {
                healWarframe();
                ticks = 0;
            }
        }

        private void healWarframe()
        {
            foreach (var thing in Map.thingGrid.ThingsAt(Position))
            {
                if (thing is not Pawn pawn)
                {
                    continue;
                }

                if (!pawn.IsWarframe())
                {
                    continue;
                }

                if ((from x in pawn.health.hediffSet.GetHediffs<Hediff_Injury>()
                    where x.CanHealNaturally() || x.CanHealFromTending()
                    select x).TryRandomElement(out var hediff_Injury))
                {
                    hediff_Injury.Heal(10f);
                    WarframeStaticMethods.ShowColorText(pawn, "HP+10", new Color(0.2f, 1, 0.1f), GameFont.Medium);
                }

                break;
            }
        }
    }
}