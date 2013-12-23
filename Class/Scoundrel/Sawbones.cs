using Distance = DarthBane.Helpers.Global.Distance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Buddy.BehaviorTree;
using DarthBane.Helpers;
using DarthBane.Managers;

namespace DarthBane.Class.Scoundrel
{
    class Sawbones : RotationBase
    {
        public override SWTorSpec KeySpec { get { return SWTorSpec.ScoundrelSawbones; } }
        public override string Name { get { return "Sawbones Healing"; } }
        public override string Revision { get { return ""; } }

        public override Composite PreCombat
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Lucky Shots"),
                    Rest.HandleRest,
                    Rest.CompanionHandler(),
                    Spell.Buff("Stealth", ret => !Rest.KeepResting() && !Rest.NeedRest()));
            }
        }

        public override Composite Pulling
        {
            get
            {
                return new PrioritySelector(
                    //Move To Range
                    CloseDistance(Distance.Ranged),

                    //Rotation
                    Spell.Cast("Distraction", ret => Me.CurrentTarget.IsCasting && Me.CurrentTarget.Distance <= 1f),
                    Spell.Buff("Crouch", ret => !Me.IsInCover() && !Me.IsMoving),
                    Spell.CastOnGround("XS Freighter Flyby", ret => ShouldAOE(3, 1f)),
                    Spell.Cast("Sabotage Charge", ret => Me.IsInCover()),
                    Spell.Cast("Shoot First", ret => Me.HasBuff("Stealth")),
                    Spell.Cast("Back Blast"),
                    Spell.Cast("Vital Shot", ret => !Me.CurrentTarget.HasDebuff("Bleeding (Tech)")),
                    Spell.Cast("Charged Burst", ret => Me.IsInCover() && Me.EnergyPercent >= 70),
                    Spell.Cast("Quick Shot", ret => Me.EnergyPercent >= 70),
                    Spell.Cast("Flurry of Bolts"));
            }
        }

        public override Composite PVERotation
        {
            get
            {
                return new PrioritySelector(
                    HealingManager.AcquireHealTargets,
                    Spell.WaitForCast(),
                    MedPack.UseItem(ret => Me.HealthPercent < 40),

                    Spell.Buff("Cool Head", ret => Me.EnergyPercent <= 20),
                    Spell.Buff("Pugnacity", ret => Me.EnergyPercent <= 70 && Me.BuffCount("Upper Hand") < 3),
                    Spell.Buff("Defense Screen", ret => Me.HealthPercent <= 75),
                    Spell.Buff("Dodge", ret => Me.HealthPercent <= 50),
                    Spell.Buff("Escape"),

                    Spell.Heal("Emergency Medpac", 30),
                    Spell.Heal("Kolto Cloud", on => Tank, 80, ret => HealingManager.ShouldAOE),
                    Spell.Heal("Slow-release Medpac", on => Tank, 100, ret => Tank != null && (Tank.BuffCount("Slow-release Medpac") < 2 || Tank.BuffTimeLeft("Slow-release Medpac") < 6)),
                    Spell.Heal("Kolto Pack", 80, ret => Me.BuffCount("Upper Hand") >= 2 && Me.EnergyPercent >= 60 && !HealTarget.HasBuff("Kolto Pack")),
                    Spell.Heal("Emergency Medpac", 80, ret => Me.BuffCount("Upper Hand") >= 2),
                    Spell.Heal("Underworld Medicine", 80),
                    Spell.Cleanse("Triage"),
                    Spell.Heal("Slow-release Medpac", 90, ret => HealTarget.BuffCount("Slow-release Medpac") < 2 || HealTarget.BuffTimeLeft("Slow-release Medpac") < 6),
                    Spell.Heal("Diagnostic Scan", 90),

                    //Move To Range
                    CloseDistance(Distance.Ranged),

                    //Rotation
                    Spell.Cast("Distraction", ret => Me.CurrentTarget.IsCasting && Me.CurrentTarget.Distance <= 1f),
                    Spell.Buff("Crouch", ret => !Me.IsInCover() && !Me.IsMoving),
                    Spell.CastOnGround("XS Freighter Flyby", ret => ShouldAOE(3, 1f)),
                    Spell.Cast("Sabotage Charge", ret => Me.IsInCover()),
                    Spell.Cast("Shoot First", ret => Me.HasBuff("Stealth")),
                    Spell.Cast("Back Blast"),
                    Spell.Cast("Vital Shot", ret => !Me.CurrentTarget.HasDebuff("Bleeding (Tech)")),
                    Spell.Cast("Charged Burst", ret => Me.IsInCover() && Me.EnergyPercent >= 70),
                    Spell.Cast("Quick Shot", ret => Me.EnergyPercent >= 70),
                    Spell.Cast("Flurry of Bolts"));
            }
        }
    }
}
