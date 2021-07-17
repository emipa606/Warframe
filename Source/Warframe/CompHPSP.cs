using Verse;

namespace Warframe
{
    public class CompHPSP : ThingComp
    {
        public CompProperties_HPSP Props => (CompProperties_HPSP) props;


        public float HP => Props.HP;

        public float SP => Props.SP;
    }
}