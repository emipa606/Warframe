using System.Collections.Generic;
using HarmonyLib;
using Verse;

namespace Warframe
{
    [HarmonyPatch(typeof(DebugWindowsOpener))]
    [HarmonyPatch("ToggleDebugActionsMenu")]
    public static class DebugWindowsOpener_ToggleDebugActionsMenu_Patch
    {
        // Token: 0x060002AF RID: 687 RVA: 0x00010FF8 File Offset: 0x0000F1F8
        [HarmonyPriority(800)]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var from = AccessTools.Constructor(typeof(Dialog_DebugActionsMenu));
            var to = AccessTools.Constructor(typeof(Dialog_WarframeDebugActionMenu));
            return instructions.MethodReplacer(from, to);
        }
    }
}