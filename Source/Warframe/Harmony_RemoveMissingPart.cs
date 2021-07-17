using HarmonyLib;
using Verse;

namespace Warframe
{
    [HarmonyPatch(typeof(HediffSet), "AddDirect", typeof(Hediff), typeof(DamageInfo?),
        typeof(DamageWorker.DamageResult))]
    public static class Harmony_RemoveMissingPart
    {
        public static bool Prefix(HediffSet __instance, Hediff hediff, DamageInfo? dinfo = null,
            DamageWorker.DamageResult damageResult = null)
        {
            var traverse = Traverse.Create(__instance);
            var pawn = traverse.Field("pawn").GetValue<Pawn>();
            if (pawn == null || !pawn.IsWarframe())
            {
                return true;
            }

            if (!hediff.def.isBad)
            {
                return true;
            }

            if (hediff.Part.def.defName == "wf_armor")
            {
                return true;
            }

            if (dinfo == null)
            {
                return false;
            }

            var ddinfo = (DamageInfo) dinfo;
            ddinfo.SetHitPart(pawn.health.hediffSet.GetRandomNotMissingPart(ddinfo.Def,
                BodyPartHeight.Undefined, BodyPartDepth.Outside));
            pawn.TakeDamage(ddinfo);

            return false;
        }
    }
}