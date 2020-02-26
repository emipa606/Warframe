using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using Verse;

namespace Warframe
{
    public class CompWarframeSkill:ThingComp
    {

        public CompProperties_WarframeSkill Props
        {
            get
            {
                return (CompProperties_WarframeSkill)this.props;
            }
        }

        public float skill1mana
        {
            get
            {
                return this.Props.mana1 * (1- WarframeStaticMethods.getWFLevel((this.parent as WarframeArmor).Wearer)/60f);
            }
        }

        public float skill2mana
        {
            get
            {
                return this.Props.mana2 * (1 - WarframeStaticMethods.getWFLevel((this.parent as WarframeArmor).Wearer) / 60f);
            }
        }

        public float skill3mana
        {
            get
            {
                return this.Props.mana3 * (1 - WarframeStaticMethods.getWFLevel((this.parent as WarframeArmor).Wearer) / 60f);
            }
        }

        public float skill4mana
        {
            get
            {
                return this.Props.mana4 * (1 - WarframeStaticMethods.getWFLevel((this.parent as WarframeArmor).Wearer) / 60f);
            }
        }
    }
}
