using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Warframe.Skills.Mags
{
    public class Hediff_Magnetize : HediffWithComps
    {
        // Token: 0x040033BA RID: 13242
        public Pawn self;

        private float getMaxTick => 600 * (1 + (WarframeStaticMethods.GetWFLevel(self) * 1.0f / 60f));


        public override void Tick()
        {
            ageTicks++;
            if (ageTicks > 600 * (1 + (WarframeStaticMethods.GetWFLevel(self) * 1.0f / 60f)))
            {
                TimeOut();
            }

            DrawHediffExtras();


            //GenDraw.DrawFieldEdges(this.CellsAdjacent8WayAndInsidePlus(this.pawn).ToList());
            foreach (var ic in CellsAdjacent8WayAndInsidePlus(pawn))
            {
                if (ic == pawn.Position)
                {
                    continue;
                }

                foreach (var th in pawn.Map.thingGrid.ThingsAt(ic))
                {
                    if (th is not Projectile projectile)
                    {
                        continue;
                    }

                    if (projectile.def.projectile.flyOverhead)
                    {
                        continue;
                    }

                    //bullets.Add(th as Projectile);

                    var bdef = projectile.def;


                    projectile.Destroy();

                    var projectile2 = (Projectile) GenSpawn.Spawn(bdef, pawn.Position, pawn.Map);

                    var hitTypes = ProjectileHitFlags.All;
                    Thing gun = null;
                    if (pawn.equipment is {Primary: { }})
                    {
                        gun = pawn.equipment.Primary;
                    }

                    projectile2.Launch(self, pawn.Position.ToVector3(), new LocalTargetInfo(pawn), pawn,
                        hitTypes, false, gun);
                }
            }
        }

        // Token: 0x06004BF8 RID: 19448 RVA: 0x00232324 File Offset: 0x00230724
        private void TimeOut()
        {
            pawn.health.RemoveHediff(this);
        }

        private IEnumerable<IntVec3> CellsAdjacent8WayAndInsidePlus(Thing thing)
        {
            var center = thing.Position;
            var size = thing.def.size;
            var rotation = thing.Rotation;
            GenAdj.AdjustForRotation(ref center, ref size, rotation);
            var minX = center.x - ((size.x - 3) / 2) - 3;
            var minZ = center.z - ((size.z - 3) / 2) - 3;
            var maxX = minX + size.x + 3;
            var maxZ = minZ + size.z + 3;
            for (var i = minX; i <= maxX; i++)
            {
                for (var j = minZ; j <= maxZ; j++)
                {
                    yield return new IntVec3(i, 0, j);
                }
            }
        }

        public void DrawHediffExtras()
        {
            var num = 6f; //Mathf.Lerp(1.8f, 1.2f, (this.ageTicks)*1.0f/getMaxTick);
            var vector = pawn.Drawer.DrawPos;
            vector.y = AltitudeLayer.MoteOverhead.AltitudeFor();

            float angle = Rand.Range(0, 360);
            var s = new Vector3(num, 1f, num);
            var matrix = default(Matrix4x4);
            matrix.SetTRS(vector, Quaternion.AngleAxis(angle, Vector3.up), s);
            Graphics.DrawMesh(MeshPool.plane10, matrix,
                MaterialPool.MatFrom("Other/ShieldBubble", ShaderDatabase.Transparent, new Color(0.2f, 0.6f, 0.8f)), 0);
        }


        // Token: 0x06004BFA RID: 19450 RVA: 0x002324EB File Offset: 0x002308EB
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref self, "self");
        }
    }
}