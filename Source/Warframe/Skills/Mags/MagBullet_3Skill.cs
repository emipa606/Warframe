using System;
using RimWorld;
using Verse;

namespace Warframe.Skills.Mags
{
    public class MagBullet_3Skill : Projectile
    {
        public override void Tick()
        {
            base.Tick();
            if (!Spawned)
            {
                return;
            }

            if (!ExactPosition.InBounds(Map))
            {
                return;
            }

            foreach (var th in Map.thingGrid.ThingsAt(ExactPosition.ToIntVec3()))
            {
                if (th is Pawn)
                {
                    aImpact(th);
                }
            }
        }

        protected override void Impact(Thing hitThing)
        {
            Destroy();
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

            var tg = hitThing as Pawn;


            var damageDef = def.projectile.damageDef;
            float amount = DamageAmount;
            var armorPenetration = ArmorPenetration;
            var y = ExactRotation.eulerAngles.y;
            instigator = launcher;
            weaponDef = equipmentDef;

            var bpr = tg?.health.hediffSet.GetRandomNotMissingPart(damageDef, BodyPartHeight.Undefined,
                BodyPartDepth.Inside);

            var dinfo = new DamageInfo(damageDef, amount, armorPenetration, y, instigator, null, weaponDef,
                DamageInfo.SourceCategory.ThingOrUnknown, intendedTarget.Thing);
            dinfo.SetHitPart(bpr);
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

            Destroy();
        }
    }
}