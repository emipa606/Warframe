using RimWorld;
using UnityEngine;
using Verse;

namespace Warframe.Skills.Volts
{
    public class Hediff_Volt4Skill : HediffWithComps
    {
        public float damage;

        public int level;


        private float getMaxTick => 300 * (1 + (level * 1f / 30f));

        public override void Tick()
        {
            ageTicks++;
            if (ageTicks > getMaxTick)
            {
                TimeOut();
            }

            if (ageTicks % 30 == 0)
            {
                DrawHediffExtras();
            }
        }


        // Token: 0x06004BF8 RID: 19448 RVA: 0x00232324 File Offset: 0x00230724
        private void TimeOut()
        {
            pawn.health.RemoveHediff(this);
        }

        public void DrawHediffExtras()
        {
            var dinfo = new DamageInfo(DefDatabase<DamageDef>.GetNamed("Mag"), damage, 0, -1, pawn, null, null,
                DamageInfo.SourceCategory.ThingOrUnknown, pawn);
            pawn.TakeDamage(dinfo);
            WarframeStaticMethods.ShowDamageAmount(pawn, damage.ToString("f0"));
            FleckMaker.ThrowExplosionInterior(new Vector3(pawn.TrueCenter().x, 0, pawn.TrueCenter().z), pawn.Map,
                FleckDefOf.MicroSparks);
        }


        // Token: 0x06004BFA RID: 19450 RVA: 0x002324EB File Offset: 0x002308EB
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref level, "level");
            Scribe_Values.Look(ref damage, "damage");
        }
    }
}