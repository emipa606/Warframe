using RimWorld;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using Verse;

namespace Warframe.Skills.Mags
{
    public class Mag3SkillThing:ThingWithComps
    {
        public int range;
        public Pawn self;
        public IntVec3 bombPos;
        public int ticks;
        public List<Pawn> affected;
        public float damage;
        private bool startBomb
        {
            get
            {
                return ticks >= 60;
            }
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look<Pawn>(ref self,"self",false);
            Scribe_Values.Look<int>(ref range,"range",0,false);
            Scribe_Values.Look<int>(ref ticks, "ticks", 0, false);
            Scribe_Values.Look<float>(ref damage, "damage", 0, false);
            Scribe_Values.Look<IntVec3>(ref bombPos, "bombPos", default(IntVec3), false);
            Scribe_Collections.Look<Pawn>(ref affected,"affected",LookMode.Reference, new object[0]);


        }
        public override void Draw()
        {
            //base.Draw();
        }
        public override void Tick()
        {
            base.Tick();
            if (ticks > 240) Destroy();
            if (!Spawned) return;

            ticks++;
            if (ticks == 59) { bombPos = self.Position;
                affected = new List<Pawn>();
            }

           

            
            if (startBomb )
            {
                GenDraw.DrawFieldEdges(MagNowCellsAround(bombPos, Map, range * ((ticks - 60) * 1f / 180f)),new Color(0.2f,0.8f,1));

                foreach(IntVec3 ic in MagNowCellsAround(bombPos, Map, range * ((ticks-60) * 1f / 180f)))
                {
                    foreach(Thing th in Map.thingGrid.ThingsAt(ic))
                    {
                        if(th is Pawn)
                        {
                            Pawn pa = th as Pawn;
                            if (affected.Contains(pa)||pa==self) continue;

                            if (pa.Faction != self.Faction)
                            {
                                DamageInfo dinfo = new DamageInfo(DefDatabase<DamageDef>.GetNamed("Mag",true),damage,0,-1,self,null,null,DamageInfo.SourceCategory.ThingOrUnknown,pa);

                                bool absorbed;
                                pa.PreApplyDamage(ref dinfo,out absorbed);
                                if (absorbed)
                                {
                                    try {
                                        foreach(Apparel ap in pa.apparel.WornApparel)
                                        {
                                            Type type = ap.GetType();
                                           // Log.Warning(type+":"+ap);
                                            MethodInfo method = type.GetMethod("Break", BindingFlags.NonPublic | BindingFlags.Instance);
                                            // Log.Warning(method+"?");

                                            if (method != null)
                                            {
                                                method.Invoke(ap, null);
                                                break;
                                            }
                                        }

                                    } catch (Exception)
                                    {
                                      //  Log.Warning("ERROR");
                                    }
                                    for (int j = 1; j < 9; j++)
                                    {
                                        Projectile projectile2 = (Projectile)GenSpawn.Spawn(ThingDef.Named("Bullet_MagBullet"), pa.Position, Map, WipeMode.Vanish);
                                        ProjectileHitFlags projectileHitFlags = ProjectileHitFlags.All;
                                        Thing gun = null;
                                        if (pa.equipment != null && pa.equipment.Primary != null) gun = pa.equipment.Primary;
                                        
                                        projectile2.Launch(self, pa.Position.ToVector3(), Map.cellsInRandomOrder.Get(j), pa, projectileHitFlags, gun, null);
                                    }
                                }
                                pa.TakeDamage(dinfo);
                                WarframeStaticMethods.ShowDamageAmount(pa, damage.ToString("f0"));
                            }
                            else if(pa.Faction==self.Faction && pa.IsWarframe())
                            {
                                try
                                {
                                    WarframeBelt wb = WarframeStaticMethods.GetBelt(pa);
                                    
                                    wb.AddEnergy(damage*(1 + pa.GetLevel()/30f));
                                    WarframeStaticMethods.ShowColorText(pa,"Shield+"+(damage * (1 + pa.GetLevel() / 30f)).ToString("f0"),new Color(0.2f,0.4f,0.8f),GameFont.Small);
                                }
                                catch (Exception) { }
                            }
                            affected.Add(pa);


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
                    /*
                    float num1 = (item.x-pos.x)*(item.x-pos.x);
                    float num2 = (item.z - pos.z) * (item.z - pos.z);
                    float num3 = Mathf.Sqrt(num1+num2);
                   // Log.Warning(num3 + "/" + nowrange);
                    if (num3-1f <= nowrange && num3+1f>=nowrange)
                        result.Add(item);
                    */
                    if (item.InHorDistOf(pos, nowrange) && !item.InHorDistOf(pos,nowrange-1))
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
