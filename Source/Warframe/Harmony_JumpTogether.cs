using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Verse;

namespace Warframe
{
    [HarmonyPatch(typeof(Pawn), "GetGizmos", new Type[]
    {
    })]
    public static class Harmony_JumpTogether
    {
        public static void Postfix(Pawn __instance, ref IEnumerable<Gizmo> __result)
        {
            if (!__instance.IsColonistPlayerControlled)
            {
                return;
            }

            var nes = __result.ToList();


            foreach (var attack in GetTeleGizmos(__instance))
            {
                //yield return attack;

                nes.Add(attack);
                //  Log.Warning(nes.Count+"/");
            }

            __result = nes.AsEnumerable();
        }

        private static IEnumerable<Gizmo> GetTeleGizmos(Pawn pawn)
        {
            if (ShouldUseSquadTeleGizmo())
            {
                yield return WarframeStaticMethods.GetMulJump(pawn);
            }
        }


        private static bool ShouldUseSquadTeleGizmo()
        {
            var selectedObjectsListForReading = Find.Selector.SelectedObjectsListForReading;
            if (selectedObjectsListForReading.Count < 2)
            {
                return false;
            }

            foreach (var pawn in selectedObjectsListForReading)
            {
                if (pawn is not Pawn pawn1)
                {
                    continue;
                }

                if (!pawn1.IsWarframe())
                {
                    return false;
                }
            }

            return true;
        }
    }
}