using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace Warframe
{
    // Token: 0x02000700 RID: 1792
    [StaticConstructorOnStartup]
    public class WarframeBelt : Apparel
    {
        // Token: 0x170005E8 RID: 1512
        // (get) Token: 0x060026FA RID: 9978 RVA: 0x0012862B File Offset: 0x00126A2B
        public float EnergyMax
        {
            get
            {
                return this.GetStatValue(StatDefOf.EnergyShieldEnergyMax, true) * (1f + WarframeStaticMethods.GetWFLevel(Wearer) / 30f);
            }
        }
        private float EnergyGainPerTick
        {
            get
            {
                return this.GetStatValue(StatDefOf.EnergyShieldRechargeRate, true) / 60f;
            }
        }
        public float Energy
        {
            get
            {
                return energy;
            }
        }
        public float HP
        {
            get
            {
                return hp;
            }
        }
        public float MHP
        {
            get
            {
                return maxhp * (1f + WarframeStaticMethods.GetWFLevel(Wearer) / 15f);
            }
        }
        public float SP
        {
            get
            {
                return sp;
            }
            set
            {
                sp = value;
            }
        }
        public float MSP
        {
            get
            {
                return maxsp * (1f+ WarframeStaticMethods.GetWFLevel(Wearer) / 15f);
            }
        }

        public void AddEnergy(float amount)
        {
            energy += ((amount * 1f) / 100f);
            
        }
        // Token: 0x170005EB RID: 1515
        // (get) Token: 0x060026FD RID: 9981 RVA: 0x00128655 File Offset: 0x00126A55
        public ShieldState ShieldState
        {
            get
            {
                if (ticksToReset > 0)
                {
                    return ShieldState.Resetting;
                }
                return ShieldState.Active;
            }
        }

        // Token: 0x170005EC RID: 1516
        // (get) Token: 0x060026FE RID: 9982 RVA: 0x00128668 File Offset: 0x00126A68
        private bool ShouldDisplay
        {
            get
            {
                Pawn wearer = Wearer;
                return wearer.Spawned && !wearer.Dead && !wearer.Downed && (wearer.InAggroMentalState || wearer.Drafted || (wearer.Faction.HostileTo(Faction.OfPlayer) && !wearer.IsPrisoner) || Find.TickManager.TicksGame < lastKeepDisplayTick + KeepDisplayingTicks);
            }
        }

        // Token: 0x060026FF RID: 9983 RVA: 0x001286FC File Offset: 0x00126AFC
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<float>(ref energy, "energy", 0f, false);
            Scribe_Values.Look<float>(ref sp, "sp", 0f, false);
            Scribe_Values.Look<int>(ref ticksToReset, "ticksToReset", -1, false);
            Scribe_Values.Look<int>(ref lastKeepDisplayTick, "lastKeepDisplayTick", 0, false);
        }

        // Token: 0x06002700 RID: 9984 RVA: 0x0012874C File Offset: 0x00126B4C
        public override IEnumerable<Gizmo> GetWornGizmos()
        {
            /*
            foreach (Gizmo gz in base.GetGizmos())
            {
                yield return gz;
            }
            */
            if (Find.Selector.SingleSelectedThing == Wearer)
            {
                yield return new Gizmo_WarframeBeltStatus
                {
                    shield = this,
                    btype = "HP",
                    orderadd = 1
                };
                yield return new Gizmo_WarframeBeltStatus
                {
                    shield = this,
                    btype = "SP",
                    orderadd = 2
                };
                yield return new Gizmo_WarframeBeltStatus
                {
                    shield = this,
                    btype = "Shield",
                    orderadd = 3
                };
            }
            yield break;
        }

        // Token: 0x06002701 RID: 9985 RVA: 0x0012876F File Offset: 0x00126B6F
        public override float GetSpecialApparelScoreOffset()
        {
            return EnergyMax * ApparelScorePerEnergyMax;
        }

        // Token: 0x06002702 RID: 9986 RVA: 0x00128780 File Offset: 0x00126B80
        public override void Tick()
        {
            base.Tick();
            if (Wearer == null)
            {
                energy = 0f;
                hp = 0f;
                sp = 0f;
                return;
            }
            if (hp<1) {
               OnWearItReset();
            }
            if (ShieldState == ShieldState.Resetting)
            {
                ticksToReset--;
                if (ticksToReset <= 0)
                {
                    Reset();
                }
            }
            else if (ShieldState == ShieldState.Active)
            {
                if(Find.TickManager.TicksGame-lastAbsorbDamageTick>=StartingTicksToReset)
                energy += EnergyGainPerTick;

                if (energy > EnergyMax)
                {
                    energy = EnergyMax;
                }
            }
            hp = MHP - WarframeStaticMethods.GetHP(Wearer);

            if (Find.TickManager.TicksGame % 60 == 0 && SP<MSP) {
                WarframeArmor wa = WarframeStaticMethods.GetArmor(Wearer);
                if(wa.tillSkillOpen<=0)
                 SP+=1.5f;
            }
            if (SP > MSP) {
                SP = MSP;
            }
            if (SP < 0)
            {
                SP = 0;
            }
          //  Log.Warning(this.ShieldState+"?");

        }

        // Token: 0x06002703 RID: 9987 RVA: 0x00128818 File Offset: 0x00126C18
        public override bool CheckPreAbsorbDamage(DamageInfo dinfo)
        {//&& !dinfo.Instigator.Position.AdjacentTo8WayOrInside(base.Wearer.Position)
            if (ShieldState == ShieldState.Active && ((dinfo.Instigator != null ) || dinfo.Def.isExplosive))
            {
                if (dinfo.Instigator != null)
                {
                    if (dinfo.Instigator is AttachableThing attachableThing && attachableThing.parent == Wearer)
                    {
                        return false;
                    }
                }
                energy -= dinfo.Amount * 0.01f;
               /* TODO
                if (dinfo.Def == DamageDefOf.TornadoScratch)
                {
                    this.SP = -1f;
                    
                }
                */
                if (energy < 0f)
                {
                    Break();
                }
                else
                {
                    AbsorbedDamage(dinfo);
                }
                return true;
            }
            return false;
        }

        // Token: 0x06002704 RID: 9988 RVA: 0x001288FC File Offset: 0x00126CFC
        public void KeepDisplaying()
        {
            lastKeepDisplayTick = Find.TickManager.TicksGame;
        }

        // Token: 0x06002705 RID: 9989 RVA: 0x00128910 File Offset: 0x00126D10
        private void AbsorbedDamage(DamageInfo dinfo)
        {
            SoundDefOf.EnergyShield_AbsorbDamage.PlayOneShot(new TargetInfo(Wearer.Position, Wearer.Map, false));
            impactAngleVect = Vector3Utility.HorizontalVectorFromAngle(dinfo.Angle);
            Vector3 loc = Wearer.TrueCenter() + impactAngleVect.RotatedBy(180f) * 0.5f;
            float num = Mathf.Min(10f, 2f + dinfo.Amount / 10f);
            MoteMaker.MakeStaticMote(loc, Wearer.Map, ThingDefOf.Mote_ExplosionFlash, num);
            int num2 = (int)num;
            for (int i = 0; i < num2; i++)
            {
                MoteMaker.ThrowDustPuff(loc, Wearer.Map, Rand.Range(0.8f, 1.2f));
            }
            lastAbsorbDamageTick = Find.TickManager.TicksGame;
           
            KeepDisplaying();
        }

        // Token: 0x06002706 RID: 9990 RVA: 0x00128A08 File Offset: 0x00126E08
        private void Break()
        {
            SoundDefOf.EnergyShield_Broken.PlayOneShot(new TargetInfo(Wearer.Position, Wearer.Map, false));
            MoteMaker.MakeStaticMote(Wearer.TrueCenter(), Wearer.Map, ThingDefOf.Mote_ExplosionFlash, 12f);
            for (int i = 0; i < 6; i++)
            {
                Vector3 loc = Wearer.TrueCenter() + Vector3Utility.HorizontalVectorFromAngle((float)Rand.Range(0, 360)) * Rand.Range(0.3f, 0.6f);
                MoteMaker.ThrowDustPuff(loc, Wearer.Map, Rand.Range(0.8f, 1.2f));
            }
            energy = 0f;
            ticksToReset = StartingTicksToReset;
        }

        // Token: 0x06002707 RID: 9991 RVA: 0x00128AE8 File Offset: 0x00126EE8
        private void Reset()
        {
            if (Wearer.Spawned)
            {
                SoundDefOf.EnergyShield_Reset.PlayOneShot(new TargetInfo(Wearer.Position, Wearer.Map, false));
                MoteMaker.ThrowLightningGlow(Wearer.TrueCenter(), Wearer.Map, 3f);
            }
            ticksToReset = -1;
            energy = EnergyOnReset;

        }
        private void OnWearItReset() {
            if (Wearer != null && Wearer.Spawned) {
                hp = this.TryGetComp<CompHPSP>().HP;
                maxhp = this.TryGetComp<CompHPSP>().HP;
                sp = this.TryGetComp<CompHPSP>().SP;
                maxsp = this.TryGetComp<CompHPSP>().SP;
            }
        }

        // Token: 0x06002708 RID: 9992 RVA: 0x00128B64 File Offset: 0x00126F64
        public override void DrawWornExtras()
        {
            if (ShieldState == ShieldState.Active && ShouldDisplay)
            {
                float num = Mathf.Lerp(1.2f, 1.55f, energy);
                Vector3 vector = Wearer.Drawer.DrawPos;
                vector.y = AltitudeLayer.MoteOverhead.AltitudeFor();
                int num2 = Find.TickManager.TicksGame - lastAbsorbDamageTick;
                if (num2 < 8)
                {
                    float num3 = (float)(8 - num2) / 8f * 0.05f;
                    vector += impactAngleVect * num3;
                    num -= num3;
                }
                float angle = (float)Rand.Range(0, 360);
                Vector3 s = new Vector3(num, 1f, num);
                Matrix4x4 matrix = default;
                matrix.SetTRS(vector, Quaternion.AngleAxis(angle, Vector3.up), s);
                Graphics.DrawMesh(MeshPool.plane10, matrix, BubbleMat, 0);
            }
        }

        // Token: 0x06002709 RID: 9993 RVA: 0x00128C49 File Offset: 0x00127049
        public override bool AllowVerbCast(IntVec3 root, Map map, LocalTargetInfo targ, Verb verb)
        {
            return true;//!(verb is Verb_LaunchProjectile) || ReachabilityImmediate.CanReachImmediate(root, targ, map, PathEndMode.Touch, null);
        }

        // Token: 0x04001612 RID: 5650
        private float energy;
        private float sp;
        private float hp;
        private float maxhp;
        private float maxsp;
        // Token: 0x04001613 RID: 5651
        private int ticksToReset = -1;

        // Token: 0x04001614 RID: 5652
        private int lastKeepDisplayTick = -9999;

        // Token: 0x04001615 RID: 5653
        private Vector3 impactAngleVect;

        // Token: 0x04001616 RID: 5654
        private int lastAbsorbDamageTick = -9999;

        // Token: 0x04001617 RID: 5655
        private const float MinDrawSize = 1.2f;

        // Token: 0x04001618 RID: 5656
        private const float MaxDrawSize = 1.55f;

        // Token: 0x04001619 RID: 5657
        private const float MaxDamagedJitterDist = 0.05f;

        // Token: 0x0400161A RID: 5658
        private const int JitterDurationTicks = 8;

        // Token: 0x0400161B RID: 5659
        private readonly int StartingTicksToReset = 300;

        // Token: 0x0400161C RID: 5660
        private readonly float EnergyOnReset = 0.2f;

        // Token: 0x0400161D RID: 5661
     //   private float EnergyLossPerDamage = 0.033f;

        // Token: 0x0400161E RID: 5662
        private readonly int KeepDisplayingTicks = 1000;

        // Token: 0x0400161F RID: 5663
        private readonly float ApparelScorePerEnergyMax = 0.25f;

        // Token: 0x04001620 RID: 5664
        private static readonly Material BubbleMat = MaterialPool.MatFrom("Other/ShieldBubble", ShaderDatabase.Transparent);
    }
}
