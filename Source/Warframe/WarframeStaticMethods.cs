using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;
using Warframe.Skills;

namespace Warframe
{
    public static class WarframeStaticMethods
    {
        public static bool isWarframe(this Pawn pawn) {
            return pawn.RaceProps.FleshType.defName.EqualsIgnoreCase("warframe");
        }
        /*
        //创建一个wf
        public static Pawn CreateAWarframe(String def) {
            PawnGenerationRequest request = new PawnGenerationRequest(DefDatabase<PawnKindDef>.GetNamed(def, true), Faction.OfPlayer, PawnGenerationContext.NonPlayer, -1, false, false, false, false, false, true, 1f, false, true, true, false, false, false, false, null, null, null, null, null, null, null);
            Pawn item = PawnGenerator.GeneratePawn(request);
            Pawn_StoryTracker ps = item.story;
            ps.adulthood = null;
            ps.traits.allTraits = new List<Trait>();


            Pawn_WorkSettings pws = item.workSettings;
            pws.DisableAll();

            Pawn_PlayerSettings pps = item.playerSettings;
            pps.hostilityResponse = HostilityResponseMode.Attack;

            NameTriple triple = NameTriple.FromString(item.kindDef.label.Replace(" ", ""));
            item.Name = triple;
            item.inventory.DestroyAll();

            Pawn_EquipmentTracker pe = item.equipment;
            pe.DestroyAllEquipment();
            pe.AddEquipment((ThingWithComps)ThingMaker.MakeThing(ThingDef.Named("WF_"+def.Replace("Warframe_","")+"_Head")));
            pe.AddEquipment((ThingWithComps)ThingMaker.MakeThing(ThingDef.Named("WF_" + def.Replace("Warframe_", "") + "_Armor")));
            pe.AddEquipment((ThingWithComps)ThingMaker.MakeThing(ThingDef.Named("WF_" + def.Replace("Warframe_", "") + "_Belt")));

            


            return item;


        }
        */
        //设置性别
        public static Gender setGender(String kdef) {
            String def = kdef.Replace("Warframe_","");
            switch (def)
            {
                case "Excalibur":return Gender.Male;
                case "Volt":return Gender.Male;
                case "Ash":return Gender.Male;
                
            }
            return Gender.Female;
        }
        //获取目前受的伤害数值
        public static float getHP(Pawn pawn) {
            float num = 0f;
            Pawn_HealthTracker __instance = pawn.health;
            for (int i = 0; i < __instance.hediffSet.hediffs.Count; i++)
            {
                if (__instance.hediffSet.hediffs[i] is Hediff_Injury)
                {
                    num += __instance.hediffSet.hediffs[i].Severity;
                }else
                if (__instance.hediffSet.hediffs[i] is Hediff_MissingPart)
                {
                    num += __instance.hediffSet.hediffs[i].Part.def.hitPoints;
                }

            }
            //foreach (__instance.health.hediffSet.GetMissingPartsCommonAncestors()) { }

            return num;
        }
        //死亡条件
        public static bool shouldDie(float num,Pawn pawn) {

            bool flag1 = pawn.apparel.WornApparel != null && pawn.apparel.WornApparelCount>0;
            if (flag1) {
                foreach(Apparel ap in pawn.apparel.WornApparel)
                {
                    if (ap.def.defName.StartsWith("WF_") && ap.def.defName.EndsWith("_Belt"))
                    {
      
                        WarframeBelt wb = ap as WarframeBelt;
                        if(num > wb.MHP)
                        {
                            return true;
                        }
                        break;
                    }

                }
            }
            return false;
        }
        //获取所有战甲kind
        public static IEnumerable<PawnKindDef> getAllWarframeKind() {
            foreach (PawnKindDef pk in DefDatabase<PawnKindDef>.AllDefs) {
                if (pk.defName.StartsWith("Warframe_")) {
                    yield return pk;
                }
            }
            yield break;    
        }
        //获取战甲
        public static Pawn getWarframePawn(PawnKindDef pk) {

            PawnGenerationRequest request = new PawnGenerationRequest(pk, Faction.OfPlayer, PawnGenerationContext.NonPlayer, -1, false, false, false, false, true, true, 0, false, true, true, false, false, false, false, false, 0, null, 1, null, null, null, null, 12, null, null, setGender(pk.defName), null);
            Pawn item = PawnGenerator.GeneratePawn(request);
            item.story.adulthood = null;
            if(setGender(pk.defName) == Gender.Male)
            {
                item.story.bodyType = BodyTypeDefOf.Male;
            }else
            {
                item.story.bodyType = BodyTypeDefOf.Female;
            }



            item.inventory.DestroyAll();
            item.apparel.WornApparel.Clear();
            
            item.workSettings.DisableAll();
            String pname = pk.defName.Replace("Warframe_","");
            String nowgear = "Head";
            try
            {
                for (int i = 0; i < 3; i++)
                {
                    if (i == 1)
                        nowgear = "Armor";
                    else if (i == 2)
                        nowgear = "Belt";



                    Apparel ap = (Apparel)ThingMaker.MakeThing(ThingDef.Named("WF_" + pname + "_" + nowgear));
                    item.apparel.Wear(ap);
                }
            }catch(Exception e)
            {
                Log.Error(pk.defName +"'s apparel is not exist!");
                Log.Error("Details:"+e);
            }
            item.story.traits.allTraits.Clear();
            
            NameTriple triple = NameTriple.FromString(pk.label.Replace(" ", ""));
            triple.ResolveMissingPieces(pk.label[0]+(Find.TickManager.TicksGame%100).ToString());
            item.Name = triple;
            
            foreach (SkillRecord sr in item.skills.skills)
            {
                if(sr.def == SkillDefOf.Shooting || sr.def== SkillDefOf.Melee)
                sr.Level = 20;
            }
            item.playerSettings.hostilityResponse = HostilityResponseMode.Attack;
            item.story.traits.GainTrait(new Trait(TraitDefOf.NaturalMood, 1));

            return item;

        }
        //获取制作材料
        public static int getCraftCost(PawnKindDef pk) {
            string kdef = pk.defName.Replace("Warframe_","");
            switch (kdef) {
                case "Excalibur":return 1;
                
            }

            return 1;
        }
        //获取战甲等级
        public static int getWFLevel(Pawn pawn) {
            int level = 1;
            Pawn_RecordsTracker pr = pawn.records;
            if (pr != null)
            {
                float killh = pr.GetValue(RecordDefOf.KillsHumanlikes);
                float killm = pr.GetValue(RecordDefOf.KillsMechanoids);
                level = (int)(1f + (killh*2f+ killm*5f) / 10f);
            }
            return level>30?30:level;
        }
        //获取战甲等级 plus
        public static float getLevel(this Pawn pawn)
        {
            float level = 1;
            Pawn_RecordsTracker pr = pawn.records;
            if (pr != null)
            {
                float killh = pr.GetValue(RecordDefOf.KillsHumanlikes);
                float killm = pr.GetValue(RecordDefOf.KillsMechanoids);
                level = (1f + (killh * 2f + killm * 5f) / 10f);
            }
            return level > 30 ? 30f : level;
        }
        //获取某战甲某技能 反射
        public static Command_CastSkill getSkillCommand(String def, int slot)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            if (def=="Ash"||def=="Mesa"||def== "Valkyr")
            {
                assembly = Assembly.Load("WarframeMAV");
            }
            Type wfclass = assembly.GetType("Warframe.Skills."+def);
            if (wfclass != null)
            {
                MethodInfo method = wfclass.GetMethod("Skill"+slot, BindingFlags.Public|BindingFlags.Static);
                if (method != null)
                {
                    return (Command_CastSkill)(method.Invoke(null, null));
                }
            }
            return null;

        }
        //获取某些持续技能结束后的action 反射
        public static void getSkillEndAction(Pawn self, int slot)
        {
            String def = self.kindDef.defName.Replace("Warframe_", "");
            Assembly assembly = Assembly.GetExecutingAssembly();
            if (def == "Ash" || def == "Mesa" || def == "Valkyr")
            {
                assembly = Assembly.Load("WarframeMAV");
            }
            Type wfclass = assembly.GetType("Warframe.Skills." + def);
            if (wfclass != null)
            {
                MethodInfo method = wfclass.GetMethod("EndSkill" + slot, BindingFlags.Public | BindingFlags.Static);
                if (method != null)
                {
                    method.Invoke(null,new object[]{ self});
                }
            }
            /*
            switch (def)
            {
                case "Excalibur":
                    switch (slot)
                    {
                        case 4:
                             Excalibur.EndSkill4(self);break;
                    }
                    break;


            }
            */
           

        }
        //选定建筑和小人的targetingP
        public static TargetingParameters BuildingAndPawn()
        {
            TargetingParameters tp = new TargetingParameters();
            tp.canTargetBuildings = true;
            tp.canTargetFires = false;
            tp.canTargetItems = false;
            tp.canTargetLocations = true;
            tp.canTargetPawns = true;
            tp.canTargetSelf = false;
            return tp;
        }
        //选定小人的targetingP
        public static TargetingParameters onlyPawn()
        {
            TargetingParameters tp = new TargetingParameters();
            tp.canTargetBuildings = false;
            tp.canTargetFires = false;
            tp.canTargetItems = false;
            tp.canTargetLocations = false;
            tp.canTargetPawns = true;
            tp.canTargetSelf = false;
            return tp;
        }
        //跳跃用Tp
        public static TargetingParameters jumpTP()
        {
            TargetingParameters tp = new TargetingParameters();
            tp.canTargetBuildings = false;
            tp.canTargetFires = true;
            tp.canTargetItems = true;
            tp.canTargetLocations = true;
            tp.canTargetPawns = true;
            tp.canTargetSelf = false;
            return tp;
        }
        //获得可见范围坐标列表
        public static List<IntVec3> getCellsAround(IntVec3 pos, Map map,float range)
        {
            List<IntVec3> result = new List<IntVec3>();
            if (!pos.InBounds(map))
            {
                return result;
            }
            Region region = pos.GetRegion(map, RegionType.Set_Passable);
            if (region == null)
            {
                return result;
            }
            RegionTraverser.BreadthFirstTraverse(region, (Region from, Region r) => r.door == null, delegate (Region r)
            {
                foreach (IntVec3 item in r.Cells)
                {
                    if (item.InHorDistOf(pos, range))
                    {
                        result.Add(item);
                    }
                }
                return false;
            }, 99999, RegionType.Set_Passable);
            return result;
        }
        //探测直线攻击
        public static List<Pawn> getLineCell(Pawn wf,Thing target) {
            bool left = wf.Position.x > target.Position.x;
            bool up = wf.Position.z < target.Position.z;
            int xc = Math.Abs(wf.Position.x - target.Position.x);
            int zc = Math.Abs(wf.Position.z - target.Position.z);
            Map map = target.Map;
            List<Pawn> plist = new List<Pawn>();
           
            for(int i = 0; i < xc+1; i++)
            {
                // for(int j = 0; j < zc+1; j++)
                int j = i;
                if (j > zc) j = zc;
                {
                  IEnumerable<Thing>tlist= map.thingGrid.ThingsAt(wf.Position+new IntVec3(left?-i:i,0,up?j:-j));
                    
                    foreach (Thing t in tlist)
                    {
         
                        
                        if (t is Building)
                        {
                            Building b = t as Building;
                            if(b.def.passability == Traversability.Impassable)
                            {
                                return null;
                            }
                        }
                        
                        if(t is Pawn)
                        {
                            if(t!=wf)
                            plist.Add(t as Pawn);
                        }

                    }
                    
                }
            }
            
            return plist;

        }
        //两点距离计算
        public static bool outRange(float maxrange,Thing ps,Vector3 p2)
        {
            float maxRange = maxrange;

            if (!p2.InBounds(ps.Map)) return false;
            Vector3 p = ps.Position.ToVector3();

            float value = (float)Math.Sqrt(Math.Abs(p.x - p2.x) * Math.Abs(p.x - p2.x) + Math.Abs(p.z - p2.z) * Math.Abs(p.z - p2.z));

            return value >= maxRange;
        }
        //持续性消耗SP检测
        public static bool consumeSP(Pawn pawn, float mul, int slot) {
            WarframeBelt wb = getBelt(pawn);
            WarframeArmor wa = getArmor(pawn);
            CompWarframeSkill cp = wa.TryGetComp<CompWarframeSkill>();

            switch (slot) {
                case 1:
                   if (wb.SP > (cp.skill1mana * mul) * (1f / 60f))
                     {
                       wb.SP -= (cp.skill1mana * mul) * (1f / 60f);
                       return true;
                     }   
                   else
                       return false;
                case 4:
                    if (wb.SP > (cp.skill4mana * mul) * (1f / 60f))
                    {
                        wb.SP -= (cp.skill4mana * mul) * (1f / 60f);
                        return true;
                    }
                    else
                        return false;

            }
            return false;
          
        }
        //伤害显示
        public static void showDamageAmount(Thing target,string damage) {
           // MoteMaker.ThrowText(target.Position.ToVector3(),target.Map,"-"+damage,new Color(1,0.2f,0.2f));

            IntVec3 intVec = target.Position;
            try
            {
                if (!intVec.InBounds(target.Map))
                {
                    return;
                }

            MoteText moteText = (MoteText)ThingMaker.MakeThing(ThingDefOf.Mote_Text, null);
            moteText.exactPosition = target.Position.ToVector3();
            moteText.SetVelocity((float)Rand.Range(5, 35), Rand.Range(0.42f, 0.45f));
            moteText.text = "-" + damage;
            moteText.textColor = new Color(1, 0.2f, 0.2f);
            moteText.Scale = 30f;
            GenSpawn.Spawn(moteText, intVec, target.Map, WipeMode.Vanish);
            }
            catch (Exception) { }

        }
        //丢出彩色字
        public static void showColorText(Thing target, string text,Color color,GameFont size)
        {
            // MoteMaker.ThrowText(target.Position.ToVector3(),target.Map,"-"+damage,new Color(1,0.2f,0.2f));

            IntVec3 intVec = target.Position;
            try
            {
                if (!intVec.InBounds(target.Map))
                {
                    return;
                }

            MoteBigText moteText = (MoteBigText)ThingMaker.MakeThing(ThingDef.Named("Mote_Text_Big"), null);
            moteText.exactPosition = (target.Position+new IntVec3(0,0,1)).ToVector3();
            moteText.SetVelocity((float)Rand.Range(5, 35), Rand.Range(0.42f, 0.45f));
            moteText.text = text;
            moteText.textColor = color;
            moteText.size = size;
            GenSpawn.Spawn(moteText, intVec, target.Map, WipeMode.Vanish);
            }
            catch (Exception) { }

        }

        //控制仓里面有无人
        public static bool pawnInControlCell(Pawn wf) {
            if (WFModBase.Instance._WFcontrolstorage.checkBeControlerExist(wf))
            {
                Building_ControlCell bc = WFModBase.Instance._WFcontrolstorage.BeControlerAndControlCell.TryGetValue(wf);
                if(bc !=null & bc.HasAnyContents)
                {
                    return true;
                }

            }
            return false;
        }

        public static WarframeBelt getBelt(Pawn pawn)
        {
            if (pawn.apparel.WornApparelCount < 1) return null;

            foreach (Apparel ap in pawn.apparel.WornApparel)
            {
                if (ap.def.defName.StartsWith("WF_") && ap.def.defName.EndsWith("_Belt"))
                {
                    return (ap as WarframeBelt);
                }
            }
            return null;
        }
        public static WarframeArmor getArmor(Pawn pawn)
        {
            if (pawn.apparel.WornApparelCount < 1) return null;

            foreach (Apparel ap in pawn.apparel.WornApparel)
            {
                if (ap.def.defName.StartsWith("WF_") && ap.def.defName.EndsWith("_Armor"))
                {
                    return (ap as WarframeArmor);
                }
            }
            return null;
        }


        //启动冷却
        public static bool startCooldown(Pawn pawn,float cooldownTime,int slot,float SP) {
            if (pawn.apparel.WornApparelCount > 0)
            {
                
                foreach(Apparel apa in pawn.apparel.WornApparel)
                {
                    if (apa.def.defName.StartsWith("WF_") && apa.def.defName.EndsWith("_Armor"))
                    {
                        
                        if (apa != null)
                        {
                           
                            WarframeArmor ap = apa as WarframeArmor;
                            switch (slot)
                            {
                                case 1:
                                    ap.cooldownTime1 = cooldownTime;
                                    getBelt(pawn).SP -= SP;
                                    break;
                                case 2:
                                    ap.cooldownTime2 = cooldownTime; getBelt(pawn).SP -= SP; break;
                                case 3:
                                    ap.cooldownTime3 = cooldownTime; getBelt(pawn).SP -= SP; break;
                                case 4:
                                    ap.cooldownTime4 = cooldownTime; getBelt(pawn).SP -= SP; break;
                            }
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        //获取跳跃类型
        public static int getJumpType(Pawn wf,IntVec3 target) {
            Room wfr = wf.Position.GetRoom(wf.Map);
            Room tr = target.GetRoom(wf.Map);
          
            if(wfr.ID!=0 &&tr.ID!=0)
             if (wfr == tr) return 1;



            return 0;
        }
        //获取多人起跳
        public static Gizmo getMulJump(Pawn pawn) {
            Command_CastSkillTargetingFloor command_Target = new Command_CastSkillTargetingFloor();
            command_Target.self = pawn;
            command_Target.targetingParams = WarframeStaticMethods.jumpTP();
            command_Target.defaultLabel = "WarframeJumpGizmo.name".Translate();
            command_Target.defaultDesc = "WarframeJumpGizmo.desc".Translate();
            command_Target.range = 14f;
            command_Target.icon = ContentFinder<Texture2D>.Get("Skills/Jump");
            command_Target.cooldownTime = 60;
            command_Target.hotKey = KeyBindingDefOf.Command_ItemForbid;
            command_Target.disabled = !pawn.Drafted ||pawn.stances.stunner.Stunned;

            command_Target.action = delegate (Pawn p,LocalTargetInfo poc)
            {
                IEnumerable<Pawn> enumerable = Find.Selector.SelectedObjects.Where(delegate (object x)
                {
                    Pawn pawn3 = x as Pawn;
                    return pawn3 != null && pawn3.IsColonistPlayerControlled && pawn3.Drafted &&pawn3.isWarframe();
                }).Cast<Pawn>();
                foreach (Pawn pawn2 in enumerable)
                {
                    if (WarframeStaticMethods.outRange(command_Target.range, pawn2, poc.Cell.ToVector3()))
                    {
                        SoundDefOf.ClickReject.PlayOneShotOnCamera();
                        return;
                    }
                    if (!poc.Cell.Walkable(pawn2.Map))
                    {
                        Messages.Message("WFCantJumpToThere".Translate(), MessageTypeDefOf.RejectInput, false);
                        return;
                    }
                    int jtype = getJumpType(pawn2,poc.Cell);


                    if (jtype == 0)
                    {
                        RoofDef wfroof = pawn2.Map.roofGrid.RoofAt(pawn2.Position);
                        if (wfroof != null)
                        {
                            if (wfroof != RoofDefOf.RoofConstructed)
                            {
                                Messages.Message("WFJumpRockRoof".Translate(), MessageTypeDefOf.RejectInput, false);
                                return;
                            }

                            if (!wfroof.soundPunchThrough.NullOrUndefined())
                            {
                                wfroof.soundPunchThrough.PlayOneShot(new TargetInfo(pawn2.Position, pawn2.Map, false));
                                CellRect.CellRectIterator iterator = CellRect.CenteredOn(pawn2.Position, 1).GetIterator();
                                while (!iterator.Done())
                                {
                                    Find.CurrentMap.roofGrid.SetRoof(iterator.Current, null);
                                    iterator.MoveNext();
                                }
                            }

                        }
                        RoofDef locroof = pawn2.Map.roofGrid.RoofAt(poc.Cell);
                        if (locroof != null)
                        {
                            if (locroof != RoofDefOf.RoofConstructed)
                            {
                                Messages.Message("WFJumpRockRoof".Translate(), MessageTypeDefOf.RejectInput, false);
                                return;
                            }
                            if (!locroof.soundPunchThrough.NullOrUndefined())
                            {
                                locroof.soundPunchThrough.PlayOneShot(new TargetInfo(poc.Cell, pawn2.Map, false));
                                CellRect.CellRectIterator iterator = CellRect.CenteredOn(poc.Cell, 1).GetIterator();
                                while (!iterator.Done())
                                {
                                    Find.CurrentMap.roofGrid.SetRoof(iterator.Current, null);
                                    iterator.MoveNext();
                                }
                            }

                        }

                    }



                    pawn2.pather.StartPath(poc, PathEndMode.Touch);
                    pawn2.Position = poc.Cell;

                    pawn2.pather.StopDead();
                    if (pawn2.jobs.curJob != null)
                    {
                        pawn2.jobs.curDriver.Notify_PatherArrived();
                    }

                    SoundDef.Named("Warframe_Jump").PlayOneShot(pawn2);
                    pawn2.stances.stunner.StunFor((int)command_Target.cooldownTime,pawn2);









                }
            };
            command_Target.finishAction = delegate {
                IEnumerable<Pawn> enumerable = Find.Selector.SelectedObjects.Where(delegate (object x)
                {
                    Pawn pawn3 = x as Pawn;
                    return pawn3 != null && pawn3.IsColonistPlayerControlled && pawn3.Drafted && pawn3.isWarframe();
                }).Cast<Pawn>();
                foreach (Pawn pawn2 in enumerable)
                    GenDraw.DrawRadiusRing(pawn2.Position, command_Target.range);//DrawFieldEdges(WarframeStaticMethods.getCellsAround(ck1.self.Position, ck1.self.Map, ck1.range));
            };
            return command_Target;
        }
        
    }
}
