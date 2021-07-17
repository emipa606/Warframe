using System;
using System.Collections.Generic;
using System.Reflection;
using RimWorld;
using UnityEngine;
using Verse;

namespace Warframe.Skills.Volts
{
    public class Volt3SkillThing : ThingWithComps
    {
        public List<IntVec3> dire;
        public float rotat;
        public Pawn self;

        public int ticks;
        // public List<Projectile> affected;


        private bool startBomb => ticks >= 40;

        private int maxTick => (int) (600 * (1 + (self.GetLevel() * 1f / 20f)));

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref self, "self");
            Scribe_Values.Look(ref ticks, "ticks");
            Scribe_Values.Look(ref rotat, "rotat");
            //  Scribe_Collections.Look<Projectile>(ref this.affected,"affected",LookMode.Reference, new object[0]);
            Scribe_Collections.Look(ref dire, "dire");
        }

        public override void Draw()
        {
            if (!startBomb)
            {
                return;
            }

            //base.Draw();
            Comps_PostDraw();
            var matrix4x = default(Matrix4x4);
            matrix4x.SetTRS(DrawPos + Altitudes.AltIncVect, rotat.ToQuat(), new Vector3(1.5f, 1, 1.5f));
            Graphics.DrawMesh(MeshPool.plane20, matrix4x, MaterialPool.MatFrom(def.graphicData.texPath), 0);
        }

        public override void Tick()
        {
            base.Tick();
            if (ticks > maxTick)
            {
                Destroy();
            }

            if (!Spawned)
            {
                return;
            }

            ticks++;
            /*
            if (ticks == 49) {
               
                this.affected = new List<Projectile>();
            }
            */
            //GenDraw.DrawFieldEdges(dire);


            var vlist = Map.listerThings.ThingsOfDef(ThingDef.Named("VoltSkill3Item"));
            foreach (var thing in vlist)
            {
                var sh = (Volt3SkillThing) thing;
                if (vlist.Count <= 6)
                {
                    break;
                }

                try
                {
                    sh.Destroy();
                }
                catch (Exception)
                {
                    // ignored
                }
            }


            if (!startBomb)
            {
                return;
            }

            foreach (var ic in dire)
            {
                foreach (var th in Map.thingGrid.ThingsAt(ic))
                {
                    if (th is not Projectile pj)
                    {
                        continue;
                    }

                    if (pj.def.projectile.flyOverhead)
                    {
                        continue;
                    }

                    // if (this.affected.Contains(pj)) continue;
                    try
                    {
                        var ty = pj.GetType();
                        var pi = ty.GetField("launcher",
                            BindingFlags.NonPublic |
                            BindingFlags
                                .Instance); //GetProperty("launcher",BindingFlags.|BindingFlags.Instance);
                        if (pi is not null)
                        {
                            var launcher = (Pawn) pi.GetValue(pj);
                            if (launcher.Faction.HostileTo(self.Faction))
                            {
                                var mi = ty.GetMethod("Impact", BindingFlags.NonPublic | BindingFlags.Instance);
                                if (mi != null)
                                {
                                    mi.Invoke(pj, new object[] {this});
                                }

                                if (!pj.Destroyed)
                                {
                                    pj.Destroy();
                                }
                            }
                        }

                        // this.affected.Add(pj);
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }
            }
        }


        public List<IntVec3> VoltNowCellsAround(IntVec3 pos, Map map, float nowrange)
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
                    float num1 = (item.x - pos.x) * (item.x - pos.x);
                    float num2 = (item.z - pos.z) * (item.z - pos.z);
                    var num3 = Mathf.Sqrt(num1 + num2);
                    // Log.Warning(num3 + "/" + nowrange);
                    if (num3 - 1f <= nowrange && num3 + 1f >= nowrange)
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