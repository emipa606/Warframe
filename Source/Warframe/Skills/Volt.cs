using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;
using Warframe.Skills.Volts;

namespace Warframe.Skills
{
    public static class Volt
    {
        //咖喱技能1
        public static Command_CastSkill Skill1()
        {
            Command_CastSkillTargetingFloor ck = new Command_CastSkillTargetingFloor
            {
                defaultLabel = "VoltSkill1.name".Translate(),
                icon = ContentFinder<Texture2D>.Get("Skills/VoltSkill1"),
                targetingParams = WarframeStaticMethods.JumpTP(),
                cooldownTime = 0.2f,
                range = 11f
            };
            ck.finishAction = delegate {
                GenDraw.DrawRadiusRing(UI.MouseCell(),5f * (1+ (ck.self.GetLevel()*1f/30f)));
                GenDraw.DrawFieldEdges(WarframeStaticMethods.GetCellsAround(ck.self.Position, ck.self.Map, ck.range));
            };
            ck.hotKey = KeyBindingDefOf.Misc5;
            ck.action = delegate (Pawn self, LocalTargetInfo target)
            {
              
                // GenExplosion.DoExplosion(self.Position, self.Map, 3.5f, DaVolteDefOf.Bomb, self, -1, -1, null, null, null, null, null, 0, 1, false, null, 0, 1, 0, false);
                if (!WarframeStaticMethods.GetCellsAround(self.Position, self.Map, ck.range).Contains(target.Cell))
                {
                    SoundDefOf.ClickReject.PlayOneShotOnCamera();
                    return;
                }
                List<IntVec3> cells = new List<IntVec3>();
                int num = GenRadial.NumCellsInRadius(5f * (1 + (ck.self.GetLevel() * 1f / 30f)));
                for (int i = 0; i < num; i++)
                {
                    cells.Add(target.Cell + GenRadial.RadialPattern[i]);
                }
                int ps = 0;
                float damage = 20 + (2 * WarframeStaticMethods.GetWFLevel(self) / 5);
                DamageInfo dinfo = new DamageInfo(DefDatabase<DamageDef>.GetNamed("Mag", true),damage,0,-1,self,null,null,DamageInfo.SourceCategory.ThingOrUnknown,null);
                foreach (IntVec3 ic in cells)
                {
                    foreach(Thing th in self.Map.thingGrid.ThingsAt(ic))
                    {
                        if(th is Pawn && (th as Pawn).Faction != self.Faction)
                        {
                            ps++;
                            (th as Pawn).stances.stunner.StunFor(180,self);
                            (th as Pawn).TakeDamage(dinfo);
                            WarframeStaticMethods.ShowDamageAmount(th, damage.ToString("f0"));
                            if (ps >= 2 * (1 + (self.GetLevel() * 1f / 10f))) break;
                        }
                       
                    }
                    if (ps >= 2 * (1 + (self.GetLevel() * 1f / 10f))) break;
                }
                

                SoundDef.Named("Volt_1Skill").PlayOneShot(self);
               
                WarframeStaticMethods.StartCooldown(self, ck.cooldownTime, 1, WarframeStaticMethods.GetArmor(self).TryGetComp<CompWarframeSkill>().Props.mana1);


            };


            return ck;

        }

        //咖喱技能2
        public static Command_CastSkill Skill2()
        {
            Command_CastSkill ck = new Command_CastSkill
            {
                defaultLabel = "VoltSkill2.name".Translate(),
                icon = ContentFinder<Texture2D>.Get("Skills/VoltSkill2"),
                targetingParams = WarframeStaticMethods.OnlyPawn(),
                cooldownTime = 0.2f,
                range = 2f,
                hotKey = KeyBindingDefOf.Misc8
            };
            ck.action = delegate (Pawn self)
            {
                SoundDef.Named("Volt_2Skill").PlayOneShot(self);
                foreach (IntVec3 ic in WarframeStaticMethods.GetCellsAround(self.Position, self.Map, ck.range))
                {
                    foreach (Thing th in self.Map.thingGrid.ThingsAt(ic))
                    {
                        if (!(th is Pawn) || th.Faction == null || th.Faction != self.Faction || !(th as Pawn).IsWarframe()) continue;

                        Hediff_VoltSpeedUp hediff_Magnetize = (Hediff_VoltSpeedUp)HediffMaker.MakeHediff(HediffDef.Named("VoltSpeedUp"), self, null);
                        hediff_Magnetize.level = (int)self.GetLevel();
                        (th as Pawn).health.AddHediff(hediff_Magnetize, null, null, null);
                        WarframeStaticMethods.ShowColorText(th,"Speed Up!",new Color(0.2f,0.4f,0.8f),GameFont.Small);


                    }
                }
                WarframeStaticMethods.StartCooldown(self, ck.cooldownTime, 2, WarframeStaticMethods.GetArmor(self).TryGetComp<CompWarframeSkill>().skill2mana);

            };


            return ck;

        }


        //咖喱技能3
        public static Command_CastSkill Skill3()
        {
            Command_CastSkillTargetingFloor ck = new Command_CastSkillTargetingFloor
            {
                defaultLabel = "VoltSkill3.name".Translate(),
                icon = ContentFinder<Texture2D>.Get("Skills/VoltSkill3"),
                targetingParams = WarframeStaticMethods.JumpTP(),
                cooldownTime = 0.2f,
                range = 2f,
                hotKey = KeyBindingDefOf.Misc4
            };
            ck.finishAction = delegate
            {
                GenDraw.DrawFieldEdges(ck.self.CellsAdjacent8WayAndInside().ToList());
            };
            ck.action = delegate (Pawn self,LocalTargetInfo target)
            {
                if (!self.CellsAdjacent8WayAndInside().Contains(target.Cell) || target.Cell==self.Position)
                {
                    SoundDefOf.ClickReject.PlayOneShotOnCamera();
                    return;
                }

                List<IntVec3> dire = new List<IntVec3>();
                dire.Add(target.Cell);
                float rotat = 0f;
                if (target.Cell.x == self.Position.x)
                {
                        dire.Add(target.Cell + new IntVec3(-1, 0, 0));
                        dire.Add(target.Cell + new IntVec3(1, 0, 0));
                }
                else
                if (target.Cell.z == self.Position.z)
                {
                    dire.Add(target.Cell + new IntVec3(0, 0, -1));
                    dire.Add(target.Cell + new IntVec3(0, 0, 1));
                    rotat = 90f;
                }
                else
                if (target.Cell.x > self.Position.x)
                {
                    if (target.Cell.z > self.Position.z)
                    {
                        dire.Add(target.Cell + new IntVec3(-1, 0, 1));
                        dire.Add(target.Cell + new IntVec3(1, 0, -1));
                        dire.Add(target.Cell + new IntVec3(-1, 0, 0));
                        dire.Add(target.Cell + new IntVec3(0, 0, -1));
                        rotat = 45f;
                    }
                    else
                    {
                        dire.Add(target.Cell + new IntVec3(-1, 0, -1));
                        dire.Add(target.Cell + new IntVec3(1, 0, 1));
                        dire.Add(target.Cell + new IntVec3(-1, 0, 0));
                        dire.Add(target.Cell + new IntVec3(0, 0, 1));
                        rotat = 135f;
                    }
                }
                else
                if (target.Cell.x < self.Position.x)
                {
                    if (target.Cell.z > self.Position.z)
                    {
                        dire.Add(target.Cell + new IntVec3(-1, 0, -1));
                        dire.Add(target.Cell + new IntVec3(1, 0, 1));
                        dire.Add(target.Cell + new IntVec3(1, 0, 0));
                        dire.Add(target.Cell + new IntVec3(0, 0, -1));
                        rotat = 135f;
                    }
                    else
                    {
                        dire.Add(target.Cell + new IntVec3(-1, 0, 1));
                        dire.Add(target.Cell + new IntVec3(1, 0, -1));
                        dire.Add(target.Cell + new IntVec3(1, 0, 0));
                        dire.Add(target.Cell + new IntVec3(0, 0, 1));
                        rotat = 45f;
                    }
                }








                Volt3SkillThing Volt3s= (Volt3SkillThing)ThingMaker.MakeThing(ThingDef.Named("VoltSkill3Item"));
                Volt3s.self = self;
                Volt3s.dire = dire;
                Volt3s.rotat = rotat;
                GenSpawn.Spawn(Volt3s,target.Cell,self.Map);
                SoundDef.Named("Volt_3Skill").PlayOneShot(self);
                
                WarframeStaticMethods.StartCooldown(self, ck.cooldownTime, 3, WarframeStaticMethods.GetArmor(self).TryGetComp<CompWarframeSkill>().skill3mana);
                

            };


            return ck;

        }

        //咖喱技能4
        public static Command_CastSkill Skill4()
        {
            Command_CastSkill ck = new Command_CastSkill
            {
                defaultLabel = "VoltSkill4.name".Translate(),
                icon = ContentFinder<Texture2D>.Get("Skills/VoltSkill4"),
                targetingParams = WarframeStaticMethods.OnlyPawn(),
                cooldownTime = 4f,
                range = 16f,
                hotKey = KeyBindingDefOf.Misc7
            };
            // WarframeArmor sa = WarframeStaticMethods.getArmor(ck.self);

            ck.action = delegate (Pawn self)
            {
             


                SoundDef.Named("Volt_4Skill").PlayOneShot(self);
                {
                    Mote mote = (Mote)ThingMaker.MakeThing(ThingDef.Named("Mote_2ExFlash"), null);
                    mote.exactPosition = self.Position.ToVector3Shifted();
                    mote.Scale = (float)Mathf.Max(10f, 15f);
                    mote.rotationRate = 1.2f;
                    GenSpawn.Spawn(mote, self.Position + new IntVec3(0, 1, 0), self.Map, WipeMode.Vanish);
                }

                Volt4SkillThing thing = (Volt4SkillThing)ThingMaker.MakeThing(ThingDef.Named("VoltSkill4Item"));
                thing.self = self;
                thing.range = (int)ck.range;
                thing.damage= 50 * (1 + self.GetLevel() / 60f);
                GenSpawn.Spawn(thing, self.Position, self.Map);
                self.stances.stunner.StunFor(180,self);

                WarframeStaticMethods.StartCooldown(self, ck.cooldownTime, 4, WarframeStaticMethods.GetArmor(self).TryGetComp<CompWarframeSkill>().skill4mana);


            };


            return ck;

        }
       
    }
}
