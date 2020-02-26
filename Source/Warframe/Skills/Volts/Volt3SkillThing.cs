using RimWorld;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using Verse;

namespace Warframe.Skills.Volts
{
    public class Volt3SkillThing:ThingWithComps
    {
        public List<IntVec3> dire;
        public Pawn self;
        public float rotat;
        public int ticks;
       // public List<Projectile> affected;


        private bool startBomb
        {
            get
            {
                return this.ticks >= 40;
            }
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look<Pawn>(ref this.self,"self",false);
            Scribe_Values.Look<int>(ref this.ticks, "ticks", 0, false);
            Scribe_Values.Look<float>(ref this.rotat, "rotat", 0, false);
          //  Scribe_Collections.Look<Projectile>(ref this.affected,"affected",LookMode.Reference, new object[0]);
            Scribe_Collections.Look<IntVec3>(ref this.dire, "dire", LookMode.Undefined, new object[0]);

        }
        public override void Draw()
        {
            if (!startBomb) { return; }
            //base.Draw();
            this.Comps_PostDraw();
            Matrix4x4 matrix4x = default(Matrix4x4);
            matrix4x.SetTRS(this.DrawPos + Altitudes.AltIncVect,rotat.ToQuat(), new Vector3(1.5f,1,1.5f));
            Graphics.DrawMesh(MeshPool.plane20, matrix4x, MaterialPool.MatFrom(this.def.graphicData.texPath), 0);


        }
        private int maxTick{
            get
            {
                return (int)(600 * (1 + (self.getLevel() * 1f / 20f)));
            }
        }
        public override void Tick()
        {
            base.Tick();
            if (ticks > maxTick) this.Destroy();
            if (!this.Spawned) return;

            this.ticks++;
            /*
            if (ticks == 49) {
               
                this.affected = new List<Projectile>();
            }
            */
            //GenDraw.DrawFieldEdges(dire);



            List<Thing> vlist = this.Map.listerThings.ThingsOfDef(ThingDef.Named("VoltSkill3Item"));
            foreach (Volt3SkillThing sh in vlist)
            {
                if (vlist.Count <= 6) break;
                try { sh.Destroy(DestroyMode.Vanish); }
                catch (Exception) { }
            }




            if (startBomb)
            {
                foreach(IntVec3 ic in this.dire)
                {
                    foreach(Thing th in this.Map.thingGrid.ThingsAt(ic))
                    {
                        Projectile pj = th as Projectile;
                        if (pj != null)
                        {
                            if (pj.def.projectile.flyOverhead) continue;
                           // if (this.affected.Contains(pj)) continue;
                            try
                            {
                               
                                Type ty = pj.GetType();
                                FieldInfo pi = ty.GetField("launcher", BindingFlags.NonPublic | BindingFlags.Instance);//GetProperty("launcher",BindingFlags.|BindingFlags.Instance);
                                Pawn launcher = (Pawn)pi.GetValue(pj);
                                if (launcher.Faction.HostileTo(self.Faction))
                                {
                                    
                                    MethodInfo mi = ty.GetMethod("Impact",BindingFlags.NonPublic|BindingFlags.Instance);
                                    if (mi != null)
                                        mi.Invoke(pj,new object[] { this});

                                    if(pj!=null && !pj.Destroyed)
                                      pj.Destroy(DestroyMode.Vanish);
                                }
                               // this.affected.Add(pj);


                            }
                            catch (Exception)
                            {

                            }
                        }
                        
                    }
                }
            }


            
            
        }


        public List<IntVec3> VoltNowCellsAround(IntVec3 pos, Map map,float nowrange)
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
                    float num1 = (item.x-pos.x)*(item.x-pos.x);
                    float num2 = (item.z - pos.z) * (item.z - pos.z);
                    float num3 = Mathf.Sqrt(num1+num2);
                   // Log.Warning(num3 + "/" + nowrange);
                    if (num3-1f <= nowrange && num3+1f>=nowrange)
                        result.Add(item);
                }
                return false;
            }, (int)nowrange, RegionType.Set_Passable);
            return result;
        }
    }
}
