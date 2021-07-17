using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Warframe
{
    [HarmonyPatch(typeof(Pawn_DraftController), "GetGizmos")]
    public static class Harmony_RemoveDraftGizmo
    {
        public static void Postfix(Pawn_DraftController __instance, ref IEnumerable<Gizmo> __result)
        {
            var tv = Traverse.Create(__instance);
            var pawn = tv.Field("pawn").GetValue<Pawn>();
            if (!pawn.kindDef.defName.StartsWith("Warframe_"))
            {
                return;
            }

            if (WarframeStaticMethods.PawnInControlCell(pawn))
            {
                return;
            }

            var list = __result.ToList();
            list[0].disabled = !WarframeStaticMethods.PawnInControlCell(pawn);
            list[0].disabledReason = "OnlyControlCell".Translate();
            __result = list.AsEnumerable();
        }
    }
}