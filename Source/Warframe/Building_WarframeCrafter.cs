using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.Sound;

namespace Warframe
{
    public class Building_WarframeCrafter:Building
    {


        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo gz in base.GetGizmos()) {
                yield return gz;
            }

            if (DebugSettings.godMode && curState==CraftState.Crafting)
            {
                yield return new Command_Action
                {
                    defaultLabel = "Debug:Finish",
                    action = delegate
                    {
                        curState = CraftState.Done;
                    }
                };
            }
            yield return new Command_Action {
                defaultLabel="openWFCWindow".Translate(),
                icon = TexCommand.ForbidOff,
                action = delegate
                {
                    Find.WindowStack.Add(new Window_CraftWarframe(this));
                }
            };

            yield break;
           
        }
        public override void Tick()
        {
            base.Tick();
            if (curState==CraftState.Crafting && this.TryGetComp<CompPowerTrader>().PowerOn) {
                if (this.TryGetComp<CompRefuelable>().Fuel >= fuelCost)
                {
                    ticks++;
                    if (ticks % 120 == 0)
                    {
                        MoteMaker.ThrowMicroSparks(Position.ToVector3(),Map);
                        //SoundDefOf.FloatMenu_Open.PlayOneShot(this);
                    }
                    if (ticks >= oneDayTicks)
                    {
                        curState = CraftState.Done;
                    }
                }
            }
            if (curState==CraftState.Done) {
                ticks = 0;
                Pawn pawn = WarframeStaticMethods.GetWarframePawn(nowCraftKind);
                IntVec3 loc = CellFinder.RandomClosewalkCellNear(Position, Map, 2, null);
                Pawn pp = (Pawn)(GenSpawn.Spawn(pawn, loc, Map, WipeMode.Vanish));
                SoundDefOf.TinyBell.PlayOneShotOnCamera();
                this.TryGetComp<CompRefuelable>().ConsumeFuel(fuelCost);
                nowCraftKind = null;
                curState = CraftState.Stop;
                Head = null;
                Body = null;
                Inside = null;
            }


        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look<PawnKindDef>(ref nowCraftKind, "nowCraftKind");
            Scribe_Values.Look<int>(ref ticks, "ticks", 0, false);
            Scribe_Values.Look<Building_WarframeCrafter.CraftState>(ref curState,"curState",default(CraftState),false);
            Scribe_Defs.Look<ThingDef>(ref Head,"Head");
            Scribe_Defs.Look<ThingDef>(ref Body, "Body");
            Scribe_Defs.Look<ThingDef>(ref Inside, "Inside");
            Scribe_Values.Look<int>(ref fuelCost,"fuelCost",0,false);

        }

        public override string GetInspectString()
        {
            String text = base.GetInspectString();
            StringBuilder sb = new StringBuilder();
            sb.Append(text);
            if (nowCraftKind != null)
            {
                sb.Append("\n"+"WFCrafting".Translate()+":"+nowCraftKind.label);
                string part = "WFCAllPartDone".Translate();
                if (Inside == null)
                {
                    part= "PleaseInputWFPart".Translate(new object[] { ThingDef.Named("WFPart_"+nowCraftKind.defName.Replace("Warframe_","")+"_Inside").label});
                    if (Body == null)
                    {
                        part = "PleaseInputWFPart".Translate(new object[] { ThingDef.Named("WFPart_" + nowCraftKind.defName.Replace("Warframe_", "") + "_Body").label });
                        if (Head == null)
                        {
                            part = "PleaseInputWFPart".Translate(new object[] { ThingDef.Named("WFPart_" + nowCraftKind.defName.Replace("Warframe_", "") + "_Head").label });
                        }
                    }
                }
                sb.Append("\n"+part);

                if (fuelCost > this.TryGetComp<CompRefuelable>().Fuel)
                {
                    sb.Append("\n"+"NeedMoreOrokinFuel".Translate(new object[] { this.TryGetComp<CompRefuelable>().Props.fuelFilter.AllowedThingDefs.ToList()[0].label})+":"+(fuelCost-this.TryGetComp<CompRefuelable>().Fuel));
                }
            }
            if (curState == CraftState.Crafting)
            {
                sb.Append("\n"+"WFCraftingTicks".Translate()+":"+(1.0f - (ticks*1.0f/oneDayTicks*1.0f)).ToString("f1")+ "DaysLower".Translate());
            }else
            {
                if (DebugSettings.godMode)
                {
                    sb.Append("\n"+"DEBUGNOWSTATE:"+curState);

                }
            }







            return sb.ToString();
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
           // this.ticks = 0;
            /*
            if(this.curState <0)
             this.curState = CraftState.Stop;
             */

        }
        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            tryDropAllParts();
            base.Destroy(mode);
         
        }
        public bool allPartAlready() {
            return Head != null && Body != null && Inside != null;
        }
        public ThingDef findNextPart() {
            if (curState != CraftState.Filling) return null;

            if (Head == null)
                return ThingDef.Named("WFPart_" + nowCraftKind.defName.Replace("Warframe_", "") + "_Head");
            if (Body == null)
                return ThingDef.Named("WFPart_" + nowCraftKind.defName.Replace("Warframe_", "") + "_Body");
            if (Inside == null)
                return ThingDef.Named("WFPart_" + nowCraftKind.defName.Replace("Warframe_", "") + "_Inside");

            return null;
        }
        public void AddPart(Thing thing)
        {
            if (thing.def.defName.EndsWith("_Head"))
            {
                Head = thing.def;
            }
            if (thing.def.defName.EndsWith("_Body"))
            {
                Body = thing.def;
            }
            if (thing.def.defName.EndsWith("_Inside"))
            {
                Inside = thing.def;
            }
        }
        public void tryDropAllParts() {
            if (Head != null)
             GenPlace.TryPlaceThing(ThingMaker.MakeThing(Head), Position,Map,ThingPlaceMode.Near);
            if (Body != null)
                GenPlace.TryPlaceThing(ThingMaker.MakeThing(Body), Position, Map, ThingPlaceMode.Near);
            if (Inside != null)
                GenPlace.TryPlaceThing(ThingMaker.MakeThing(Inside), Position, Map, ThingPlaceMode.Near);
        }
        public PawnKindDef nowCraftKind = null;

        public CraftState curState = CraftState.Stop;
        public int ticks=0;
        public int fuelCost = 0;
        private readonly int oneDayTicks = 60000;
        public ThingDef Head;
        public ThingDef Body;
        public ThingDef Inside;

        public enum CraftState
        {
            Stop,
            Filling,
            Crafting,
            Done
        }
    }
}
