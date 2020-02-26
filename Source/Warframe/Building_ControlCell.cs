using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace Warframe
{
    // Token: 0x020006C2 RID: 1730
    public class Building_ControlCell : Building_Casket
    {
        public Pawn becontroler = null;
        public override string GetInspectString()
        {
           
            StringBuilder sb = new StringBuilder();
            sb.Append(base.GetInspectString());
            sb.Append("\n");
            sb.Append("CellControlWF".Translate() + ":");
            if(this.becontroler!=null)
             sb.Append(this.becontroler.kindDef.label);
            else
             sb.Append("NothingLower".Translate());

            return sb.ToString();
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look<Pawn>(ref this.becontroler,"becontroler",false);
        }
        // Token: 0x060024DC RID: 9436 RVA: 0x00114DDF File Offset: 0x001131DF
        public override bool TryAcceptThing(Thing thing, bool allowSpecialEffects = true)
        {
            if (base.TryAcceptThing(thing, allowSpecialEffects))
            {
                if (allowSpecialEffects)
                {
                    SoundDefOf.CryptosleepCasket_Accept.PlayOneShot(new TargetInfo(base.Position, base.Map, false));
                }
                return true;
            }
            return false;
        }

        public override void Tick()
        {
            base.Tick();
            if(this.becontroler!=null)
             WFModBase.Instance._WFcontrolstorage.checkControlerExist(this);
        }

        // Token: 0x060024DD RID: 9437 RVA: 0x00114E18 File Offset: 0x00113218
        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn myPawn)
        {
            foreach (FloatMenuOption o in base.GetFloatMenuOptions(myPawn))
            {
                yield return o;
            }
            if (this.innerContainer.Count == 0)
            {
                if (!myPawn.CanReach(this, PathEndMode.InteractionCell, Danger.Deadly, false, TraverseMode.ByPawn))
                {
                    FloatMenuOption failer = new FloatMenuOption("CannotUseNoPath".Translate(), null, MenuOptionPriority.Default, null, null, 0f, null, null);
                    yield return failer;
                } else if (myPawn.isWarframe()) {
                    FloatMenuOption failer = new FloatMenuOption("CannotWFenter".Translate(), null, MenuOptionPriority.Default, null, null, 0f, null, null);
                    yield return failer;
                }
                else
                {
                    /*
                    bool flag = false;
                    foreach(Trait tt in myPawn.story.traits.allTraits)
                    {
                        if (tt.def.defName.Equals("Warframe_Trait"))
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        FloatMenuOption failer = new FloatMenuOption("PawnWithOutWFTrait".Translate(new object[] {myPawn.Name} ), null, MenuOptionPriority.Default, null, null, 0f, null, null);
                        yield return failer;
                        yield break;
                    }
                    */


                    JobDef jobDef = DefDatabase<JobDef>.GetNamed("EnterControlCell", true);
                    string jobStr = "EnterControlCell".Translate();
                    Action jobAction = delegate
                    {
                        Job job = new Job(jobDef, this);
                        myPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                    };
                    yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(jobStr, jobAction, MenuOptionPriority.Default, null, null, 0f, null, null), myPawn, this, "ReservedBy");
                }
            }
            yield break;
        }

        // Token: 0x060024DE RID: 9438 RVA: 0x00114E44 File Offset: 0x00113244
        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo c in base.GetGizmos())
            {
                yield return c;
            }


            if (base.Faction == Faction.OfPlayer && this.innerContainer.Count > 0 && this.def.building.isPlayerEjectable)
            {
                Command_Action eject = new Command_Action();
                eject.action = new Action(this.EjectContents);
                eject.defaultLabel = "CommandPodEject".Translate();
                eject.defaultDesc = "CommandPodEjectDesc".Translate();
                if (this.innerContainer.Count == 0)
                {
                    eject.Disable("CommandPodEjectFailEmpty".Translate());
                }
                eject.hotKey = KeyBindingDefOf.Misc1;
                eject.icon = ContentFinder<Texture2D>.Get("UI/Commands/PodEject", true);
                yield return eject;
            }


            TargetingParameters tp = new TargetingParameters();
            tp.canTargetBuildings = false;
            tp.canTargetFires = false;
            tp.canTargetItems = false;
            tp.canTargetLocations = false;
            tp.canTargetPawns = true;
            tp.canTargetSelf = false;

            Command_Target tar = new Command_Target();
            tar.targetingParams = tp;
            tar.icon = TexCommand.Attack;
            tar.action = delegate(Thing target) {
              if(target is Pawn)
                {
                    


                    if((target as Pawn).isWarframe())
                    {
                        if(this.becontroler != null||WFModBase.Instance._WFcontrolstorage.checkBeControlerExist((target as Pawn)))
                        {
                            SoundDefOf.ClickReject.PlayOneShotOnCamera();
                            return;
                        }
                        this.becontroler = target as Pawn;
                        WFModBase.Instance._WFcontrolstorage.ControlSomeone(this,becontroler);//.add2pawnRelation(this,becontroler);

                    }

                }
            };
            tar.defaultLabel = "ControlCellChooseWarframe".Translate();
            tar.defaultDesc = "ControlCellChooseWarframeDesc".Translate();

            yield return tar;
            /*
            if (this.becontroler != null && this.HasAnyContents)
            {

                Command_Action ca = new Command_Action();
                ca.defaultLabel = "CommandDraftLabel".Translate();
                ca.defaultDesc = "CommandToggleDraftDesc".Translate();
                ca.icon = TexCommand.Draft;
                ca.hotKey = KeyBindingDefOf.Command_ColonistDraft;
                ca.activateSound= SoundDefOf.DraftOn;
                ca.action = delegate
                {
                  //  Log.Warning(this.becontroler.Position+"|");
                    this.becontroler.drafter.Drafted = !this.becontroler.drafter.Drafted;

                };
                yield return ca;
            }
            */
            Command_Action cancela = new Command_Action();
            cancela.defaultLabel = "RemoveChooseWFLabel".Translate();
            cancela.defaultDesc = "RemoveChooseWFDesc".Translate();
            cancela.icon = TexCommand.RemoveRoutePlannerWaypoint;
            cancela.hotKey = KeyBindingDefOf.Cancel;
            cancela.activateSound = SoundDefOf.ClickReject;
            cancela.action = delegate
            {
                if (this.becontroler != null)
                    WFModBase.Instance._WFcontrolstorage.remove2pawnRelation(this,this.becontroler);

                this.becontroler = null;

            };
            yield return cancela;

            yield break;
        }

        // Token: 0x060024DF RID: 9439 RVA: 0x00114E68 File Offset: 0x00113268
        public override void EjectContents()
        {
            /*
            ThingDef filth_Slime = ThingDefOf.Filth_Slime;
            foreach (Thing thing in ((IEnumerable<Thing>)this.innerContainer))
            {
                Pawn pawn = thing as Pawn;
                if (pawn != null)
                {
                    PawnComponentsUtility.AddComponentsForSpawn(pawn);
                    pawn.filth.GainFilth(filth_Slime);
                    if (pawn.RaceProps.IsFlesh)
                    {
                        pawn.health.AddHediff(HediffDefOf.CryptosleepSickness, null, null, null);
                    }
                }
            }
            */
            if (!base.Destroyed)
            {
                SoundDefOf.CryptosleepCasket_Eject.PlayOneShot(SoundInfo.InMap(new TargetInfo(base.Position, base.Map, false), MaintenanceType.None));
            }
            /*
            try {
                WFModBase.Instance._WFcontrolstorage.remove2pawnRelation(this,becontroler);
            } catch (Exception e) {
                Log.Warning(e+"");
            }
            */
            base.EjectContents();
        }

      
    }
}
