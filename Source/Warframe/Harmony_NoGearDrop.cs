using HarmonyLib;
using RimWorld;
using Verse;

namespace Warframe
{
    [HarmonyPatch(typeof(ITab_Pawn_Gear), "InterfaceDrop", typeof(Thing))]
    public static class Harmony_NoGearDrop
    {
        public static bool Prefix(Thing t)
        {
            if (!t.def.defName.StartsWith("WF_") && (t.def.weaponTags == null || !t.def.weaponTags.Contains("WFGun")))
            {
                return true;
            }

            Messages.Message("CanNotDropGearMsg".Translate(), MessageTypeDefOf.RejectInput, false);
            return false;
        }
    }
}