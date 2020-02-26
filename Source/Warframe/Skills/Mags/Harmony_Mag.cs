using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using Verse;

namespace Warframe.Skills.Mags
{
    //mag hediff 
    [HarmonyPatch(typeof(Corpse), "TickRare")]
    public static class Harmony_WarframeNoWearMenu
    {
        public static void Prefix(Corpse __instance)
        {
            Hediff hedi=null;
                foreach (Hediff hedif in __instance.InnerPawn.health.hediffSet.hediffs)
                {
                    if (hedif.def.defName.Equals("Magnetize"))
                    {
                       
                       // Traverse tv = Traverse.Create(__instance);
                        //Pawn pawn = tv.Field("pawn").GetValue<Pawn>();
                        GenExplosion.DoExplosion(__instance.InnerPawn.Corpse.Position, __instance.InnerPawn.Corpse.Map, 4f, DefDatabase<DamageDef>.GetNamed("Mag", true), null, 15, -1, null, null, null, null, null, 0, 1, false, null, 0, 1, 0, false);
                    hedi = hedif;
                    }
                }
            if(hedi!=null)
                __instance.InnerPawn.health.RemoveHediff(hedi);
        }

    }
    
}
