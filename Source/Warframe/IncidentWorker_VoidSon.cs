using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Text;
using Verse;

namespace Warframe
{
    public class IncidentWorker_VoidSon : IncidentWorker
    {
        // Token: 0x06000F08 RID: 3848 RVA: 0x0006F29C File Offset: 0x0006D69C
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            int num;
            return base.CanFireNowSub(parms)  && this.TryFindTile(out num);
        }

        // Token: 0x06000F09 RID: 3849 RVA: 0x0006F2D0 File Offset: 0x0006D6D0
        protected override bool TryExecuteWorker(IncidentParms parms)
        {

            int tile;
            if (!this.TryFindTile(out tile))
            {
                return false;
            }
            WorldObject_VoidSon voidSon = (WorldObject_VoidSon)WorldObjectMaker.MakeWorldObject(DefDatabase<WorldObjectDef>.GetNamed("VoidSon", true));
            voidSon.Tile = tile;
            int randomInRange = SiteTuning.QuestSiteTimeoutDaysRange.RandomInRange;
            voidSon.GetComponent<TimeoutComp>().StartTimeout(randomInRange * 60000);
            Find.WorldObjects.Add(voidSon);
            string text = "VoidSonWOBJText".Translate(new object[]{ randomInRange});//string.Format(randomInRange).CapitalizeFirst();
            Find.LetterStack.ReceiveLetter("VoidSon.text".Translate(), text, this.def.letterDef, voidSon, null, null);
            return true;
        }



        // Token: 0x06000F0B RID: 3851 RVA: 0x0006F3D4 File Offset: 0x0006D7D4
        private bool TryFindTile(out int tile)
        {
            IntRange voidSonQuestSiteDistanceRange = new IntRange(5, 13);
            return TileFinder.TryFindNewSiteTile(out tile, voidSonQuestSiteDistanceRange.min, voidSonQuestSiteDistanceRange.max, false, false, -1);
        }

        // Token: 0x06000F0C RID: 3852 RVA: 0x0006F400 File Offset: 0x0006D800
        private bool voidSonExist()
        {
            foreach (WorldObject wbj in Find.WorldObjects.AllWorldObjects) {
                if(wbj is WorldObject_VoidSon)
                {
                    return true;
                }
            }//AnyWorldObjectOfDefAt(DefDatabase<WorldObjectDef>.GetNamed("WOBJ_VoidSon",true));
           
            return false;
        }
    }
}
