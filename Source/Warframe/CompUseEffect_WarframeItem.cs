﻿using RimWorld;
using Verse;
using Verse.Sound;

namespace Warframe
{
    internal class CompUseEffect_WarframeItem : CompUseEffect
    {
        // Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
        public override void DoEffect(Pawn usedBy)
        {
            base.DoEffect(usedBy);

            var pk = WarframeStaticMethods.GetAllWarframeKind().RandomElement();
            if (pk != null)
            {
                var wf = WarframeStaticMethods.GetWarframePawn(pk);
                GenSpawn.Spawn(wf, usedBy.Position, usedBy.Map);
            }

            usedBy.story.traits.GainTrait(new Trait(TraitDef.Named("Warframe_Trait")));
            FinishInstantly();
        }

        // Token: 0x06000002 RID: 2 RVA: 0x000021E0 File Offset: 0x000003E0
        public override bool CanBeUsedBy(Pawn p, out string failReason)
        {
            failReason = null;
            return true;
        }

        // Token: 0x06000003 RID: 3 RVA: 0x000021E6 File Offset: 0x000003E6
        private void FinishInstantly()
        {
            SoundDefOf.DialogBoxAppear.PlayOneShotOnCamera();
        }
    }
}