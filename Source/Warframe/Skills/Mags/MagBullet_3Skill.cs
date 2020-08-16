using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using Verse;

namespace Warframe.Skills.Mags
{
    public class MagBullet_3Skill : Projectile
    {

        public override void Tick()
        {
            base.Tick();
            if (!Spawned) return;
          
           if(ExactPosition.InBounds(Map))
           foreach(Thing th in Map.thingGrid.ThingsAt(ExactPosition.ToIntVec3()))
            {
                if(th is Pawn)
                {
                    aImpact(th);
                }
            }
        }

        protected override void Impact(Thing hitThing) { Destroy(DestroyMode.Vanish); }

        protected void aImpact(Thing hitThing)
        {

            Map map = Map;
            GenClamor.DoClamor(this, 2.1f, ClamorDefOf.Impact);
            BattleLogEntry_RangedImpact battleLogEntry_RangedImpact = new BattleLogEntry_RangedImpact(launcher, hitThing, intendedTarget.Thing, equipmentDef, def, targetCoverDef);
            Find.BattleLog.Add(battleLogEntry_RangedImpact);
            if (hitThing != null)
            {
                if (hitThing is Pawn)
                {
                    if ((hitThing as Pawn).Faction == this.launcher.Faction)
                    {
                        return;
                    }
                }
                Pawn tg = hitThing as Pawn;
                

                DamageDef damageDef = def.projectile.damageDef;
                float amount = (float)DamageAmount;
                float armorPenetration = ArmorPenetration;
                float y = ExactRotation.eulerAngles.y;
                Thing launcher = this.launcher;
                ThingDef equipmentDef = this.equipmentDef;

                BodyPartRecord bpr = tg.health.hediffSet.GetRandomNotMissingPart(damageDef,BodyPartHeight.Undefined,BodyPartDepth.Inside);

                DamageInfo dinfo = new DamageInfo(damageDef, amount, armorPenetration, y, launcher, null, equipmentDef, DamageInfo.SourceCategory.ThingOrUnknown, intendedTarget.Thing);
                dinfo.SetHitPart(bpr);
                WarframeStaticMethods.ShowDamageAmount(hitThing, amount.ToString("f0"));
                hitThing.TakeDamage(dinfo).AssociateWithLog(battleLogEntry_RangedImpact);
                try
                {
                    if (hitThing is Pawn pawn && pawn.stances != null && pawn.BodySize <= def.projectile.StoppingPower + 0.001f)
                    {
                        pawn.stances.StaggerFor(95);
                    }
                }
                catch (Exception) { }
                Destroy(DestroyMode.Vanish);
            }

        }
    }
}
