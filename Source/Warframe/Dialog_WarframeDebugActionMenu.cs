using RimWorld;
using Verse;

namespace Warframe
{
    public class Dialog_WarframeDebugActionMenu : Dialog_DebugActionsMenu
    {
        private void GiveLevel()
        {
            var map = Find.CurrentMap;
            foreach (var th in map.thingGrid.ThingsAt(UI.MouseCell()))
            {
                if (th is not Pawn pawn)
                {
                    continue;
                }

                if (pawn.IsWarframe())
                {
                    pawn.records.AddTo(RecordDefOf.KillsHumanlikes, 150);
                }
            }
        }

        // Token: 0x0600004F RID: 79 RVA: 0x00004428 File Offset: 0x00002628
        protected override void DoListingItems()
        {
            base.DoListingItems();
            if (Current.ProgramState != ProgramState.Playing)
            {
                return;
            }

            var map = Find.CurrentMap;
            if (map == null)
            {
                return;
            }

            DoGap();
            DoLabel("Tools - Warframe");
            DebugToolMap("Apply: Warframe 30Level", GiveLevel, false);
            DebugToolMap("Apply: Max Sp", delegate
            {
                foreach (var thing in Find.CurrentMap.thingGrid.ThingsAt(UI.MouseCell()))
                {
                    if (thing is not Pawn pawn || !pawn.IsWarframe())
                    {
                        continue;
                    }

                    var wb = WarframeStaticMethods.GetBelt(pawn);
                    wb.SP += 9999;
                }
            }, false);
        }
    }
}