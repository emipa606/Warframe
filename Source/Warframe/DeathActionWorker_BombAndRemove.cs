using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using Verse;

namespace Warframe
{
    public class DeathActionWorker_BombAndRemove : DeathActionWorker
    {
        // Token: 0x060000BE RID: 190 RVA: 0x0000AE2C File Offset: 0x0000902C
        public override void PawnDied(Corpse corpse)
        {
            bool flag = corpse == null;
            if (!flag)
            {
                Map map = corpse.Map;
                IntVec3 position = corpse.Position;
                Pawn innerPawn = corpse.InnerPawn;
                GenExplosion.DoExplosion(corpse.Position, corpse.Map, 1.9f, DamageDefOf.Extinguish, corpse.InnerPawn, -1, -1f, null, null, null, null, null, 0f, 1, false, null, 0f, 1, 0f, false);
                corpse.Destroy(DestroyMode.Vanish);
                ThingOwner<Thing> thingOwner = new ThingOwner<Thing>();
                for (int i = 0; i < innerPawn.def.butcherProducts.Count; i++)
                {
                    Thing thing = ThingMaker.MakeThing(innerPawn.def.butcherProducts[i].thingDef, null);
                    thing.stackCount = innerPawn.def.butcherProducts[i].count;
                    thingOwner.TryAdd(thing, true);
                }
                for (int j = 0; j < thingOwner.Count; j++)
                {
                    GenPlace.TryPlaceThing(thingOwner[j], position,map, ThingPlaceMode.Near, null, null);
                }
            }
        }

   
    }
}
