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
                return this.GetStatValue(StatDefOf.EnergyShieldEnergyMax, true) * (1f + WarframeStaticMethods.getWFLevel(this.Wearer) / 30f);
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
                return this.energy;
            }
        }
        public float HP
        {
            get
            {
                return this.hp;
            }
        }
        public float MHP
        {
            get
            {
                return this.maxhp * (1f + WarframeStaticMethods.getWFLevel(this.Wearer) / 15f);
            }
        }
        public float SP
        {
            get
            {
                return this.sp;
            }
            set
            {
                this.sp = value;
            }
        }
        public float MSP
        {
            get
            {
                return this.maxsp * (1f+ WarframeStaticMethods.getWFLevel(this.Wearer) / 15f);
            }
        }

        public void addEnergy(float amount)
        {
            this.energy += ((amount * 1f) / 100f);
            
        }
        // Token: 0x170005EB RID: 1515
        // (get) Token: 0x060026FD RID: 9981 RVA: 0x00128655 File Offset: 0x00126A55
        public ShieldState ShieldState
        {
            get
            {
                if (this.ticksToReset > 0)
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
                Pawn wearer = base.Wearer;
                return wearer.Spawned && !wearer.Dead && !wearer.Downed && (wearer.InAggroMentalState || wearer.Drafted || (wearer.Faction.HostileTo(Faction.OfPlayer) && !wearer.IsPrisoner) || Find.TickManager.TicksGame < this.lastKeepDisplayTick + this.KeepDisplayingTicks);
            }
        }

        // Token: 0x060026FF RID: 9983 RVA: 0x001286FC File Offset: 0x00126AFC
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<float>(ref this.energy, "energy", 0f, false);
            Scribe_Values.Look<float>(ref this.sp, "sp", 0f, false);
            Scribe_Values.Look<int>(ref this.ticksToReset, "ticksToReset", -1, false);
            Scribe_Values.Look<int>(ref this.lastKeepDisplayTick, "lastKeepDisplayTick", 0, false);
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
            if (Find.Selector.SingleSelectedThing == base.Wearer)
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
            return this.EnergyMax * this.ApparelScorePerEnergyMax;
        }

        // Token: 0x06002702 RID: 9986 RVA: 0x00128780 File Offset: 0x00126B80
        public override void Tick()
        {
            base.Tick();
            if (base.Wearer == null)
            {
                this.energy = 0f;
                this.hp = 0f;
                this.sp = 0f;
                return;
            }
            if (this.hp<1) {
               this.onWearItReset();
            }
            if (this.ShieldState == ShieldState.Resetting)
            {
                this.ticksToReset--;
                if (this.ticksToReset <= 0)
                {
                    this.Reset();
                }
            }
            else if (this.ShieldState == ShieldState.Active)
            {
                if(Find.TickManager.TicksGame-this.lastAbsorbDamageTick>=this.StartingTicksToReset)
                this.energy += this.EnergyGainPerTick;

                if (this.energy > this.EnergyMax)
                {
                    this.energy = this.EnergyMax;
                }
            }
            this.hp = this.MHP - WarframeStaticMethods.getHP(this.Wearer);

            if (Find.TickManager.TicksGame % 60 == 0 && this.SP<this.MSP) {
                WarframeArmor wa = WarframeStaticMethods.getArmor(this.Wearer);
                if(wa.tillSkillOpen<=0)
                 this.SP+=1.5f;
            }
            if (this.SP > this.MSP) {
                this.SP = this.MSP;
            }
            if (this.SP < 0)
            {
                this.SP = 0;
            }
          //  Log.Warning(this.ShieldState+"?");

        }

        // Token: 0x06002703 RID: 9987 RVA: 0x00128818 File Offset: 0x00126C18
        public override bool CheckPreAbsorbDamage(DamageInfo dinfo)
        {//&& !dinfo.Instigator.Position.AdjacentTo8WayOrInside(base.Wearer.Position)
            if (this.ShieldState == ShieldState.Active && ((dinfo.Instigator != null ) || dinfo.Def.isExplosive))
            {
                if (dinfo.Instigator != null)
                {
                    AttachableThing attachableThing = dinfo.Instigator as AttachableThing;
                    if (attachableThing != null && attachableThing.parent == base.Wearer)
                    {
                        return false;
                    }
                }
                this.energy -= dinfo.Amount * 0.01f;
               /* TODO
                if (dinfo.Def == DamageDefOf.TornadoScratch)
                {
                    this.SP = -1f;
                    
                }
                */
                if (this.energy < 0f)
                {
                    this.Break();
                }
                else
                {
                    this.AbsorbedDamage(dinfo);
                }
                return true;
            }
            return false;
        }

        // Token: 0x06002704 RID: 9988 RVA: 0x001288FC File Offset: 0x00126CFC
        public void KeepDisplaying()
        {
            this.lastKeepDisplayTick = Find.TickManager.TicksGame;
        }

        // Token: 0x06002705 RID: 9989 RVA: 0x00128910 File Offset: 0x00126D10
        private void AbsorbedDamage(DamageInfo dinfo)
        {
            SoundDefOf.EnergyShield_AbsorbDamage.PlayOneShot(new TargetInfo(base.Wearer.Position, base.Wearer.Map, false));
            this.impactAngleVect = Vector3Utility.HorizontalVectorFromAngle(dinfo.Angle);
            Vector3 loc = base.Wearer.TrueCenter() + this.impactAngleVect.RotatedBy(180f) * 0.5f;
            float num = Mathf.Min(10f, 2f + dinfo.Amount / 10f);
            MoteMaker.MakeStaticMote(loc, base.Wearer.Map, ThingDefOf.Mote_ExplosionFlash, num);
            int num2 = (int)num;
            for (int i = 0; i < num2; i++)
            {
                MoteMaker.ThrowDustPuff(loc, base.Wearer.Map, Rand.Range(0.8f, 1.2f));
            }
            this.lastAbsorbDamageTick = Find.TickManager.TicksGame;
           
            this.KeepDisplaying();
        }

        // Token: 0x06002706 RID: 9990 RVA: 0x00128A08 File Offset: 0x00126E08
        private void Break()
        {
            SoundDefOf.EnergyShield_Broken.PlayOneShot(new TargetInfo(base.Wearer.Position, base.Wearer.Map, false));
            MoteMaker.MakeStaticMote(base.Wearer.TrueCenter(), base.Wearer.Map, ThingDefOf.Mote_ExplosionFlash, 12f);
            for (int i = 0; i < 6; i++)
            {
                Vector3 loc = base.Wearer.TrueCenter() + Vector3Utility.HorizontalVectorFromAngle((float)Rand.Range(0, 360)) * Rand.Range(0.3f, 0.6f);
                MoteMaker.ThrowDustPuff(loc, base.Wearer.Map, Rand.Range(0.8f, 1.2f));
            }
            this.energy = 0f;
            this.ticksToReset = this.StartingTicksToReset;
        }

        // Token: 0x06002707 RID: 9991 RVA: 0x00128AE8 File Offset: 0x00126EE8
        private void Reset()
        {
            if (base.Wearer.Spawned)
            {
                SoundDefOf.EnergyShield_Reset.PlayOneShot(new TargetInfo(base.Wearer.Position, base.Wearer.Map, false));
                MoteMaker.ThrowLightningGlow(base.Wearer.TrueCenter(), base.Wearer.Map, 3f);
            }
            this.ticksToReset = -1;
            this.energy = this.EnergyOnReset;

        }
        private void onWearItReset() {
            if (base.Wearer != null && base.Wearer.Spawned) {
                this.hp = this.TryGetComp<CompHPSP>().HP;
                this.maxhp = this.TryGetComp<CompHPSP>().HP;
                this.sp = this.TryGetComp<CompHPSP>().SP;
                this.maxsp = this.TryGetComp<CompHPSP>().SP;
            }
        }

        // Token: 0x06002708 RID: 9992 RVA: 0x00128B64 File Offset: 0x00126F64
        public override void DrawWornExtras()
        {
            if (this.ShieldState == ShieldState.Active && this.ShouldDisplay)
            {
                float num = Mathf.Lerp(1.2f, 1.55f, this.energy);
                Vector3 vector = base.Wearer.Drawer.DrawPos;
                vector.y = AltitudeLayer.MoteOverhead.AltitudeFor();
                int num2 = Find.TickManager.TicksGame - this.lastAbsorbDamageTick;
                if (num2 < 8)
                {
                    float num3 = (float)(8 - num2) / 8f * 0.05f;
                    vector += this.impactAngleVect * num3;
                    num -= num3;
                }
                float angle = (float)Rand.Range(0, 360);
                Vector3 s = new Vector3(num, 1f, num);
                Matrix4x4 matrix = default(Matrix4x4);
                matrix.SetTRS(vector, Quaternion.AngleAxis(angle, Vector3.up), s);
                Graphics.DrawMesh(MeshPool.plane10, matrix, WarframeBelt.BubbleMat, 0);
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
        private int StartingTicksToReset = 300;

        // Token: 0x0400161C RID: 5660
        private float EnergyOnReset = 0.2f;

        // Token: 0x0400161D RID: 5661
     //   private float EnergyLossPerDamage = 0.033f;

        // Token: 0x0400161E RID: 5662
        private int KeepDisplayingTicks = 1000;

        // Token: 0x0400161F RID: 5663
        private float ApparelScorePerEnergyMax = 0.25f;

        // Token: 0x04001620 RID: 5664
        private static readonly Material BubbleMat = MaterialPool.MatFrom("Other/ShieldBubble", ShaderDatabase.Transparent);
    }
}
