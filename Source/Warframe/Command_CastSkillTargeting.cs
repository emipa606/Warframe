using System;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Warframe
{
    public class Command_CastSkillTargeting : Command_CastSkill
    {
        public new Action<Pawn, Thing> action;
        public Action finishAction; // Token: 0x06005899 RID: 22681 RVA: 0x00288809 File Offset: 0x00286C09

        public override void ProcessInput(Event ev)
        {
            if (CurActivateSound != null)
            {
                CurActivateSound.PlayOneShotOnCamera();
            }

            SoundDefOf.Tick_Tiny.PlayOneShotOnCamera();
            Find.Targeter.BeginTargeting(targetingParams,
                delegate(LocalTargetInfo target) { action(self, target.Thing); }, self, finishAction);
        }

        // public Thing target;
    }
}