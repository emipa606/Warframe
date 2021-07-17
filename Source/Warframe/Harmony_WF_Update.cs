using System;
using HarmonyLib;
using Verse;

namespace Warframe
{
    [HarmonyPatch(typeof(Pawn_HealthTracker), "HealthTick", new Type[] { })]
    public static class Harmony_WF_Update
    {
        public static void Postfix(Pawn_HealthTracker __instance)
        {
            var traverse = Traverse.Create(__instance);

            var pawn = traverse.Field("pawn").GetValue<Pawn>();
            if (!pawn.IsWarframe())
            {
                return;
            }

            if (!pawn.Drafted)
            {
                return;
            }

            if (!WFModBase.Instance._WFcontrolstorage.checkBeControlerExist(pawn))
            {
                pawn.drafter.Drafted = false;
            }
            else
            {
                var bc = WFModBase.Instance._WFcontrolstorage.BeControlerAndControlCell.TryGetValue(pawn);
                if (!bc.HasAnyContents)
                {
                    pawn.drafter.Drafted = false;
                }
            }
            //WFModBase.Instance._WFcontrolstorage.checkControlerExist(pawn);
        }
    }
}