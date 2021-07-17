using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Warframe.Skills
{
    internal class ExcaliburBullet_Blade : Projectile
    {
        public List<Thing> hitThings = new List<Thing>();

        public override void Tick()
        {
            base.Tick();
            if (outRange())
            {
                hitThings.Clear();
                Destroy();
                return;
            }

            var ts = Map.thingGrid.ThingsAt(ExactPosition.ToIntVec3());

            foreach (var t in ts)
            {
                if ((t is not Pawn || t == launcher) && t is not Building)
                {
                    continue;
                }

                if (hitThings.Contains(t))
                {
                    continue;
                }

                //  Log.Warning(this.hitThings+" not contains "+t);
                aImpact(t);
                hitThings.Add(t);
            }
        }

        private bool outRange()
        {
            var maxRange = def.projectile.explosionRadius;
            var p = origin;
            var p2 = ExactPosition;
            if (!p2.InBounds(launcher.Map))
            {
                return false;
            }

            var value = (float) Math.Sqrt((Math.Abs(p.x - p2.x) * Math.Abs(p.x - p2.x)) +
                                          (Math.Abs(p.z - p2.z) * Math.Abs(p.z - p2.z)));

            return value >= maxRange;
        }

        protected override void Impact(Thing hitThing)
        {
        }

        protected void aImpact(Thing hitThing)
        {
            var unused = Map;
            GenClamor.DoClamor(this, 2.1f, ClamorDefOf.Impact);
            var instigator = launcher;
            var weaponDef = equipmentDef;
            var battleLogEntry_RangedImpact = new BattleLogEntry_RangedImpact(instigator, hitThing,
                intendedTarget.Thing,
                weaponDef, def, targetCoverDef);
            Find.BattleLog.Add(battleLogEntry_RangedImpact);
            if (hitThing == null)
            {
                return;
            }

            if (hitThing is Pawn thing)
            {
                if (thing.Faction == launcher.Faction)
                {
                    return;
                }
            }

            var damageDef = def.projectile.damageDef;
            float amount = DamageAmount;
            var armorPenetration = ArmorPenetration;
            var y = ExactRotation.eulerAngles.y;
            instigator = launcher;
            weaponDef = equipmentDef;
            var dinfo = new DamageInfo(damageDef, amount, armorPenetration, y, instigator, null, weaponDef,
                DamageInfo.SourceCategory.ThingOrUnknown, intendedTarget.Thing);
            WarframeStaticMethods.ShowDamageAmount(hitThing, amount.ToString("f0"));
            hitThing.TakeDamage(dinfo).AssociateWithLog(battleLogEntry_RangedImpact);
            try
            {
                if (hitThing is Pawn {stances: { }} pawn && pawn.BodySize <= def.projectile.StoppingPower + 0.001f)
                {
                    pawn.stances.StaggerFor(95);
                }
            }
            catch (Exception)
            {
                // ignored
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