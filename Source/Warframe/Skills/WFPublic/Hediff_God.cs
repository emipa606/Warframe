﻿using Verse;

namespace Warframe.Skills.WFPublic
{
    public class Hediff_God : HediffWithComps
    {
        public bool del;

        public override void Tick()
        {
            if (del)
            {
                TimeOut();
            }
        }

        // Token: 0x06004BF8 RID: 19448 RVA: 0x00232324 File Offset: 0x00230724
        private void TimeOut()
        {
            pawn.health.RemoveHediff(this);
        }


        // Token: 0x06004BFA RID: 19450 RVA: 0x002324EB File Offset: 0x002308EB
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref del, "del");
        }
    }
}