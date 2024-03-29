﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace Warframe
{
    public static class WarframeStaticMethods
    {
        public static bool IsWarframe(this Pawn pawn)
        {
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
        public static Gender SetGender(string kdef)
        {
            var def = kdef.Replace("Warframe_", "");
            switch (def)
            {
                case "Excalibur":
                    return Gender.Male;
                case "Volt":
                    return Gender.Male;
                case "Ash":
                    return Gender.Male;
            }

            return Gender.Female;
        }

        //获取目前受的伤害数值
        public static float GetHP(Pawn pawn)
        {
            var num = 0f;
            var __instance = pawn.health;
            foreach (var hediff in __instance.hediffSet.hediffs)
            {
                if (hediff is Hediff_Injury)
                {
                    num += hediff.Severity;
                }
                else if (hediff is Hediff_MissingPart)
                {
                    num += hediff.Part.def.hitPoints;
                }
            }
            //foreach (__instance.health.hediffSet.GetMissingPartsCommonAncestors()) { }

            return num;
        }

        //死亡条件
        public static bool ShouldDie(float num, Pawn pawn)
        {
            if (pawn.apparel.WornApparel == null || pawn.apparel.WornApparelCount <= 0)
            {
                return false;
            }

            foreach (var ap in pawn.apparel.WornApparel)
            {
                if (!ap.def.defName.StartsWith("WF_") || !ap.def.defName.EndsWith("_Belt"))
                {
                    continue;
                }

                if (ap is WarframeBelt wb && num > wb.MHP)
                {
                    return true;
                }

                break;
            }

            return false;
        }

        //获取所有战甲kind
        public static IEnumerable<PawnKindDef> GetAllWarframeKind()
        {
            foreach (var pk in DefDatabase<PawnKindDef>.AllDefs)
            {
                if (pk.defName.StartsWith("Warframe_"))
                {
                    yield return pk;
                }
            }
        }

        //获取战甲
        public static Pawn GetWarframePawn(PawnKindDef pk)
        {
            var request = new PawnGenerationRequest(pk, Faction.OfPlayer, PawnGenerationContext.NonPlayer, -1, false,
                false, false, false, true, true, 0, false, true, true, false, false, false, false, false, 0, 0, null, 1,
                null, null, null, null, 12, null, null, SetGender(pk.defName));
            var item = PawnGenerator.GeneratePawn(request);
            item.story.adulthood = null;
            item.story.bodyType = SetGender(pk.defName) == Gender.Male ? BodyTypeDefOf.Male : BodyTypeDefOf.Female;


            item.inventory.DestroyAll();
            item.apparel.WornApparel.Clear();

            item.workSettings.DisableAll();
            var pname = pk.defName.Replace("Warframe_", "");
            var nowgear = "Head";
            try
            {
                for (var i = 0; i < 3; i++)
                {
                    if (i == 1)
                    {
                        nowgear = "Armor";
                    }
                    else if (i == 2)
                    {
                        nowgear = "Belt";
                    }


                    var ap = (Apparel) ThingMaker.MakeThing(ThingDef.Named("WF_" + pname + "_" + nowgear));
                    item.apparel.Wear(ap);
                }
            }
            catch (Exception e)
            {
                Log.Error(pk.defName + "'s apparel is not exist!");
                Log.Error("Details:" + e);
            }

            item.story.traits.allTraits.Clear();

            var triple = NameTriple.FromString(pk.label.Replace(" ", ""));
            triple.ResolveMissingPieces(pk.label[0] + (Find.TickManager.TicksGame % 100).ToString());
            item.Name = triple;

            foreach (var sr in item.skills.skills)
            {
                if (sr.def == SkillDefOf.Shooting || sr.def == SkillDefOf.Melee)
                {
                    sr.Level = 20;
                }
            }

            item.playerSettings.hostilityResponse = HostilityResponseMode.Attack;
            item.story.traits.GainTrait(new Trait(TraitDefOf.NaturalMood, 1));

            return item;
        }

        //获取制作材料
        public static int GetCraftCost(PawnKindDef pk)
        {
            var kdef = pk.defName.Replace("Warframe_", "");
            switch (kdef)
            {
                case "Excalibur":
                    return 1;
            }

            return 1;
        }

        //获取战甲等级
        public static int GetWFLevel(Pawn pawn)
        {
            var level = 1;
            if (pawn?.records == null)
            {
                return level;
            }

            var killh = pawn.records.GetValue(RecordDefOf.KillsHumanlikes);
            var killm = pawn.records.GetValue(RecordDefOf.KillsMechanoids);
            level = (int) (1f + (((killh * 2f) + (killm * 5f)) / 10f));

            return level > 30 ? 30 : level;
        }

        //获取战甲等级 plus
        public static float GetLevel(this Pawn pawn)
        {
            float level = 1;
            var pr = pawn.records;
            if (pr == null)
            {
                return level > 30 ? 30f : level;
            }

            var killh = pr.GetValue(RecordDefOf.KillsHumanlikes);
            var killm = pr.GetValue(RecordDefOf.KillsMechanoids);
            level = 1f + (((killh * 2f) + (killm * 5f)) / 10f);

            return level > 30 ? 30f : level;
        }

        //获取某战甲某技能 反射
        public static Command_CastSkill GetSkillCommand(string def, int slot)
        {
            var assembly = Assembly.GetExecutingAssembly();
            if (def == "Ash" || def == "Mesa" || def == "Valkyr")
            {
                assembly = Assembly.Load("WarframeMAV");
            }

            var wfclass = assembly.GetType("Warframe.Skills." + def);
            if (wfclass == null)
            {
                return null;
            }

            var method = wfclass.GetMethod("Skill" + slot, BindingFlags.Public | BindingFlags.Static);
            if (method != null)
            {
                return (Command_CastSkill) method.Invoke(null, null);
            }

            return null;
        }

        //获取某些持续技能结束后的action 反射
        public static void GetSkillEndAction(Pawn self, int slot)
        {
            var def = self.kindDef.defName.Replace("Warframe_", "");
            var assembly = Assembly.GetExecutingAssembly();
            if (def == "Ash" || def == "Mesa" || def == "Valkyr")
            {
                assembly = Assembly.Load("WarframeMAV");
            }

            var wfclass = assembly.GetType("Warframe.Skills." + def);
            if (wfclass == null)
            {
                return;
            }

            var method = wfclass.GetMethod("EndSkill" + slot, BindingFlags.Public | BindingFlags.Static);
            if (method != null)
            {
                method.Invoke(null, new object[] {self});
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
            var tp = new TargetingParameters
            {
                canTargetBuildings = true,
                canTargetFires = false,
                canTargetItems = false,
                canTargetLocations = true,
                canTargetPawns = true,
                canTargetSelf = false
            };
            return tp;
        }

        //选定小人的targetingP
        public static TargetingParameters OnlyPawn()
        {
            var tp = new TargetingParameters
            {
                canTargetBuildings = false,
                canTargetFires = false,
                canTargetItems = false,
                canTargetLocations = false,
                canTargetPawns = true,
                canTargetSelf = false
            };
            return tp;
        }

        //跳跃用Tp
        public static TargetingParameters JumpTP()
        {
            var tp = new TargetingParameters
            {
                canTargetBuildings = false,
                canTargetFires = true,
                canTargetItems = true,
                canTargetLocations = true,
                canTargetPawns = true,
                canTargetSelf = false
            };
            return tp;
        }

        //获得可见范围坐标列表
        public static List<IntVec3> GetCellsAround(IntVec3 pos, Map map, float range)
        {
            var result = new List<IntVec3>();
            if (!pos.InBounds(map))
            {
                return result;
            }

            var region = pos.GetRegion(map);
            if (region == null)
            {
                return result;
            }

            RegionTraverser.BreadthFirstTraverse(region, (_, r) => r.door == null, delegate(Region r)
            {
                foreach (var item in r.Cells)
                {
                    if (item.InHorDistOf(pos, range))
                    {
                        result.Add(item);
                    }
                }

                return false;
            }, 99999);
            return result;
        }

        //探测直线攻击
        public static List<Pawn> GetLineCell(Pawn wf, Thing target)
        {
            var left = wf.Position.x > target.Position.x;
            var up = wf.Position.z < target.Position.z;
            var xc = Math.Abs(wf.Position.x - target.Position.x);
            var zc = Math.Abs(wf.Position.z - target.Position.z);
            var map = target.Map;
            var plist = new List<Pawn>();

            for (var i = 0; i < xc + 1; i++)
            {
                // for(int j = 0; j < zc+1; j++)
                var j = i;
                if (j > zc)
                {
                    j = zc;
                }

                {
                    var tlist = map.thingGrid.ThingsAt(wf.Position + new IntVec3(left ? -i : i, 0, up ? j : -j));

                    foreach (var t in tlist)
                    {
                        if (t is Building building)
                        {
                            if (building.def.passability == Traversability.Impassable)
                            {
                                return null;
                            }
                        }

                        if (t is not Pawn pawn)
                        {
                            continue;
                        }

                        if (t != wf)
                        {
                            plist.Add(pawn);
                        }
                    }
                }
            }

            return plist;
        }

        //两点距离计算
        public static bool OutRange(float maxrange, Thing ps, Vector3 p2)
        {
            if (!p2.InBounds(ps.Map))
            {
                return false;
            }

            var p = ps.Position.ToVector3();

            var value = (float) Math.Sqrt((Math.Abs(p.x - p2.x) * Math.Abs(p.x - p2.x)) +
                                          (Math.Abs(p.z - p2.z) * Math.Abs(p.z - p2.z)));

            return value >= maxrange;
        }

        //持续性消耗SP检测
        public static bool ConsumeSP(Pawn pawn, float mul, int slot)
        {
            var wb = GetBelt(pawn);
            var wa = GetArmor(pawn);
            var cp = wa.TryGetComp<CompWarframeSkill>();

            switch (slot)
            {
                case 1:
                    if (wb.SP > cp.skill1mana * mul * (1f / 60f))
                    {
                        wb.SP -= cp.skill1mana * mul * (1f / 60f);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case 4:
                    if (wb.SP > cp.skill4mana * mul * (1f / 60f))
                    {
                        wb.SP -= cp.skill4mana * mul * (1f / 60f);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
            }

            return false;
        }

        //伤害显示
        public static void ShowDamageAmount(Thing target, string damage)
        {
            // MoteMaker.ThrowText(target.Position.ToVector3(),target.Map,"-"+damage,new Color(1,0.2f,0.2f));

            var intVec = target.Position;
            try
            {
                if (!intVec.InBounds(target.Map))
                {
                    return;
                }

                var moteText = (MoteText) ThingMaker.MakeThing(ThingDefOf.Mote_Text);
                moteText.exactPosition = target.Position.ToVector3();
                moteText.SetVelocity(Rand.Range(5, 35), Rand.Range(0.42f, 0.45f));
                moteText.text = "-" + damage;
                moteText.textColor = new Color(1, 0.2f, 0.2f);
                moteText.Scale = 30f;
                GenSpawn.Spawn(moteText, intVec, target.Map);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        //丢出彩色字
        public static void ShowColorText(Thing target, string text, Color color, GameFont size)
        {
            // MoteMaker.ThrowText(target.Position.ToVector3(),target.Map,"-"+damage,new Color(1,0.2f,0.2f));

            var intVec = target.Position;
            try
            {
                if (!intVec.InBounds(target.Map))
                {
                    return;
                }

                var moteText = (MoteBigText) ThingMaker.MakeThing(ThingDef.Named("Mote_Text_Big"));
                moteText.exactPosition = (target.Position + new IntVec3(0, 0, 1)).ToVector3();
                moteText.SetVelocity(Rand.Range(5, 35), Rand.Range(0.42f, 0.45f));
                moteText.text = text;
                moteText.textColor = color;
                moteText.size = size;
                GenSpawn.Spawn(moteText, intVec, target.Map);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        //控制仓里面有无人
        public static bool PawnInControlCell(Pawn wf)
        {
            if (!WFModBase.Instance._WFcontrolstorage.checkBeControlerExist(wf))
            {
                return false;
            }

            var bc = WFModBase.Instance._WFcontrolstorage.BeControlerAndControlCell.TryGetValue(wf);
            if (bc != null && true & bc.HasAnyContents)
            {
                return true;
            }

            return false;
        }

        public static WarframeBelt GetBelt(Pawn pawn)
        {
            if (pawn.apparel.WornApparelCount < 1)
            {
                return null;
            }

            foreach (var ap in pawn.apparel.WornApparel)
            {
                if (ap.def.defName.StartsWith("WF_") && ap.def.defName.EndsWith("_Belt"))
                {
                    return ap as WarframeBelt;
                }
            }

            return null;
        }

        public static WarframeArmor GetArmor(Pawn pawn)
        {
            if (pawn.apparel.WornApparelCount < 1)
            {
                return null;
            }

            foreach (var ap in pawn.apparel.WornApparel)
            {
                if (ap.def.defName.StartsWith("WF_") && ap.def.defName.EndsWith("_Armor"))
                {
                    return ap as WarframeArmor;
                }
            }

            return null;
        }


        //启动冷却
        public static bool StartCooldown(Pawn pawn, float cooldownTime, int slot, float SP)
        {
            if (pawn.apparel.WornApparelCount <= 0)
            {
                return false;
            }

            foreach (var apa in pawn.apparel.WornApparel)
            {
                if (!apa.def.defName.StartsWith("WF_") || !apa.def.defName.EndsWith("_Armor"))
                {
                    continue;
                }

                var ap = apa as WarframeArmor;
                switch (slot)
                {
                    case 1:
                        if (ap != null)
                        {
                            ap.cooldownTime1 = cooldownTime;
                        }

                        GetBelt(pawn).SP -= SP;
                        break;
                    case 2:
                        if (ap != null)
                        {
                            ap.cooldownTime2 = cooldownTime;
                        }

                        GetBelt(pawn).SP -= SP;
                        break;
                    case 3:
                        if (ap != null)
                        {
                            ap.cooldownTime3 = cooldownTime;
                        }

                        GetBelt(pawn).SP -= SP;
                        break;
                    case 4:
                        if (ap != null)
                        {
                            ap.cooldownTime4 = cooldownTime;
                        }

                        GetBelt(pawn).SP -= SP;
                        break;
                }

                return true;
            }

            return false;
        }

        //获取跳跃类型
        public static int GetJumpType(Pawn wf, IntVec3 target)
        {
            var wfr = wf.Position.GetRoom(wf.Map);
            var tr = target.GetRoom(wf.Map);

            if (wfr.ID == 0 || tr.ID == 0)
            {
                return 0;
            }

            if (wfr == tr)
            {
                return 1;
            }


            return 0;
        }

        //获取多人起跳
        public static Gizmo GetMulJump(Pawn pawn)
        {
            var command_Target = new Command_CastSkillTargetingFloor
            {
                self = pawn,
                targetingParams = JumpTP(),
                defaultLabel = "WarframeJumpGizmo.name".Translate(),
                defaultDesc = "WarframeJumpGizmo.desc".Translate(),
                range = 14f,
                icon = ContentFinder<Texture2D>.Get("Skills/Jump"),
                cooldownTime = 60,
                hotKey = KeyBindingDefOf.Command_ItemForbid,
                disabled = !pawn.Drafted || pawn.stances.stunner.Stunned
            };

            command_Target.action = delegate(Pawn _, LocalTargetInfo poc)
            {
                var enumerable = Find.Selector.SelectedObjects.Where(x =>
                        x is Pawn {IsColonistPlayerControlled: true, Drafted: true} pawn3 && pawn3.IsWarframe())
                    .Cast<Pawn>();
                foreach (var pawn2 in enumerable)
                {
                    if (OutRange(command_Target.range, pawn2, poc.Cell.ToVector3()))
                    {
                        SoundDefOf.ClickReject.PlayOneShotOnCamera();
                        return;
                    }

                    if (!poc.Cell.Walkable(pawn2.Map))
                    {
                        Messages.Message("WFCantJumpToThere".Translate(), MessageTypeDefOf.RejectInput, false);
                        return;
                    }

                    var jtype = GetJumpType(pawn2, poc.Cell);


                    if (jtype == 0)
                    {
                        var wfroof = pawn2.Map.roofGrid.RoofAt(pawn2.Position);
                        if (wfroof != null)
                        {
                            if (wfroof != RoofDefOf.RoofConstructed)
                            {
                                Messages.Message("WFJumpRockRoof".Translate(), MessageTypeDefOf.RejectInput, false);
                                return;
                            }

                            if (!wfroof.soundPunchThrough.NullOrUndefined())
                            {
                                wfroof.soundPunchThrough.PlayOneShot(new TargetInfo(pawn2.Position, pawn2.Map));
                                var iterator = CellRect.CenteredOn(pawn2.Position, 1).GetIterator();
                                while (!iterator.Done())
                                {
                                    Find.CurrentMap.roofGrid.SetRoof(iterator.Current, null);
                                    iterator.MoveNext();
                                }
                            }
                        }

                        var locroof = pawn2.Map.roofGrid.RoofAt(poc.Cell);
                        if (locroof != null)
                        {
                            if (locroof != RoofDefOf.RoofConstructed)
                            {
                                Messages.Message("WFJumpRockRoof".Translate(), MessageTypeDefOf.RejectInput, false);
                                return;
                            }

                            if (!locroof.soundPunchThrough.NullOrUndefined())
                            {
                                locroof.soundPunchThrough.PlayOneShot(new TargetInfo(poc.Cell, pawn2.Map));
                                var iterator = CellRect.CenteredOn(poc.Cell, 1).GetIterator();
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
                    pawn2.stances.stunner.StunFor((int) command_Target.cooldownTime, pawn2);
                }
            };
            command_Target.finishAction = delegate
            {
                var enumerable = Find.Selector.SelectedObjects.Where(x =>
                        x is Pawn {IsColonistPlayerControlled: true, Drafted: true} pawn3 && pawn3.IsWarframe())
                    .Cast<Pawn>();
                foreach (var pawn2 in enumerable)
                {
                    GenDraw.DrawRadiusRing(pawn2.Position,
                        command_Target
                            .range); //DrawFieldEdges(WarframeStaticMethods.getCellsAround(ck1.self.Position, ck1.self.Map, ck1.range));
                }
            };
            return command_Target;
        }
    }
}