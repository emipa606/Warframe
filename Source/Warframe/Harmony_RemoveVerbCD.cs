using HarmonyLib;
using Verse;

namespace Warframe
{
    [HarmonyPatch(typeof(VerbProperties), "AdjustedCooldown", typeof(Tool), typeof(Pawn), typeof(Thing))]
    public static class Harmony_RemoveVerbCD
    {
        public static void Postfix(VerbProperties __instance, Tool tool, Pawn attacker, Thing equipment,
            ref float __result)
        {
            var unused = Traverse.Create(__instance);
            // float num = tv.Field("defaultCooldownTime").GetValue<float>();
            if (attacker == null || !attacker.IsWarframe())
            {
                return;
            }

            if (equipment != null && !__instance.IsMeleeAttack)
            {
                __result = 0f;
            }
            else if (__instance.IsMeleeAttack && tool != null)
            {
                __result = tool.cooldownTime / 3f;
            }
        }
    }
}