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
            if (!this.Spawned) return;
          
           if(this.ExactPosition.InBounds(base.Map))
           foreach(Thing th in this.Map.thingGrid.ThingsAt(this.ExactPosition.ToIntVec3()))
            {
                if(th is Pawn)
                {
                    aImpact(th);
                }
            }
        }

        protected override void Impact(Thing hitThing) { this.Destroy(DestroyMode.Vanish); }

        protected void aImpact(Thing hitThing)
        {

            Map map = base.Map;
            GenClamor.DoClamor(this, 2.1f, ClamorDefOf.Impact);
            BattleLogEntry_RangedImpact battleLogEntry_RangedImpact = new BattleLogEntry_RangedImpact(this.launcher, hitThing, this.intendedTarget.Thing, this.equipmentDef, this.def, this.targetCoverDef);
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
                

                DamageDef damageDef = this.def.projectile.damageDef;
                float amount = (float)base.DamageAmount;
                float armorPenetration = base.ArmorPenetration;
                float y = this.ExactRotation.eulerAngles.y;
                Thing launcher = this.launcher;
                ThingDef equipmentDef = this.equipmentDef;

                BodyPartRecord bpr = tg.health.hediffSet.GetRandomNotMissingPart(damageDef,BodyPartHeight.Undefined,BodyPartDepth.Inside);

                DamageInfo dinfo = new DamageInfo(damageDef, amount, armorPenetration, y, launcher, null, equipmentDef, DamageInfo.SourceCategory.ThingOrUnknown, this.intendedTarget.Thing);
                dinfo.SetHitPart(bpr);
                WarframeStaticMethods.showDamageAmount(hitThing, amount.ToString("f0"));
                hitThing.TakeDamage(dinfo).AssociateWithLog(battleLogEntry_RangedImpact);
                try
                {
                    Pawn pawn = hitThing as Pawn;
                    if (pawn != null && pawn.stances != null && pawn.BodySize <= this.def.projectile.StoppingPower + 0.001f)
                    {
                        pawn.stances.StaggerFor(95);
                    }
                }
                catch (Exception) { }
                this.Destroy(DestroyMode.Vanish);
            }

        }
    }
}
