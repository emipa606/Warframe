using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using Verse;

namespace Warframe
{
    public class Dialog_WarframeDebugActionMenu : Dialog_DebugActionsMenu
    {
        private void GiveLevel()
        {
            Map map = Find.CurrentMap;
            foreach(Thing th in map.thingGrid.ThingsAt(UI.MouseCell()))
            {
                if(th is Pawn)
                {
                    if((th as Pawn).IsWarframe())
                    {
                        (th as Pawn).records.AddTo(RecordDefOf.KillsHumanlikes,150);
                    }
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
            Map map = Find.CurrentMap;
            if (map == null)
            {
                return;
            }
            DoGap();
            DoLabel("Tools - Warframe");
            DebugToolMap("Apply: Warframe 30Level", delegate
            {
                GiveLevel();
            });
            DebugToolMap("Apply: Max Sp", delegate
            {
                foreach (Thing thing in Find.CurrentMap.thingGrid.ThingsAt(UI.MouseCell()))
                {
                    if (thing is Pawn pawn && (pawn.IsWarframe()))
                    {
                        WarframeBelt wb = WarframeStaticMethods.GetBelt(pawn);
                        wb.SP += 9999;
                    }
                }
            });

        }
    }
}
