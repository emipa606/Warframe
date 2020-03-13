using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;

namespace Warframe
{
    public class WorldObject_VoidSon : WorldObject
    {

        public override Material Material
        {
            get
            {
                if (this.cachedMat == null)
                {

                    Color color = Color.magenta;
                    
                    this.cachedMat = MaterialPool.MatFrom(this.def.texture, ShaderDatabase.WorldOverlayTransparentLit, color, WorldMaterials.WorldObjectRenderQueue);
                }
                return this.cachedMat;
            }
        }

        // Token: 0x06001F41 RID: 8001 RVA: 0x000F1D50 File Offset: 0x000F0150
        public void Notify_CaravanArrived(Caravan caravan)
        {
          
            if (caravan.pawns.Count<1)
            {
                Messages.Message("MessagePeaceTalksNoDiplomat".Translate(), caravan, MessageTypeDefOf.NegativeEvent, false);
                return;
            }

            int chance = new System.Random(Find.TickManager.TicksGame).Next(0, 100);

            float num = chance*1f/100f;
            if (num <= BaseWeight_Success)
            {
                this.Outcome_Success(caravan);
            }else
            {
                this.Outcome_Fail(caravan);
            }
       

               



            Find.WorldObjects.Remove(this);
        }

        // Token: 0x06001F42 RID: 8002 RVA: 0x000F1EC0 File Offset: 0x000F02C0
        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Caravan caravan)
        {
            foreach (FloatMenuOption o in base.GetFloatMenuOptions(caravan))
            {
                yield return o;
            }
            foreach (FloatMenuOption f in VoidSonGetFloatMenuOptions(caravan))
            {
                yield return f;
            }
            yield break;
        }

        public  IEnumerable<FloatMenuOption> VoidSonGetFloatMenuOptions(Caravan caravan)
        {
            return CaravanArrivalActionUtility.GetFloatMenuOptions<CaravanArrivalAction_VoidSon>(() => CaravanArrivalAction_VoidSon.CanVisit(caravan, this), () => new CaravanArrivalAction_VoidSon(this), "VisitVoidSon".Translate(new object[]
            {
                this.Label
            }), caravan, this.Tile, this);
        }




        private void Outcome_Fail(Caravan caravan)
        {
            Find.LetterStack.ReceiveLetter("LetterLabelVoidSon_Fail".Translate(), this.GetLetterText("LetterVoidSon_Fail".Translate(new object[]
            {
                caravan.pawns[0].LabelCap
            }), caravan), LetterDefOf.NeutralEvent, caravan, base.Faction, null);
        }

        // Token: 0x06001F46 RID: 8006 RVA: 0x000F2028 File Offset: 0x000F0428
        private void Outcome_Success(Caravan caravan)
        {
            caravan.pawns[0].story.traits.GainTrait(new Trait(TraitDef.Named("Warframe_Trait")));

            
            Find.LetterStack.ReceiveLetter("LetterLabelVoidSon_Success".Translate(), this.GetLetterText("LetterVoidSon_Success".Translate(new object[]
            {
                 caravan.pawns[0].LabelCap
            }), caravan), LetterDefOf.PositiveEvent, caravan, base.Faction, null);
        }



        // Token: 0x06001F48 RID: 8008 RVA: 0x000F21AC File Offset: 0x000F05AC
        private string GetLetterText(string baseText, Caravan caravan)
        {
            string text = baseText;
           
           
            return text;
        }




        // Token: 0x040012A8 RID: 4776
        private Material cachedMat;

        // Token: 0x040012AD RID: 4781
        private const float BaseWeight_Success = 0.75f;



    }
}
