using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace Warframe.Skills.Volts
{
    public class Hediff_Volt4Skill : HediffWithComps
    {

        public int level;
        public float damage;
        public override void Tick()
        {
            this.ageTicks++;
            if(this.ageTicks> getMaxTick   )
            {
                this.TimeOut();
            }
            if(this.ageTicks%30==0)
              DrawHediffExtras();
 
           
        }
        

        // Token: 0x06004BF8 RID: 19448 RVA: 0x00232324 File Offset: 0x00230724
        private void TimeOut()
        {
            this.pawn.health.RemoveHediff(this);
        }

       
        private float getMaxTick
        {
            get
            {
                return (300 * (1 + (level*1f / 30f)));
            }
        }
        public void DrawHediffExtras()
        {
            DamageInfo dinfo = new DamageInfo(DefDatabase<DamageDef>.GetNamed("Mag",true),damage,0,-1,this.pawn,null,null,DamageInfo.SourceCategory.ThingOrUnknown,this.pawn);
            this.pawn.TakeDamage(dinfo);
            WarframeStaticMethods.showDamageAmount(pawn, damage.ToString("f0"));
            MoteMaker.ThrowExplosionInteriorMote(new Vector3(this.pawn.TrueCenter().x,0,this.pawn.TrueCenter().z),this.pawn.Map,ThingDefOf.Mote_ShotHit_Spark);
        }


        // Token: 0x06004BFA RID: 19450 RVA: 0x002324EB File Offset: 0x002308EB
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.level,"level",0,false);
            Scribe_Values.Look<float>(ref this.damage, "damage", 0, false);
        }





    }
}
