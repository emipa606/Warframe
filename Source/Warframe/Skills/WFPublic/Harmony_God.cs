using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using Verse;

namespace Warframe.Skills.WFPublic
{


    //GOD1
    [HarmonyPatch(typeof(Thing), "TakeDamage", new Type[]
{
    typeof(DamageInfo)
})]
    public static class Harmony_GODA
    {
        public static bool Prefix(Thing __instance, ref DamageWorker.DamageResult __result)
        {

            if(__instance is Pawn)
            {
                Pawn wf = __instance as Pawn;
                if(wf!=null&& wf.isWarframe())
                {
                    foreach (Hediff hed in wf.health.hediffSet.hediffs)
                    {
                        if (hed.def.defName == "WFGod")
                        {
                            __result=new DamageWorker.DamageResult();
                            return false;
                        }
                    }
                }
            }
            return true;

        }
    }








/*
    [HarmonyPatch(typeof(HediffSet), "AddDirect", new Type[] {
       typeof(Hediff),
       typeof(DamageInfo?),
        typeof(DamageWorker.DamageResult)
    })]
    public static class Harmony_God
    {
        public static bool Prefix(HediffSet __instance, Hediff hediff, DamageInfo? dinfo = null, DamageWorker.DamageResult damageResult = null)
        {
            Traverse traverse = Traverse.Create(__instance);
            Pawn pawn = traverse.Field("pawn").GetValue<Pawn>();
            if (pawn != null && pawn.isWarframe())
            {
                foreach(Hediff hed in pawn.health.hediffSet.hediffs)
                {
                    if (hed.def.defName == "WFGod")
                    {
                        return false;
                    }
                }
            }
            return true;

        }
    }


    */







}
