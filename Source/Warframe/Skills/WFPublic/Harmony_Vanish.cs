using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using Verse;

namespace Warframe.Skills.WFPublic
{
    //隐身时敌人不打你
    [HarmonyPatch(typeof(GenHostility), "HostileTo", new Type[] {
            typeof(Thing),
            typeof(Thing)
        })]
    public static class Harmony_VanishAttackNo
    {
        public static bool Prefix(Thing b, ref bool __result)
        {
            if (b is Pawn)
            {
                Pawn tar = b as Pawn;
                foreach (Hediff hef in tar.health.hediffSet.hediffs)
                {
                    if (hef.def.defName.StartsWith("WFVanish"))
                    {
                        __result = false;
                        return false;
                    }
                }
            }
            return true;

        }
    }
}
