using Verse;

namespace Warframe
{
    public class CompWarframeSkill : ThingComp
    {
        public CompProperties_WarframeSkill Props => (CompProperties_WarframeSkill) props;

        public float skill1mana =>
            Props.mana1 * (1 - (WarframeStaticMethods.GetWFLevel((parent as WarframeArmor)?.Wearer) / 60f));

        public float skill2mana =>
            Props.mana2 * (1 - (WarframeStaticMethods.GetWFLevel((parent as WarframeArmor)?.Wearer) / 60f));

        public float skill3mana =>
            Props.mana3 * (1 - (WarframeStaticMethods.GetWFLevel((parent as WarframeArmor)?.Wearer) / 60f));

        public float skill4mana =>
            Props.mana4 * (1 - (WarframeStaticMethods.GetWFLevel((parent as WarframeArmor)?.Wearer) / 60f));
    }
}