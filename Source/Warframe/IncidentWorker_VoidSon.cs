using RimWorld;
using RimWorld.Planet;
using Verse;

namespace Warframe
{
    public class IncidentWorker_VoidSon : IncidentWorker
    {
        // Token: 0x06000F08 RID: 3848 RVA: 0x0006F29C File Offset: 0x0006D69C
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            return base.CanFireNowSub(parms) && TryFindTile(out _);
        }

        // Token: 0x06000F09 RID: 3849 RVA: 0x0006F2D0 File Offset: 0x0006D6D0
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            if (!TryFindTile(out var tile))
            {
                return false;
            }

            var voidSon =
                (WorldObject_VoidSon) WorldObjectMaker.MakeWorldObject(DefDatabase<WorldObjectDef>.GetNamed("VoidSon"));
            voidSon.Tile = tile;
            var randomInRange = SiteTuning.QuestSiteTimeoutDaysRange.RandomInRange;
            voidSon.GetComponent<TimeoutComp>().StartTimeout(randomInRange * 60000);
            Find.WorldObjects.Add(voidSon);
            var text = "VoidSonWOBJText".Translate(randomInRange); //string.Format(randomInRange).CapitalizeFirst();
            Find.LetterStack.ReceiveLetter("VoidSon.text".Translate(), text, def.letterDef, voidSon);
            return true;
        }


        // Token: 0x06000F0B RID: 3851 RVA: 0x0006F3D4 File Offset: 0x0006D7D4
        private bool TryFindTile(out int tile)
        {
            var voidSonQuestSiteDistanceRange = new IntRange(5, 13);
            return TileFinder.TryFindNewSiteTile(out tile, voidSonQuestSiteDistanceRange.min,
                voidSonQuestSiteDistanceRange.max);
        }

        // Token: 0x06000F0C RID: 3852 RVA: 0x0006F400 File Offset: 0x0006D800
        private bool voidSonExist()
        {
            foreach (var wbj in Find.WorldObjects.AllWorldObjects)
            {
                if (wbj is WorldObject_VoidSon)
                {
                    return true;
                }
            } //AnyWorldObjectOfDefAt(DefDatabase<WorldObjectDef>.GetNamed("WOBJ_VoidSon",true));

            return false;
        }
    }
}