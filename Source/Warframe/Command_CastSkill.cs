using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Warframe
{
    public class Command_CastSkill:Command
	{
		// Token: 0x06005899 RID: 22681 RVA: 0x00288809 File Offset: 0x00286C09
		public override void ProcessInput(Event ev)
    {
        base.ProcessInput(ev);
        SoundDefOf.Tick_Tiny.PlayOneShotOnCamera(null);
          
                this.action(self);
        
     }

    // Token: 0x0600589A RID: 22682 RVA: 0x0028883C File Offset: 0x00286C3C
    public override bool InheritInteractionsFrom(Gizmo other)
    {
        return false;
    }


        public override void GizmoUpdateOnMouseover()
        {
            // GenDraw.DrawFieldEdges(WarframeStaticMethods.getCellsAround(self.Position,self.Map,range));
            GenDraw.DrawRadiusRing(self.Position,range);
        }

        // Token: 0x04003B2D RID: 15149
        public Action<Pawn> action;
        public Pawn self;
        public float range;
        public float cooldownTime;
    // Token: 0x04003B2E RID: 15150
    public TargetingParameters targetingParams;
}
}
