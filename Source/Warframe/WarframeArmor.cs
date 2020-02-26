using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using Verse;

namespace Warframe
{
    public class WarframeArmor:Apparel
    {
        public float cooldownTime1=0;
        public float cooldownTime2=0;
        public float cooldownTime3=0;
        public float cooldownTime4=0;

        public List<ThingWithComps> oldWeapon=new List<ThingWithComps>();

        public int tillSkillOpen = 0;
        public float tillSkillMul = 1;


        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<float>(ref this.cooldownTime1, "cooldownTime1", 0, false);
            Scribe_Values.Look<float>(ref this.cooldownTime2, "cooldownTime2", 0, false);
            Scribe_Values.Look<float>(ref this.cooldownTime3, "cooldownTime3", 0, false);
            Scribe_Values.Look<float>(ref this.cooldownTime4, "cooldownTime4", 0, false);
            Scribe_Values.Look<float>(ref this.tillSkillMul, "tillSkillMul", 1, false);
            Scribe_Values.Look<int>(ref this.tillSkillOpen, "tillSkillOpen", 0, false);
            Scribe_Collections.Look<ThingWithComps>(ref this.oldWeapon, "oldWeapon", LookMode.Deep, new object[0]);
            


        }

        public override void Tick()
        {

            base.Tick();
            
            if (this.cooldownTime1 > 0f) this.cooldownTime1 -= ((float)(1f / 60f));
            if (this.cooldownTime2 > 0f) this.cooldownTime2 -= ((float)(1f / 60f));
            if (this.cooldownTime3 > 0f) this.cooldownTime3 -= ((float)(1f / 60f));
            if (this.cooldownTime4 > 0f) this.cooldownTime4 -= ((float)(1f / 60f));


            if (this.tillSkillOpen>0)
            {
              if (!WarframeStaticMethods.consumeSP(this.Wearer, this.tillSkillMul,this.tillSkillOpen))
                {
                    WarframeStaticMethods.getSkillEndAction(this.Wearer,tillSkillOpen);
                    this.tillSkillOpen = 0;
                    this.tillSkillMul = 1;
                }
            }

        }


        public override IEnumerable<Gizmo> GetWornGizmos()
        {
            if (Find.Selector.SingleSelectedThing != base.Wearer)
            {
                yield break;
            }

         
            Pawn pawn = this.Wearer;
            if (pawn == null || !pawn.isWarframe()) yield break;
            String def = pawn.kindDef.defName.Replace("Warframe_", "");


            

            Command_CastSkill ck1 = WarframeStaticMethods.getSkillCommand(def, 1);
            ck1.self = this.Wearer;
            ck1.defaultDesc = (def+"Skill1.desc").Translate() + "\n" + "SP:" + WarframeStaticMethods.getArmor(this.Wearer).TryGetComp<CompWarframeSkill>().skill1mana;
            ck1.disabled = (!this.Wearer.Drafted|| this.cooldownTime1 > 0 || this.Wearer.stances.stunner.Stunned || WarframeStaticMethods.getBelt(this.Wearer).SP < this.getSkillComp().skill1mana) && this.tillSkillOpen != 1;


            yield return ck1;

            Command_CastSkill ck2 = WarframeStaticMethods.getSkillCommand(def, 2);
            ck2.self = this.Wearer;
            ck2.defaultDesc = (def + "Skill2.desc").Translate() + "\n" + "SP:" + WarframeStaticMethods.getArmor(this.Wearer).TryGetComp<CompWarframeSkill>().skill2mana;
            ck2.disabled = (!this.Wearer.Drafted || this.cooldownTime2 > 0 || this.Wearer.stances.stunner.Stunned || WarframeStaticMethods.getBelt(this.Wearer).SP < this.getSkillComp().skill2mana) && this.tillSkillOpen != 2;


            yield return ck2;

            Command_CastSkill ck3 = WarframeStaticMethods.getSkillCommand(def, 3);
            ck3.self = this.Wearer;
            ck3.defaultDesc = (def + "Skill3.desc").Translate() + "\n" + "SP:" + WarframeStaticMethods.getArmor(this.Wearer).TryGetComp<CompWarframeSkill>().skill3mana;
            ck3.disabled = (!this.Wearer.Drafted || this.cooldownTime3 > 0 || this.Wearer.stances.stunner.Stunned || WarframeStaticMethods.getBelt(this.Wearer).SP < this.getSkillComp().skill3mana) && this.tillSkillOpen != 3;


            yield return ck3;
            
            Command_CastSkill ck4 = WarframeStaticMethods.getSkillCommand(def, 4);
            ck4.self = this.Wearer;
            ck4.defaultDesc = (def + "Skill4.desc").Translate() + "\n" + "SP:" + WarframeStaticMethods.getArmor(this.Wearer).TryGetComp<CompWarframeSkill>().skill4mana;
            ck4.disabled = (!this.Wearer.Drafted || this.cooldownTime4 > 0 || this.Wearer.stances.stunner.Stunned || WarframeStaticMethods.getBelt(this.Wearer).SP < this.getSkillComp().skill4mana) && this.tillSkillOpen != 4;


            yield return ck4;
           
            yield break;
        }



        public CompWarframeSkill getSkillComp() {
            return this.TryGetComp<CompWarframeSkill>();
        }




    }
}
