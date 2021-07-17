using System.Collections.Generic;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace Warframe
{
    // Token: 0x020006C2 RID: 1730
    public class Building_ControlCell : Building_Casket
    {
        public Pawn becontroler;

        public override string GetInspectString()
        {
            var sb = new StringBuilder();
            sb.Append(base.GetInspectString());
            sb.Append("\n");
            sb.Append("CellControlWF".Translate() + ":");
            if (becontroler != null)
            {
                sb.Append(becontroler.kindDef.label);
            }
            else
            {
                sb.Append("NothingLower".Translate());
            }

            return sb.ToString();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref becontroler, "becontroler");
        }

        // Token: 0x060024DC RID: 9436 RVA: 0x00114DDF File Offset: 0x001131DF
        public override bool TryAcceptThing(Thing thing, bool allowSpecialEffects = true)
        {
            if (!base.TryAcceptThing(thing, allowSpecialEffects))
            {
                return false;
            }

            if (allowSpecialEffects)
            {
                SoundDefOf.CryptosleepCasket_Accept.PlayOneShot(new TargetInfo(Position, Map));
            }

            return true;
        }

        public override void Tick()
        {
            base.Tick();
            if (becontroler != null)
            {
                WFModBase.Instance._WFcontrolstorage.checkControlerExist(this);
            }
        }

        // Token: 0x060024DD RID: 9437 RVA: 0x00114E18 File Offset: 0x00113218
        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn myPawn)
        {
            foreach (var o in base.GetFloatMenuOptions(myPawn))
            {
                yield return o;
            }

            if (innerContainer.Count != 0)
            {
                yield break;
            }

            if (!myPawn.CanReach(this, PathEndMode.InteractionCell, Danger.Deadly))
            {
                var failer = new FloatMenuOption("CannotUseNoPath".Translate(), null);
                yield return failer;
            }
            else if (myPawn.IsWarframe())
            {
                var failer = new FloatMenuOption("CannotWFenter".Translate(), null);
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


                var jobDef = DefDatabase<JobDef>.GetNamed("EnterControlCell");
                string jobStr = "EnterControlCell".Translate();

                void JobAction()
                {
                    var job = new Job(jobDef, this);
                    myPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                }

                yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(jobStr, JobAction),
                    myPawn, this);
            }
        }

        // Token: 0x060024DE RID: 9438 RVA: 0x00114E44 File Offset: 0x00113244
        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (var c in base.GetGizmos())
            {
                yield return c;
            }


            if (Faction == Faction.OfPlayer && innerContainer.Count > 0 && def.building.isPlayerEjectable)
            {
                var eject = new Command_Action
                {
                    action = EjectContents,
                    defaultLabel = "CommandPodEject".Translate(),
                    defaultDesc = "CommandPodEjectDesc".Translate()
                };
                if (innerContainer.Count == 0)
                {
                    eject.Disable("CommandPodEjectFailEmpty".Translate());
                }

                eject.hotKey = KeyBindingDefOf.Misc1;
                eject.icon = ContentFinder<Texture2D>.Get("UI/Commands/PodEject");
                yield return eject;
            }


            var tp = new TargetingParameters
            {
                canTargetBuildings = false,
                canTargetFires = false,
                canTargetItems = false,
                canTargetLocations = false,
                canTargetPawns = true,
                canTargetSelf = false
            };

            var tar = new Command_Target
            {
                targetingParams = tp,
                icon = TexCommand.Attack,
                action = delegate(LocalTargetInfo target)
                {
                    if (target.Pawn is not { })
                    {
                        return;
                    }

                    if (!target.Pawn.IsWarframe())
                    {
                        return;
                    }

                    if (becontroler != null ||
                        WFModBase.Instance._WFcontrolstorage.checkBeControlerExist(target.Pawn))
                    {
                        SoundDefOf.ClickReject.PlayOneShotOnCamera();
                        return;
                    }

                    becontroler = target.Pawn;
                    WFModBase.Instance._WFcontrolstorage.ControlSomeone(this,
                        becontroler); //.add2pawnRelation(this,becontroler);
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
            var cancela = new Command_Action
            {
                defaultLabel = "RemoveChooseWFLabel".Translate(),
                defaultDesc = "RemoveChooseWFDesc".Translate(),
                icon = TexCommand.RemoveRoutePlannerWaypoint,
                hotKey = KeyBindingDefOf.Cancel,
                activateSound = SoundDefOf.ClickReject,
                action = delegate
                {
                    if (becontroler != null)
                    {
                        WFModBase.Instance._WFcontrolstorage.remove2pawnRelation(this, becontroler);
                    }

                    becontroler = null;
                }
            };
            yield return cancela;
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
                SoundDefOf.CryptosleepCasket_Eject.PlayOneShot(SoundInfo.InMap(new TargetInfo(Position, Map)));
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