using HarmonyLib;
using RimWorld;
using Verse;

namespace Warframe.Skills.WFPublic
{
    //隐身时敌人不打你
    [HarmonyPatch(typeof(GenHostility), "HostileTo", typeof(Thing), typeof(Thing))]
    public static class Harmony_VanishAttackNo
    {
        public static bool Prefix(Thing b, ref bool __result)
        {
            if (b is not Pawn pawn)
            {
                return true;
            }

            foreach (var hef in pawn.health.hediffSet.hediffs)
            {
                if (!hef.def.defName.StartsWith("WFVanish"))
                {
                    continue;
                }

                __result = false;
                return false;
            }

            return true;
        }
    }
}