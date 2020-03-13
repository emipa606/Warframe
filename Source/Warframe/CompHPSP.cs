using System;
using System.Collections.Generic;
using System.Text;
using Verse;

namespace Warframe
{
    public class CompHPSP:ThingComp
    {

        public CompProperties_HPSP Props
        {
            get
            {
                return (CompProperties_HPSP)base.props;
            }
        }




        public float HP
        {
            get
            {
                return this.Props.HP;
            }

        }
        public float SP
        {
            get
            {
                return this.Props.SP;
            }

        }

    }
}
