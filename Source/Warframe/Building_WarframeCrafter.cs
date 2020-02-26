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

            if (DebugSettings.godMode && this.curState==CraftState.Crafting)
            {
                yield return new Command_Action
                {
                    defaultLabel = "Debug:Finish",
                    action = delegate
                    {
                        this.curState = CraftState.Done;
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
            if (this.curState==CraftState.Crafting && this.TryGetComp<CompPowerTrader>().PowerOn) {
                if (this.TryGetComp<CompRefuelable>().Fuel >= this.fuelCost)
                {
                    this.ticks++;
                    if (this.ticks % 120 == 0)
                    {
                        MoteMaker.ThrowMicroSparks(this.Position.ToVector3(),this.Map);
                        //SoundDefOf.FloatMenu_Open.PlayOneShot(this);
                    }
                    if (this.ticks >= this.oneDayTicks)
                    {
                        this.curState = CraftState.Done;
                    }
                }
            }
            if (this.curState==CraftState.Done) {
                this.ticks = 0;
                Pawn pawn = WarframeStaticMethods.getWarframePawn(nowCraftKind);
                IntVec3 loc = CellFinder.RandomClosewalkCellNear(this.Position, this.Map, 2, null);
                Pawn pp = (Pawn)(GenSpawn.Spawn(pawn, loc, this.Map, WipeMode.Vanish));
                SoundDefOf.TinyBell.PlayOneShotOnCamera();
                this.TryGetComp<CompRefuelable>().ConsumeFuel(this.fuelCost);
                this.nowCraftKind = null;
                this.curState = CraftState.Stop;
                this.Head = null;
                this.Body = null;
                this.Inside = null;
            }


        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look<PawnKindDef>(ref this.nowCraftKind, "nowCraftKind");
            Scribe_Values.Look<int>(ref this.ticks, "ticks", 0, false);
            Scribe_Values.Look<Building_WarframeCrafter.CraftState>(ref this.curState,"curState",default(CraftState),false);
            Scribe_Defs.Look<ThingDef>(ref this.Head,"Head");
            Scribe_Defs.Look<ThingDef>(ref this.Body, "Body");
            Scribe_Defs.Look<ThingDef>(ref this.Inside, "Inside");
            Scribe_Values.Look<int>(ref this.fuelCost,"fuelCost",0,false);

        }

        public override string GetInspectString()
        {
            String text = base.GetInspectString();
            StringBuilder sb = new StringBuilder();
            sb.Append(text);
            if (this.nowCraftKind != null)
            {
                sb.Append("\n"+"WFCrafting".Translate()+":"+this.nowCraftKind.label);
                string part = "WFCAllPartDone".Translate();
                if (this.Inside == null)
                {
                    part= "PleaseInputWFPart".Translate(new object[] { ThingDef.Named("WFPart_"+this.nowCraftKind.defName.Replace("Warframe_","")+"_Inside").label});
                    if (this.Body == null)
                    {
                        part = "PleaseInputWFPart".Translate(new object[] { ThingDef.Named("WFPart_" + this.nowCraftKind.defName.Replace("Warframe_", "") + "_Body").label });
                        if (this.Head == null)
                        {
                            part = "PleaseInputWFPart".Translate(new object[] { ThingDef.Named("WFPart_" + this.nowCraftKind.defName.Replace("Warframe_", "") + "_Head").label });
                        }
                    }
                }
                sb.Append("\n"+part);

                if (this.fuelCost > this.TryGetComp<CompRefuelable>().Fuel)
                {
                    sb.Append("\n"+"NeedMoreOrokinFuel".Translate(new object[] { this.TryGetComp<CompRefuelable>().Props.fuelFilter.AllowedThingDefs.ToList()[0].label})+":"+(this.fuelCost-this.TryGetComp<CompRefuelable>().Fuel));
                }
            }
            if (this.curState == CraftState.Crafting)
            {
                sb.Append("\n"+"WFCraftingTicks".Translate()+":"+(1.0f - (this.ticks*1.0f/this.oneDayTicks*1.0f)).ToString("f1")+ "DaysLower".Translate());
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
            if (this.curState != CraftState.Filling) return null;

            if (this.Head == null)
                return ThingDef.Named("WFPart_" + this.nowCraftKind.defName.Replace("Warframe_", "") + "_Head");
            if (this.Body == null)
                return ThingDef.Named("WFPart_" + this.nowCraftKind.defName.Replace("Warframe_", "") + "_Body");
            if (this.Inside == null)
                return ThingDef.Named("WFPart_" + this.nowCraftKind.defName.Replace("Warframe_", "") + "_Inside");

            return null;
        }
        public void AddPart(Thing thing)
        {
            if (thing.def.defName.EndsWith("_Head"))
            {
                this.Head = thing.def;
            }
            if (thing.def.defName.EndsWith("_Body"))
            {
                this.Body = thing.def;
            }
            if (thing.def.defName.EndsWith("_Inside"))
            {
                this.Inside = thing.def;
            }
        }
        public void tryDropAllParts() {
            if (this.Head != null)
             GenPlace.TryPlaceThing(ThingMaker.MakeThing(this.Head), this.Position,this.Map,ThingPlaceMode.Near);
            if (this.Body != null)
                GenPlace.TryPlaceThing(ThingMaker.MakeThing(this.Body), this.Position, this.Map, ThingPlaceMode.Near);
            if (this.Inside != null)
                GenPlace.TryPlaceThing(ThingMaker.MakeThing(this.Inside), this.Position, this.Map, ThingPlaceMode.Near);
        }
        public PawnKindDef nowCraftKind = null;

        public CraftState curState = CraftState.Stop;
        public int ticks=0;
        public int fuelCost = 0;
        private int oneDayTicks = 60000;
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
