using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Verse;
using Verse.AI;


namespace Warframe
{
    [StaticConstructorOnStartup]
    public static class HarmonyStartUp
    {
        static HarmonyStartUp()
        {

            new Harmony("akreedz.rimworld.warframe").PatchAll(Assembly.GetExecutingAssembly());
            Log.Message("Warframe Harmony Add");
        }
    }


    //添加等级显示
    [HarmonyPatch(typeof(Pawn), "GetInspectString", new Type[]
{
})]
    public static class Harmony_AddLevelShow
    {
        public static void Postfix(Pawn __instance, ref string __result)
        {
   
            bool flag = __instance.IsWarframe() && !__instance.Dead;
            if (flag)
            {
                
                __result +="\n"+"Lv."+__instance.GetLevel().ToString("f0");
            }


        }
    }



    //移除需求
    [HarmonyPatch(typeof(Pawn_NeedsTracker), "AddOrRemoveNeedsAsAppropriate", new Type[]
{

})]
    public static class Harmony_Need_Remove
    {
        public static void Postfix(Pawn_NeedsTracker __instance)
        {
            Traverse traverse = Traverse.Create(__instance);
            Pawn pp = traverse.Field("pawn").GetValue<Pawn>();
            bool flag = pp.IsWarframe();
            if (flag)
            {

                foreach (NeedDef nnd in getUselessNeed()) {
                    if (__instance.TryGetNeed(nnd) != null)
                    {
                        //__instance.RemoveNeed(nnd);
                        Need item = __instance.TryGetNeed(nnd);
                        List<Need> needlist = traverse.Field("needs").GetValue<List<Need>>();
                        needlist.Remove(item);

                        //    __instance.BindDirectNeedFields();
                    }
                }
            }
        }

        public static List<NeedDef> getUselessNeed()
        {
            List<NeedDef> result = new List<NeedDef>
            {
                NeedDefOf.Food,
                NeedDefOf.Rest,
                NeedDefOf.Joy,
                easyGetND("Beauty"),
                easyGetND("Comfort"),
                easyGetND("RoomSize"),
                easyGetND("Outdoors"),
                easyGetND("Mood")
            };
            return result;
        }

        private static NeedDef easyGetND(String def)
        {
            return DefDatabase<NeedDef>.GetNamed(def, true);
        }
    }




    //战甲静止不动
    [HarmonyPatch(typeof(Pawn_HealthTracker), "HealthTick", new Type[]
{

})]
    public static class Harmony_StopMoving
    {
        public static void Postfix(Pawn_HealthTracker __instance)
        {
            Traverse traverse = Traverse.Create(__instance);
            Pawn pp = traverse.Field("pawn").GetValue<Pawn>();
            bool flag = pp.IsWarframe();
            if (flag)//!pp.drafter.Drafted)
            {
                if (!pp.Spawned) return;
                
                if (!WarframeStaticMethods.PawnInControlCell(pp))
                {
                    Job job = new Job(DefDatabase<JobDef>.GetNamed("WFWait", true));
                    pp.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                }
                else
                {
                    if (pp.jobs.curJob.def == DefDatabase<JobDef>.GetNamed("WFWait",true))
                    {
                        pp.jobs.EndCurrentJob(JobCondition.None);
                    }
                }
            }




        


        }
    }


    //移除一个思想节点
    [HarmonyPatch(typeof(ThinkNode_ConditionalNeedPercentageAbove), "Satisfied", new Type[]
{
    typeof(Pawn)
})]
    public static class Harmony_RemoveSatisThink
    {
        public static bool Prefix(Pawn pawn, bool __result)
        {

            bool flag = pawn.IsWarframe();
            if (flag)
            {
                __result = true;
                return false;
            }
            return true;

        }
    }


    //移除疼痛
    [HarmonyPatch(typeof(HediffSet), "CalculatePain", new Type[]
{
})]
    public static class Harmony_RemovePain
    {
        public static void Postfix(HediffSet __instance, ref float __result)
        {
            Traverse tv = Traverse.Create(__instance);
            Pawn pawn = tv.Field("pawn").GetValue<Pawn>();
            bool flag = pawn.IsWarframe();
            if (flag)
            {
                // HealthUtility.AdjustSeverity(pawn, hediff, -0.00033333333f);
                __result = 0f;
            }


        }
    }

    //死亡的条件
    [HarmonyPatch(typeof(Pawn_HealthTracker), "ShouldBeDowned", new Type[]
{

})]
    public static class Harmony_ShouldDowned
    {
        public static void Postfix(Pawn_HealthTracker __instance, ref bool __result)
        {
            Traverse traverse = Traverse.Create(__instance);
            Pawn pawn = traverse.Field("pawn").GetValue<Pawn>();
            bool flag = pawn.IsWarframe();
            if (flag)
            {
                __result = false;
            }

            }
    }



    //死亡的条件
        [HarmonyPatch(typeof(Pawn_HealthTracker), "ShouldBeDead", new Type[]
{

})]
    public static class Harmony_ShouldDie
    {
        public static void Postfix(Pawn_HealthTracker __instance, ref bool __result)
        {
            Traverse traverse = Traverse.Create(__instance);
            Pawn pawn = traverse.Field("pawn").GetValue<Pawn>();
            bool flag = pawn.IsWarframe();
            if (flag)
            {
                float num = WarframeStaticMethods.GetHP(pawn);
                __result = WarframeStaticMethods.ShouldDie(num, pawn);
            }
        }
    }



    //移除掉落血
    [HarmonyPatch(typeof(Pawn_HealthTracker), "DropBloodFilth", new Type[]
{

})]
    public static class Harmony_NoBloodDrop
    {
        public static bool Prefix(Pawn_HealthTracker __instance)
        {
            Traverse traverse = Traverse.Create(__instance);
            Pawn pawn = traverse.Field("pawn").GetValue<Pawn>();
            return !pawn.IsWarframe();
        }
    }

    //无法脱掉装备
    [HarmonyPatch(typeof(ITab_Pawn_Gear), "InterfaceDrop", new Type[]
    {
    typeof(Thing)
    })]
    public static class Harmony_NoGearDrop
    {
        public static bool Prefix(Thing t)
        {
            if (t.def.defName.StartsWith("WF_") || (t.def.weaponTags!=null&&t.def.weaponTags.Contains("WFGun")))
            {
                Messages.Message("CanNotDropGearMsg".Translate(), MessageTypeDefOf.RejectInput, false);
                return false;
            }

            return true;
        }
    }

    //无法救援或成为囚犯
    [HarmonyPatch(typeof(JobDriver_TakeToBed), "TryMakePreToilReservations", new Type[]
    {
    typeof(bool)
    })]
    public static class Harmony_NobePrisoner
    {
        public static bool Prefix(JobDriver_TakeToBed __instance, ref bool __result)
        {
            Traverse tv = Traverse.Create(__instance);
            Job job = tv.Field("job").GetValue<Job>();
            Pawn pawn = (Pawn)job.GetTarget(TargetIndex.A).Thing;
            if (pawn.kindDef.defName.StartsWith("Warframe_"))
            {
                Messages.Message("CanBeCarryMsg".Translate(), MessageTypeDefOf.RejectInput, false);
                __result = false;
                return false;
            }

            return true;
        }
    }


    //无法点击draft
    [HarmonyPatch(typeof(Pawn_DraftController), "GetGizmos")]
    public static class Harmony_RemoveDraftGizmo
    {
        public static void Postfix(Pawn_DraftController __instance, ref IEnumerable<Gizmo> __result)
        {
            Traverse tv = Traverse.Create(__instance);
            Pawn pawn = tv.Field("pawn").GetValue<Pawn>();
            if (pawn.kindDef.defName.StartsWith("Warframe_"))
            {
                bool flag = WarframeStaticMethods.PawnInControlCell(pawn);
                if (!flag)
                {
                    List<Gizmo> list = __result.ToList();
                    list[0].disabled = !flag;
                    list[0].disabledReason = "OnlyControlCell".Translate();
                    __result = list.AsEnumerable<Gizmo>();
                }
            }
        }
    }

    // 时刻检查控制
    [HarmonyPatch(typeof(Pawn_HealthTracker), "HealthTick", new Type[] { })]
    public static class Harmony_WF_Update
    {
        public static void Postfix(Pawn_HealthTracker __instance)
        {
            Traverse traverse = Traverse.Create(__instance);
            
            Pawn pawn = traverse.Field("pawn").GetValue<Pawn>();
            if (pawn.IsWarframe())
            {
                bool flag = WFModBase.Instance._WFcontrolstorage.checkBeControlerExist(pawn);
                if (pawn.Drafted) {
                    if (!flag)
                        pawn.drafter.Drafted = false;
                    else
                    {
                        Building_ControlCell bc = WFModBase.Instance._WFcontrolstorage.BeControlerAndControlCell.TryGetValue(pawn);
                        if (!bc.HasAnyContents)
                        {
                            pawn.drafter.Drafted = false;
                        }

                    }
                }




            }
            //WFModBase.Instance._WFcontrolstorage.checkControlerExist(pawn);


        }

    }

    //绘画选择线条
    [HarmonyPatch(typeof(SelectionDrawer), "DrawSelectionOverlays", new Type[] { })]
    public static class Harmony_WF_Drae_Line
    {
        public static void Postfix()
        {
            foreach (object obj in Find.Selector.SelectedObjects)
            {

                if (obj is Thing thing)
                {
                    if (thing is Pawn)
                    {
                        if (WFModBase.Instance._WFcontrolstorage.BeControlerAndControlCell.ContainsKey(thing as Pawn))
                        {
                            Pawn becontroler = thing as Pawn;
                            Building_ControlCell controler = WFModBase.Instance._WFcontrolstorage.BeControlerAndControlCell.TryGetValue(becontroler);
                            if (controler.Map == becontroler.Map)
                                HighDrawLineBetween(thing.TrueCenter(), controler.TrueCenter(), MaterialPool.MatFrom(GenDraw.LineTexPath, ShaderDatabase.Transparent, Color.magenta));
                        }
                    }
                    else if (thing is Building_ControlCell)
                    {
                        if (WFModBase.Instance._WFcontrolstorage.ControlCellAndBeControler.ContainsKey(thing as Building_ControlCell))
                        {
                            Building_ControlCell controler = thing as Building_ControlCell;
                            Pawn becontroler = WFModBase.Instance._WFcontrolstorage.ControlCellAndBeControler.TryGetValue(controler);
                            if (controler.Map == becontroler.Map)
                                HighDrawLineBetween(becontroler.TrueCenter(), controler.TrueCenter(), MaterialPool.MatFrom(GenDraw.LineTexPath, ShaderDatabase.Transparent, Color.magenta));
                        }
                    }
                }
            }
        }

        public static void HighDrawLineBetween(Vector3 A, Vector3 B, Material mat)
        {
            if (Mathf.Abs(A.x - B.x) < 0.01f && Mathf.Abs(A.z - B.z) < 0.01f)
            {
                return;
            }
            Vector3 pos = (A + B) / 2f;
            if (A == B)
            {
                return;
            }
            A.y = B.y;
            float z = (A - B).MagnitudeHorizontal();
            Quaternion q = Quaternion.LookRotation(A - B);
            Vector3 s = new Vector3(0.2f, 1f, z);
            Matrix4x4 matrix = default(Matrix4x4);
            matrix.SetTRS(pos + new Vector3(0, 3, 0), q, s);
            Graphics.DrawMesh(MeshPool.plane10, matrix, mat, 0);
        }

    }


    //移除武器使用间隔
    [HarmonyPatch(typeof(VerbProperties), "AdjustedCooldown", new Type[] {
        typeof(Tool),
        typeof(Pawn),
        typeof(Thing)
    })]
    public static class Harmony_RemoveVerbCD
    {
        public static void Postfix(VerbProperties __instance, Tool tool,Pawn attacker, Thing equipment, ref float __result)
        {
            Traverse tv = Traverse.Create(__instance);
           // float num = tv.Field("defaultCooldownTime").GetValue<float>();
            if (attacker!=null&& attacker.IsWarframe())
            {
                if (equipment != null && !__instance.IsMeleeAttack)
                __result = 0f;
                else if (__instance.IsMeleeAttack && tool!=null)
                {
                    
                    __result = tool.cooldownTime / 3f;
                }
            }
        }
    }

    //移除减速1
    [HarmonyPatch(typeof(Pawn_PathFollower), "CostToPayThisTick")]
    public static class Harmony_RemoveStagger
    {
        public static void Postfix(Pawn_PathFollower __instance, ref float __result)
        {
            Traverse traverse = Traverse.Create(__instance);

            Pawn pawn = traverse.Field("pawn").GetValue<Pawn>();
            if (pawn.IsWarframe())
            {
                if(__result<1f)
                  __result = 1f;
            }
        }
    }
    //移除减速2
    [HarmonyPatch(typeof(Pawn_PathFollower), "CostToMoveIntoCell",new Type[] {
        typeof(Pawn),
        typeof(IntVec3)
    })]
    public static class Harmony_RemovePathCost
    {
        public static bool Prefix(Pawn_PathFollower __instance, Pawn pawn, IntVec3 c,ref int __result)
        {
           // Traverse traverse = Traverse.Create(__instance);

           // Pawn pawn = traverse.Field("pawn").GetValue<Pawn>();
            if (pawn.IsWarframe())
            {
                int num;
                if (c.x == pawn.Position.x || c.z == pawn.Position.z)
                {
                    num = pawn.TicksPerMoveCardinal;
                }
                else
                {
                    num = pawn.TicksPerMoveDiagonal;
                }
                __result = num;
                return false;
            }

            return true;
        }
    }
    //添加圈圈
    [HarmonyPatch(typeof(Targeter), "TargeterUpdate")]
    public static class Harmony_AddRadius
    {
        public static void Postfix(Targeter __instance)
        {
            Traverse traverse = Traverse.Create(__instance);

            Pawn pawn = traverse.Field("caster").GetValue<Pawn>();
            Action fiaction = traverse.Field("actionWhenFinished").GetValue<Action>();
            if (fiaction == null) return;
            if (__instance.IsTargeting && pawn!=null &&pawn.IsWarframe())
            {
                fiaction();
            }
            
        }
    }

    //防止被击倒
    [HarmonyPatch(typeof(HediffSet), "AddDirect", new Type[] {
       typeof(Hediff),
       typeof(DamageInfo?),
        typeof(DamageWorker.DamageResult)
    })]
    public static class Harmony_RemoveMissingPart
    {
        public static bool Prefix(HediffSet __instance, Hediff hediff, DamageInfo? dinfo = null, DamageWorker.DamageResult damageResult = null)
        {
            Traverse traverse = Traverse.Create(__instance);
            Pawn pawn = traverse.Field("pawn").GetValue<Pawn>();
            if (pawn != null && pawn.IsWarframe())
            {
                if (hediff.def.isBad)
                    if (hediff.Part.def.defName != "wf_armor")
                    {
                        if (dinfo != null) { 
                         DamageInfo ddinfo = (DamageInfo)dinfo;
                            ddinfo.SetHitPart(pawn.health.hediffSet.GetRandomNotMissingPart(ddinfo.Def,BodyPartHeight.Undefined,BodyPartDepth.Outside));
                            pawn.TakeDamage(ddinfo);
                         }
                    return false;
                }   
            }
            return true;

        }
    }


    //增加团体框架飞跃
    [HarmonyPatch(typeof(Pawn), "GetGizmos", new Type[]
    {
    })]
    public static class Harmony_JumpTogether
    {
        public static void Postfix(Pawn __instance, ref IEnumerable<Gizmo> __result)
        {

            if (__instance.IsColonistPlayerControlled)
            {
                List<Gizmo> nes = __result.ToList<Gizmo>();


                foreach (Gizmo attack in GetTeleGizmos(__instance))
                {
                    //yield return attack;

                    nes.Add(attack);
                    //  Log.Warning(nes.Count+"/");
                }
                __result = nes.AsEnumerable<Gizmo>();
            }

        }

        private static IEnumerable<Gizmo> GetTeleGizmos(Pawn pawn)
        {
            if (ShouldUseSquadTeleGizmo())
            {
                yield return WarframeStaticMethods.GetMulJump(pawn);
            }
            yield break;
        }


        private static bool ShouldUseSquadTeleGizmo()
        {
            List<object> selectedObjectsListForReading = Find.Selector.SelectedObjectsListForReading;
            if (selectedObjectsListForReading.Count < 2) return false;

            foreach (object pawn in selectedObjectsListForReading)
            {
                if (pawn is Pawn)
                {
                    if (!(pawn as Pawn).IsWarframe())
                    {
                        return false;
                    }
                }
            }

            return true;
        }

    }


    //移除框架死亡负面思想

    //[HarmonyPatch(typeof(PawnDiedOrDownedThoughtsUtility), "AppendThoughts_ForHumanlike", new Type[]{ typeof(Pawn),typeof(DamageInfo?),typeof(PawnDiedOrDownedThoughtsKind),typeof(List<IndividualThoughtToAdd>),typeof(List<ThoughtDef>)})]
    //public static class Harmony_WarframeNoThought
    //{
    //    public static bool Prefix(Pawn victim)
    //    {
    //        if (victim.isWarframe())
    //            return false;

    //        return true;
    //    }

    //}

    //禁止交流1
    [HarmonyPatch(typeof(Pawn_InteractionsTracker), "TryInteractWith")]
    public static class Harmony_WarframeNoTalk
    {
        public static bool Prefix(Pawn_InteractionsTracker __instance,Pawn recipient,ref bool __result)
        {
            Traverse tv = Traverse.Create(__instance);
            Pawn pawn = tv.Field("pawn").GetValue<Pawn>();
            if (pawn.IsWarframe() || recipient.IsWarframe())
            {

                __result = true;
                return false;
            }
            return true;
        }

    }

    //禁止关系1
    [HarmonyPatch(typeof(Pawn_RelationsTracker), "RelationsTrackerTick", new Type[] {
    })]
    public static class Harmony_WarframeNoRelation
    {
        public static bool Prefix(Pawn_RelationsTracker __instance)
        {
            Traverse tv = Traverse.Create(__instance);
            Pawn pawn = tv.Field("pawn").GetValue<Pawn>();
            List<DirectPawnRelation> directRelations = tv.Field("directRelations").GetValue<List<DirectPawnRelation>>();
            if (pawn.IsWarframe())
            {
                directRelations = new List<DirectPawnRelation>();
                return false;
            }
            return true;
        }

    }

    //技能不掉
    [HarmonyPatch(typeof(SkillRecord), "Interval")]
    public static class Harmony_WarframeNoSkillDown
    {
        public static bool Prefix(SkillRecord __instance)
        {
            Traverse tv = Traverse.Create(__instance);
            Pawn pawn = tv.Field("pawn").GetValue<Pawn>();
            if (pawn.IsWarframe())
            {
                
                return false;
            }
            return true;
        }

    }

    //不准穿衣服
    [HarmonyPatch(typeof(ReachabilityUtility), "CanReach")]
    public static class Harmony_WarframeNoWearMenu
    {
        public static bool Prefix(Pawn pawn, LocalTargetInfo dest,ref bool __result)
        {
            if (pawn.IsWarframe()&&!pawn.Downed)
            {
                if (dest.Thing is Apparel)
                {
                    __result = false;
                    return false;
                }
            }
            return true;
        }

    }



    [HarmonyPatch(typeof(DebugWindowsOpener))]
    [HarmonyPatch("ToggleDebugActionsMenu")]
    public static class DebugWindowsOpener_ToggleDebugActionsMenu_Patch
    {
        // Token: 0x060002AF RID: 687 RVA: 0x00010FF8 File Offset: 0x0000F1F8
        [HarmonyPriority(800)]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            ConstructorInfo from = AccessTools.Constructor(typeof(Dialog_DebugActionsMenu), null);
            ConstructorInfo to = AccessTools.Constructor(typeof(Dialog_WarframeDebugActionMenu), null);
            return instructions.MethodReplacer(from, to);
        }
    }



}






