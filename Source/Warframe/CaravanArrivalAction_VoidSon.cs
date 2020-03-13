using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Text;
using Verse;

namespace Warframe
{
    public class CaravanArrivalAction_VoidSon : CaravanArrivalAction
    {
        // Token: 0x06001D4D RID: 7501 RVA: 0x000DF377 File Offset: 0x000DD777
        public CaravanArrivalAction_VoidSon()
        {
        }

        // Token: 0x06001D4E RID: 7502 RVA: 0x000DF37F File Offset: 0x000DD77F
        public CaravanArrivalAction_VoidSon(WorldObject_VoidSon peaceTalks)
        {
            this.peaceTalks = peaceTalks;
        }

        // Token: 0x1700044B RID: 1099
        // (get) Token: 0x06001D4F RID: 7503 RVA: 0x000DF38E File Offset: 0x000DD78E
        public override string Label
        {
            get
            {
                return "VisitVoidSon".Translate(new object[]
                {
                    this.peaceTalks.Label
                });
            }
        }

        // Token: 0x1700044C RID: 1100
        // (get) Token: 0x06001D50 RID: 7504 RVA: 0x000DF3AE File Offset: 0x000DD7AE
        public override string ReportString
        {
            get
            {
                return "CaravanVisiting".Translate(new object[]
                {
                    this.peaceTalks.Label
                });
            }
        }

        // Token: 0x06001D51 RID: 7505 RVA: 0x000DF3D0 File Offset: 0x000DD7D0
        public override FloatMenuAcceptanceReport StillValid(Caravan caravan, int destinationTile)
        {
            FloatMenuAcceptanceReport floatMenuAcceptanceReport = base.StillValid(caravan, destinationTile);
            if (!floatMenuAcceptanceReport)
            {
                return floatMenuAcceptanceReport;
            }
            if (this.peaceTalks != null && this.peaceTalks.Tile != destinationTile)
            {
                return false;
            }
            return CanVisit(caravan, this.peaceTalks);
        }

        // Token: 0x06001D52 RID: 7506 RVA: 0x000DF422 File Offset: 0x000DD822
        public override void Arrived(Caravan caravan)
        {
            this.peaceTalks.Notify_CaravanArrived(caravan);
        }

        // Token: 0x06001D53 RID: 7507 RVA: 0x000DF430 File Offset: 0x000DD830
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look<WorldObject_VoidSon>(ref this.peaceTalks, "peaceTalks", false);
        }

        // Token: 0x06001D54 RID: 7508 RVA: 0x000DF449 File Offset: 0x000DD849
        public static FloatMenuAcceptanceReport CanVisit(Caravan caravan, WorldObject_VoidSon peaceTalks)
        {
            return peaceTalks != null && peaceTalks.Spawned;
        }

        // Token: 0x06001D55 RID: 7509 RVA: 0x000DF460 File Offset: 0x000DD860
        public static IEnumerable<FloatMenuOption> GetFloatMenuOptions(Caravan caravan, PeaceTalks peaceTalks)
        {
            return CaravanArrivalActionUtility.GetFloatMenuOptions<CaravanArrivalAction_VisitPeaceTalks>(() => CaravanArrivalAction_VisitPeaceTalks.CanVisit(caravan, peaceTalks), () => new CaravanArrivalAction_VisitPeaceTalks(peaceTalks), "VisitPeaceTalks".Translate(new object[]
            {
                peaceTalks.Label
            }), caravan, peaceTalks.Tile, peaceTalks);
        }

        // Token: 0x040011CF RID: 4559
        private WorldObject_VoidSon peaceTalks;
    }
}
