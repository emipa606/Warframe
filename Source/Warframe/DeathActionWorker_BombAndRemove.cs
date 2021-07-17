using RimWorld;
using Verse;

namespace Warframe
{
    public class DeathActionWorker_BombAndRemove : DeathActionWorker
    {
        // Token: 0x060000BE RID: 190 RVA: 0x0000AE2C File Offset: 0x0000902C
        public override void PawnDied(Corpse corpse)
        {
            if (corpse == null)
            {
                return;
            }

            var map = corpse.Map;
            var position = corpse.Position;
            var innerPawn = corpse.InnerPawn;
            GenExplosion.DoExplosion(corpse.Position, corpse.Map, 1.9f, DamageDefOf.Extinguish, corpse.InnerPawn);
            corpse.Destroy();
            var thingOwner = new ThingOwner<Thing>();
            foreach (var thingDefCountClass in innerPawn.def.butcherProducts)
            {
                var thing = ThingMaker.MakeThing(thingDefCountClass.thingDef);
                thing.stackCount = thingDefCountClass.count;
                thingOwner.TryAdd(thing);
            }

            foreach (var thing in thingOwner)
            {
                GenPlace.TryPlaceThing(thing, position, map, ThingPlaceMode.Near);
            }
        }
    }
}