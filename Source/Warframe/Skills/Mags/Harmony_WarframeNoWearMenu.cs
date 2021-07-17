using HarmonyLib;
using Verse;

namespace Warframe.Skills.Mags
{
    //mag hediff 
    [HarmonyPatch(typeof(Corpse), "TickRare")]
    public static class Harmony_WarframeNoWearMenu
    {
        public static void Prefix(Corpse __instance)
        {
            Hediff hedi = null;
            foreach (var hedif in __instance.InnerPawn.health.hediffSet.hediffs)
            {
                if (!hedif.def.defName.Equals("Magnetize"))
                {
                    continue;
                }

                // Traverse tv = Traverse.Create(__instance);
                //Pawn pawn = tv.Field("pawn").GetValue<Pawn>();
                GenExplosion.DoExplosion(__instance.InnerPawn.Corpse.Position, __instance.InnerPawn.Corpse.Map, 4f,
                    DefDatabase<DamageDef>.GetNamed("Mag"), null, 15);
                hedi = hedif;
            }

            if (hedi != null)
            {
                __instance.InnerPawn.health.RemoveHediff(hedi);
            }
        }
    }
}