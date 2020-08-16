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
            if(becontroler!=null)
             sb.Append(becontroler.kindDef.label);
            else
             sb.Append("NothingLower".Translate());

            return sb.ToString();
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look<Pawn>(ref becontroler,"becontroler",false);
        }
        // Token: 0x060024DC RID: 9436 RVA: 0x00114DDF File Offset: 0x001131DF
        public override bool TryAcceptThing(Thing thing, bool allowSpecialEffects = true)
        {
            if (base.TryAcceptThing(thing, allowSpecialEffects))
            {
                if (allowSpecialEffects)
                {
                    SoundDefOf.CryptosleepCasket_Accept.PlayOneShot(new TargetInfo(Position, Map, false));
                }
                return true;
            }
            return false;
        }

        public override void Tick()
        {
            base.Tick();
            if(becontroler!=null)
             WFModBase.Instance._WFcontrolstorage.checkControlerExist(this);
        }

        // Token: 0x060024DD RID: 9437 RVA: 0x00114E18 File Offset: 0x00113218
        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn myPawn)
        {
            foreach (FloatMenuOption o in base.GetFloatMenuOptions(myPawn))
            {
                yield return o;
            }
            if (innerContainer.Count == 0)
            {
                if (!myPawn.CanReach(this, PathEndMode.InteractionCell, Danger.Deadly, false, TraverseMode.ByPawn))
                {
                    FloatMenuOption failer = new FloatMenuOption("CannotUseNoPath".Translate(), null, MenuOptionPriority.Default, null, null, 0f, null, null);
                    yield return failer;
                } else if (myPawn.IsWarframe()) {
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


            if (Faction == Faction.OfPlayer && innerContainer.Count > 0 && def.building.isPlayerEjectable)
            {
                Command_Action eject = new Command_Action
                {
                    action = new Action(EjectContents),
                    defaultLabel = "CommandPodEject".Translate(),
                    defaultDesc = "CommandPodEjectDesc".Translate()
                };
                if (innerContainer.Count == 0)
                {
                    eject.Disable("CommandPodEjectFailEmpty".Translate());
                }
                eject.hotKey = KeyBindingDefOf.Misc1;
                eject.icon = ContentFinder<Texture2D>.Get("UI/Commands/PodEject", true);
                yield return eject;
            }


            TargetingParameters tp = new TargetingParameters
            {
                canTargetBuildings = false,
                canTargetFires = false,
                canTargetItems = false,
                canTargetLocations = false,
                canTargetPawns = true,
                canTargetSelf = false
            };

            Command_Target tar = new Command_Target
            {
                targetingParams = tp,
                icon = TexCommand.Attack,
                action = delegate (Thing target)
                {
                    if (target is Pawn)
                    {



                        if ((target as Pawn).IsWarframe())
                        {
                            if (becontroler != null || WFModBase.Instance._WFcontrolstorage.checkBeControlerExist((target as Pawn)))
                            {
                                SoundDefOf.ClickReject.PlayOneShotOnCamera();
                                return;
                            }
                            becontroler = target as Pawn;
                            WFModBase.Instance._WFcontrolstorage.ControlSomeone(this, becontroler);//.add2pawnRelation(this,becontroler);

                        }

                    }
                },
                defaultLabel = "ControlCellChooseWarframe".Translate(),
                defaultDesc = "ControlCellChooseWarframeDesc".Translate()
            };

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
            Command_Action cancela = new Command_Action
            {
                defaultLabel = "RemoveChooseWFLabel".Translate(),
                defaultDesc = "RemoveChooseWFDesc".Translate(),
                icon = TexCommand.RemoveRoutePlannerWaypoint,
                hotKey = KeyBindingDefOf.Cancel,
                activateSound = SoundDefOf.ClickReject,
                action = delegate
                {
                    if (becontroler != null)
                        WFModBase.Instance._WFcontrolstorage.remove2pawnRelation(this, becontroler);

                    becontroler = null;

                }
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
            if (!Destroyed)
            {
                SoundDefOf.CryptosleepCasket_Eject.PlayOneShot(SoundInfo.InMap(new TargetInfo(Position, Map, false), MaintenanceType.None));
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
