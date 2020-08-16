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
            Scribe_Values.Look<float>(ref cooldownTime1, "cooldownTime1", 0, false);
            Scribe_Values.Look<float>(ref cooldownTime2, "cooldownTime2", 0, false);
            Scribe_Values.Look<float>(ref cooldownTime3, "cooldownTime3", 0, false);
            Scribe_Values.Look<float>(ref cooldownTime4, "cooldownTime4", 0, false);
            Scribe_Values.Look<float>(ref tillSkillMul, "tillSkillMul", 1, false);
            Scribe_Values.Look<int>(ref tillSkillOpen, "tillSkillOpen", 0, false);
            Scribe_Collections.Look<ThingWithComps>(ref oldWeapon, "oldWeapon", LookMode.Deep, new object[0]);
            


        }

        public override void Tick()
        {

            base.Tick();
            
            if (cooldownTime1 > 0f) cooldownTime1 -= ((float)(1f / 60f));
            if (cooldownTime2 > 0f) cooldownTime2 -= ((float)(1f / 60f));
            if (cooldownTime3 > 0f) cooldownTime3 -= ((float)(1f / 60f));
            if (cooldownTime4 > 0f) cooldownTime4 -= ((float)(1f / 60f));


            if (tillSkillOpen>0)
            {
              if (!WarframeStaticMethods.ConsumeSP(Wearer, tillSkillMul,tillSkillOpen))
                {
                    WarframeStaticMethods.GetSkillEndAction(Wearer,tillSkillOpen);
                    tillSkillOpen = 0;
                    tillSkillMul = 1;
                }
            }

        }


        public override IEnumerable<Gizmo> GetWornGizmos()
        {
            if (Find.Selector.SingleSelectedThing != Wearer)
            {
                yield break;
            }

         
            Pawn pawn = Wearer;
            if (pawn == null || !pawn.IsWarframe()) yield break;
            String def = pawn.kindDef.defName.Replace("Warframe_", "");


            

            Command_CastSkill ck1 = WarframeStaticMethods.GetSkillCommand(def, 1);
            ck1.self = Wearer;
            ck1.defaultDesc = (def+"Skill1.desc").Translate() + "\n" + "SP:" + WarframeStaticMethods.GetArmor(Wearer).TryGetComp<CompWarframeSkill>().skill1mana;
            ck1.disabled = (!Wearer.Drafted|| cooldownTime1 > 0 || Wearer.stances.stunner.Stunned || WarframeStaticMethods.GetBelt(Wearer).SP < getSkillComp().skill1mana) && tillSkillOpen != 1;


            yield return ck1;

            Command_CastSkill ck2 = WarframeStaticMethods.GetSkillCommand(def, 2);
            ck2.self = Wearer;
            ck2.defaultDesc = (def + "Skill2.desc").Translate() + "\n" + "SP:" + WarframeStaticMethods.GetArmor(Wearer).TryGetComp<CompWarframeSkill>().skill2mana;
            ck2.disabled = (!Wearer.Drafted || cooldownTime2 > 0 || Wearer.stances.stunner.Stunned || WarframeStaticMethods.GetBelt(Wearer).SP < getSkillComp().skill2mana) && tillSkillOpen != 2;


            yield return ck2;

            Command_CastSkill ck3 = WarframeStaticMethods.GetSkillCommand(def, 3);
            ck3.self = Wearer;
            ck3.defaultDesc = (def + "Skill3.desc").Translate() + "\n" + "SP:" + WarframeStaticMethods.GetArmor(Wearer).TryGetComp<CompWarframeSkill>().skill3mana;
            ck3.disabled = (!Wearer.Drafted || cooldownTime3 > 0 || Wearer.stances.stunner.Stunned || WarframeStaticMethods.GetBelt(Wearer).SP < getSkillComp().skill3mana) && tillSkillOpen != 3;


            yield return ck3;
            
            Command_CastSkill ck4 = WarframeStaticMethods.GetSkillCommand(def, 4);
            ck4.self = Wearer;
            ck4.defaultDesc = (def + "Skill4.desc").Translate() + "\n" + "SP:" + WarframeStaticMethods.GetArmor(Wearer).TryGetComp<CompWarframeSkill>().skill4mana;
            ck4.disabled = (!Wearer.Drafted || cooldownTime4 > 0 || Wearer.stances.stunner.Stunned || WarframeStaticMethods.GetBelt(Wearer).SP < getSkillComp().skill4mana) && tillSkillOpen != 4;


            yield return ck4;
           
            yield break;
        }



        public CompWarframeSkill getSkillComp() {
            return this.TryGetComp<CompWarframeSkill>();
        }




    }
}
