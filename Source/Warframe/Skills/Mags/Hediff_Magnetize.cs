using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace Warframe.Skills.Mags
{
    public class Hediff_Magnetize : HediffWithComps
    {




      
        public override void Tick()
        {
            this.ageTicks++;
            if(this.ageTicks> (600 * (1 + ((WarframeStaticMethods.getWFLevel(self) * 1.0f) / 60f)))   )
            {
                this.TimeOut();
            }

            DrawHediffExtras();

            
            //GenDraw.DrawFieldEdges(this.CellsAdjacent8WayAndInsidePlus(this.pawn).ToList());
            foreach (IntVec3 ic in this.CellsAdjacent8WayAndInsidePlus(this.pawn))
            {
                if (ic == this.pawn.Position) continue;

                foreach(Thing th in this.pawn.Map.thingGrid.ThingsAt(ic))
                {
                    if(th is Projectile )
                    {
                        if ((th as Projectile).def.projectile.flyOverhead) continue;

                        //bullets.Add(th as Projectile);
                        
                        ThingDef bdef = (th as Projectile).def;

                        

                        th.Destroy();
                        
                        Projectile projectile2 = (Projectile)GenSpawn.Spawn(bdef, this.pawn.Position, this.pawn.Map, WipeMode.Vanish);
                        
                        ProjectileHitFlags projectileHitFlags = ProjectileHitFlags.All;
                        Thing gun = null;
                        if (pawn.equipment != null && pawn.equipment.Primary != null) gun = pawn.equipment.Primary;
                        projectile2.Launch(self, this.pawn.Position.ToVector3(), this.pawn.Position, this.pawn, projectileHitFlags, gun, null);
                        
                    }
                }
            }

            

           
        }

        // Token: 0x06004BF8 RID: 19448 RVA: 0x00232324 File Offset: 0x00230724
        private void TimeOut()
        {
            this.pawn.health.RemoveHediff(this);
        }

        private IEnumerable<IntVec3> CellsAdjacent8WayAndInsidePlus(Thing thing)
        {
            IntVec3 center = thing.Position;
            IntVec2 size = thing.def.size;
            Rot4 rotation = thing.Rotation;
            GenAdj.AdjustForRotation(ref center, ref size, rotation);
            int minX = center.x - (size.x - 3) / 2 - 3;
            int minZ = center.z - (size.z - 3) / 2 - 3;
            int maxX = minX + size.x + 3;
            int maxZ = minZ + size.z + 3;
            for (int i = minX; i <= maxX; i++)
            {
                for (int j = minZ; j <= maxZ; j++)
                {
                    yield return new IntVec3(i, 0, j);
                }
            }
            yield break;
        }
        private float getMaxTick
        {
            get
            {
                return (600 * (1 + ((WarframeStaticMethods.getWFLevel(self) * 1.0f) / 60f)));
            }
        }
        public void DrawHediffExtras()
        {

            float num = 6f;//Mathf.Lerp(1.8f, 1.2f, (this.ageTicks)*1.0f/getMaxTick);
                Vector3 vector = this.pawn.Drawer.DrawPos;
                vector.y = AltitudeLayer.MoteOverhead.AltitudeFor();

                float angle = (float)Rand.Range(0, 360);
                Vector3 s = new Vector3(num, 1f, num);
                Matrix4x4 matrix = default(Matrix4x4);
                matrix.SetTRS(vector, Quaternion.AngleAxis(angle, Vector3.up), s);
                Graphics.DrawMesh(MeshPool.plane10, matrix, MaterialPool.MatFrom("Other/ShieldBubble", ShaderDatabase.Transparent,new Color(0.2f,0.6f,0.8f)), 0);
            
        }


        // Token: 0x06004BFA RID: 19450 RVA: 0x002324EB File Offset: 0x002308EB
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look<Pawn>(ref this.self, "self", false);
        }



        // Token: 0x040033BA RID: 13242
        public Pawn self;



    }
}
