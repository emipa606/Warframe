using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Random = System.Random;

namespace Warframe
{
    public class WorldObject_VoidSon : WorldObject
    {
        // Token: 0x040012AD RID: 4781
        private const float BaseWeight_Success = 0.75f;


        // Token: 0x040012A8 RID: 4776
        private Material cachedMat;

        public override Material Material
        {
            get
            {
                if (cachedMat != null)
                {
                    return cachedMat;
                }

                var color = Color.magenta;

                cachedMat = MaterialPool.MatFrom(def.texture, ShaderDatabase.WorldOverlayTransparentLit, color,
                    WorldMaterials.WorldObjectRenderQueue);

                return cachedMat;
            }
        }

        // Token: 0x06001F41 RID: 8001 RVA: 0x000F1D50 File Offset: 0x000F0150
        public void Notify_CaravanArrived(Caravan caravan)
        {
            if (caravan.pawns.Count < 1)
            {
                Messages.Message("MessagePeaceTalksNoDiplomat".Translate(), caravan, MessageTypeDefOf.NegativeEvent,
                    false);
                return;
            }

            var chance = new Random(Find.TickManager.TicksGame).Next(0, 100);

            var num = chance * 1f / 100f;
            if (num <= BaseWeight_Success)
            {
                Outcome_Success(caravan);
            }
            else
            {
                Outcome_Fail(caravan);
            }


            Find.WorldObjects.Remove(this);
        }

        // Token: 0x06001F42 RID: 8002 RVA: 0x000F1EC0 File Offset: 0x000F02C0
        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Caravan caravan)
        {
            foreach (var o in base.GetFloatMenuOptions(caravan))
            {
                yield return o;
            }

            foreach (var f in VoidSonGetFloatMenuOptions(caravan))
            {
                yield return f;
            }
        }

        public IEnumerable<FloatMenuOption> VoidSonGetFloatMenuOptions(Caravan caravan)
        {
            return CaravanArrivalActionUtility.GetFloatMenuOptions(
                () => CaravanArrivalAction_VoidSon.CanVisit(caravan, this),
                () => new CaravanArrivalAction_VoidSon(this), "VisitVoidSon".Translate(Label), caravan, Tile, this);
        }


        private void Outcome_Fail(Caravan caravan)
        {
            Find.LetterStack.ReceiveLetter("LetterLabelVoidSon_Fail".Translate(), GetLetterText(
                "LetterVoidSon_Fail".Translate(caravan.pawns[0].LabelCap)), LetterDefOf.NeutralEvent, caravan, Faction);
        }

        // Token: 0x06001F46 RID: 8006 RVA: 0x000F2028 File Offset: 0x000F0428
        private void Outcome_Success(Caravan caravan)
        {
            caravan.pawns[0].story.traits.GainTrait(new Trait(TraitDef.Named("Warframe_Trait")));


            Find.LetterStack.ReceiveLetter("LetterLabelVoidSon_Success".Translate(), GetLetterText(
                    "LetterVoidSon_Success".Translate(caravan.pawns[0].LabelCap)), LetterDefOf.PositiveEvent, caravan,
                Faction);
        }


        // Token: 0x06001F48 RID: 8008 RVA: 0x000F21AC File Offset: 0x000F05AC
        private string GetLetterText(string baseText)
        {
            return baseText;
        }
    }
}