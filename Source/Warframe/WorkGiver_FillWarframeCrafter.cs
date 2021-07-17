using RimWorld;
using Verse;
using Verse.AI;

namespace Warframe
{
    public class WorkGiver_FillWarframeCrafter : WorkGiver_Scanner
    {
        // Token: 0x17000026 RID: 38
        // (get) Token: 0x06000121 RID: 289 RVA: 0x00008553 File Offset: 0x00006753
        public override ThingRequest PotentialWorkThingRequest =>
            ThingRequest.ForDef(ThingDef.Named("Warframe_Crafter"));

        // Token: 0x17000027 RID: 39
        // (get) Token: 0x06000122 RID: 290 RVA: 0x0000227E File Offset: 0x0000047E
        public override PathEndMode PathEndMode => PathEndMode.Touch;

        // Token: 0x06000123 RID: 291 RVA: 0x00008560 File Offset: 0x00006760
        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (!(t is Building_WarframeCrafter bwc))
            {
                return false;
            }

            if (t.IsForbidden(pawn) || !pawn.CanReserveAndReach(t, PathEndMode.Touch, pawn.NormalMaxDanger()))
            {
                JobFailReason.Is("Forbidden".Translate());
                return false;
            }

            if (pawn.Map.designationManager.DesignationOn(t, DesignationDefOf.Deconstruct) != null)
            {
                return false;
            }

            if (t.IsBurning())
            {
                return false;
            }

            if (bwc.allPartAlready())
            {
                JobFailReason.Is("WFCAllPartAlready".Translate());
                return false;
            }

            var thingDef = bwc.findNextPart();
            if (thingDef == null)
            {
                JobFailReason.Is("WFCFillError".Translate());
                return false;
            }

            if (FindPart(pawn, thingDef) != null)
            {
                return true;
            }

            JobFailReason.Is("critNoSabotRounds".Translate());
            return false;
        }

        // Token: 0x06000124 RID: 292 RVA: 0x0000862C File Offset: 0x0000682C
        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (!(t is Building_WarframeCrafter bwc))
            {
                return null;
            }

            var thing = FindPart(pawn, bwc.findNextPart());
            if (thing == null)
            {
                return null;
            }

            return new Job(DefDatabase<JobDef>.GetNamed("JobFillWFC"), t, thing)
            {
                count = 1
            };
        }

        // Token: 0x06000125 RID: 293 RVA: 0x00008678 File Offset: 0x00006878
        private Thing FindPart(Pawn pawn, ThingDef thingDef)
        {
            bool Validator(Thing x)
            {
                return !x.IsForbidden(pawn) && pawn.CanReserve(x);
            }

            return GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(thingDef),
                PathEndMode.ClosestTouch, TraverseParms.For(pawn), 9999f, Validator);
        }
    }
}