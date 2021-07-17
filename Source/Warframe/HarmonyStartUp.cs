using System.Reflection;
using HarmonyLib;
using Verse;

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


    //移除需求


    //战甲静止不动


    //移除一个思想节点


    //移除疼痛

    //死亡的条件


    //死亡的条件


    //移除掉落血

    //无法脱掉装备

    //无法救援或成为囚犯


    //无法点击draft

    // 时刻检查控制

    //绘画选择线条


    //移除武器使用间隔

    //移除减速1
    //移除减速2
    //添加圈圈

    //防止被击倒


    //增加团体框架飞跃


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

    //禁止关系1

    //技能不掉

    //不准穿衣服
}