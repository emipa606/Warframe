using System;
using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Warframe
{
    [HarmonyPatch(typeof(Pawn_NeedsTracker), "AddOrRemoveNeedsAsAppropriate", new Type[]
    {
    })]
    public static class Harmony_Need_Remove
    {
        public static void Postfix(Pawn_NeedsTracker __instance)
        {
            var traverse = Traverse.Create(__instance);
            var pp = traverse.Field("pawn").GetValue<Pawn>();
            if (!pp.IsWarframe())
            {
                return;
            }

            foreach (var nnd in getUselessNeed())
            {
                if (__instance.TryGetNeed(nnd) == null)
                {
                    continue;
                }

                //__instance.RemoveNeed(nnd);
                var item = __instance.TryGetNeed(nnd);
                var needlist = traverse.Field("needs").GetValue<List<Need>>();
                needlist.Remove(item);

                //    __instance.BindDirectNeedFields();
            }
        }

        public static List<NeedDef> getUselessNeed()
        {
            var result = new List<NeedDef>
            {
                NeedDefOf.Food,
                NeedDefOf.Rest,
                NeedDefOf.Joy,
                easyGetND("Beauty"),
                easyGetND("Comfort"),
                easyGetND("RoomSize"),
                easyGetND("Outdoors"),
                easyGetND("Mood")
            };
            return result;
        }

        private static NeedDef easyGetND(string def)
        {
            return DefDatabase<NeedDef>.GetNamed(def);
        }
    }
}