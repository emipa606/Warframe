using Verse;

namespace Warframe
{
    public class CompProperties_WarframeSkill : CompProperties
    {

        public float mana1;
        public float mana2;
        public float mana3;
        public float mana4;


        public CompProperties_WarframeSkill()
        {

            compClass = typeof(CompWarframeSkill);
        }
    }
}
