using Verse;

namespace Warframe
{
    public class CompProperties_HPSP:CompProperties
    {

        public float HP;//= -24;
        public float SP;//= -24;


        public CompProperties_HPSP()
        {

            base.compClass = typeof(CompHPSP);
        }
    }
}
