using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Warframe
{
    public class WarframeArmor : Apparel
    {
        public float cooldownTime1;
        public float cooldownTime2;
        public float cooldownTime3;
        public float cooldownTime4;

        public List<ThingWithComps> oldWeapon = new List<ThingWithComps>();
        public float tillSkillMul = 1;

        public int tillSkillOpen;


        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref cooldownTime1, "cooldownTime1");
            Scribe_Values.Look(ref cooldownTime2, "cooldownTime2");
            Scribe_Values.Look(ref cooldownTime3, "cooldownTime3");
            Scribe_Values.Look(ref cooldownTime4, "cooldownTime4");
            Scribe_Values.Look(ref tillSkillMul, "tillSkillMul", 1);
            Scribe_Values.Look(ref tillSkillOpen, "tillSkillOpen");
            Scribe_Collections.Look(ref oldWeapon, "oldWeapon", LookMode.Deep);
        }

        public override void Tick()
        {
            base.Tick();

            if (cooldownTime1 > 0f)
            {
                cooldownTime1 -= 1f / 60f;
            }

            if (cooldownTime2 > 0f)
            {
                cooldownTime2 -= 1f / 60f;
            }

            if (cooldownTime3 > 0f)
            {
                cooldownTime3 -= 1f / 60f;
            }

            if (cooldownTime4 > 0f)
            {
                cooldownTime4 -= 1f / 60f;
            }


            if (tillSkillOpen <= 0)
            {
                return;
            }

            if (WarframeStaticMethods.ConsumeSP(Wearer, tillSkillMul, tillSkillOpen))
            {
                return;
            }

            WarframeStaticMethods.GetSkillEndAction(Wearer, tillSkillOpen);
            tillSkillOpen = 0;
            tillSkillMul = 1;
        }


        public override IEnumerable<Gizmo> GetWornGizmos()
        {
            if (Find.Selector.SingleSelectedThing != Wearer)
            {
                yield break;
            }


            var pawn = Wearer;
            if (pawn == null || !pawn.IsWarframe())
            {
                yield break;
            }

            var replace = pawn.kindDef.defName.Replace("Warframe_", "");


            var ck1 = WarframeStaticMethods.GetSkillCommand(replace, 1);
            ck1.self = Wearer;
            ck1.defaultDesc = (replace + "Skill1.desc").Translate() + "\n" + "SP:" +
                              WarframeStaticMethods.GetArmor(Wearer).TryGetComp<CompWarframeSkill>().skill1mana;
            ck1.disabled =
                (!Wearer.Drafted || cooldownTime1 > 0 || Wearer.stances.stunner.Stunned ||
                 WarframeStaticMethods.GetBelt(Wearer).SP < getSkillComp().skill1mana) && tillSkillOpen != 1;


            yield return ck1;

            var ck2 = WarframeStaticMethods.GetSkillCommand(replace, 2);
            ck2.self = Wearer;
            ck2.defaultDesc = (replace + "Skill2.desc").Translate() + "\n" + "SP:" +
                              WarframeStaticMethods.GetArmor(Wearer).TryGetComp<CompWarframeSkill>().skill2mana;
            ck2.disabled =
                (!Wearer.Drafted || cooldownTime2 > 0 || Wearer.stances.stunner.Stunned ||
                 WarframeStaticMethods.GetBelt(Wearer).SP < getSkillComp().skill2mana) && tillSkillOpen != 2;


            yield return ck2;

            var ck3 = WarframeStaticMethods.GetSkillCommand(replace, 3);
            ck3.self = Wearer;
            ck3.defaultDesc = (replace + "Skill3.desc").Translate() + "\n" + "SP:" +
                              WarframeStaticMethods.GetArmor(Wearer).TryGetComp<CompWarframeSkill>().skill3mana;
            ck3.disabled =
                (!Wearer.Drafted || cooldownTime3 > 0 || Wearer.stances.stunner.Stunned ||
                 WarframeStaticMethods.GetBelt(Wearer).SP < getSkillComp().skill3mana) && tillSkillOpen != 3;


            yield return ck3;

            var ck4 = WarframeStaticMethods.GetSkillCommand(replace, 4);
            ck4.self = Wearer;
            ck4.defaultDesc = (replace + "Skill4.desc").Translate() + "\n" + "SP:" +
                              WarframeStaticMethods.GetArmor(Wearer).TryGetComp<CompWarframeSkill>().skill4mana;
            ck4.disabled =
                (!Wearer.Drafted || cooldownTime4 > 0 || Wearer.stances.stunner.Stunned ||
                 WarframeStaticMethods.GetBelt(Wearer).SP < getSkillComp().skill4mana) && tillSkillOpen != 4;


            yield return ck4;
        }


        public CompWarframeSkill getSkillComp()
        {
            return this.TryGetComp<CompWarframeSkill>();
        }
    }
}