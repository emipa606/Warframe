using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.Sound;

namespace Warframe
{
    public class Building_WarframeCrafter : Building
    {
        public enum CraftState
        {
            Stop,
            Filling,
            Crafting,
            Done
        }

        private readonly int oneDayTicks = 60000;
        public ThingDef Body;

        public CraftState curState = CraftState.Stop;
        public int fuelCost;
        public ThingDef Head;
        public ThingDef Inside;
        public PawnKindDef nowCraftKind;
        public int ticks;


        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (var gz in base.GetGizmos())
            {
                yield return gz;
            }

            if (DebugSettings.godMode && curState == CraftState.Crafting)
            {
                yield return new Command_Action
                {
                    defaultLabel = "Debug:Finish",
                    action = delegate { curState = CraftState.Done; }
                };
            }

            yield return new Command_Action
            {
                defaultLabel = "openWFCWindow".Translate(),
                icon = TexCommand.ForbidOff,
                action = delegate { Find.WindowStack.Add(new Window_CraftWarframe(this)); }
            };
        }

        public override void Tick()
        {
            base.Tick();
            if (curState == CraftState.Crafting && this.TryGetComp<CompPowerTrader>().PowerOn)
            {
                if (this.TryGetComp<CompRefuelable>().Fuel >= fuelCost)
                {
                    ticks++;
                    if (ticks % 120 == 0)
                    {
                        FleckMaker.ThrowMicroSparks(Position.ToVector3(), Map);
                        //SoundDefOf.FloatMenu_Open.PlayOneShot(this);
                    }

                    if (ticks >= oneDayTicks)
                    {
                        curState = CraftState.Done;
                    }
                }
            }

            if (curState != CraftState.Done)
            {
                return;
            }

            ticks = 0;
            var pawn = WarframeStaticMethods.GetWarframePawn(nowCraftKind);
            var loc = CellFinder.RandomClosewalkCellNear(Position, Map, 2);
            var unused = (Pawn) GenSpawn.Spawn(pawn, loc, Map);
            SoundDefOf.TinyBell.PlayOneShotOnCamera();
            this.TryGetComp<CompRefuelable>().ConsumeFuel(fuelCost);
            nowCraftKind = null;
            curState = CraftState.Stop;
            Head = null;
            Body = null;
            Inside = null;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look(ref nowCraftKind, "nowCraftKind");
            Scribe_Values.Look(ref ticks, "ticks");
            Scribe_Values.Look(ref curState, "curState");
            Scribe_Defs.Look(ref Head, "Head");
            Scribe_Defs.Look(ref Body, "Body");
            Scribe_Defs.Look(ref Inside, "Inside");
            Scribe_Values.Look(ref fuelCost, "fuelCost");
        }

        public override string GetInspectString()
        {
            var text = base.GetInspectString();
            var sb = new StringBuilder();
            sb.Append(text);
            if (nowCraftKind != null)
            {
                sb.Append("\n" + "WFCrafting".Translate() + ":" + nowCraftKind.label);
                string part = "WFCAllPartDone".Translate();
                if (Inside == null)
                {
                    part = "PleaseInputWFPart".Translate(ThingDef
                        .Named("WFPart_" + nowCraftKind.defName.Replace("Warframe_", "") + "_Inside").label);
                    if (Body == null)
                    {
                        part = "PleaseInputWFPart".Translate(ThingDef
                            .Named("WFPart_" + nowCraftKind.defName.Replace("Warframe_", "") + "_Body").label);
                        if (Head == null)
                        {
                            part = "PleaseInputWFPart".Translate(ThingDef
                                .Named("WFPart_" + nowCraftKind.defName.Replace("Warframe_", "") + "_Head")
                                .label);
                        }
                    }
                }

                sb.Append("\n" + part);

                if (fuelCost > this.TryGetComp<CompRefuelable>().Fuel)
                {
                    sb.Append("\n" +
                              "NeedMoreOrokinFuel".Translate(this.TryGetComp<CompRefuelable>().Props.fuelFilter
                                  .AllowedThingDefs.ToList()[0].label) + ":" +
                              (fuelCost - this.TryGetComp<CompRefuelable>().Fuel));
                }
            }

            if (curState == CraftState.Crafting)
            {
                sb.Append("\n" + "WFCraftingTicks".Translate() + ":" +
                          (1.0f - (ticks * 1.0f / oneDayTicks * 1.0f)).ToString("f1") + "DaysLower".Translate());
            }
            else
            {
                if (DebugSettings.godMode)
                {
                    sb.Append("\n" + "DEBUGNOWSTATE:" + curState);
                }
            }


            return sb.ToString();
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            tryDropAllParts();
            base.Destroy(mode);
        }

        public bool allPartAlready()
        {
            return Head != null && Body != null && Inside != null;
        }

        public ThingDef findNextPart()
        {
            if (curState != CraftState.Filling)
            {
                return null;
            }

            if (Head == null)
            {
                return ThingDef.Named("WFPart_" + nowCraftKind.defName.Replace("Warframe_", "") + "_Head");
            }

            if (Body == null)
            {
                return ThingDef.Named("WFPart_" + nowCraftKind.defName.Replace("Warframe_", "") + "_Body");
            }

            if (Inside == null)
            {
                return ThingDef.Named("WFPart_" + nowCraftKind.defName.Replace("Warframe_", "") + "_Inside");
            }

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

        public void tryDropAllParts()
        {
            if (Head != null)
            {
                GenPlace.TryPlaceThing(ThingMaker.MakeThing(Head), Position, Map, ThingPlaceMode.Near);
            }

            if (Body != null)
            {
                GenPlace.TryPlaceThing(ThingMaker.MakeThing(Body), Position, Map, ThingPlaceMode.Near);
            }

            if (Inside != null)
            {
                GenPlace.TryPlaceThing(ThingMaker.MakeThing(Inside), Position, Map, ThingPlaceMode.Near);
            }
        }
    }
}