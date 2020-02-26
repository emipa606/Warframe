using RimWorld;
using UnityEngine;
using System;
using Verse;
using Verse.Sound;

namespace Warframe
{
    public class Command_CastSkillTargeting:Command_CastSkill
    {   // Token: 0x06005899 RID: 22681 RVA: 0x00288809 File Offset: 0x00286C09
        public override void ProcessInput(Event ev)
        {

            if (CurActivateSound != null)
            {
                CurActivateSound.PlayOneShotOnCamera(null);
            }
            SoundDefOf.Tick_Tiny.PlayOneShotOnCamera(null);
            Find.Targeter.BeginTargeting(targetingParams, delegate (LocalTargetInfo target)
                {
                    action(self, target.Thing);
                }, self, finishAction, null);



        }

        public new Action<Pawn, Thing> action;
        public Action finishAction;

       // public Thing target;
    }
    
}
