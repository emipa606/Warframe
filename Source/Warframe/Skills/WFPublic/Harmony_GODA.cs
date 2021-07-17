using HarmonyLib;
using Verse;

namespace Warframe.Skills.WFPublic
{
    //GOD1
    [HarmonyPatch(typeof(Thing), "TakeDamage", typeof(DamageInfo))]
    public static class Harmony_GODA
    {
        public static bool Prefix(Thing __instance, ref DamageWorker.DamageResult __result)
        {
            if (__instance is not Pawn pawn)
            {
                return true;
            }

            if (!pawn.IsWarframe())
            {
                return true;
            }

            foreach (var hed in pawn.health.hediffSet.hediffs)
            {
                if (hed.def.defName != "WFGod")
                {
                    continue;
                }

                __result = new DamageWorker.DamageResult();
                return false;
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