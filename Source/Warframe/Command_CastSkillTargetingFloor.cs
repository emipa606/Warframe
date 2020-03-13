using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Warframe
{
    public class Command_CastSkillTargetingFloor:Command_CastSkill
    {   // Token: 0x06005899 RID: 22681 RVA: 0x00288809 File Offset: 0x00286C09
        public override void ProcessInput(Event ev)
        {
            if (this.CurActivateSound != null)
            {
                this.CurActivateSound.PlayOneShotOnCamera(null);
            }
            SoundDefOf.Tick_Tiny.PlayOneShotOnCamera(null);
            Find.Targeter.BeginTargeting(this.targetingParams, delegate (LocalTargetInfo target)
                {
                    this.action(self, target);
                }, self, finishAction, null);



        }




        public new Action<Pawn, LocalTargetInfo> action;
        public Action finishAction;

       // public Thing target;
    }
    
}
