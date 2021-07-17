using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Verse;

namespace Warframe.Skills.Mags
{
    public class Mag3SkillThing : ThingWithComps
    {
        public List<Pawn> affected;
        public IntVec3 bombPos;
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
            Scribe_Values.Look(ref bombPos, "bombPos");
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
                bombPos = self.Position;
                affected = new List<Pawn>();
            }


            if (!startBomb)
            {
                return;
            }

            GenDraw.DrawFieldEdges(MagNowCellsAround(bombPos, Map, range * ((ticks - 60) * 1f / 180f)),
                new Color(0.2f, 0.8f, 1));

            foreach (var ic in MagNowCellsAround(bombPos, Map, range * ((ticks - 60) * 1f / 180f)))
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

                    if (pawn.Faction != self.Faction)
                    {
                        var dinfo = new DamageInfo(DefDatabase<DamageDef>.GetNamed("Mag"), damage, 0, -1, self,
                            null, null, DamageInfo.SourceCategory.ThingOrUnknown, pawn);

                        pawn.PreApplyDamage(ref dinfo, out var absorbed);
                        if (absorbed)
                        {
                            try
                            {
                                foreach (var ap in pawn.apparel.WornApparel)
                                {
                                    var type = ap.GetType();
                                    // Log.Warning(type+":"+ap);
                                    var method = type.GetMethod("Break",
                                        BindingFlags.NonPublic | BindingFlags.Instance);
                                    // Log.Warning(method+"?");

                                    if (method == null)
                                    {
                                        continue;
                                    }

                                    method.Invoke(ap, null);
                                    break;
                                }
                            }
                            catch (Exception)
                            {
                                //  Log.Warning("ERROR");
                            }

                            for (var j = 1; j < 9; j++)
                            {
                                var projectile2 =
                                    (Projectile) GenSpawn.Spawn(ThingDef.Named("Bullet_MagBullet"), pawn.Position,
                                        Map);
                                var hitTypes = ProjectileHitFlags.All;
                                Thing gun = null;
                                if (pawn.equipment is {Primary: { }})
                                {
                                    gun = pawn.equipment.Primary;
                                }

                                projectile2.Launch(self, pawn.Position.ToVector3(), Map.cellsInRandomOrder.Get(j),
                                    pawn, hitTypes, false, gun);
                            }
                        }

                        pawn.TakeDamage(dinfo);
                        WarframeStaticMethods.ShowDamageAmount(pawn, damage.ToString("f0"));
                    }
                    else if (pawn.Faction == self.Faction && pawn.IsWarframe())
                    {
                        try
                        {
                            var wb = WarframeStaticMethods.GetBelt(pawn);

                            wb.AddEnergy(damage * (1 + (pawn.GetLevel() / 30f)));
                            WarframeStaticMethods.ShowColorText(pawn,
                                "Shield+" + (damage * (1 + (pawn.GetLevel() / 30f))).ToString("f0"),
                                new Color(0.2f, 0.4f, 0.8f), GameFont.Small);
                        }
                        catch (Exception)
                        {
                            // ignored
                        }
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
                    /*
                    float num1 = (item.x-pos.x)*(item.x-pos.x);
                    float num2 = (item.z - pos.z) * (item.z - pos.z);
                    float num3 = Mathf.Sqrt(num1+num2);
                   // Log.Warning(num3 + "/" + nowrange);
                    if (num3-1f <= nowrange && num3+1f>=nowrange)
                        result.Add(item);
                    */
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