using Distance = DarthBane.Helpers.Global.Distance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Buddy.BehaviorTree;
using DarthBane.Helpers;
using DarthBane.Managers;

namespace DarthBane.Class.Operative
{
    class Concealment : RotationBase
    {
        public override SWTorSpec KeySpec { get { return SWTorSpec.OperativeConcealment; } }
        public override string Name { get { return "DarthBane : Concealment Operative"; } }
        public override string Revision { get { return ""; } }

        public override Composite PreCombat
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Coordination"),
                    Rest.HandleRest,
                    Rest.CompanionHandler(),
                    Spell.Buff("Stealth", ret => !Rest.KeepResting()));
            }
        }

        public override Composite Pulling
        {
            get
            {
                return new PrioritySelector(
                    Spell.WaitForCast(),
                    //Move To Range
                    CloseDistance(Distance.Melee),

                    new Decorator(ret => ShouldAOE(3, Distance.MeleeAoE),
                        new PrioritySelector(
                            Spell.CastOnGround("Orbital Strike"),
                            Spell.Cast("Fragmentation Grenade"),
                            Spell.Cast("Shiv", ret => !Me.HasBuff("Tactical Advantage") || Me.BuffCount("Tactical Advantage") < 2 || Me.BuffTimeLeft("Tactical Advantage") <= 6),
                            Spell.Cast("Carbine Burst", ret => Me.HasBuff("Tactical Advantage")))),

                    //Rotation
                    Spell.Cast("Stealth", ret => !Me.InCombat && !Me.HasBuff("Stealth")),
                    Spell.Cast("Cloaking Screen", ret => Me.InCombat && !Me.HasBuff("Stealth")),
                    Spell.Cast("Hidden Assault", ret => Me.HasBuff("Acid Blade") && Me.IsBehind(Me.CurrentTarget) && Me.IsStealthed),
                    Spell.Cast("Backstab", ret => Me.HasBuff("Acid Blade") && !Me.HasBuff("Stealth")),
                    Spell.Cast("Shiv", ret => !Me.HasBuff("Tactical Advantage") || Me.BuffCount("Tactical Advantage") < 2 || Me.BuffTimeLeft("Tactical Advantage") < 6),
                    Spell.Cast("Corrosive Dart", ret => !Me.CurrentTarget.HasDebuff("Poisoned (Tech)")),
                    Spell.Cast("Laceration"),
                    Spell.Cast("Rifle Shot"),
                    Spell.Cast("Overload Shot", ret => Me.EnergyPercent >= 70 && Me.IsInCover()));
            }
        }

        public override Composite PVERotation
        {
            get
            {
                return new PrioritySelector(
                    HealingManager.AcquireHealTargets,
                    Spell.WaitForCast(),
                    
                    Spell.Heal("Kolto Infusion", on => HealTarget, 60, ret => Me.BuffCount("Tactical Advantage") >= 2 && Me.EnergyPercent >= 50),
                    Spell.Heal("Kolto Injection", on => HealTarget, 60, ret => Me.EnergyPercent >= 25),
                    Spell.Heal("Diagnostic Scan", on => HealTarget, 70),

                    Spell.Buff("Adrenaline Probe", ret => Me.EnergyPercent <= 45),
                    MedPack.UseItem(ret => Me.HealthPercent <= 20),
                    Spell.Buff("Stim Boost", ret => Me.BuffCount("Tactical Advantage") <= 2),
                    Spell.Buff("Acid Blade", ret => Me.EnergyPercent >= 70),
                    Spell.Buff("Shield Probe", ret => Me.HealthPercent <= 75),
                    Spell.Buff("Evasion", ret => Me.HealthPercent <= 50),

                    //Move To Range
                    CloseDistance(Distance.Melee),

                    new Decorator(ret => ShouldAOE(3, Distance.MeleeAoE),
                        new PrioritySelector(
                            Spell.CastOnGround("Orbital Strike"),
                            Spell.Cast("Fragmentation Grenade"),
                            Spell.Cast("Shiv", ret => !Me.HasBuff("Tactical Advantage") || Me.BuffCount("Tactical Advantage") < 2 || Me.BuffTimeLeft("Tactical Advantage") <= 6),
                            Spell.Cast("Carbine Burst", ret => Me.HasBuff("Tactical Advantage")))),

                    //Rotation
                    Spell.Cast("Stealth", ret => !Me.InCombat && !Me.HasBuff("Stealth")),
                    Spell.Cast("Cloaking Screen", ret => Me.InCombat && !Me.HasBuff("Stealth")),
                    Spell.Cast("Hidden Assault", ret => Me.HasBuff("Acid Blade") && Me.IsBehind(Me.CurrentTarget) && Me.IsStealthed),
                    Spell.Cast("Backstab", ret => Me.HasBuff("Acid Blade") && !Me.HasBuff("Stealth")),
                    Spell.Cast("Shiv", ret => !Me.HasBuff("Tactical Advantage") || Me.BuffCount("Tactical Advantage") < 2 || Me.BuffTimeLeft("Tactical Advantage") < 6),
                    Spell.Cast("Corrosive Dart", ret => !Me.CurrentTarget.HasDebuff("Poisoned (Tech)")),
                    Spell.Cast("Laceration"),
                    Spell.Cast("Rifle Shot"),
                    Spell.Cast("Overload Shot", ret => Me.EnergyPercent >= 70 && Me.IsInCover()));
            }
        }
    }
}
