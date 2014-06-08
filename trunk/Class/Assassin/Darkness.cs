using System;
using System.Diagnostics;
using Buddy.CommonBot;
using Distance = DarthBane.Helpers.Global.Distance;
using Action = Buddy.BehaviorTree.Action;
using Buddy.BehaviorTree;
using DarthBane.Helpers;

namespace DarthBane.Class.Assassin
{
    class Darkness : RotationBase
    {
        public override SWTorSpec KeySpec { get { return SWTorSpec.AssassinDarkness; } }
        public override string Name { get { return "The Darkness"; } }
        public override string Revision { get { return ""; } }

        public override Composite PreCombat
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Dark Charge"),
                    Spell.Buff("Mark of Power"),
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
                    //Movement
                    CloseDistance(Distance.Melee),

                    //Rotation
                    Spell.Cast("Jolt", ret => Me.CurrentTarget.IsCasting),
                    Spell.Cast("Electrocute", ret => Me.CurrentTarget.IsCasting),
                    Spell.Cast("Low Slash", ret => Me.CurrentTarget.IsCasting),
                    Spell.Cast("Force Lightning", ret => Me.BuffCount("Harnessed Darkness") == 3),
                    Spell.Cast("Wither"),
                    Spell.Cast("Discharge"),
                    Spell.Cast("Shock", ret => Me.HasBuff("Energize")),
                    Spell.Cast("Maul", ret => Me.HasBuff("Conspirator's Cloak")),
                    Spell.Cast("Assassinate", ret => Me.CurrentTarget.HealthPercent <= 30),
                    Spell.Cast("Thrash", ret => Me.ForcePercent >= 25),
                    Spell.Cast("Saber Strike"),
                    Spell.Cast("Force Speed", ret => Me.CurrentTarget.Distance >= 1.1f && Me.IsMoving && Me.InCombat));
            }
        }

        public override Composite PVERotation
        {
            get
            {
                return new PrioritySelector(
                    Spell.WaitForCast(),
                    MedPack.UseItem(ret => Me.HealthPercent < 40),
                    Spell.Cast("Dark Ward", ret => Me.BuffCount("Dark Ward") <= 1 || Me.BuffTimeLeft("Dark Ward") < 3),
                    Spell.Cast("Unbreakable Will"),
                    Spell.Cast("Overcharge Saber", ret => Me.HealthPercent <= 60),
                    Spell.Cast("Deflection", ret => Me.HealthPercent <= 60),
                    Spell.Cast("Force Shroud", ret => Me.HealthPercent <= 50),
                    Spell.Cast("Recklessness"),

                    new Decorator(ret => ShouldAOE(3, Distance.MeleeAoE),
                        new PrioritySelector(
                            Spell.Cast("Wither"),
                            Spell.Cast("Discharge"),
                            Spell.Cast("Lacerate", ret => Me.ForcePercent >= 60 && Me.CurrentTarget.Distance <= 0.5f))),

                    Spell.Buff("Force Speed", ret => Me.CurrentTarget.Distance >= 1f && Me.CurrentTarget.Distance <= 3f),

                    //Movement
                    CloseDistance(Distance.Melee),

                    //Rotation
                    Spell.Cast("Jolt", ret => Me.CurrentTarget.IsCasting),
                    Spell.Cast("Electrocute", ret => Me.CurrentTarget.IsCasting || Me.HealthPercent < Me.CurrentTarget.HealthPercent),
                    Spell.Cast("Low Slash", ret => Me.CurrentTarget.IsCasting),
                    Spell.Cast("Spike", ret => Me.CurrentTarget.IsCasting),
                    Spell.Cast("Force Lightning", ret => Me.BuffCount("Harnessed Darkness") == 3),
                    Spell.Cast("Wither"),
                    Spell.Cast("Discharge"),
                    Spell.Cast("Shock", ret => Me.HasBuff("Energize")),
                    Spell.Cast("Force Pull"),
                    Spell.Cast("Maul", ret => Me.HasBuff("Conspirator's Cloak")),
                    Spell.Cast("Assassinate", ret => Me.CurrentTarget.HealthPercent <= 30),
                    Spell.Cast("Thrash", ret => Me.ForcePercent >= 25),
                    Spell.Cast("Saber Strike"),
                    Spell.Cast("Force Speed", ret => Me.CurrentTarget.Distance >= 1.1f && Me.IsMoving && Me.InCombat));
            }
        }

        private static bool needToStop()
        {
            return Me.IsMoving && !Me.InCombat && Me.CurrentTarget == null &&
                   (Me.HealthPercent < 100 || Me.ForcePercent < 100 || Me.Companion.HealthPercent < 100);
        }
    }
}
