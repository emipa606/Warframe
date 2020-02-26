using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;
using Warframe.Skills.Mags;

namespace Warframe.Skills
{
    public static class Mag
    {
        //咖喱技能1
        public static Command_CastSkillTargeting Skill1()
        {
            Command_CastSkillTargeting ck = new Command_CastSkillTargeting();
            ck.defaultLabel = "MagSkill1.name".Translate();
            ck.icon = ContentFinder<Texture2D>.Get("Skills/MagSkill1");
            ck.targetingParams= WarframeStaticMethods.onlyPawn(); 
            ck.cooldownTime = 0.2f;
            ck.range = 20f;
            ck.finishAction = delegate {
                GenDraw.DrawFieldEdges(WarframeStaticMethods.getCellsAround(ck.self.Position, ck.self.Map, ck.range));
            };
            ck.hotKey = KeyBindingDefOf.Misc5;
            ck.action = delegate (Pawn self, Thing target)
            {
              
                // GenExplosion.DoExplosion(self.Position, self.Map, 3.5f, DamageDefOf.Bomb, self, -1, -1, null, null, null, null, null, 0, 1, false, null, 0, 1, 0, false);
                if (!WarframeStaticMethods.getCellsAround(self.Position, self.Map, ck.range).Contains(target.Position))
                {
                    SoundDefOf.ClickReject.PlayOneShotOnCamera();
                    return;
                }
                List < Pawn > linec= WarframeStaticMethods.getLineCell(self,target);
                if (linec == null)
                {
                    Messages.Message("BeBlockedByBuilding".Translate(),MessageTypeDefOf.RejectInput,false);
                    return;
                }
                if (!(target is Pawn)) return;
                Pawn tg = target as Pawn;


                List<IntVec3> eway = self.CellsAdjacent8WayAndInside().ToList();
                IntVec3 finalpoc = self.Position;
                if (tg.Position.x == self.Position.x)
                {
                    if (tg.Position.z > self.Position.z)
                        finalpoc = eway[5];
                    else
                        finalpoc = eway[3];
                }else
                if(tg.Position.z == self.Position.z)
                {
                    if (tg.Position.x > self.Position.x)
                        finalpoc = eway[7];
                    else
                        finalpoc = eway[1];
                }else
                if (tg.Position.x > self.Position.x)
                {
                    if (tg.Position.z > self.Position.z)
                        finalpoc = eway[8];
                    else
                        finalpoc = eway[6];
                }else
                if (tg.Position.x < self.Position.x)
                {
                    if (tg.Position.z > self.Position.z)
                        finalpoc = eway[2];
                    else
                        finalpoc = eway[0];
                }

                
                    List<Pawn> finalPawn = new List<Pawn>();
                    foreach (IntVec3 ic in tg.CellsAdjacent8WayAndInside()) {
                       foreach(Thing tt in tg.Map.thingGrid.ThingsAt(ic))
                        {
                            if (tt is Pawn && tt !=self && tt!=tg && tt.Position != self.Position)
                            {
                            if ((tt as Pawn).Faction != self.Faction)
                                finalPawn.Add(tt as Pawn);
                            }
                        }
                     }

                    if (tg.Faction != self.Faction)
                          finalPawn.Add(tg);

                    foreach (Pawn ttg in finalPawn) {
                        ttg.pather.StartPath(finalpoc, PathEndMode.Touch);
                        ttg.Position = finalpoc;
                        ttg.pather.StopDead();
                        if (ttg.jobs.curJob != null)
                        {
                            ttg.jobs.curDriver.Notify_PatherArrived();
                        }
                        ttg.stances.stunner.StunFor(120, self);
                    }
                
                

                SoundDef.Named("Mag_1Skill").PlayOneShot(self);
                float damage = 30 + (2 * WarframeStaticMethods.getWFLevel(self) / 5);
                DamageInfo dinfo = new DamageInfo(DefDatabase<DamageDef>.GetNamed("Mag",true), damage, 1, -1, self, null, null, DamageInfo.SourceCategory.ThingOrUnknown, target);
                foreach (Pawn p in finalPawn)
                {
                    if (p.Faction != self.Faction)
                    {
                        WarframeStaticMethods.showDamageAmount(p, damage.ToString("f0"));
                        p.TakeDamage(dinfo);
                    }
                }

               // WarframeStaticMethods.showDamageAmount(self, damage.ToString("f0"));
                WarframeStaticMethods.startCooldown(self, ck.cooldownTime, 1, WarframeStaticMethods.getArmor(self).TryGetComp<CompWarframeSkill>().Props.mana1);


            };


            return ck;

        }

        //咖喱技能2
        public static Command_CastSkill Skill2()
        {
            Command_CastSkillTargeting ck = new Command_CastSkillTargeting();
            ck.defaultLabel = "MagSkill2.name".Translate();
            ck.icon = ContentFinder<Texture2D>.Get("Skills/MagSkill2");
            ck.targetingParams = WarframeStaticMethods.onlyPawn();
            ck.cooldownTime = 0.2f;
            ck.range = 18f;
            ck.hotKey = KeyBindingDefOf.Misc8;
            ck.finishAction = delegate {
                GenDraw.DrawFieldEdges(WarframeStaticMethods.getCellsAround(ck.self.Position, ck.self.Map, ck.range));
            };
            ck.action = delegate (Pawn self,Thing target)
            {

                
                
                if (!WarframeStaticMethods.getCellsAround(self.Position, self.Map, ck.range).Contains(target.Position) || target.Faction==self.Faction)
                {
                    SoundDefOf.ClickReject.PlayOneShotOnCamera();
                    return;
                }
                SoundDef.Named("Mag_2Skill").PlayOneShot(self);
                List<Pawn> linec = WarframeStaticMethods.getLineCell(self, target);
                if (linec == null)
                {
                    Messages.Message("BeBlockedByBuilding".Translate(), MessageTypeDefOf.RejectInput, false);
                    return;
                }


                Hediff_Magnetize hediff_Magnetize = (Hediff_Magnetize)HediffMaker.MakeHediff(HediffDef.Named("Magnetize"), target as Pawn, null);
                hediff_Magnetize.self = self;
                (target as Pawn).health.AddHediff(hediff_Magnetize, null, null, null);






                WarframeStaticMethods.startCooldown(self, ck.cooldownTime, 2, WarframeStaticMethods.getArmor(self).TryGetComp<CompWarframeSkill>().skill2mana);


            };


            return ck;

        }


        //咖喱技能3
        public static Command_CastSkill Skill3()
        {
            Command_CastSkill ck = new Command_CastSkill();
            ck.defaultLabel = "MagSkill3.name".Translate();
            ck.icon = ContentFinder<Texture2D>.Get("Skills/MagSkill3");
            ck.targetingParams = WarframeStaticMethods.onlyPawn();
            ck.cooldownTime = 3f;
            ck.range = 18f;
            ck.hotKey = KeyBindingDefOf.Misc4;
            ck.action = delegate (Pawn self)
            {
                Mag3SkillThing mag3s= (Mag3SkillThing)ThingMaker.MakeThing(ThingDef.Named("MagSkill3Item"));
                mag3s.self = self;
                mag3s.range = (int)ck.range;
                mag3s.damage = 20 * (1 + self.getLevel()/60f);
                GenSpawn.Spawn(mag3s,self.Position,self.Map);
                SoundDef.Named("Mag_3Skill").PlayOneShot(self);
                
                WarframeStaticMethods.startCooldown(self, ck.cooldownTime, 3, WarframeStaticMethods.getArmor(self).TryGetComp<CompWarframeSkill>().skill3mana);


            };


            return ck;

        }

        //咖喱技能4
        public static Command_CastSkill Skill4()
        {
            Command_CastSkill ck = new Command_CastSkill();
            ck.defaultLabel = "MagSkill4.name".Translate();
            ck.icon = ContentFinder<Texture2D>.Get("Skills/MagSkill4");
            ck.targetingParams = WarframeStaticMethods.onlyPawn();
            ck.cooldownTime = 4f;
            ck.range = 12f;
            ck.hotKey = KeyBindingDefOf.Misc7;
           // WarframeArmor sa = WarframeStaticMethods.getArmor(ck.self);
           
            ck.action = delegate (Pawn self)
            {



                SoundDef.Named("Mag_4Skill").PlayOneShot(self);
                {
                    Mote mote = (Mote)ThingMaker.MakeThing(ThingDef.Named("Mote_2ExFlash"), null);
                    mote.exactPosition = self.Position.ToVector3Shifted();
                    mote.Scale = (float)Mathf.Max(10f, 15f);
                    mote.rotationRate = 1.2f;
                    GenSpawn.Spawn(mote, self.Position + new IntVec3(0, 1, 0), self.Map, WipeMode.Vanish);
                }

                Mag4SkillThing thing = (Mag4SkillThing)ThingMaker.MakeThing(ThingDef.Named("MagSkill4Item"));
                thing.self = self;
                thing.range = (int)ck.range;
                thing.damage= 60 * (1 + self.getLevel() / 60f);
                GenSpawn.Spawn(thing, self.Position, self.Map);
                self.stances.stunner.StunFor(180,self);

                WarframeStaticMethods.startCooldown(self, ck.cooldownTime, 4, WarframeStaticMethods.getArmor(self).TryGetComp<CompWarframeSkill>().skill4mana);


            };


            return ck;

        }
       
    }
}
