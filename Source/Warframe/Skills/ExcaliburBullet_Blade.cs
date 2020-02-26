using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Warframe.Skills
{
    class ExcaliburBullet_Blade : Projectile
    {
        public List<Thing> hitThings = new List<Thing>();

        public override void Tick()
        {
            base.Tick();
            if (outRange()) {
                hitThings.Clear();
                this.Destroy(DestroyMode.Vanish);
                return;
            }
           
                IEnumerable<Thing> ts = this.Map.thingGrid.ThingsAt(this.ExactPosition.ToIntVec3());
                
                foreach (Thing t in ts)
                {
                    if ((t is Pawn&&t!=this.launcher) || t is Building)
                    {
                     if (!this.hitThings.Contains(t))
                     {
                      //  Log.Warning(this.hitThings+" not contains "+t);
                        aImpact(t);
                        this.hitThings.Add(t);
                      }
                    }
                }
                
            

        }

        private bool outRange() {
            float maxRange = this.def.projectile.explosionRadius;
            Vector3 p = this.origin;
            Vector3 p2 = this.ExactPosition;
            if (!p2.InBounds(this.launcher.Map)) return false;
            
            float value = (float)Math.Sqrt(Math.Abs(p.x - p2.x) * Math.Abs(p.x - p2.x) + Math.Abs(p.z - p2.z) * Math.Abs(p.z - p2.z));

            return value >= maxRange;
        }

        protected override void Impact(Thing hitThing) {

        }

        protected void aImpact(Thing hitThing)
        {

            Map map = base.Map;
            GenClamor.DoClamor(this, 2.1f, ClamorDefOf.Impact);
            BattleLogEntry_RangedImpact battleLogEntry_RangedImpact = new BattleLogEntry_RangedImpact(this.launcher, hitThing, this.intendedTarget.Thing, this.equipmentDef, this.def, this.targetCoverDef);
            Find.BattleLog.Add(battleLogEntry_RangedImpact);
            if (hitThing != null)
            {
                if(hitThing is Pawn)
                {
                    if((hitThing as Pawn).Faction == this.launcher.Faction)
                    {
                        return;
                    }
                }

                DamageDef damageDef = this.def.projectile.damageDef;
                float amount = (float)base.DamageAmount;
                float armorPenetration = base.ArmorPenetration;
                float y = this.ExactRotation.eulerAngles.y;
                Thing launcher = this.launcher;
                ThingDef equipmentDef = this.equipmentDef;
                DamageInfo dinfo = new DamageInfo(damageDef, amount, armorPenetration, y, launcher, null, equipmentDef, DamageInfo.SourceCategory.ThingOrUnknown, this.intendedTarget.Thing);
                WarframeStaticMethods.showDamageAmount(hitThing, amount.ToString("f0"));
                hitThing.TakeDamage(dinfo).AssociateWithLog(battleLogEntry_RangedImpact);
                try
                {
                    Pawn pawn = hitThing as Pawn;
                    if (pawn != null && pawn.stances != null && pawn.BodySize <= this.def.projectile.StoppingPower + 0.001f)
                    {
                        pawn.stances.StaggerFor(95);
                    }
                }catch (Exception) { }
            }
            /*
            else
            {
               // SoundDefOf.BulletImpact_Ground.PlayOneShot(new TargetInfo(base.Position, map, false));
              //  MoteMaker.MakeStaticMote(this.ExactPosition, map, ThingDefOf.Mote_ShotHit_Dirt, 1f);
                if (base.Position.GetTerrain(map).takeSplashes)
                {
                    MoteMaker.MakeWaterSplash(this.ExactPosition, map, Mathf.Sqrt((float)base.DamageAmount) * 1f, 4f);
                }
            }
            */
        }
    }
}
