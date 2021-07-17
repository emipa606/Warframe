using System.Collections.Generic;
using System.Linq;
using RimWorld;
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
            var ck = new Command_CastSkillTargeting
            {
                defaultLabel = "MagSkill1.name".Translate(),
                icon = ContentFinder<Texture2D>.Get("Skills/MagSkill1"),
                targetingParams = WarframeStaticMethods.OnlyPawn(),
                cooldownTime = 0.2f,
                range = 20f
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

                if (!(target is Pawn pawn))
                {
                    return;
                }


                var eway = self.CellsAdjacent8WayAndInside().ToList();
                var finalpoc = self.Position;
                if (pawn.Position.x == self.Position.x)
                {
                    finalpoc = pawn.Position.z > self.Position.z ? eway[5] : eway[3];
                }
                else if (pawn.Position.z == self.Position.z)
                {
                    finalpoc = pawn.Position.x > self.Position.x ? eway[7] : eway[1];
                }
                else if (pawn.Position.x > self.Position.x)
                {
                    finalpoc = pawn.Position.z > self.Position.z ? eway[8] : eway[6];
                }
                else if (pawn.Position.x < self.Position.x)
                {
                    finalpoc = pawn.Position.z > self.Position.z ? eway[2] : eway[0];
                }


                var finalPawn = new List<Pawn>();
                foreach (var ic in pawn.CellsAdjacent8WayAndInside())
                {
                    foreach (var tt in pawn.Map.thingGrid.ThingsAt(ic))
                    {
                        if (tt is not Pawn thing || tt == self || tt == pawn || thing.Position == self.Position)
                        {
                            continue;
                        }

                        if (thing.Faction != self.Faction)
                        {
                            finalPawn.Add(thing);
                        }
                    }
                }

                if (pawn.Faction != self.Faction)
                {
                    finalPawn.Add(pawn);
                }

                foreach (var ttg in finalPawn)
                {
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
                float damage = 30 + (2 * WarframeStaticMethods.GetWFLevel(self) / 5);
                var dinfo = new DamageInfo(DefDatabase<DamageDef>.GetNamed("Mag"), damage, 1, -1, self, null, null,
                    DamageInfo.SourceCategory.ThingOrUnknown, pawn);
                foreach (var p in finalPawn)
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
            var ck = new Command_CastSkillTargeting
            {
                defaultLabel = "MagSkill2.name".Translate(),
                icon = ContentFinder<Texture2D>.Get("Skills/MagSkill2"),
                targetingParams = WarframeStaticMethods.OnlyPawn(),
                cooldownTime = 0.2f,
                range = 18f,
                hotKey = KeyBindingDefOf.Misc8
            };
            ck.finishAction = delegate
            {
                GenDraw.DrawFieldEdges(
                    WarframeStaticMethods.GetCellsAround(ck.self.Position, ck.self.Map, ck.range));
            };
            ck.action = delegate(Pawn self, Thing target)
            {
                if (!WarframeStaticMethods.GetCellsAround(self.Position, self.Map, ck.range)
                    .Contains(target.Position) || target.Faction == self.Faction)
                {
                    SoundDefOf.ClickReject.PlayOneShotOnCamera();
                    return;
                }

                SoundDef.Named("Mag_2Skill").PlayOneShot(self);
                var linec = WarframeStaticMethods.GetLineCell(self, target);
                if (linec == null)
                {
                    Messages.Message("BeBlockedByBuilding".Translate(), MessageTypeDefOf.RejectInput, false);
                    return;
                }


                var hediff_Magnetize =
                    (Hediff_Magnetize) HediffMaker.MakeHediff(HediffDef.Named("Magnetize"), target as Pawn);
                hediff_Magnetize.self = self;
                (target as Pawn)?.health.AddHediff(hediff_Magnetize);


                WarframeStaticMethods.StartCooldown(self, ck.cooldownTime, 2,
                    WarframeStaticMethods.GetArmor(self).TryGetComp<CompWarframeSkill>().skill2mana);
            };


            return ck;
        }


        //咖喱技能3
        public static Command_CastSkill Skill3()
        {
            var ck = new Command_CastSkill
            {
                defaultLabel = "MagSkill3.name".Translate(),
                icon = ContentFinder<Texture2D>.Get("Skills/MagSkill3"),
                targetingParams = WarframeStaticMethods.OnlyPawn(),
                cooldownTime = 3f,
                range = 18f,
                hotKey = KeyBindingDefOf.Misc4
            };
            ck.action = delegate(Pawn self)
            {
                var mag3s = (Mag3SkillThing) ThingMaker.MakeThing(ThingDef.Named("MagSkill3Item"));
                mag3s.self = self;
                mag3s.range = (int) ck.range;
                mag3s.damage = 20 * (1 + (self.GetLevel() / 60f));
                GenSpawn.Spawn(mag3s, self.Position, self.Map);
                SoundDef.Named("Mag_3Skill").PlayOneShot(self);

                WarframeStaticMethods.StartCooldown(self, ck.cooldownTime, 3,
                    WarframeStaticMethods.GetArmor(self).TryGetComp<CompWarframeSkill>().skill3mana);
            };


            return ck;
        }

        //咖喱技能4
        public static Command_CastSkill Skill4()
        {
            var ck = new Command_CastSkill
            {
                defaultLabel = "MagSkill4.name".Translate(),
                icon = ContentFinder<Texture2D>.Get("Skills/MagSkill4"),
                targetingParams = WarframeStaticMethods.OnlyPawn(),
                cooldownTime = 4f,
                range = 12f,
                hotKey = KeyBindingDefOf.Misc7
            };
            // WarframeArmor sa = WarframeStaticMethods.getArmor(ck.self);

            ck.action = delegate(Pawn self)
            {
                SoundDef.Named("Mag_4Skill").PlayOneShot(self);
                {
                    var mote = (Mote) ThingMaker.MakeThing(ThingDef.Named("Mote_2ExFlash"));
                    mote.exactPosition = self.Position.ToVector3Shifted();
                    mote.Scale = Mathf.Max(10f, 15f);
                    mote.rotationRate = 1.2f;
                    GenSpawn.Spawn(mote, self.Position + new IntVec3(0, 1, 0), self.Map);
                }

                var thing = (Mag4SkillThing) ThingMaker.MakeThing(ThingDef.Named("MagSkill4Item"));
                thing.self = self;
                thing.range = (int) ck.range;
                thing.damage = 60 * (1 + (self.GetLevel() / 60f));
                GenSpawn.Spawn(thing, self.Position, self.Map);
                self.stances.stunner.StunFor(180, self);

                WarframeStaticMethods.StartCooldown(self, ck.cooldownTime, 4,
                    WarframeStaticMethods.GetArmor(self).TryGetComp<CompWarframeSkill>().skill4mana);
            };


            return ck;
        }
    }
}