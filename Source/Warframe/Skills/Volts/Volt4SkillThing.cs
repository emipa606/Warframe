using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace Warframe.Skills.Volts
{
    public class Volt4SkillThing : ThingWithComps
    {
        public List<Pawn> affected;
        public float damage;
        public int range;
        public Pawn self;
        public int ticks;

        private bool startBomb => ticks >= 60;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref self, "self");
            Scribe_Values.Look(ref range, "range");
            Scribe_Values.Look(ref ticks, "ticks");
            Scribe_Values.Look(ref damage, "damage");
            Scribe_Collections.Look(ref affected, "affected", LookMode.Reference);
        }

        public override void Draw()
        {
            //base.Draw();
        }

        public override void Tick()
        {
            base.Tick();
            if (ticks > 240)
            {
                Destroy();
            }

            if (!Spawned)
            {
                return;
            }

            ticks++;
            if (ticks == 59)
            {
                affected = new List<Pawn>();
            }


            if (!startBomb)
            {
                return;
            }

            GenDraw.DrawFieldEdges(MagNowCellsAround(self.Position, Map, range * ((ticks - 60) * 1f / 180f)),
                new Color(0.4f, 0.4f, 0.8f));

            foreach (var ic in MagNowCellsAround(self.Position, Map, range * ((ticks - 60) * 1f / 180f)))
            {
                foreach (var th in Map.thingGrid.ThingsAt(ic))
                {
                    if (th is not Pawn pawn)
                    {
                        continue;
                    }

                    if (affected.Contains(pawn) || pawn == self)
                    {
                        continue;
                    }

                    if (pawn.Faction.HostileTo(self.Faction))
                    {
                        var dinfo = new DamageInfo(DefDatabase<DamageDef>.GetNamed("Mag"), damage, 0, -1, self,
                            null, null, DamageInfo.SourceCategory.ThingOrUnknown, pawn);
                        pawn.TakeDamage(dinfo);
                        var hediff_Magnetize =
                            (Hediff_Volt4Skill) HediffMaker.MakeHediff(HediffDef.Named("Volt4Skill"), self);
                        hediff_Magnetize.level = (int) self.GetLevel();
                        hediff_Magnetize.damage = 3;
                        pawn.health.AddHediff(hediff_Magnetize);
                        WarframeStaticMethods.ShowDamageAmount(pawn, damage.ToString("f0"));
                        pawn.stances.stunner.StunFor((int) (180 * (1 + (self.GetLevel() * 1f / 30f))), self);
                    }

                    affected.Add(pawn);
                }
            }
        }


        public List<IntVec3> MagNowCellsAround(IntVec3 pos, Map map, float nowrange)
        {
            var result = new List<IntVec3>();
            if (!pos.InBounds(map))
            {
                return result;
            }

            var region = pos.GetRegion(map);
            if (region == null)
            {
                return result;
            }

            RegionTraverser.BreadthFirstTraverse(region, (_, r) => r.door == null, delegate(Region r)
            {
                foreach (var item in r.Cells)
                {
                    if (item.InHorDistOf(pos, nowrange) && !item.InHorDistOf(pos, nowrange - 1))
                    {
                        result.Add(item);
                    }
                }

                return false;
            }, (int) nowrange);
            return result;
        }
    }
}