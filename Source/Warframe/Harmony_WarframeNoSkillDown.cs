using HarmonyLib;
using RimWorld;
using Verse;

namespace Warframe
{
    [HarmonyPatch(typeof(SkillRecord), "Interval")]
    public static class Harmony_WarframeNoSkillDown
    {
        public static bool Prefix(SkillRecord __instance)
        {
            var tv = Traverse.Create(__instance);
            var pawn = tv.Field("pawn").GetValue<Pawn>();
            if (pawn.IsWarframe())
            {
                return false;
            }

            return true;
        }
    }
}