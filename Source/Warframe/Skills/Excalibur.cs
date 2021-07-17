using System;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace Warframe.Skills
{
    public static class Excalibur
    {
        //咖喱技能1
        public static Command_CastSkillTargeting Skill1()
        {
            var ck = new Command_CastSkillTargeting
            {
                defaultLabel = "ExcaliburSkill1.name".Translate(),
                icon = ContentFinder<Texture2D>.Get("Skills/ExcaliburSkill1"),
                targetingParams = WarframeStaticMethods.OnlyPawn(),
                cooldownTime = 0.2f,
                range = 10f
            };
            ck.finishAction = delegate
            {
                GenDraw.DrawFieldEdges(
                    WarframeStaticMethods.GetCellsAround(ck.self.Position, ck.self.Map, ck.range));
            };
            ck.hotKey = KeyBindingDefOf.Misc5;
            ck.action = delegate(Pawn self, Thing target)
            {
                // GenExplosion.DoExplosion(self.Position, self.Map, 3.5f, DamageDefOf.Bomb, self, -1, -1, null, null, null, null, null, 0, 1, false, null, 0, 1, 0, false);
                if (!WarframeStaticMethods.GetCellsAround(self.Position, self.Map, ck.range).Contains(target.Position))
                {
                    SoundDefOf.ClickReject.PlayOneShotOnCamera();
                    return;
                }

                var linec = WarframeStaticMethods.GetLineCell(self, target);
                if (linec == null)
                {
                    Messages.Message("BeBlockedByBuilding".Translate(), MessageTypeDefOf.RejectInput, false);
                    return;
                }


                self.pather.StartPath(target, PathEndMode.Touch);
                self.Position = target.Position;
                self.pather.StopDead();
                if (self.jobs.curJob != null)
                {
                    self.jobs.curDriver.Notify_PatherArrived();
                }

                SoundDef.Named("Excalibur_SlashDash").PlayOneShot(self);
                float damage = 30 + (2 * WarframeStaticMethods.GetWFLevel(self) / 5);
                var dinfo = new DamageInfo(DamageDefOf.Cut, damage, 1, -1, self, null, null,
                    DamageInfo.SourceCategory.ThingOrUnknown, target);
                foreach (var p in linec)
                {
                    if (p.Faction == self.Faction)
                    {
                        continue;
                    }

                    WarframeStaticMethods.ShowDamageAmount(p, damage.ToString("f0"));
                    p.TakeDamage(dinfo);
                }

                // WarframeStaticMethods.showDamageAmount(self, damage.ToString("f0"));
                WarframeStaticMethods.StartCooldown(self, ck.cooldownTime, 1,
                    WarframeStaticMethods.GetArmor(self).TryGetComp<CompWarframeSkill>().Props.mana1);
            };


            return ck;
        }

        //咖喱技能2
        public static Command_CastSkill Skill2()
        {
            var ck = new Command_CastSkill
            {
                defaultLabel = "ExcaliburSkill2.name".Translate(),
                icon = ContentFinder<Texture2D>.Get("Skills/ExcaliburSkill2"),
                targetingParams = WarframeStaticMethods.OnlyPawn(),
                cooldownTime = 0.2f,
                range = 18f,
                hotKey = KeyBindingDefOf.Misc8
            };
            ck.action = delegate(Pawn self)
            {
                SoundDef.Named("Excalibur_RadialBlind").PlayOneShot(self);
                foreach (var iv in WarframeStaticMethods.GetCellsAround(self.Position, self.Map, ck.range))
                {
                    foreach (var t in self.Map.thingGrid.ThingsAt(iv))
                    {
                        if (t is not Pawn pawn)
                        {
                            continue;
                        }

                        if (pawn != self)
                        {
                            if (pawn.Faction != self.Faction)
                            {
                                pawn.stances.stunner.StunFor(
                                    (int) (7f * 60f * (1f + (WarframeStaticMethods.GetWFLevel(self) / 30f))), self);
                            }
                        }
                        else
                        {
                            self.stances.stunner.StunFor(60, self);
                        }
                    }
                }

                {
                    var mote = (Mote) ThingMaker.MakeThing(ThingDef.Named("Mote_ExFlash"));
                    mote.exactPosition = self.Position.ToVector3Shifted();
                    mote.Scale = Mathf.Max(23, 25) * 6f;
                    mote.rotationRate = 1.2f;

                    GenSpawn.Spawn(mote, self.Position + new IntVec3(0, 1, 0), self.Map);
                }

                WarframeStaticMethods.StartCooldown(self, ck.cooldownTime, 2,
                    WarframeStaticMethods.GetArmor(self).TryGetComp<CompWarframeSkill>().Props.mana2);
            };


            return ck;
        }


        //咖喱技能3
        public static Command_CastSkill Skill3()
        {
            var ck = new Command_CastSkill
            {
                defaultLabel = "ExcaliburSkill3.name".Translate(),
                icon = ContentFinder<Texture2D>.Get("Skills/ExcaliburSkill3"),
                targetingParams = WarframeStaticMethods.OnlyPawn(),
                cooldownTime = 0.2f,
                range = 18f,
                hotKey = KeyBindingDefOf.Misc4
            };
            ck.action = delegate(Pawn self)
            {
                /*
                float damage = 120 + (8 * WarframeStaticMethods.getWFLevel(self) / 5);
                
                SoundDef.Named("Excalibur_RadialJavelin").PlayOneShot(self);
                foreach (IntVec3 iv in WarframeStaticMethods.getCellsAround(self.Position, self.Map, ck.range))
                {
                    foreach (Thing t in self.Map.thingGrid.ThingsAt(iv))
                    {
                        if (t is Pawn)
                        {
                            if ((t as Pawn) != self && (t as Pawn).Faction!=self.Faction)
                            {
                                WarframeStaticMethods.showDamageAmount(t, damage.ToString("f0"));
                                float totaldamage = 0;
                                DamageInfo dinfo = new DamageInfo(DamageDefOf.Cut, damage, 1, -1, self, null, null, DamageInfo.SourceCategory.ThingOrUnknown, null);
                                foreach (BodyPartRecord bpr in (t as Pawn).health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined,BodyPartDepth.Outside))
                                {
                                    
                 
                                    
                                        //dinfo.SetHitPart(bpr);

                                    (t as Pawn).TakeDamage(dinfo);
                                        totaldamage += bpr.def.hitPoints;
                                        dinfo.SetAmount(damage-totaldamage);
                                    

                                    if (totaldamage > damage)
                                    {
                                        break;
                                    }
                                }
                                







                                {
                                     Mote mote = (Mote)ThingMaker.MakeThing(ThingDef.Named("Mote_2ExFlash"), null);
                                    mote.exactPosition = t.Position.ToVector3Shifted();
                                    mote.Scale = (float)Mathf.Max(10f,15f);
                                    mote.rotationRate = 1.2f;
                                   // mote.Scale = 0.2f;
                                    GenSpawn.Spawn(mote, t.Position + new IntVec3(0, 1, 0), self.Map, WipeMode.Vanish);
                                }
                            }
                        }
                    }
                }
                */
                SoundDef.Named("Excalibur_RadialJavelin").PlayOneShot(self);
                self.stances.stunner.StunFor(60, self);
                {
                    var mote = (Mote) ThingMaker.MakeThing(ThingDef.Named("Mote_2ExFlash"));
                    mote.exactPosition = self.Position.ToVector3Shifted();
                    mote.Scale = Mathf.Max(10f, 15f);
                    mote.rotationRate = 1.2f;
                    GenSpawn.Spawn(mote, self.Position + new IntVec3(0, 1, 0), self.Map);
                }
                var thing = (ExcaliburSkill3Item) ThingMaker.MakeThing(ThingDef.Named("ExcaliburSkill3Item"));
                thing.self = self;
                thing.range = ck.range;

                thing.createdTick = Find.TickManager.TicksGame;
                GenSpawn.Spawn(thing, self.Position, self.Map);
                WarframeStaticMethods.StartCooldown(self, ck.cooldownTime, 3,
                    WarframeStaticMethods.GetArmor(self).TryGetComp<CompWarframeSkill>().Props.mana3);
            };


            return ck;
        }

        //咖喱技能4
        public static Command_CastSkill Skill4()
        {
            var ck = new Command_CastSkill
            {
                defaultLabel = "ExcaliburSkill4.name".Translate(),
                icon = ContentFinder<Texture2D>.Get("Skills/ExcaliburSkill4"),
                targetingParams = WarframeStaticMethods.OnlyPawn(),
                cooldownTime = 0.2f,
                range = 1f,
                hotKey = KeyBindingDefOf.Misc7
            };
            // WarframeArmor sa = WarframeStaticMethods.getArmor(ck.self);

            ck.action = delegate(Pawn self)
            {
                var wa = WarframeStaticMethods.GetArmor(self);
                if (wa.tillSkillOpen > 0)
                {
                    EndSkill4(self);

                    return;
                }


                Find.CameraDriver.shaker.DoShake(20000f * 15f /
                                                 (self.Position.ToVector3Shifted() - Find.Camera.transform.position)
                                                 .magnitude);
                SoundDef.Named("Excalibur_ExaltedBladePre").PlayOneShot(self);
                {
                    var mote = (Mote) ThingMaker.MakeThing(ThingDef.Named("Mote_2ExFlash"));
                    mote.exactPosition = self.Position.ToVector3Shifted();
                    mote.Scale = Mathf.Max(10f, 15f);
                    mote.rotationRate = 1.2f;
                    GenSpawn.Spawn(mote, self.Position + new IntVec3(0, 1, 0), self.Map);
                }

                //WFModBase.Instance._WFcontrolstorage.saveOldGun(self,self.equipment.Primary);
                if (self.equipment.Primary != null)
                {
                    wa.oldWeapon.Add(self.equipment.Primary);
                }

                wa.tillSkillOpen = 4;
                wa.tillSkillMul = 0.2f;
                self.equipment.Remove(self.equipment.Primary); //Primary.Destroy(DestroyMode.Vanish);
                self.equipment.AddEquipment(
                    (ThingWithComps) ThingMaker.MakeThing(ThingDef.Named("Excalibur_SkillBlade")));
                self.stances.stunner.StunFor(60, self);

                WarframeStaticMethods.StartCooldown(self, ck.cooldownTime, 4,
                    WarframeStaticMethods.GetArmor(self).TryGetComp<CompWarframeSkill>().Props.mana4);
            };


            return ck;
        }

        //咖喱4结束action
        public static void EndSkill4(Pawn self)
        {
            SoundDef.Named("Excalibur_ExaltedBladeOff").PlayOneShot(self);
            var wa = WarframeStaticMethods.GetArmor(self);
            self.equipment.Remove(self.equipment.Primary); //.Primary.Destroy(DestroyMode.Vanish);
            ThingWithComps gun = null;
            try
            {
                // gun = WFModBase.Instance._WFcontrolstorage.getOldGun(self);
                gun = wa.oldWeapon[0];
            }
            catch (Exception)
            {
                // Log.Warning("gun is null");
            }

            // WFModBase.Instance._WFcontrolstorage.clearWFandOG(self);
            wa.oldWeapon.Clear();

            if (gun != null)
            {
                self.equipment.AddEquipment(gun);
            }


            wa.tillSkillOpen = 0;
            wa.tillSkillMul = 1;
        }
    }
}