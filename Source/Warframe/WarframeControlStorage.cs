using HugsLib.Utils;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Text;
using Verse;

namespace Warframe
{
    public class WarframeControlStorage : UtilityWorldObject, IExposable
    {
        public Dictionary<Building_ControlCell, Pawn> ControlCellAndBeControler = new Dictionary<Building_ControlCell, Pawn>();
        public Dictionary<Pawn, Building_ControlCell> BeControlerAndControlCell = new Dictionary<Pawn, Building_ControlCell>();

       // public Dictionary<Pawn, ThingWithComps> WarframeAndOldgun = new Dictionary<Pawn, ThingWithComps>();

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref this.ControlCellAndBeControler, "ControlCellAndBeControler", LookMode.Reference, LookMode.Reference);
            Scribe_Collections.Look(ref this.BeControlerAndControlCell, "BeControlerAndControlCell", LookMode.Reference, LookMode.Reference);
           // Scribe_Collections.Look(ref this.WarframeAndOldgun, "WarframeAndOldgun", LookMode.Reference, LookMode.Reference);
        }
        /*
        public void saveOldGun(Pawn wf,ThingWithComps gun) {
            clearWFandOG(wf);
            WarframeAndOldgun.Add(wf,gun);
        }
        public ThingWithComps getOldGun(Pawn wf) {
            return WarframeAndOldgun.TryGetValue(wf);
        }
        public void clearWFandOG(Pawn wf) {
            if (WarframeAndOldgun.ContainsKey(wf))
            {
                WarframeAndOldgun.Remove(wf);
            }
        }
        */

        //清除建筑和战甲关系
        public void remove2pawnRelation(Building_ControlCell build, Pawn becontroler)
        {
            try
            {
                /*
                bool check1 = false;
                bool check2 = false;
                if (ControlCellAndBeControler.ContainsKey(build) && ControlCellAndBeControler.TryGetValue(build) == becontroler)
                {
                    check1 = true;
                }
                if (BeControlerAndControlCell.ContainsKey(becontroler) && BeControlerAndControlCell.TryGetValue(becontroler) == build)
                {
                    check2 = true;
                }
                if (check1 && check2)
                {
                */
                    //if (build != null && build.Spawned)
                        ControlCellAndBeControler.Remove(build);
                    build.becontroler = null;
                    //if (becontroler != null && becontroler.Spawned)
                        BeControlerAndControlCell.Remove(becontroler);
                    //SoundDef.Named("ra2_yuri_nocontrol").PlayOneShot(becontroler);
                    if (becontroler.Dead) return;
                    becontroler.drafter.Drafted = false;

                //}
            }catch (Exception)
            {

            }
        }
        //添加建筑战甲关系
        public void add2pawnRelation(Building_ControlCell build, Pawn becontroler)
        {

            ControlCellAndBeControler.Add(build, becontroler);
            BeControlerAndControlCell.Add(becontroler, build);
        }
        //
        public bool wfSpawned(Pawn wf) {
            foreach (Pawn ppod in PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_Colonists)
            {
                if (ppod==wf)
                {
                   return true;
                }
            }
            return false;
        }
        //检查控制仓是否存在,并看看战甲是否存在
        public void checkControlerExist(Building_ControlCell build)
        {
            
            if (ControlCellAndBeControler.ContainsKey(build))
            {
                Pawn becontroler = ControlCellAndBeControler.TryGetValue(build);
                if (build == null || !build.Spawned || build.Destroyed)
                {
                    remove2pawnRelation(build, becontroler);
                }
                if (becontroler == null || (!becontroler.Spawned && !wfSpawned(becontroler)) || becontroler.Dead )
                {
                   // Log.Warning(becontroler+"/"+becontroler.Spawned+"/"+becontroler.Dead+"/"+ becontroler.IsWorldPawn());
                    remove2pawnRelation(build, becontroler);
                }
            }
        }
        //检查被控制者是否存在
        public bool checkBeControlerExist(Pawn becontroler)
        {
            if (BeControlerAndControlCell.ContainsKey(becontroler))
            {
               
                Building_ControlCell build = BeControlerAndControlCell.TryGetValue(becontroler);
                if (becontroler == null || (!becontroler.Spawned && !wfSpawned(becontroler)) || becontroler.Dead)
                {
                    remove2pawnRelation(build, becontroler);
                    return false;
                }
                if (build == null || !build.Spawned || build.Destroyed)
                {
                    remove2pawnRelation(build, becontroler);
                    return false;
                }
                return true;
            }
            return false;

        }
        //检查控制者是否有控制人
        public bool hasControledSomeone(Building_ControlCell controler)
        {
            if (controler != null)
                if (ControlCellAndBeControler.ContainsKey(controler))
                {
                    return true;
                }
            return false;
        }
        public bool ControlSomeone(Building_ControlCell controler, Pawn becontroler)
        {
           
            if (hasControledSomeone(controler))
            {
                remove2pawnRelation(controler, ControlCellAndBeControler.TryGetValue(controler));
            }

            add2pawnRelation(controler, becontroler);
           
            return true;
        }





    }
}
